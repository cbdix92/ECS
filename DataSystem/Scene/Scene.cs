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

        public void DestroyGameObject(ref ID id)
        {
            Data.DestroyGameObject(ref id);
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