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
            int pos = Math.Max(_count - 1, 0);

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

        public void OnMove(int previousIndexPosition, int newIndexPosition)
        {

        }

        #endregion

        #region PRIVATE_METHODS

        private void SliceCheck(int newIndexPosition)
        {
            if (RightIsSequential(newIndexPosition) && RightIsSlice(newIndexPosition))
            {
                // Remove the right neighbor
                Remove(newIndexPosition + 1);

                return;
            }

            if (LeftIsSequential(newIndexPosition) && LeftIsSlice(newIndexPosition))
            {
                // Remove the left neighbor
                Remove(newIndexPosition - 1);
                
                return;
            }

            if (RightIsSequential(newIndexPosition) && LeftIsNeagtiveOne(newIndexPosition))
            {
                Remove(newIndexPosition);
                
                return;
            }

            if (LeftIsSequential(newIndexPosition) && RightIsNegativeOne(newIndexPosition))
            {
                Remove(newIndexPosition);
                
                return;
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

        private bool RightIsSequential(int pos)
        {
            return _data[pos] + 1 == CheckRight(pos);
        }

        private bool LeftIsSequential(int pos)
        {
            return _data[pos] - 1 == CheckLeft(pos);
        }

        private bool RightIsSlice(int pos)
        {
            return _data[pos + 2] == -1;
        }

        private bool LeftIsSlice(int pos)
        {
            return _data[pos - 2] == -1;
        }

        private bool RightIsNegativeOne(int pos)
        {
            return _data[pos + 1] == -1;
        }

        private bool LeftIsNeagtiveOne(int pos)
        {
            return _data[pos - 1] == -1;
        }

        private void Insert(int pos, int index)
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
