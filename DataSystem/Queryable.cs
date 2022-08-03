using System;
using System.Collections.Generic;

namespace CMDR.DataSystem
{
    public class Queryable
    {
        
        #region PUBLIC_MEMBERS

        internal Dictionary<Query, QueryBuilder> Queries { get; private set; }

        #endregion

        #region PRIVATE_MEMBERS

        private readonly Dictionary<Type, Query[]> _typeToQueryLookup;

        protected private Dictionary<Type, ComponentCollection> _componentsQueryRef;
        #endregion

        protected private Queryable()
        {
            Queries = new Dictionary<Query, QueryBuilder>();

            _typeToQueryLookup = new Dictionary<Type, Query[]>();
        }

        #region PUBLIC_METHODS

        public Query RegisterQuery<T>(Filter filter) where T : struct, IComponent<T>
        {
            Type tComp = typeof(T);

            Query query = new Query(tComp, filter);

            if(Queries.ContainsKey(query) == false)
            {
                Queries.Add(query, new QueryBuilder(_componentsQueryRef[tComp]));

                if(_typeToQueryLookup.ContainsKey(tComp) == false)
                {
                    _typeToQueryLookup.Add(tComp, new Query[0]);
                }

                Query[] array = _typeToQueryLookup[tComp];
                
                Array.Resize(ref array, _typeToQueryLookup[tComp].Length + 1);

                array[^1] = query;

                _typeToQueryLookup[tComp] = array;

                // Subscribe to ComponentCollection Events
                _componentsQueryRef[tComp].ComponentDestroyedEvent += Queries[query].OnComponentDestroyed;
                _componentsQueryRef[tComp].ComponentMovedEvent += Queries[query].OnComponentMoved;
            }

            return query;
        }

        public bool GetQuery<T>(Query query, out Span<T> components) where T : struct, IComponent<T>
        {
            return Queries[query].GetQuery(out components);
        }

        #endregion

        #region PRIVATE_METHODS

        /// <summary>
        /// Sort a GameObject into the query system.
        /// Note that the Components of the GameObject must be stored in the Data System prior to sorting.
        /// </summary>
        /// <param name="gameObject"> The GameObject that is to be sorted. </param>
        protected private void Sort(GameObject gameObject)
        {
            Type[] componentTypes = gameObject.Components;

            for (int i = 0; i < componentTypes.Length; i++)
            {
                if (_typeToQueryLookup.ContainsKey(componentTypes[i]) == false)
                {
                    continue;
                }

                Query[] queriesByType = _typeToQueryLookup[componentTypes[i]];
                
                for (int j = 0; j < queriesByType.Length; j++)
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