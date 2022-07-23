using System;

namespace CMDR.DataSystem
{
    internal sealed class QueryBuilder<T> : QueryList, IQueryBuilder<T> where T : struct, IComponent
    {
        #region PUBLIC_MEMBERS

        

        #endregion

        #region PRIVATE_MEMBERS

        private readonly IComponentCollection<T> _collection;

        #endregion

        public QueryBuilder(IComponentCollection<IComponent> collection) : base()
        {
            _collection = collection as IComponentCollection<T>;
        }

        #region PUBLIC_METHODS

        public bool GetQuery(out Span<T> components)
        {
            if (_nextSlice == SliceCount)
            {
                components = default;
                _nextSlice = 0;
                return false;
            }

            components = new Span<T>(_collection.ToArray(), _data[_nextSlice].Start, _data[_nextSlice].End);
            
            _nextSlice++;

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

        bool GetQuery(out Span<T> components);
    }

    internal interface IQueryBuilder : IQueryBuilder<IComponent>
    {
    }
}