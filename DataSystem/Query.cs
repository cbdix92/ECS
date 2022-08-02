using System;

namespace CMDR
{

    public struct Query
    {
        internal readonly Type Type;

        internal readonly Filter Filter;

        public Query(Type type, Filter filter) => (Type, Filter) = (type, filter);

        /// <summary>
        /// Checks if a GameObject meets the criteria for this Query.
        /// </summary>
        /// <param name="gameObejct"> The GameObject that is to be checked. </param>
        /// <returns> Returns True if the GameObject meets the criteria for this Query. Otherwise Returns False. </returns>
        internal bool Sort(GameObject gameObject)
        {
            return gameObject.ContainsComponent(Type) && Filter(gameObject);
        }
    }
}
