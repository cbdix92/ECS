using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace CMDR.DataSystem
{
    public static class Data
    {
        #region PUBLIC_MEMBERS
        
        public static readonly int StorageScale = byte.MaxValue;

        public static int NumberOfComponentTypes { get; private set; }

        public static readonly ulong MetaDataMask = 0xffffffff00000000;

        public static readonly uint IDMask = 0xffffffff;

        #endregion

        #region INTERNAL_MEMBERS

        internal static int GetMaxIDBitPosition => _idProvider.GetMaxIDBitPosition();

        #endregion

        #region PRIVATE_MEMBERS

        private static Dictionary<ID, GameObject> _gameObjects;

        private static Dictionary<Type, IComponentCollection<IComponent>> _components;

        private static Queryable _queries = new Queryable();

        private readonly static IEnumerable _types;

        private static IDProvider _idProvider;

        #endregion

        static Data()
        {
            _types = Assembly.GetExecutingAssembly().GetTypes().Where(T => T.GetInterfaces().Contains(typeof(IComponent)));

            GenerateComponentStorage();

            _idProvider = new IDProvider();
        }

        #region PUBLIC_METHODS

        public static bool DestroyGameObject(ref ID id)
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
            
            // Remove Query entries
            Queries.Remove(id);

            // Remove GameObject
            _gameObjects.Remove(id);
            
            // Make ID unusable
            id.Retire();

            return true;
        }

        public static bool GetGameObject(ID id, out GameObject gameObject)
        {
            if(_gameObjects.ContainsKey(id))
            {
                gameObject = _gameObjects[id];

                return true;
            }
            
            gameObject = default;
            
            return false;
        }
        
        public static bool GetComponent<T>(ID id, out T component) where T : struct, IComponent<T>
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

        public static Query RegisterQuery<T>(Filter filter) where T : struct, IComponent<T>
        {
            return _queries.Register<T>(filter);
        }

        public static QueryList<T> GetQuery<T>(Query query) where T : struct, IComponent<T>
        {
            return _queries.GetQueryList<T>(query, _components[query.Type].ToArray());
        }

        /// <summary>
        /// Update a GameObject within the data storage.
        /// </summary>
        /// <param name="gameObject"> The GameObject that is to be updated. </param>
        /// <returns> Return True if the GameObject was updated, otherwise returns False. </returns>
        public static bool Update(GameObject gameObject)
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
        public static bool Update<T>(T component) where T : struct, IComponent<T>
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

        #region INTERNAL_METHODS
        
        internal static void GenerateComponentStorage()
        {
            _gameObjects = new Dictionary<ID, GameObject>(StorageScale);

            _components = new Dictionary<Type, IComponentCollection<IComponent>>(_types.Length - 1);

            Type TComponentCollection = typeof(ComponentCollection<>);

            foreach(Type TComponent in _types)
            {
                if(TComponent.Name == typeof(IComponent<>).Name)
                    continue;

                Type TNew = TComponentCollection.MakeGenericType(TComponent);

                _components[TComponent] = Activator.CreateInstance(TNew) as IComponentCollection;

                NumberOfComponentTypes++;
            }
        }

        #endregion
    }
}