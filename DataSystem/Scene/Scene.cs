using System;
using System.Collections.Generic;
using CMDR.DataSystem;


namespace CMDR
{
    [Serializable]
    public sealed class Scene
    {
        #region PUBLIC_MEMBERS

        public int ID { get; internal set; }
        
        #endregion

        #region PRIVATE_MEMBERS

        #endregion

        #region INTERNAL_MEMBERS

        internal ComponentTable Components;

        internal GameObjectCollection GameObjects;

        #endregion

        #region CONSTRUCTORS

        public Scene()
        {

            Components = new ComponentTable();

            GameObjects = new GameObjectCollection();

            //SceneManager.NewScene(this);

        }

        #endregion

        #region PUBLIC_METHODS

        public void DestroyComponent<T>(int id) where T : struct, IComponent<T>
        {

        }

        public void DestroyGameObject(uint id)
        {
            GameObjects.Remove(id);
        }

        public ID Add(GameObject gameObject)
        {
            
            Data.GenerateGameObjectID(ref gameObject);

            // Generate Component IDs

            // Store Components

            GameObjects.Add(gameObject);

            return default;
        }

        public T Get<T>(uint id) where T : struct, IComponent<T>
        {
            throw new NotImplementedException();
            // Return Component<T> of id
        }

        public GameObject Get(ID id)
        {
            return GameObjects[id];
        }

        public void Pair<T>(T component, ref GameObject gameObject) where T : struct, IComponent<T>
        {
            // Pair Component<T> with GameObject
            // Store Component id and type in GameObject as well as store Component in ComponentTable 
        }

        public ID Populate(GameObject gameObject)
        {
            // Assign ID to GameObject and each of it's components.

            // Store in DataSystem

            // Query sort

            // Return GameObject new ID

            throw new NotImplementedException();
        }

        #endregion

        #region PRIVATE_METHODS

        #endregion

        #region INTERNAL_METHODS

        #endregion

    }
}