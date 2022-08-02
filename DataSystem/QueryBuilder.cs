using System;

namespace CMDR.DataSystem
{
    internal sealed class QueryBuilder<T> : QueryList, IQueryBuilder<T> where T : struct, IComponent<T>
    {
        #region PUBLIC_MEMBERS

        

        #endregion

        #region PRIVATE_MEMBERS

        private readonly ComponentCollection _collection;

        #endregion

        public QueryBuilder(ComponentCollection collection) : base()
        {
            _collection = collection;
        }

        #region PUBLIC_METHODS

        /// <summary>
        /// Gets a queried slice of the target components.
        /// </summary>
        /// <param name="components"> The output slice of the component collection. </param>
        /// <returns> Returns True if there are more slices, otherwise returns false. </returns>
        public bool GetQuery(out Span<T> components)
        {
            if (_nextSlice == SliceCount)
            {
                components = default;
                _nextSlice = 0;
                return false;
            }

            components = new Span<T>(_collection.ToArray<T>(), _data[_nextSlice].Start, _data[_nextSlice].End);
            
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
        void OnComponentMoved(int previousIndex, int newIndex);

        void OnComponentDestroyed(int index);

        void AddNew(ID id);

        bool GetQuery(out Span<T> components);
    }

    internal interface IQueryBuilder : IQueryBuilder<IComponent>
    {
    }
}