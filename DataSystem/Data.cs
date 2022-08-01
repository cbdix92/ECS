using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;

namespace CMDR.DataSystem
{
    public class Data : Queryable
    {
        #region PUBLIC_MEMBERS
        
        public static readonly int StorageScale = byte.MaxValue;

        public static int NumberOfComponentTypes { get => Types.Length - 1; }

        public static Type[] Types
        {
            get
            {
                if(_types == null)
                {
                    Type[] tComps = Assembly.GetExecutingAssembly().GetTypes()
                        .Where(T => T.GetInterfaces().Contains(typeof(IComponent)) 
                        && T.Name != typeof(IComponent<>).Name).ToArray();
                }

                return _types;
            }
        }

        #endregion

        #region INTERNAL_MEMBERS

        internal int GetMaxIDBitPosition => _idProvider.GetMaxIDBitPosition();

        #endregion

        #region PRIVATE_MEMBERS

        private readonly Dictionary<ID, GameObject> _gameObjects;

        private readonly Dictionary<Type, ComponentCollection> _components;

        private static Type[] _types;

        private readonly IDProvider _idProvider;

        #endregion

        public Data() : base()
        {

            _gameObjects = new Dictionary<ID, GameObject>(StorageScale);

            _components = new Dictionary<Type, ComponentCollection>(Types.Length - 1);

#pragma warning disable
            base._componentsQueryRef = _components;
#pragma warning enable

            _idProvider = new IDProvider();

            foreach (Type TComponent in Types)
            {
                if (TComponent.Name == typeof(IComponent<>).Name)
                    continue;

                _components.Add(TComponent, new ComponentCollection(TComponent));
            }

        }

        #region PUBLIC_METHODS

        public override Query RegisterQuery<T>(Filter filter)
        {
            Query query = base.RegisterQuery<T>(filter);

            // Subscribe QueryList to Component Collection events.
            Type tComp = typeof(T);
            _components[tComp].ComponentDestroyedEvent += Queries[query].OnComponentDestroyed;
            _components[tComp].ComponentMovedEvent += Queries[query].OnComponentMoved;

            return query;
        }

        #endregion

        #region PRIVATE_METHODS
        
        protected private bool DestroyGameObject(ref ID id)
        {
            if(_gameObjects.ContainsKey(id) == false)
            {
                return false;
            }

            GameObject gameObject = _gameObjects[id];

            // Remove all components
            foreach(Type t in gameObject.Components)
            {
                //_components[t].Remove(id);
            }

            // Remove GameObject
            _gameObjects.Remove(id);
            
            // Make ID unusable
            id.Retire();

            return true;
        }

        protected private ID GenerateNewID()
        {
            return _idProvider.GenerateID();
        }
        
        protected private bool GetComponent<T>(ID id, out T component) where T : struct, IComponent<T>
        {
            Type tComp = typeof(T);

            if (_components[tComp].Contains(id) == false)
            {
                component = default;

                return false;
            }

            component = _components[tComp].Get<T>(id);

            return true;
        }

        protected private bool GetGameObject(ID id, out GameObject gameObject)
        {
            if(_gameObjects.ContainsKey(id))
            {
                gameObject = _gameObjects[id];

                return true;
            }
            
            gameObject = default;
            
            return false;
        }

        protected private void StoreComponent<T>(T component) where T : struct, IComponent<T>
        {
            unsafe
            {
                _components[typeof(T)].Add<T>(component);
            }
        }
        
        protected private void StoreGameObject(GameObject gameObject) => _gameObjects.Add(gameObject.ID, gameObject);

        #endregion
    }
}