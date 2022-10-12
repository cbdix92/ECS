using System;

namespace CMDR.DataSystem
{
    internal sealed class QueryBuilder : QueryList
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
        public bool GetQuery<T>(out Span<T> components) where T : struct, IComponent<T>
        {
            if (_nextSlice == SliceCount + 1)
            {
                components = default;
                _nextSlice = 0;
                return false;
            }

            unsafe
            {
                components = new Span<T>(_collection.GetPtrAtIndex(_slices[_nextSlice].Start), _slices[_nextSlice].Length);
            }
            
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
}