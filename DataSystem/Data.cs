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

        public static byte NumberOfComponentTypes { get; private set; }

        public static readonly ulong MetaDataMask = 0xffffffff00000000;

        public static readonly uint IDMask = 0xffffffff;

        public static Dictionary<ID, GameObject> GameObjects { get; private set; }

        #endregion

        #region  INTERNAL_MEMBERS

        internal static Dictionary<Type, IComponentCollection<IComponent>> Components { get; private set; }

        internal static Dictionary<Query, IComponentCollection<IComponent>> Queries { get; private set; }

        #endregion

        #region PRIVATE_MEMBERS

        private readonly static IEnumerable _types;

        private static IDProvider _idProvider;

        #endregion

        #region PUBLIC_METHODS

        public static Data()
        {
            _types = Assembly.GetExecutingAssembly().GetTypes().Where(T => T.GetInterfaces().Contains(typeof(IComponent)));

            GenerateComponentStorage();

            _idProvider = new IDProvier();
        }

        #endregion

        #region INTERNAL_METHODS
        
        internal static void GenerateComponentStorage()
        {

            Components = new Dictionary<Type, IComponentCollection<IComponent>>();

            Queries = new Dictionary<Query, IComponentCollection<IComponent>>();

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

        internal static Query RegisterQuery<T>(Filter filter) where T : struct, IComponent
        {
            Query query = new Query(typeof(T), filter);

            if (Queries.ContainsKey(query) == false)
            {
                var TNew = typeof(ComponentCollection<>).MakeGenericType(typeof(T));

                Queries.Add(query, Activator.CreateInstance(TNew) as IComponentCollection);
            }

            return query;
        }

        internal static T GetComponent<T>(ID id) where T : struct, IComponent
        {
            return (T)Components[typeof(T)].Get(id);
        }

        #endregion

        #region PRIVATE_METHODS



        #endregion
    }
}