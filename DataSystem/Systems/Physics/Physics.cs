using System;

namespace CMDR.Systems
{
    internal sealed class Physics : SpatialIndexer, ISystem
    {
        #region PRIVATE_MEMBERS

        private readonly Scene _scene;

        private readonly Query _queryTransformCollisionPhase;

        private readonly Query _queryTransformMoveOnlyPhase;

        #endregion

        public Physics(Scene scene) : base()
        {
            _scene = scene;

            _queryTransformCollisionPhase = Scene.Active.RegisterQuery<Transform>(HasTransformAndCollider);

            _queryTransformMoveOnlyPhase = Scene.Active.RegisterQuery<Transform>(HasOnlyTransform);
        }

        #region PUBLIC_METHODS

        public void Update(long ticks)
        {
            bool cameraMoved = MoveCamera(ticks);

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

            // Move and Collision Phase
            while (Scene.Active.GetQuery(_queryTransformCollisionPhase, out transforms))
            {
                for (int i = 0; i < transforms.Length; i++)
                {
                    if (Move(transforms[i]) && _scene.GetComponent(transforms[i].ID, out Collider collider))
                    {
                        CalculateGridPos(ref collider, transforms[i]);

                        ID[] nearby = GetNearby(ref collider);

                        if (nearby.Length == 0)
                        {
                            continue;
                        }

                        //Start Broad Phase
                        BoundingBox bb = new BoundingBox(transforms[i].Position, collider.Size);

                        for (int j = 0; j < nearby.Length; j++)
                        {
                            _scene.GetComponent(nearby[j], out Collider otherCollider);
                            
                            _scene.GetComponent(nearby[j], out Transform otherTransform);
                            
                            BoundingBox other = new BoundingBox(otherTransform.Position, otherCollider.Size);

                            if (bb.BouningBoxCollisionTest(other))
                            {
                                // Start Narrow Phase
                            }
                        }
                    }
                }
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
