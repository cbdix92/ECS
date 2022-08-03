using System;

namespace CMDR.DataSystem
{
    public class QueryList
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

        protected Slice[] _slices;

        private int _count;

        #endregion

        public QueryList()
        {
            _slices = new Slice[Data.StorageScale];
        }

        #region PUBLIC_METHODS

        public void OnComponentMoved(int previousIndex, int newIndex)
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

        public void OnComponentDestroyed(int index)
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

        private bool BinarySearch(int index, int low, int high)
        {
            int mid = low + high / 2;

            if (_slices[mid].Contains(index))
            {
                return true;
            }

            if (low >= high)
            {
                return false;
            }

            if (_slices[mid].Start < index)
            {
                return BinarySearch(index, mid + 1, high);
            }
            else
            {
                return BinarySearch(index, low, mid - 1);
            }
        }

        private void Combine(int pos1, int pos2)
        {
            _slices[pos1] = _slices[pos1] + _slices[pos2];
            
            Remove(pos2);
        }

        private bool Contains(int index)
        {
            return BinarySearch(index, 0, _slices.Length - 1);
        }


        /// <summary>
        /// Finds the Slice index for a given index.
        /// </summary>
        /// <param name="index"> The ComponentCollection index. </param>
        /// <returns> A slice index for the ComponentCollection index. </returns>
        private int FindPosition(int index)
        {

            int pos = _count;

            while (index > _slices[pos].Start)
            {
                pos--;
            }

            pos++;

            return pos;
        }

        private void Insert(int pos, int index)
        {
            // Check if storage is at capacity.
            if(_count == _slices.Length)
            {
                Array.Resize(ref _slices, _slices.Length + Data.StorageScale);
            }

            // Make room to insert the new index. 
            if (pos != _count)
            {
                for (int i = _count - 1; i >= pos; i--)
                {
                    _slices[i + 1] = _slices[i];
                }
            }

            _slices[pos] = new Slice(index, index);

            _count++;
        }

        private bool LeftIsSequential(int pos)
        {
            return _slices[pos].Start == _slices[pos - 1].End - 1;
        }

        private void Remove(int indexPosition)
        {
            for (int i = indexPosition; i < _count; i++)
            {
                _slices[i] = _slices[i + 1];
            }

            _count--;
        }

        private bool RightIsSequential(int pos)
        {
            return _slices[pos].End == _slices[pos + 1].Start + 1;
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

        #endregion

    }
}
