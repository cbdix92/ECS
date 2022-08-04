using System;

namespace CMDR.DataSystem
{
    public class QueryList
    {
        protected struct Slice
        {
            public readonly int Start;

            public readonly int End;

            public int Length => Math.Max(End - Start, 1);

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

        /// <summary>
        /// Adds a new index to the Slice collection.
        /// </summary>
        /// <param name="index"> The index to add to the Slice collection. </param>
        protected void Add(int index)
        {
            int pos = FindPosition(index);

            Insert(pos, index);

            SliceCheck(pos);
        }

        private bool BinarySearch(int index, int low, int high)
        {
            int mid = (low + high) / 2;

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

        /// <summary>
        /// Combines two Slices together.
        /// </summary>
        /// <param name="pos1"> The first Slice. Left or right order does not matter. </param>
        /// <param name="pos2"> The second Slice. Left or right order does not matter. </param>
        private void Combine(int pos1, int pos2)
        {
            _slices[pos1] = _slices[pos1] + _slices[pos2];
            
            Remove(pos2);
        }

        /// <summary>
        /// Checks if an index is contained within this QueryList using BinarySearch. 
        /// </summary>
        /// <param name="index"> The index that is to be searched for. </param>
        /// <returns> Returns True if the index was found, otherwise returns False. </returns>
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
            for (int i = _count; i > 0; i--)
            {
                if (_slices[i].Start <= index)
                {
                    return i;
                }
            }

            return 0;
        }

        /// <summary>
        /// Insert a new index into the Slice Collection.
        /// </summary>
        /// <param name="pos"> The position to insert the new index at. </param>
        /// <param name="index"> The index that is to be inserted. </param>
        private void Insert(int pos, int index)
        {
            // Check if storage is at capacity.
            if(_count == _slices.Length - 1)
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

        /// <summary>
        /// Check if the Slice to the left of the current Slice is sequential.
        /// </summary>
        /// <param name="pos"> The position of the current Slice. </param>
        /// <returns> Returns True if the Slice is sequential, otherwise return False. </returns>
        private bool LeftIsSequential(int pos)
        {
            if (pos == 0)
            {
                return false;
            }

            return _slices[pos].Start - 1 == _slices[pos - 1].End;
        }

        /// <summary>
        /// Removes a Slice from the collection.
        /// </summary>
        /// <param name="pos"> The posittion of the Slice that is to be removed. </param>
        private void Remove(int pos)
        {
            for (int i = pos; i < _count; i++)
            {
                _slices[i] = _slices[i + 1];
            }

            _count--;
        }

        /// <summary>
        /// Checks if the Slice to the right of the current Slice is sequential.
        /// </summary>
        /// <param name="pos"> The position of the current Slice. </param>
        /// <returns> Returns True if the Slice is sequential, otherwise returns False. </returns>
        private bool RightIsSequential(int pos)
        {
            return _slices[pos].End + 1 == _slices[pos + 1].Start;
        }

        /// <summary>
        /// Checks if a new Slice can be combined with it's neighbor Slices.
        /// </summary>
        /// <param name="newSlicePosition"> The index position of the new Slice. </param>
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
