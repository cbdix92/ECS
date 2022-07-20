using System;

namespace CMDR.DataSystem
{
    internal sealed class QueryBuilder<T> : IQueryBuilder<T> where T : struct, IComponent
    {
        #region PUBLIC_MEMBERS

        public int Count { get; private set; }

        public int Capacity { get; private set; }

        public int SliceCount { get; private set; }

        public int TotalComponents { get; private set; }

        #endregion

        #region PRIVATE_MEMBERS

        private int[] _data;

        private QueryList<T> _queryList;

        #endregion

        public QueryBuilder()
        {
            _data = new int[Data.StorageScale];

            Capacity = _data.Length;

            _queryList = new QueryList<T>();
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

        public QueryList<T> GetQueryList(IComponentCollection<IComponent> components)
        {
            _queryList.Build(components.Size * TotalComponents, SliceCount);

            int i = 0;
            while (i < _data.Length)
            {
                if (_data[i + 1] == -1)
                {
                    // Store a slice
                    _queryList.Add(new Memory<IComponent>(components.ToArray(), _data[i], _data[i + 2]));

                    i += 2;
                }
                else
                {
                    // Store a single
                    _queryList.Add(new Memory<IComponent>(components.ToArray(), _data[i], _data[i]));

                    i++;
                }
            }

            return _queryList;
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

        public void Insert(int pos, int index)
        {
            // Check if storage is at capacity.
            if(Count == Capacity)
            {
                Array.Resize<int>(ref _data, _data.Length + Data.StorageScale);
            }

            // Make room to insert the new index. 
            if (pos != Count)
            {
                for (int i = Count; i >= pos; i--)
                {
                    _data[i + 1] = _data[i];
                }
            }

            _data[pos] = index;

            Count++;
        }

        #endregion
    }

    internal interface IQueryBuilder<T>
    {
        void OnAdd(int index);

        void OnRemove(int index);

        void OnMove(int previousIndex, int newIndex);

        QueryList<T> GetQueryList(IComponentCollection<IComponent> components);
    }

    internal interface IQueryBuilder : IQueryBuilder<IComponent>
    {
    }
}