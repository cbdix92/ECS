using System;
using System.Collections.Generic;
using CMDR.DataSystem;

namespace CMDR
{
    internal class ComponentCollection<T> : IComponentCollection<T> where T : struct, IComponent
    {

        #region PUBLIC_MEMBERS

        public int Count { get => _count; }

        public int Capacity { get => _capacity; }

        public T this[int index]
        {
            get
            {
                if(index >= _count)
                {
                    throw new IndexOutOfRangeException($"index: {index} is outside the bounds of the array.");
                }
                return _components[index];
            }
        }

        #endregion

        #region PRIVATE_MEMBERS

        private int _count;

        private int _capacity;

        private T[] _components;

        private Dictionary<ID, int> _idToIndexLookUp;

        #endregion

        internal ComponentCollection() : this(Data.StorageScale) {}

        internal ComponentCollection(int capacity)
        {
            Capacity = capacity;

            _components = new T[capacity];
            
            _idToIndexLookUp = new Dictionary<ID int>(capacity);
        }

        
        #region PUBLIC_METHODS

        public void Add(T component)
        {
            if(_count + 1 == Capacity)
            {
                _capacity += Data.StorageScale;

                Array.Resize<T>(ref _components, _capacity);
            }

            _components[_count++] = component;
        }

        public void AddRange(T[] components)
        {
            for(int i = 0; i < components.Length; i++)
            {
                Add(components[i]);
            }
        }

        public bool Contains(ID id) => _idToIndexLookUp.ContainsKey(id);

        public bool Contains(T component) => Contains(component.ID);

        public T Get(ID id) => _components[_idToIndexLookUp[id]];

        public int GetIndex(T component)
        {
            return GetIndex(component.id);
        }

        public int GetIndex(ID id)
        {
            return _idToIndexLookUp.ContainsKey(id) ? _idToIndexLookUp[id] : -1;
        }

        public Span<T> GetSpan()
        {
            return new Span<T>(_components, 0, _count);
        }

        public void Remove(T component)
        {
            RemoveAt(GetIndex(component));
        }

        public void Remove(ID id)
        {
            RemoveAt(GetIndex(id));
        }

        public void RemoveAt(int index)
        {
            // Remove components ID lookup.
            _idToIndexLookUp.Remove(_components[index]);
            
            if(index == --_count)
            {
                _components[_count] = Default(T);
                return;
            }

            // Replace the component with the one at the end of the array. 
            _components[index] = _components[_count];

            // Change ID lookup to reflect the changes.
            _idToIndexLookUp[_components[index].ID] = index;
        }

        public static void SortComponents()
        {
            _components = InternalSortComponents(Data.GetMaxIDRange(), 0, 0xf, this.GetSpan(), new Span<T>(new T[_components.Length])).ToArray();

            for(int i = 0; i < _count; i++)
            {
                _idToIndexLookUp[_components[i].ID] = i;
            }
        }

        

        #endregion

        #region PRIVATE_METHODS

        private T _getComponent(ID id) => _components[_idToIndexLookUp[id]];

        private static T[] InternalSortComponents<T>(int max, int pos, Span<T> input, Span<T> output)
        {
            count = _countPool.Rent(16);

            // Count
            for(int i = 0; i < input.Length; i++)
            {
                count[(input[i].ID & mask) >> pos]++;
            }

            // Prefix Sum
            for(int i = 0; i < 15; i++)
            {
                count[i + 1] += count[1];
            }

            // Build Output
            for(int i = input.Length - 1; 1 > -1; i--)
            {
                output[--count[(input[i] & mask) >> pos]] = input[i];
            }

            // Exit Conditions
            if(pos >= max)
                return output;
            
            // Recursion
            return InternalSortComponents<T>(max, pos + 4, mask << 4, output, input);
        }

        #endregion
    }

    internal interface IComponentCollection<T>
    {
        uint Count { get; }

        void Add(T component);

        void AddRange(T[] components);

        bool Contains(ID id);

        bool Contains(T component);

        T Get(ID id);

        int GetIndex(T component);

        int GetIndex(ID id);

        Span<T> GetSpan();

        void Remove(T component);

        void Remove(ID id);

        void RemoveAt(int index);
    }

    internal interface IComponentCollection : IComponentCollection<IComponent>
    {
    }

    
}