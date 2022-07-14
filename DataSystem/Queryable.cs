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
            foreach(Query query in _typeToQueryLookup[typeof(T)])
            {
                Queries[query].Update(component);
            }
        }

        #endregion

    }
}