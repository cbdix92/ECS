using System;
using System.Collections.Generic;

namespace CMDR.DataSystem
{
    internal sealed class Queryable
    {
        
        #region PUBLIC_MEMBERS

        public Dictionary<Query, IQueryBuilder<IComponent>> Queries { get; private set; }

        #endregion

        #region PRIVATE_MEMBERS

        private Dictionary<Type, Query[]> _typeToQueryLookup;

        #endregion

        public Queryable()
        {
            Queries = new Dictionary<Query, IQueryBuilder<IComponent>>();

            _typeToQueryLookup = new Dictionary<Type, Query[]>();
        }

        #region PUBLIC_METHODS

        public Query Register<T>(Filter filter) where T : struct, IComponent<T>
        {
            Type type = typeof(T);

            Query query = new Query(type, filter);

            if(Queries.ContainsKey(query) == false)
            {
                Type TNew = typeof(QueryBuilder<>).MakeGenericType(typeof(T));

                object[] args = new object[] { (object)Data.GetComponentsCollectionReference(type) };

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