using System;
using System.Collections.Generic;
using CMDR.DataSystem;

namespace CMDR
{
    public sealed class GameObjectBuilder
    {

        #region PUBLIC_MEMBERS

        public int NumberOfComponents
        {
            get
            {
                if (_children != null)
                    return _children.Count;
                
                return 0;
            }
        }

        public Type[] ComponentTypes { get => _componentTypes; }

        #endregion

        #region PRIVATE_MEMBERS

        private Dictionary<Type, IComponent> _children;

        private IComponent[] _components;

        private Type[] _componentTypes;

        #endregion

        public GameObjectBuilder()
        {
            _children = new Dictionary<Type, IComponent>(Data.NumberOfComponentTypes);
            _components = new IComponent[1];
            _componentTypes = new Type[1];
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

        /// <summary>
        /// Bind a Component to this GameObjectBuilder.
        /// </summary>
        /// <typeparam name="T"> Type of IComponent that will be bound. </typeparam>
        /// <param name="component"> The Component that will be bound to this GameObjectBuilder. </param>
        public void Bind<T>(T component) where T : struct, IComponent<T>
        {
            Type tComp = typeof(T);

            if(_children.ContainsKey(tComp))
            {
                _children[tComp] = component;
                return;
            }

            Array.Resize(ref _componentTypes, _componentTypes.Length + 1);

            _componentTypes[^1] = tComp;

            _children.Add(tComp, component);
        }

        /// <summary>
        /// Unbind a Component from this GameObjectBuilder.
        /// </summary>
        /// <typeparam name="T"> Type of IComponent that will be unbound. </typeparam>
        public void UnBind<T>() where T : struct, IComponent<T>
        {
            Type tComp = typeof(T);

            UnBind(tComp);
        }

        public void UnBind(Type tComp)
        {
            if (_children.ContainsKey(tComp))
            {
                _children.Remove(tComp);
            }
        }

        public void RemoveAll()
        {
            _children.Clear();
        }

        #endregion

        internal void GetComponents(out Type[] types, out IComponent[] components)
        {
            types = _componentTypes;

            components = _components;
        }

    }
}