using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Buffers;
using System.Runtime.InteropServices;

namespace CMDR.DataSystem
{
    internal delegate void OnComponentDestroyedHandler(int index);
    internal delegate void OnComponentMovedHandler(int previousIndex, int newIndex);

    internal sealed class ComponentCollection
    {
        #region EXCEPTIONS

        public class IncorrectTypeArgumentForCollection : Exception
        {
            public IncorrectTypeArgumentForCollection() { }

            public IncorrectTypeArgumentForCollection(string message) : base(message) { }

            public IncorrectTypeArgumentForCollection(string message, Exception inner) : base(message, inner) { }

            public IncorrectTypeArgumentForCollection(Type expectedType, Type providedType) : base($"Incorrect type argument! Expected {expectedType.Name} but received {providedType.Name}.") { }
        }

        #endregion

        #region PUBLIC_MEMBERS

        public unsafe byte* Ptr => _componentsPtr;

        public int Count => _count;

        public int Capacity => _capacity;

        public Type ComponentType { get; private set; }

        public int ComponentSizeInBytes => _componentSizeInBytes;

        public event OnComponentDestroyedHandler ComponentDestroyedEvent;

        public event OnComponentMovedHandler ComponentMovedEvent;

        #endregion

        #region PRIVATE_MEMBERS

        private unsafe byte* _componentsPtr;

        private int _count;

        private int _capacity;

        private int _componentSizeInBytes;

        private readonly Dictionary<ID, int> _idToIndexLookUp;

        private readonly Dictionary<int, ID> _indexToIDLookup;

        /// <summary>
        /// Used for radix sorting.
        /// </summary>
        private readonly static ArrayPool<int> _countPool = ArrayPool<int>.Create();

        #endregion

        public ComponentCollection(Type tComp)
        {
            ComponentType = tComp;

            _componentSizeInBytes = Marshal.SizeOf(tComp);

            _capacity = Data.StorageScale * _componentSizeInBytes;

            _idToIndexLookUp = new Dictionary<ID, int>();

            _indexToIDLookup = new Dictionary<int, ID>();

            unsafe
            {
                _componentsPtr = (byte*)Marshal.AllocHGlobal(_capacity);
            }
        }

        ~ComponentCollection()
        {
            unsafe
            {
                Marshal.FreeHGlobal((IntPtr)_componentsPtr);
            }
        }

        #region PUBLIC_METHODS

        public void Add<T>(T component) where T : struct, IComponent<T>
        {
            TypeCheckHelper(typeof(T));

            if (_count * _componentSizeInBytes == _capacity)
            {
                IncreaseCapacity();
            }

            unsafe
            {
                Unsafe.Write(_componentsPtr + (_count * _componentSizeInBytes), component);
            }

            _count++;

            _idToIndexLookUp.Add(component.ID, _count - 1);

            _indexToIDLookup.Add(_count - 1, component.ID);
        }

        public bool Contains(ID id) => _idToIndexLookUp.ContainsKey(id);

        public ref T GetRef<T>(ID id) where T : struct, IComponent<T>
        {
            TypeCheckHelper(typeof(T));

            int index = _idToIndexLookUp[id];

            unsafe
            {
                return ref Unsafe.AsRef<T>(Unsafe.Add<T>(_componentsPtr, index));
            }
        }

        public T Get<T>(ID id)
        {
            int index = _idToIndexLookUp[id];

            unsafe
            {
                Span<T> _ = new Span<T>(_componentsPtr, _count);

                return _[index];
            }

        }

        public int GetIndex(ID id)
        {
            return _idToIndexLookUp.ContainsKey(id) ? _idToIndexLookUp[id] : -1;
        }

        /// <summary>
        /// Return a pointer starting at the given index.
        /// </summary>
        /// <param name="index"> A zero based index for the collection. </param>
        /// <returns> A pointer starting at the given index. </returns>
        /// <exception cref="IndexOutOfRangeException"> The index was not  in range of the collection.  </exception>
        public unsafe byte* GetPtrAtIndex(int index)
        {
            if (index * _componentSizeInBytes > _count * _componentSizeInBytes)
            {
                throw new IndexOutOfRangeException("The index was not in range of the collection.");
            }


            return _componentsPtr + (index * _componentSizeInBytes);
        }

        public Span<T> GetSpan<T>()
        {
            TypeCheckHelper(typeof(T));

            unsafe
            {
                return new Span<T>(_componentsPtr, _count);
            }
        }

