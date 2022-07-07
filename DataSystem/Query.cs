using System;

namespace CMDR.DataSystem
{
    internal struct Query
    {
        internal Type Type;

        internal Filter Filter;

        internal bool Sort(GameObject gameObject)
        {
            return GameObject.Contains(Type) && Filter(gameObject);
        }

        public Query(Type type, Filter filter) => (Type, Filter) = (type, filter);
    }

    public delegate bool Filter(GameObject);
}
