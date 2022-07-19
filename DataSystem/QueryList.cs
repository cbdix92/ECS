using System;
using System.Runtime.InteropServices;

namespace CMDR.DataSystem
{
    public class QueryList<T> : IDisposable
    {

        #region PUBLIC_MEMBERS
        
        public int Count { get; private set; }

        public int Capacity { get; private set; }

        public IntPtr Alloc { get; private set; }

        #endregion

        #region PRIVATE_MEMBERS

        private bool _disposed = false;

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
        /// Deallocate internal data if needed then allocate new memory on the stack for a new internal Span. 
        /// </summary>
        public void Rebuild(int maxSlices)
        {
            Purge();
            
            Capacity = maxSlices;
            
            Alloc = Marshal.AllocHGlobal(sizeof(Span<T>) * maxSlices);

            unsafe
            {
                data = new Span<Span<T>>(Alloc.ToPointer(), MaxSlices);
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

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(_disposed)
                return;
            if(disposing)
            {
                Marshal.FreeHGlobal(Alloc);
            }

            _disposed = true;
        }

        #endregion
    }
}