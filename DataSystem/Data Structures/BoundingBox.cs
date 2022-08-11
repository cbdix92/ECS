using System;

namespace CMDR
{
    public struct BoundingBox
    {
        #region PUBLIC_MEMBERS

        public Vector3 Point1;

        public Vector3 Point2;

        #endregion

        public BoundingBox(Vector3 position, Vector3 size)
        {
            size /= 2;

            Point1 = new Vector3(position.X - size.X, position.Y - size.Y, position.Z - size.Z);
            
            Point2 = new Vector3(position.X + size.X, position.Y + size.Z, position.Z + size.Z);

        }

        #region PUBLIC_METHODS

        /// <summary>
        /// Performs a bounding box collision test with another bounding box.\
        /// Both bounding boxes should be translated prior to calling this method.
        /// </summary>
        /// <param name="other"> The other Bounding box to perform the collision test with. </param>
        /// <returns> Returns True if the bounding boxs are intesecting; otherwise returns False. </returns>
        public bool BouningBoxCollisionTest(BoundingBox other)
        {
            return Point1.X <= other.Point2.X
                && Point2.X >= other.Point1.X
                && Point1.Y <= other.Point2.Y
                && Point2.Y >= other.Point1.Y
                && Point1.Z <= other.Point2.Z
                && Point2.Z >= other.Point1.Z;
        }

        /// <summary>
        /// Translates a bounding box from local coordinates to world coordinates.
        /// </summary>
        /// <param name="transform"> The transform that represent the world coordintes. </param>
        /// <returns> A BoundingBox translated to world coordinates. </returns>
        public BoundingBox Translate(Transform transform)
        {
            return new BoundingBox(transform.Position, Point2 + transform.Position);
        }

        public static BoundingBox operator /(BoundingBox boundingBox, float number)
        {
            boundingBox.Point1 /= number;

            boundingBox.Point2 /= number;
            
            return boundingBox;
        }

        #endregion
    }
}
