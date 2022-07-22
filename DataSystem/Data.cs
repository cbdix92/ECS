using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CMDR.DataSystem
{
    public sealed class Data : Queryable, IDProvider
    {
        #region PUBLIC_MEMBERS
        
        public static readonly int StorageScale = byte.MaxValue;

        public int NumberOfComponentTypes { get => _types.Length - 1; }

        public readonly ulong MetaDataMask = 0xffffffff00000000;

        public readonly uint IDMask = 0xffffffff;

        #endregion

        #region INTERNAL_MEMBERS

        internal static int GetMaxIDBitPosition => _idProvider.GetMaxIDBitPosition();

        #endregion

        #region PRIVATE_MEMBERS

        private static Dictionary<ID, GameObject> _gameObjects;

        private static Dictionary<Type, IComponentCollection<IComponent>> _components;

        private readonly static Queryable _queries = new Queryable();

        private readonly static Type[] _types;

        private readonly static IDProvider _idProvider;

        #endregion

        static Data()
        {
            _types = Assembly.GetExecutingAssembly().GetTypes().Where(T => T.GetInterfaces().Contains(typeof(IComponent))).ToArray();

            _gameObjects = new Dictionary<ID, GameObject>(StorageScale);

            _components = new Dictionary<Type, IComponentCollection<IComponent>>(_types.Length - 1);

            Type TComponentCollection = typeof(ComponentCollection<>);

            foreach (Type TComponent in _types)
            {
                if (TComponent.Name == typeof(IComponent<>).Name)
                    continue;

                Type TNew = TComponentCollection.MakeGenericType(TComponent);

                object[] args = new object[] { StorageScale, Marshal.SizeOf(TComponent) };

                _components[TComponent] = Activator.CreateInstance(TNew, args) as IComponentCollection;
            }

            _idProvider = new IDProvider();
        }

        #region PUBLIC_METHODS

        public ID GenerateNewID()
        {
            return _idProvider.GenerateID();
        }

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

        public Query RegisterQuery<T>(Filter filter) where T : struct, IComponent<T>
        {
            return _queries.Register<T>(filter);
        }

        public bool GetQuery(Query query, out Span<IComponent> components)
        {
            return _queries.GetQuery(query, out components);
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

        #region INTERNAL_METHODS

        internal IComponentCollection<IComponent> GetComponentsCollectionReference(Type type)
        {
            return _components[type];
        }

        #endregion
    }
}