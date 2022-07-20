using System;
using System.Runtime.InteropServices;

namespace CMDR.DataSystem
{
    public ref struct QueryListOLD<T>
    {

        #region PUBLIC_MEMBERS
        
        public int Count { get; private set; }

        public int Capacity { get; private set; }

        public int Size { get; private set; }

        public IntPtr Alloc { get; private set; }

        #endregion

        #region PRIVATE_MEMBERS

        private Span<Memory<IComponent>> _data;

        #endregion

        #region PUBLIC_METHODS

        /// <summary>
        /// Add a Memory<T> to this QueryList
        /// </summary>
        /// <param name="memory"> The Memory<T> that will be added to the collection. </param>
        public void Add(Memory<IComponent> memory)
        {
            if(Count >= Capacity)
            {
                // The QueryBuilder for this QueryList didn't correctly track the SliceCount.
                throw new Exception(); 
            }

            _data[Count] = memory;
            
            Count++;
        }

        /// <summary>
        /// Allocate new memory on the stack for a new internal Span. 
        /// </summary>
        public void Build(int size, int sliceCount)
        {
            if(size != Size)
            {
                Purge();

                Size = size;

                unsafe
                {
                    Alloc = Marshal.AllocHGlobal(size);

                    _data = new Span<Memory<IComponent>>(Alloc.ToPointer(), sliceCount);
                }

            }
            
            Capacity = sliceCount;

        }

        /// <summary>
        /// Deallocate internal data and reset state. This will prepare this QueryList for the next rebuild.
        /// </summary>
        public void Purge()
        {
            Marshal.FreeHGlobal(Alloc);
            Alloc = IntPtr.Zero;
            Count = 0;
        }

        #endregion
    }
}