using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace CMDR.DataSystem
{
    public static class Data
    {
        #region  PUBLIC_MEMBERS
        
        public static readonly int StorageScale = byte.MaxValue;

        public static int NumberOfComponentTypes { get; private set; }

        public static readonly ulong MetaDataMask = 0xffffffff00000000;

        public static readonly uint IDMask = 0xffffffff;

        public static Dictionary<ID, GameObject> GameObjects { get; private set; }

        #endregion

        #region  INTERNAL_MEMBERS

        internal static Dictionary<Type, IComponentCollection<IComponent>> Components { get; private set; }

        internal static Queryable Queries = new Queryable();

        internal static int GetMaxIDBitPosition => _idProvider.GetMaxIDBitPosition();

        #endregion

        #region PRIVATE_MEMBERS

        private readonly static IEnumerable _types;

        private static IDProvider _idProvider;

        #endregion

        #region PUBLIC_METHODS

        static Data()
        {
            _types = Assembly.GetExecutingAssembly().GetTypes().Where(T => T.GetInterfaces().Contains(typeof(IComponent)));

            GenerateComponentStorage();

            _idProvider = new IDProvider();
        }

        public static bool DestroyGameObject(ref ID id)
        {
            if(GameObjects.ContainsKey(id) == false)
            {
                return false;
            }

            GameObject gameObject = GameObjects[id];

            // Remove all components
            foreach(Type t in gameObject.Components)
            {
                Components[t].Remove(id);
            }
            
            // Remove Query entries
            Queries.Remove(id);

            // Remove GameObject
            GameObjects.Remove(id);
            
            // Make ID unusable
            id.Retire();

            return true;
        }

        public static bool GetComponent<T>(ID id, out T component) where T : struct, IComponent<T>
        {
            if(Components[typeof(T)].Contains(id))
            {
                component = (T)Components[typeof(T)].Get(id);

                return true;
            }

            component = default;

            return false;
        }

        public static bool GetGameObject(ID id, out GameObject gameObject)
        {
            if(GameObjects.ContainsKey(id))
            {
                gameObject = GameObjects[id];

                return true;
            }
            
            gameObject = default;
            
            return false;
        }

        public static bool Update(GameObject gameObject)
        {
            if(GameObjects.ContainsKey(gameObject.ID))
            {
                GameObjects[gameObject.ID] = gameObject;
                
                return true;
            }

            return false;
        }

        public static bool Update<T>(T component) where T : struct, IComponent<T>
        {
            if(Components[typeof(T)].Contains(component.ID))
            {
                Components[typeof(T)].Update(component);
                
                return true;
            }

            return false;
        }

        #endregion

        #region INTERNAL_METHODS
        
        internal static void GenerateComponentStorage()
        {

            Components = new Dictionary<Type, IComponentCollection<IComponent>>();

            Type TComponentCollection = typeof(ComponentCollection<>);

            foreach(Type TComponent in _types)
            {
                if(TComponent.Name == typeof(IComponent<>).Name)
                    continue;

                var TNew = TComponentCollection.MakeGenericType(TComponent);

                Components[TComponent] = Activator.CreateInstance(TNew) as IComponentCollection;

                NumberOfComponentTypes++;
            }
        }

        #endregion

        #region PRIVATE_METHODS



        #endregion
    }
}