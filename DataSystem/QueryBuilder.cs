using System;

namespace CMDR.DataSystem
{
    internal sealed class QueryBuilder<T> : QueryList, IQueryBuilder<T> where T : struct, IComponent
    {
        #region PUBLIC_MEMBERS

        #endregion

        #region PRIVATE_MEMBERS

        private IComponentCollection _collection;

        #endregion

        public QueryBuilder(IComponentCollection collection) : base()
        {
            _collection = collection;
        }

        #region PUBLIC_METHODS

        public bool GetQuery(out Span<T> components)
        {
            if(_nextSlice == SliceCount)
            {
                components = default;
                _nextSlice = 0;
                return false;
            }

            if(_data[_nextSlice + 1] == -1)
            {
                components = new Span<T>(_collection, _nextSlice, _nextSlice + 2)
                _nextSlice += 2;
            }
            else
            {
                components = new Span<T>(_collection, _nextSlice, _nextSlice);
                _nextSlice++;
            }
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