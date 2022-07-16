using System;
using System.Collections.Generic;

namespace CMDR.DataSystem
{
    public struct GameObject
    {
        #region PUBLIC_MEMBERS

        public readonly Scene Scene;

        public ID ID;

        public readonly int ComponentCount;

        public readonly Type[] Components;

        #endregion

        public GameObject(Scene scene, ID id, Type[] components)
        {
            Scene = scene;
            ID = id;
            ComponentCount = components.Length;
            Components = components;
        }

        #region PUBLIC_METHODS

        public bool ContainsComponent<T>() => ContainsComponent(typeof(T));

        public bool ContainsComponent(Type type)
        {
            foreach (Type t in Components)
                if (t == type)
                    return true;
            return false;
        }

        #endregion

        #region  PRIVATE_METHODS

        #endregion
    }
}