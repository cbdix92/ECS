using System;
using System.Collections.Generic;

namespace CMDR.DataSystem
{
    public class Queryable
    {
        
        #region PUBLIC_MEMBERS

        public Dictionary<Query, IQueryBuilder<IComponent>> Queries { get; private set; }

        #endregion

        #region PRIVATE_MEMBERS

        private readonly Dictionary<Type, Query[]> _typeToQueryLookup;

        private readonly Dictionary<Type, IComponentCollection<IComponent>> _componentsRef;
        #endregion

        protected private Queryable(Dictionary<Type, IComponentCollection<IComponent>> components)
        {
            Queries = new Dictionary<Query, IQueryBuilder<IComponent>>();

            _typeToQueryLookup = new Dictionary<Type, Query[]>();

            _componentsRef = components;
        }

        #region PUBLIC_METHODS

        public Query RegisterQuery<T>(Filter filter) where T : struct, IComponent<T>
        {
            Type type = typeof(T);

            Query query = new Query(type, filter);

            if(Queries.ContainsKey(query) == false)
            {
                Type TNew = typeof(QueryBuilder<>).MakeGenericType(typeof(T));

                object[] args = new object[] { (object)_componentsRef[type] };

                Queries.Add(query, Activator.CreateInstance(TNew) as IQueryBuilder);

                if(_typeToQueryLookup.ContainsKey(type))
                {
                    Query[] array = _typeToQueryLookup[type];
                    Array.Resize<Query>(ref array, _typeToQueryLookup[type].Length + 1);
                }

                _typeToQueryLookup[type][_typeToQueryLookup[type].Length - 1] = query;
            }

            return query;
        }

        public bool GetQuery(Query query, out Span<IComponent> components)
        {
            return Queries[query].GetQuery(out components);
        }

        /// <summary>
        /// Sort a GameObject into the query system.
        /// Note that the Components of the GameObject must be stored in the Data System prior to sorting.
        /// </summary>
        /// <param name="gameObject"> The GameObject that is to be sorted. </param>
        public void Sort(GameObject gameObject, Dictionary<Type, IComponentCollection<IComponent>> components)
        {
            foreach(Query query in Queries.Keys)
            {
                if (query.Sort(gameObject))
                {
                    Queries[query].AddNew(gameObject.ID);
                }
            }
        }

        #endregion

    }
}