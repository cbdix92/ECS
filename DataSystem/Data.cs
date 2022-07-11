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

        public static byte ComponentTotal { get; private set; }

        public static readonly ulong MetaDataMask = 0xffffffff00000000;

        public static readonly uint IDMask = 0xffffffff;

        public static Dictionary<uint, GameObject> GameObjects { get; private set; }

        #endregion

        #region  INTERNAL_MEMBERS

        internal static Dictionary<Type, IComponentCollection<IComponent>> Components { get; private set; }

        internal static Dictionary<Query, IComponentCollection<IComponent>> Queries { get; private set; }

        /// <summary>
        /// Provides an unused GameObject ID.
        /// </summary>
        internal static uint NewGameObjectID
        {
            get
            {
                if(_availableGameObjectIDs.Any())
                {
                    return _availableGameObjectIDs.Dequeue();
                }

                try
                {
                    return ++_currentGameObjectID;
                }
                catch(OverflowException)
                {
                    throw new OverflowException("Max number of 4,294,967,295 GameObjects has been reached. . Quite impressive.");
                }
            }
        }

        #endregion

        #region PRIVATE_MEMBERS

        private readonly static IEnumerable _types = Assembly.GetExecutingAssembly().GetTypes().Where(T => T.GetInterfaces().Contains(typeof(IComponent)));

        private static Queue<uint> _availableGameObjectIDs = new Queue<uint>();

        private static uint _currentGameObjectID;

        #endregion

        #region PUBLIC_METHODS

        public static Data()
        {
            
        }

        #endregion

        #region INTERNAL_METHODS

        internal static int GetMaxIDRange()
        {
            int max = 1;

            while(max < _currentGameObjectID)
            {
                max <<= 4;
            }

            return max;
        }
        
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

                ComponentTotal++;
            }
        }

        /// <summary>
        /// Generate a new unused GameObject ID for the provided GameObject. 
        /// Bit 64 (Alive/Dead)
        /// Bit 33 - 63 (Reserved)
        /// Bit 1 - 32 (GameObejct ID)
        /// </summary>
        /// <param name="gameObject"> GameObject to be give a new ID. </param>
        internal static void GenerateGameObjectID(ref GameObject gameObject)
        {
            gameObject.ID = 0;
            gameObject.ID |= 0x8000000000000000 | NewGameObjectID;
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

        internal static T GetComponent<T>(uint id) where T : struct, IComponent
        {
            return (T)Components[typeof(T)].Get(id);
        }

        #endregion

        #region PRIVATE_METHODS



        #endregion
    }
}