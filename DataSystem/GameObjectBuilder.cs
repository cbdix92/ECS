using System;
using System.Collections.Generic;
using CMDR.DataSystem;

namespace CMDR
{
    public sealed class GameObjectBuilder
    {

        #region PUBLIC_MEMBERS

        #endregion

        #region PRIVATE_MEMBERS

        private Dictionary<Type, IComponent> _children;

        #endregion

        public GameObjectBuilder()
        {
            _children = new Dictionary<Type, IComponent>(Data.NumberOfComponentTypes);
        }

        #region PUBLIC_METHODS

        public bool ContainsComponent<T>() where T : struct, IComponent<T>
        {
            return ContainsComponent(typeof(T));
        }

        public bool ContainsComponent(Type typeOfComponent)
        {
            if(_children.ContainsKey(typeOfComponent))
            {
                return true;
            }

            return false;
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