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

        public Type[] Types
        {
            get
            {
                if (_types != null)
                {
                    return _types;
                }

                _types = new Type[_children.Keys.Count];
                
                _children.Keys.CopyTo(_types, 0);
                
                return _types;
            }
        }

        public IComponent[] Components
        {
            get
            {
                if (_components != null)
                {
                    return _components;
                }

                _components = new IComponent[_children.Values.Count];

                _children.Values.CopyTo(_components, 0);

                return _components;
            }
        }

        #endregion

        #region PRIVATE_MEMBERS

        private readonly Dictionary<Type, IComponent> _children;

        private Type[] _types;

        private IComponent[] _components;

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

        /// <summary>
        /// Bind a Component to this GameObjectBuilder.
        /// </summary>
        /// <typeparam name="T"> Type of IComponent that will be bound. </typeparam>
        /// <param name="component"> The Component that will be bound to this GameObjectBuilder. </param>
        public void Bind<T>(T component) where T : struct, IComponent<T>
        {
            // Reset _types annd _components so it can be rebuilt by the ComponentTypes and Components properties.
            _types = null;
            _components = null;

            Type tComp = typeof(T);

            if(_children.ContainsKey(tComp))
            {
                _children[tComp] = component;
                return;
            }

            _children.Add(tComp, component);
        }

        /// <summary>
        /// Unbinds all current components from this ComponentBuilder.
        /// </summary>
        public void UnbindAll()
        {
            // Reset _types and _components so it can be rebuilt by the ComponentTypes and Components properties.
            _types = null;
            _components = null;

            _children.Clear();
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
            // Reset _types annd _components so it can be rebuilt by the ComponentTypes and Components properties.
            _types = null;
            _components = null;

            if (_children.ContainsKey(tComp))
            {
                _children.Remove(tComp);
            }
        }

        #endregion

        public void GetComponents(out Type[] types, out IComponent[] components)
        {
            types = Types;

            components = Components;
        }

    }
}