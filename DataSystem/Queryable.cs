using System.Generic.Collections;

namespace CMDR.DataSystem
{
    internal sealed class Queryable
    {
        
        #region PUBLIC_MEMBERS

        public Dictionary<Query, IComponentCollection<IComponent>> Queries { get; private set; }

        #endregion

        #region PRIVATE_MEMBERS

        public Queryable()
        {
            Queries = new Dictionary<Query, IComponentCollection<IComponent>>();
        }

        #endregion

        #region PUBLIC_METHODS

        public Query Register<T>(Filter filter) where T : struct, IComponent<T>
        {
            Query query = new Query(typeof(T), filter);

            if(Queries.ContainsKey(query) == false)
            {
                var TNew = typeof(ComponentCollection<>).MakeGenericType(typeof(T));

                Queries.Add(query, Activator.CreateInstance(TNew) as IComponentCollection);
            }

            return query;
        }

        public void Sort(GameObject gameObject)
        {
            foreach(Query query in Queries.Keys)
            {
                query.Sort(gameObject) ? Queries[query].Add(Data.Components[query.Type].Get(gameObject.ID)) : continue;
            }
        }

        #endregion

    }
}