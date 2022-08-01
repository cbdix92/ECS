using System;
using System.Collections.Generic;

namespace CMDR.DataSystem
{
    public class Queryable
    {
        
        #region PUBLIC_MEMBERS

        internal Dictionary<Query, IQueryBuilder<IComponent>> Queries { get; private set; }

        #endregion

        #region PRIVATE_MEMBERS

        private readonly Dictionary<Type, Query[]> _typeToQueryLookup;

        protected private Dictionary<Type, ComponentCollection> _componentsQueryRef;
        #endregion

        protected private Queryable()
        {
            Queries = new Dictionary<Query, IQueryBuilder<IComponent>>();

            _typeToQueryLookup = new Dictionary<Type, Query[]>();
        }

        #region PUBLIC_METHODS

        public virtual Query RegisterQuery<T>(Filter filter) where T : struct, IComponent<T>
        {
            Type type = typeof(T);

            Query query = new Query(type, filter);

            if(Queries.ContainsKey(query) == false)
            {
                Type TNew = typeof(QueryBuilder<>).MakeGenericType(typeof(T));

                object[] args = new object[] { _componentsQueryRef[type] };

                Queries.Add(query, Activator.CreateInstance(TNew, args) as IQueryBuilder);

                if(_typeToQueryLookup.ContainsKey(type))
                {
                    Query[] array = _typeToQueryLookup[type];
                    Array.Resize<Query>(ref array, _typeToQueryLookup[type].Length + 1);
                }

                _typeToQueryLookup[type][^1] = query;
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
        protected private void Sort(GameObject gameObject)
        {
            Type[] componentTypes = gameObject.Components;

            for (int i = 0; i < componentTypes.Length - 1; i++)
            {
                if (_typeToQueryLookup.ContainsKey(componentTypes[i]) == false)
                {
                    continue;
                }

                Query[] queriesByType = _typeToQueryLookup[componentTypes[i]];
                
                for (int j = 0; j < queriesByType.Length - 1; j++)
                {
                    if (queriesByType[j].Sort(gameObject))
                    {
                        Queries[queriesByType[j]].AddNew(gameObject.ID);
                    }
                }
            }
        }

        #endregion

    }
}