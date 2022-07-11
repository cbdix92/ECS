using System;
using System.Collections.Generic;

namespace CMDR.DataSystem
{
    public struct GameObject
    {
        #region PUBLIC_MEMBERS

        public readonly Scene Scene;

        public ID ID { get; internal set; }

        public int ComponentCount { get; internal set; }

        public Type[] Components { get; internal set; }

        #endregion

        #region PRIVATE_MEMBERS

        #endregion

        #region INTERNAL_MEMBERS

        internal static GameObject Default = new GameObject(null, 0, new Type[0]);

        #endregion

        #region CONTRUCTOR

        public GameObject(Scene scene, uint id, Type[] components)
        {
            Scene = scene;
            ID = id;
            ComponentCount = components.Length;
            Components = components;
        }

        #endregion

        #region PUBLIC_METHODS

        public bool Contains<T>()
        {
            return Contains(typeof(T));
        }

        public bool Contains(Type type)
        {
            foreach (Type t in Components)
                if (t == type)
                    return true;
            return false;
        }

        public T Get<T>() where T : struct, IComponent<T>
        {
            throw new NotImplementedException();
        }

        public void Use<T>(T component)
        {

        }

        #endregion

        #region  PRIVATE_METHODS

        #endregion
    }
}