        /// <summary>
        /// Removes a component from this collection.
        /// </summary>
        /// <param name="id"> The ID of the compoent that is to be removed. </param>
        public void Remove(ID id)
        {
            int index = GetIndex(id);

            if (index == -1)
            {
                return;
            }

            // Remove components ID lookup.
            _idToIndexLookUp.Remove(id);

            // If the Component is at the end of the array, it can be simply overwritten by the next component.
            if (index == --_count)
            {
                ComponentDestroyedEvent?.Invoke(index);

                return;
            }

            // Replace the component with the one at the end of the collection. 
            //unsafe
            //{
            //    Unsafe.AsRef<T>(Unsafe.Add<T>(_componentsPtr, index)) = Unsafe.AsRef<T>(Unsafe.Add<T>(_componentsPtr, _count));
            //}

            // Replace the component with the one at the end of the collection.
            unsafe
            {
                //new Span<byte>(GetPtrAtIndex(index), _componentSizeInBytes)[0] = new Span<byte>(GetPtrAtIndex(_count), _componentSizeInBytes)[0];
                
                Span<byte> oldComp = new Span<byte>(GetPtrAtIndex(index), _componentSizeInBytes);
                
                Span<byte> newComp = new Span<byte>(GetPtrAtIndex(_count), _componentSizeInBytes);
                
                for (int i = 0; i < oldComp.Length; i++)
                {
                    newComp[i] = oldComp[i];
                }
            }

            // Change ID lookup to reflect the changes.
            _idToIndexLookUp[_indexToIDLookup[_count]] = index;
            
            _indexToIDLookup[index] = _indexToIDLookup[_count];
            
            _indexToIDLookup.Remove(index);

            ComponentDestroyedEvent?.Invoke(index);

            ComponentMovedEvent?.Invoke(_count, index);
        }

        /// <summary>
        /// Sort the Components by the ID in ascending order using base16 Radix sort.
        /// </summary>
        /// <param name="maxBitPos"> The max bit position for generated IDs. </param>
        public void SortComponents<T>(int maxBitPos) where T : struct, IComponent<T>
        {
            TypeCheckHelper(typeof(T));

            // Sort components with Base16 Radix sort.
            Span<T> sortedOutput = InternalSortComponents(maxBitPos, 0, 0xf, GetSpan<T>(), new Span<T>(new T[_count]));

            sortedOutput.CopyTo(GetSpan<T>());

            // Rebuild the ID lookup to reflect the changes.
            for (int i = 0; i < _count; i++)
            {
                int previousIndex = _idToIndexLookUp[sortedOutput[i].ID];

                if (previousIndex != i)
                {
                    ComponentMovedEvent?.Invoke(previousIndex, i);

                    _idToIndexLookUp[sortedOutput[i].ID] = i;
                }
            }
        }

        public T[] ToArray<T>() where T : struct, IComponent<T>
        {
            TypeCheckHelper(typeof(T));

            Span<T> result = GetSpan<T>();

            return result.ToArray();
        }

        #endregion

        #region PRIVATE_METHODS

        private unsafe void IncreaseCapacity()
        {
            int newSize = _capacity + (Data.StorageScale * _componentSizeInBytes);

            byte* newMemory = (byte*)Marshal.AllocHGlobal(newSize);

            Unsafe.CopyBlock(newMemory, _componentsPtr, (uint)_capacity);

            Span<byte> memoryToInit = new Span<byte>(newMemory + _capacity, newSize - _capacity);

            memoryToInit.Fill(0);

            _capacity = newSize;

            Marshal.FreeHGlobal((IntPtr)_componentsPtr);

            _componentsPtr = newMemory;
        }

        /// <summary>
        /// Base16 Radix Sort in ascending order. The method is called recursively and swaps the input and output arrays. Use the return value as output. 
        /// </summary>
        /// <param name="max"> The most significant bit for generated IDs. </param>
        /// <param name="pos"> The current bit position. </param>
        /// <param name="mask"> The current bit mask. </param>
        /// <param name="input"> The input array that will be sorted. </param>
        /// <param name="output"> The output array for sorted components. </param>
        /// <returns> Returns a sorted Component array in ascending order. </returns>
        private Span<T> InternalSortComponents<T>(int max, int pos, uint mask, Span<T> input, Span<T> output) where T : struct, IComponent<T>
        {
            int[] count = _countPool.Rent(16);

            // Count
            for (int i = 0; i < input.Length; i++)
            {
                count[(input[i].ID & mask) >> pos]++;
            }

            // Prefix Sum
            for (int i = 0; i < 15; i++)
            {
                count[i + 1] += count[i];
            }

            // Build Output
            for (int i = input.Length - 1; i > -1; i--)
            {
                output[--count[(input[i].ID & mask) >> pos]] = input[i];
            }

            // Exit Conditions
            if (pos >= max)
            {
                return output;
            }

            // Recursion
            return InternalSortComponents(max, pos + 4, mask << 4, output, input);
        }

        /// <summary>
        /// Validates the given Component type for this collection.
        /// </summary>
        /// <param name="received"> The provided Component type. </param>
        /// <exception cref="IncorrectTypeArgumentForCollection"> The provided Component type is incorrect for this collection. </exception>
        private void TypeCheckHelper(Type received)
        {
            if (received != ComponentType)
            {
                throw new IncorrectTypeArgumentForCollection(ComponentType, received);
            }
        }

        #endregion
    }
}
