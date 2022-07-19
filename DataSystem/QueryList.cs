using System;
using System.Runtime.InteropServices;

namespace CMDR.DataSystem
{
    public ref struct QueryList<T>
    {

        #region PUBLIC_MEMBERS
        
        public int Count { get; private set; }

        public int Capacity { get; private set; }

        public IntPtr Alloc { get; private set; }

        #endregion

        #region PRIVATE_MEMBERS

        private Span<T> _data;

        #endregion

        public QueryList()
        {
            Count = 0;
        }

        #region PUBLIC_METHODS

        /// <summary>
        /// Add a Span<T> to this QueryList
        /// </summary>
        /// <param name="span"> The Span<T> that will be added to the collection. </param>
        public void Add(Span<T> span)
        {
            if(Count == Capacity)
            {
                throw new Exception(); 
            }

            _data[Count] = span;
            Count++;
        }

        /// <summary>
        /// Allocate new memory on the stack for a new internal Span. 
        /// </summary>
        public void Build(int maxSlices)
        {
            Purge();
            
            Capacity = maxSlices;
            

            unsafe
            {
                Alloc = Marshal.AllocHGlobal(size);
                _data = new Span<T>(Alloc.ToPointer(), maxSlices);
            }

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