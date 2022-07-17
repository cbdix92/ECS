using System;
using System.Collections.Generic;

namespace CMDR.DataSystem
{
    internal sealed class Queryable
    {
        
        #region PUBLIC_MEMBERS

        public Dictionary<Query, IComponentCollection<IComponent>> Queries { get; private set; }

        public Dictionary<Query, int[]> Queries { get; private set; }

        #endregion

        #region PRIVATE_MEMBERS

        private Dictionary<Type, Query[]> _typeToQueryLookup;

        #endregion

        public Queryable()
        {
            Queries = new Dictionary<Query, IComponentCollection<IComponent>>();

            Queries = new Dictionary<Query, int[]>();

            _typeToQueryLookup = new Dictionary<Type, Query[]>();
        }

        #region PUBLIC_METHODS

        public Query Register<T>(Filter filter) where T : struct, IComponent<T>
        {
            Type type = typeof(T);

            Query query = new Query(type, filter);

            if(Queries.ContainsKey(query) == false)
            {
                Type TNew = typeof(ComponentCollection<>).MakeGenericType(typeof(T));

                Queries.Add(query, Activator.CreateInstance(TNew) as IComponentCollection);

                if(_typeToQueryLookup.ContainsKey(type))
                {
                    Query[] array = _typeToQueryLookup[type];
                    Array.Resize<Query>(ref array, _typeToQueryLookup[type].Length + 1);
                }

                _typeToQueryLookup[type][_typeToQueryLookup[type].Length - 1] = query;
            }

            return query;
        }

        public QueryList GetQuery<T>(Query query, Span<T> components)
        {

            /// If spans cannot be sliced by other spans then this will have to be created in the ComponentCollection if passing the array causes allocations.
            /// The Indices will have to be given to the ComponentCollection to build the QueryList.

            Span<int> indices = Queries[query];

            // TODO ...
            // This could be stored elsewhere and purged before being rebuit. This would eliminate unnecessary allocations.
            QueryList result = new QueryList();

            for(int i = 0; i < indices.Length; )
            {
                if(indices[i+1] == -1)
                {
                    // Store a slice
                    result.Add(new Span<T>(components, indices[i] indices[i+2]));
                    
                    i +=2
                }
                else
                {
                    // Store a single
                    result.Add(new Span<T>(components, indices[i], indices[i]));

                    i++;
                }
            }
        }

        /// <summary>
        /// Remove all occurrences within the query system. 
        /// </summary>
        /// <param name="id"> The ID of the object that is being removed. </param>
        public void Remove(ID id)
        {
            foreach(Query query in Queries.Keys)
            {
                if(Queries[query].Contains(id))
                {
                    Queries[query].Remove(id);
                }
            }
        }

        /// <summary>
        /// Sort a GameObject into the query system.
        /// </summary>
        /// <param name="gameObject"> The GameObject that is to be sorted. </param>
        public void Sort(GameObject gameObject)
        {
            foreach(Query query in Queries.Keys)
            {
                if (query.Sort(gameObject))
                {
                    Queries[query].Add(Data.Components[query.Type].Get(gameObject.ID));
                }
            }
        }

        #endregion

    }
}