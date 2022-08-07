using System;

namespace CMDR.Systems
{
    internal sealed class Physics : SpatialIndexer, ISystem
    {
        #region PRIVATE_MEMBERS

        private Query _queryTransform;

        private Query _queryCollider;

        #endregion

        public Physics()
        {
            _queryTransform = Scene.Active.RegisterQuery<Transform>(Filter);

            _queryCollider = Scene.Active.RegisterQuery<Transform>(Filter);
        }

        #region PUBLIC_METHODS

        public void Update(long ticks)
        {
            bool cameraMoved = MoveCamera(ticks);

            Scene scene = Scene.Active;

            GameObject[] gameObjects = scene.GameObjects.Get();

            Transform[] transforms = scene.Components.Get<Transform>();

            Collider[] colliders = scene.Components.Get<Collider>();

            // Update all transforms
            foreach (GameObject gameObject in gameObjects)
            {
                int transformID = gameObject.Get<Transform>();
                
                int colliderID = gameObject.Get<Collider>();

                #region COLLISION_LOGIC

                int result = Convert.ToByte(transformID != -1) | (Convert.ToByte(colliderID != -1) << 1) | (Convert.ToByte(Move(transforms[transformID])) << 2);

                switch (result)
                {
                    case 7:

                        Transform transform = transforms[gameObject.Get<Transform>()];

                        Collider collider = colliders[gameObject.Get<Collider>()];

                        CalcGridPos(ref collider, transform);
                        
                        int[] gameObjectColliders = GetNearbyColliders(collider);

                        foreach (int i in gameObjectColliders)
                        {
                            Transform transform2 = transforms[gameObjects[i].Get<Transform>()];

                            Collider collider2 = colliders[gameObjects[i].Get<Collider>()];
                            
                            // Bounding box check
                            bool rectCol = 
                                   transform.X <= transform2.X + collider2.ScaleX
                                && transform.X + collider.ScaleX >= transform2.X
                                && transform.Y <= transform2.Y + collider2.ScaleY
                                && transform.Y + collider.ScaleY >= transform2.Y
                                && transform.Z <= transform2.Z + collider2.ScaleZ
                                && transform.Z + collider.ScaleZ >= transform2.Z;


                            // Compare bounding box checks and then bit collider check
                            if (rectCol && BitCollider.BitColliderCheck(transform, transform2, collider, collider2))
                            {
                                continue; // Resolve collision here ...
                            }
                        }

                        continue;

                    case 3:
                        // Nothing needs to be done. Restart the loop
                        if (colliders[colliderID].GridKeys == null )
                        {
                            CalcGridPos(ref colliders[colliderID], transforms[transformID]);
                        }
                        continue;
                    
                    default:
                        continue;
                }
                #endregion

            }
        }

        #endregion

        #region PRIVATE_METHODS

        private bool Move(Transform transform)
        {
            transform.X += transform.XVel;

            transform.Y += transform.YVel;

            return transform.XVel != 0 || transform.YVel != 0;
        }

        private bool MoveCamera(long ticks)
        {
            Camera.MoveCamera(Camera.Xvel);

            Camera.StrafeCamera(Camera.Zvel);
            
            Camera.Y += Camera.Yvel;
            
            return Camera.Xvel + Camera.Yvel + Camera.Zvel != 0;
        }

        private bool CanMove(Transform transform)
        {
            return transform.XVel != 0 || transform.YVel != 0;
        }

        private bool Filter(GameObject gameObject)
        {
            if (gameObject.ContainsComponent<Transform>() && gameObject.ContainsComponent<Collider>())
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
