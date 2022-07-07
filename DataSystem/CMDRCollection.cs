using System;

namespace CMDR
{
    internal abstract class CMDRCollection<T>
    where T : struct, IComponent<T>
    {
        #region PUBLIC_MEMBERS

        public uint Count { get; private set; }

        public T this[int index]
        {
            get
            {
                if(index >= Count || index < 0)
                    throw new IndexOutOfRangeException($"{index} is out of range for {typeof(T).Name} Collection");
                
                return _data[index];
            }
        }

        #endregion

        #region PRIVATE_MEMBERS

        private T[] _data;

        #endregion

        #region CONSTRUCTORS

        internal CMDRCollection()
        {
            _data = new T[Data.StorageScale];
        }
        
        #endregion

        #region PUBLIC_METHODS

        public virtual uint Add(T obj)
        {
            if(Count == _data.Length)
                Array.Resize(ref _data, _data.Length + Data.StorageScale);
            
            _data[Count] = obj;

            return Count++;
        }

        public virtual void Remove(IComponent component)
        {
            if(component is GameObject gameObject)
                return;
            uint index = Data.ComponentIndexLookUp[(byte)(component.ID & Data.ComponentTypeMask)][(uint)(component.ID & Data.IDMask)];

            if(_data[index].ID != component.ID)
                return; // Todo ... Log error here.
        }

        public T BinarySearch(uint id)
        {
            return BinarySearch(id, 0, Count - 1);
        }

        public T BinarySearch(uint id, uint low, uint high)
        {
            if (Count == 0)
                throw new Exception("Cannot search empty collection.");
            
            if(high >= Count)
                high = Count - 1;
            
            if(low < 0)
                low = 0;

            return BinarySearchInternal(id, low, high);
        }

        public virtual T[] ToArray()
        {
            ArraySegment<T> _ = new ArraySegment<T>(_data, 0, (int)Count);

            return _.ToArray();
        }

        #endregion

        #region PRIVATE_METHODS

        private T BinarySearchInternal(uint id, uint low, uint high)
        {
            uint mid = high / 2;

            if((_data[mid].ID & 0xffffffff) == id)
            {
                return _data[mid];
            }
            else if(id < mid)
            {
                return BinarySearch(id, low, mid - 1);
            }
            else
            {
                return BinarySearch(id, mid + 1, high);
            }
        }

        #endregion
    }
}