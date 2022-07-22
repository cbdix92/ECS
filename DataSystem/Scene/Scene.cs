using System;
using CMDR.DataSystem;


namespace CMDR
{
    [Serializable]
    public sealed class Scene
    {
        #region PUBLIC_MEMBERS

        public int ID { get; internal set; }

        #endregion

        internal Data Data;


        public Scene()
        {
            Data = new Data();

            SceneManager.LoadScene(this);
        }

        #region PUBLIC_METHODS

        public void DestroyGameObject(ref ID id)
        {
            Data.DestroyGameObject(ref id);
        }

        public ID Populate(GameObjectBuilder gameObjectBuilder)
        {
            // Generate ID
            ID id = Data.GenerateNewID();

            // Generate GameObject
            GameObject gameObject = new GameObject(this, id, gameObjectBuilder.ComponentTypes);

            // Store Components

            // Store GameObject

            // Sort GameObject


            // Return Generated ID
            throw new NotImplementedException();
        }

        public bool GetComponent<T>(ID id, out T component) where T : struct, IComponent<T>
        {
            return Data.GetComponent(id, out component);
        }

        public bool GetGameObject(ID id, out GameObject gameObject)
        {
            return Data.GetGameObject(id, out gameObject);
        }

        #endregion

    }
}