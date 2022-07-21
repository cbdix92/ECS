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

        public bool GetQuery(out Span<IComponent> components)
        {
            if(_nextSlice == SliceCount)
            {
                components = default;
                _nextSlice = 0;
                return false;
            }

            if(_data[_nextSlice + 1] == -1)
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

        #endregion

        #region PRIVATE_METHODS



        #endregion
    }

    internal interface IQueryBuilder<T>
    {
        void OnAdd(int index);

        void OnRemove(int index);

        void OnMove(int previousIndex, int newIndex);

        bool GetQuery(out Span<IComponent> components);
    }

    internal interface IQueryBuilder : IQueryBuilder<IComponent>
    {
    }
}