using System.Generic.Collections;
using CMDR.DataSystem;

namespace CMDR
{
    public sealed class GameObjectBuilder
    {

        #region PUBLIC_MEMBERS

        int NumberOfComponents { get; private set; }

        #endregion

        #region PRIVATE_MEMBERS

        private Dictionary<Type, IComponent> _children;

        #endregion

        public GameObjectBuilder()
        {
            _children = new Dictionary<Type, IComponent>(Data.NumberOfComponents);
        }

        #region PUBLIC_METHODS

        public bool ContainsComponent(IComponent component)
        {

        }

        public void Use<T>(T component) where T : struct, IComponent<T>
        {
            
        }

        public void Remove(IComponent component)
        {

        }

        public void RemoveAll()
        {

        }

        #endregion

    }
}