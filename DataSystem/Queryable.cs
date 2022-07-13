using System;
using System.Collections.Generic;

namespace CMDR.DataSystem
{
    internal sealed class Queryable
    {
        
        #region PUBLIC_MEMBERS

        public Dictionary<Query, IComponentCollection<IComponent>> Queries { get; private set; }

        #endregion

        #region PRIVATE_MEMBERS

        private Dictionary<Type, Query[]> _typeToQueryLookup;

        #endregion

        public Queryable()
        {
            Queries = new Dictionary<Query, IComponentCollection<IComponent>>();

            _typeToQueryLookup = new Dictionary<Type, Query[]>();
        }

        #region PUBLIC_METHODS

        public Query Register<T>(Filter filter) where T : struct, IComponent<T>
        {
            Query query = new Query(typeof(T), filter);

            if(Queries.ContainsKey(query) == false)
            {
                var TNew = typeof(ComponentCollection<>).MakeGenericType(typeof(T));

                Queries.Add(query, Activator.CreateInstance(TNew) as IComponentCollection);

                if(_typeToQueryLookup.ContainsKey(query.Type))
                {
                    Array.Resize<Query>(ref _typeToQueryLookup[query], _typeToQueryLookup[query].Length + 1);
                }

                _typeToQueryLookup[query][_typeToQueryLookup[query].Length - 1] = query;
            }

            return query;
        }

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

        public void Update<T>(T component)
        {
            foreach(Query query in _typeToQueryLookup[component.Type])
            {
                Queries[query].Update(component);
            }
        }

        #endregion

    }
}