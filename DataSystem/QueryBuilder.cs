using System;

namespace CMDR.DataSystem
{
    internal sealed class QueryBuilder<T> : QueryList, IQueryBuilder<T> where T : struct, IComponent
    {
        #region PUBLIC_MEMBERS

        #endregion

        #region PRIVATE_MEMBERS

        private readonly IComponentCollection _collection;

        #endregion

        public QueryBuilder(IComponentCollection collection) : base()
        {
            _collection = collection;
        }

        #region PUBLIC_METHODS

        public bool GetQuery(out Span<IComponent> components)
        {
            if (_nextSlice == SliceCount)
            {
                components = default;
                _nextSlice = 0;
                return false;
            }

            if (_data[_nextSlice + 1] == -1)
            {
                components = new Span<IComponent>(_collection.ToArray(), _nextSlice, _nextSlice + 2);
                _nextSlice += 2;
            }
            else
            {
                components = new Span<IComponent>(_collection.ToArray(), _nextSlice, _nextSlice);
                _nextSlice++;
            }

            return true;
        }

        public void AddNew(ID id)
        {
            Add(_collection.GetIndex(id));
        }

        public bool Contains(ID id)
        {
            return _collection.Contains(id);
        }

        #endregion
    }

    internal interface IQueryBuilder<T>
    {
        void AddNew(ID id);

        bool GetQuery(out Span<IComponent> components);
    }

    internal interface IQueryBuilder : IQueryBuilder<IComponent>
    {
    }
}