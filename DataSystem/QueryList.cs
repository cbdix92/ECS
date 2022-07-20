using System;

namespace CMDR.DataSystem
{
    public sealed class QueryList<T>
    {
        #region PUBLIC_MEMBERS



        #endregion

        #region PRIVATE_MEMBERS

        private IComponentCollection _collection;

        #endregion

        public QueryList(IComponentCollection collection)
        {
            _collection = collection;
        }

        #region PUBLIC_METHODS

        public bool Next(out Span<T> components)
        {

        }

        #endregion

    }
}
