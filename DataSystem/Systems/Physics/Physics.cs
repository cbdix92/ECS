using System;

namespace CMDR.Systems
{
    internal sealed class Physics : SpatialIndexer, ISystem
    {
        #region PRIVATE_MEMBERS

        private Query _queryTransformCollisionPhase;

        private Query _queryColliderCollisionPhase;

        private Query _queryTransformMoveOnlyPhase;

        #endregion

        public Physics()
        {
            _queryTransformCollisionPhase = Scene.Active.RegisterQuery<Transform>(HasTransformAndCollider);

            _queryColliderCollisionPhase = Scene.Active.RegisterQuery<Collider>(HasTransformAndCollider);

            _queryTransformMoveOnlyPhase = Scene.Active.RegisterQuery<Transform>(HasOnlyTransform);
        }

        #region PUBLIC_METHODS

        public void Update(long ticks)
        {
            bool cameraMoved = MoveCamera(ticks);

            //GameObject[] gameObjects = scene.GameObjects.Get();

            //Transform[] transforms = scene.Components.Get<Transform>();

            //Collider[] colliders = scene.Components.Get<Collider>();

            Span<Transform> transforms;

            Span<Collider> colliders;

            // Move Only Phase
            while (Scene.Active.GetQuery(_queryTransformMoveOnlyPhase, out transforms))
            {
                for (int i = 0; i < transforms.Length; i++)
                {
                    Move(transforms[i]);
                }
            }

            while (Scene.Active.GetQuery(_queryTransformCollisionPhase, out transforms))
            {
                for (int i = 0; i < transforms.Length; i++)
                {
                    if (Move(transforms[i]) && Scene.Active.GetComponent(transforms[i].ID, out Collider collider))
                    {
                        CalcGridPos(ref collider, transforms[i]);
                    }
                }
            }
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
                                   transform.X <= transform2.X + collider2.SizeX
                                && transform.X + collider.SizeX >= transform2.X
                                && transform.Y <= transform2.Y + collider2.SizeY
                                && transform.Y + collider.SizeY >= transform2.Y
                                && transform.Z <= transform2.Z + collider2.SizeZ
                                && transform.Z + collider.SizeZ >= transform2.Z;


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

        private bool HasTransformAndCollider(GameObject gameObject)
        {
            return gameObject.ContainsComponent<Transform>() && gameObject.ContainsComponent<Collider>();
        }

        private bool HasOnlyTransform(GameObject gameObject)
        {
            return gameObject.ContainsComponent<Transform>() && !gameObject.ContainsComponent<Collider>();
        }

        #endregion
    }
}
