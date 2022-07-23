using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CMDR.DataSystem
{
    internal sealed class Data : Queryable
    {
        #region PUBLIC_MEMBERS
        
        public static readonly int StorageScale = byte.MaxValue;

        public static int NumberOfComponentTypes { get => _types.Length - 1; }

        #endregion

        #region INTERNAL_MEMBERS

        internal int GetMaxIDBitPosition => _idProvider.GetMaxIDBitPosition();

        #endregion

        #region PRIVATE_MEMBERS

        private Dictionary<ID, GameObject> _gameObjects;

        private Dictionary<Type, IComponentCollection<IComponent>> _components;

        private static Type[] _types;

        private readonly IDProvider _idProvider;

        #endregion

        public Data() : base()
        {
            _types = Assembly.GetExecutingAssembly().GetTypes().Where(T => T.GetInterfaces().Contains(typeof(IComponent))).ToArray();

            _gameObjects = new Dictionary<ID, GameObject>(StorageScale);

            _components = new Dictionary<Type, IComponentCollection<IComponent>>(_types.Length - 1);

#pragma warning disable
            base._componentsQueryRef = _components;
#pragma warning enable

            _idProvider = new IDProvider();

            Type TComponentCollection = typeof(ComponentCollection<>);

            foreach (Type TComponent in _types)
            {
                if (TComponent.Name == typeof(IComponent<>).Name)
                    continue;

                Type TNew = TComponentCollection.MakeGenericType(TComponent);

                object[] args = new object[] { StorageScale, Marshal.SizeOf(TComponent) };

                _components[TComponent] = Activator.CreateInstance(TNew, args) as IComponentCollection;
            }

        }

        #region PUBLIC_METHODS

        public ID GenerateNewID()
        {
            return _idProvider.GenerateID();
        }

        public void StoreComponent(Type type, IComponent component) => _components[type].Add(component);

        public void StoreGameObject(GameObject gameObject) => _gameObjects.Add(gameObject.ID, gameObject);

        public bool DestroyGameObject(ref ID id)
        {
            if(_gameObjects.ContainsKey(id) == false)
            {
                return false;
            }

            GameObject gameObject = _gameObjects[id];

            // Remove all components
            foreach(Type t in gameObject.Components)
            {
                _components[t].Remove(id);
            }

            // Remove GameObject
            _gameObjects.Remove(id);
            
            // Make ID unusable
            id.Retire();

            return true;
        }

        public bool GetGameObject(ID id, out GameObject gameObject)
        {
            if(_gameObjects.ContainsKey(id))
            {
                gameObject = _gameObjects[id];

                return true;
            }
            
            gameObject = default;
            
            return false;
        }
        
        public bool GetComponent<T>(ID id, out T component) where T : struct, IComponent<T>
        {
            Type tComp = typeof(T);

            if(_components[tComp].Contains(id))
            {
                component = (T)_components[tComp].Get(id);

                return true;
            }

            component = default;

            return false;
        }

        /// <summary>
        /// Update a GameObject within the data storage.
        /// </summary>
        /// <param name="gameObject"> The GameObject that is to be updated. </param>
        /// <returns> Return True if the GameObject was updated, otherwise returns False. </returns>
        public bool Update(GameObject gameObject)
        {
            if(_gameObjects.ContainsKey(gameObject.ID))
            {
                _gameObjects[gameObject.ID] = gameObject;
                
                return true;
            }

            return false;
        }

        /// <summary>
        /// Update a Component within the data storage.
        /// </summary>
        /// <typeparam name="T"> Type of IComponent. </typeparam>
        /// <param name="component"> The Component that is to be updated. </param>
        /// <returns> Returns True if the Component was updated, otherwise returns False. </returns>
        public bool Update<T>(T component) where T : struct, IComponent<T>
        {
            Type tComp = typeof(T);

            if(_components[tComp].Contains(component.ID))
            {
                _components[tComp].Update(component);
                
                return true;
            }

            return false;
        }

        #endregion
    }
}