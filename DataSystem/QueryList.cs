using System;

namespace CMDR.DataSystem
{
    public abstract class QueryList
    {
        #region PUBLIC_MEMBERS

        public int SliceCount { get; private set; }

        #endregion

        #region PRIVATE_MEMBERS

        protected int _nextSlice;

        protected int[] _data;

        private int _count;

        #endregion

        public QueryList()
        {
            _data = new int[Data.StorageScale];
        }

        #region PUBLIC_METHODS

        public void OnAdd(int index)
        {
            int pos = Math.Max(Count - 1, 0);

            while (_data[pos] > index || _data[pos] == -1)
            {
                pos--;
            }
            
            pos++;
            
            Insert(pos, index);

            SliceCheck(pos);
            
        }

        public void OnRemove(int index)
        {

        }

        public void OnMove(int previousIndex, int newIndex)
        {

        }

        #endregion

        #region PRIVATE_METHODS

        private void SliceCheck(int newIndex)
        {
            // Check if number to the right is sequential
            if (_data[newIndex] + 1 == _data[newIndex + 1])
            {
                // Right side is part of a slice.
                if (_data[newIndex + 2] == -1)
                {
                    // Remove the right neighbor and shift the array left.
                    for(int i = newIndex + 1; i < Count; i++)
                    {
                        _data[i] = _data[i + 1];
                    }
                }
            }
        }

        private int CheckRight(int pos)
        {
            return _data[pos + 1];
        }

        private int CheckLeft(int pos)
        {
            return _data[pos - 1];
        }

        private bool CheckRightSequential(int pos)
        {
            return _data[pos] + 1 == CheckRight(pos);
        }

        private bool CheckLeftSequential(int pos)
        {
            return _data[pos] - 1 == CheckLeft(pos);
        }

        public void Insert(int pos, int index)
        {
            // Check if storage is at capacity.
            if(_count == _data.Length)
            {
                Array.Resize<int>(ref _data, _data.Length + Data.StorageScale);
            }

            // Make room to insert the new index. 
            if (pos != _count)
            {
                for (int i = _count - 1; i >= pos; i--)
                {
                    _data[i + 1] = _data[i];
                }
            }

            _data[pos] = index;

            _count++;
        }

        #endregion

    }
}
