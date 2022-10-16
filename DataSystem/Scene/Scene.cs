using System;
using CMDR.DataSystem;


namespace CMDR
{
    [Serializable]
    public sealed class Scene : Data
    {
        #region PUBLIC_MEMBERS

        public static Scene Active { get; private set; }

        #endregion

        public Scene() : base()
        {
            if (Active == null)
            {
                Active = this;
            }
        }

        #region PUBLIC_METHODS

        public new void DestroyGameObject(ref ID id)
        {
            base.DestroyGameObject(ref id);
        }

        public new bool GetComponent<T>(ID id, out T component) where T : struct, IComponent<T>
        {
            return base.GetComponent(id, out component);
        }

        public new bool GetGameObject(ID id, out GameObject gameObject)
        {
            return base.GetGameObject(id, out gameObject);
        }
        
        public ID Populate(GameObjectBuilder gameObjectBuilder)
        {
            // Generate ID
            ID id = base.GenerateNewID();

            // Generate GameObject
            GameObject gameObject = new GameObject(this, id, gameObjectBuilder.Types);

            // Store Components
            gameObjectBuilder.GetComponents(out Type[] types, out IComponent[] components);

            for(int i = 0; i < types.Length; i++)
            {
                dynamic comp = components[i];

                ID compID = comp.ID;

                compID.InlayID(id.Id);

                comp.ID = compID;

                StoreComponent(comp);
            }

            // Store GameObject
            base.StoreGameObject(gameObject);

            // Sort GameObject
            base.Sort(gameObject);

            // Return Generated ID
            return id;
        }

        #endregion

    }
}