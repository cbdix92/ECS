using System;

namespace CMDR.DataSystem
{
    public abstract class QueryList
    {
        protected struct Slice
        {
            public readonly int Start;

            public readonly int End;

            public Slice(int start, int end) => (Start, End) = (start, end);

            public bool Contains(int index)
            {
                return (index >= Start) && (index <= End);
            }

            public static Slice operator +(Slice left, Slice right)
            {
                return new Slice(Math.Min(left.Start, right.Start), Math.Max(left.End, right.End));
            }
        }

        #region PUBLIC_MEMBERS

        public int SliceCount { get => _count; }

        #endregion

        #region PRIVATE_MEMBERS

        protected int _nextSlice;

        protected Slice[] _data;

        private int _count;

        #endregion

        public QueryList()
        {
            _data = new Slice[Data.StorageScale];
        }

        #region PUBLIC_METHODS

        OnComponentMoved(int previousIndex, int newIndex)
        {
            // Check if this QueryList contains previousIndex
            if(Contains(previousIndex) == false)
            {
                return;
            }

            // If so, remove it and add newIndex
            Remove(previousIndex);

            Add(newIndex);
        }

        OnComponentDestroyed(int index)
        {
            // Check if this QueryList contains index
            if(Contains(index) == false)
            {
                return;
            }

            // If so, remove it.
            int pos = FindPosition(index);

            Remove(index);

            SliceCheck(pos);
        }
        

        #endregion

        #region PRIVATE_METHODS

        protected void Add(int index)
        {
            int pos = FindPosition(index);

            Insert(pos, index);

            SliceCheck(pos);

        }

        private bool Contains(int index)
        {
            for(int i = 0; i < _data.Length - 1; i++)
            {

            }
            return false;
        }

        private bool BinarySearch(int index, int low, int high)
        {
            int mid = low + high / 2;

            if (_data[mid].Start)
        }

        private int FindPosition(int index)
        {
            int pos = Math.Max(_count - 1, 0);

            while (index < _data[pos].Start)
            {
                pos--;
            }

            pos++;

            return pos;
        }

        private void SliceCheck(int newSlicePosition)
        {
            if (RightIsSequential(newSlicePosition))
            {
                Combine(newSlicePosition, newSlicePosition + 1);
            }
            
            if (LeftIsSequential(newSlicePosition))
            {
                Combine(newSlicePosition - 1, newSlicePosition);
            }
        }

        private bool RightIsSequential(int pos)
        {
            return _data[pos].End == _data[pos + 1].Start + 1;
        }

        private bool LeftIsSequential(int pos)
        {
            return _data[pos].Start == _data[pos - 1].End - 1;
        }

        private void Combine(int pos1, int pos2)
        {
            _data[pos1] = _data[pos1] + _data[pos2];
            
            Remove(pos2);
        }

        private void Insert(int pos, int index)
        {
            // Check if storage is at capacity.
            if(_count == _data.Length)
            {
                Array.Resize(ref _data, _data.Length + Data.StorageScale);
            }

            // Make room to insert the new index. 
            if (pos != _count)
            {
                for (int i = _count - 1; i >= pos; i--)
                {
                    _data[i + 1] = _data[i];
                }
            }

            _data[pos] = new Slice(index, index);

            _count++;
        }

        private void Remove(int indexPosition)
        {
            for (int i = indexPosition; i < _count; i++)
            {
                _data[i] = _data[i + 1];
            }

            _count--;
        }

        #endregion

    }
}
