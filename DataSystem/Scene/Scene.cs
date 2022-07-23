using System;
using CMDR.DataSystem;


namespace CMDR
{
    [Serializable]
    public sealed class Scene
    {
        #region PUBLIC_MEMBERS

        public static Scene Active { get; private set; }

        public int ID { get; internal set; }

        #endregion

        internal Data Data;


        public Scene()
        {
            Data = new Data();

            if (Active == null)
            {
                Active = this;
            }
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
            gameObjectBuilder.GetComponents(out Type[] types, out IComponent[] components);

            for(int i = 0; i < types.Length - 1; i++)
            {
                components[i].ID = new ID(components[i].ID | id.BaseID);

                Data.StoreComponent(types[i], components[i]);
            }

            // Store GameObject
            Data.StoreGameObject(gameObject);

            // Sort GameObject
            Data.Sort(gameObject);

            // Return Generated ID
            return id;
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