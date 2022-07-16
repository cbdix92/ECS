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


        public Scene()
        {
            SceneManager.LoadScene(this);
        }

        #region PUBLIC_METHODS

        public void DestroyGameObject(ID id)
        {
            throw new NotImplementedException();
        }

        public ID Populate(GameObjectBuilder gameObjectBuilder)
        {
            // Generate ID

            // Generate GameObject

            // Store Components
            // Store GameObject

            // Sort GameObject


            // Return Generated ID
            throw new NotImplementedException();
        }

        public T Get<T>(ID id) where T : struct, IComponent<T>
        {
            throw new NotImplementedException();
        }

        public GameObject Get(ID id)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}