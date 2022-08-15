using System;
using System.Collections.Generic;


namespace CMDR.Systems
{
	internal class SpatialIndexer
    {
		#region PUBLIC_MEMBERS

		public readonly Dictionary<Vector3, List<ID>> GridCells;

		public readonly Dictionary<ID, List<Vector3>> IDToGridLookUp;

		public float CellSize => _cellSize;

		#endregion

		#region PRIVATE_MEMBERS

		private int _storageThreshold = 500;

		private int _storageStep = 50;

		private float _cellSize = 30;

        #endregion

		protected private SpatialIndexer()
        {
			GridCells = new Dictionary<Vector3, List<ID>>();

			IDToGridLookUp = new Dictionary<ID, List<Vector3>>();
        }

        #region PUBLIC_METHODS

		/// <summary>
		/// Caclculates a new grid position for an object.
		/// </summary>
		/// <param name="collider"> The collider for the object. </param>
		/// <param name="transform"> The Transform for the object. </param>
		public void CalculateGridPos(ref Collider collider, Transform transform)
        {
			if (IDToGridLookUp.ContainsKey(collider.ID) == false)
            {
				IDToGridLookUp.Add(collider.ID, new List<Vector3>(4));
            }

			Remove(ref collider);

			BoundingBox boundingBox = new BoundingBox(Vector3.Floor(transform.Position), Vector3.Floor(collider.Size));

			boundingBox /= CellSize;

			Vector3 point1 = boundingBox.Point1;

			Vector3 point2 = boundingBox.Point2;

            IDToGridLookUp[collider.ID].Clear();
			
			List<Vector3> gridKeys = IDToGridLookUp[collider.ID];

			// Place collider in grid cells that it occupies
			for (float z = point1.Z; z <= point2.Z; z++)
				for (float y = point1.Y; y <= point2.Y; y++)
					for (float x = point1.X; x <= point2.X; x++)
					{
						Vector3 gridKey = new Vector3(x, y, z);
						
						// Create new cells that don't exist
						if (GridCells.ContainsKey(gridKey) == false)
						{
							GridCells.Add(gridKey, new List<ID>());
						}

						GridCells[gridKey].Add(collider.ID);

						gridKeys.Add(gridKey);
					}
		}

		/// <summary>
		/// Return objects that occupy the same grid cells as the given Collider.
		/// </summary>
		/// <param name="collider"> The Collider that is checked for nearby objects. </param>
		/// <returns> An aray of ID's that reference the nearby objects. </returns>
		public ID[] GetNearby(ref Collider collider)
        {
			if (IDToGridLookUp.ContainsKey(collider.ID) == false)
            {
				IDToGridLookUp.Add(collider.ID, new List<Vector3>(4));
            }

			// Using a HashSet prevent duplicate items.
			HashSet<ID> nearby = new HashSet<ID>();

			List<Vector3> gridKeys = IDToGridLookUp[collider.ID];

			for (int i = 0; i < gridKeys.Count; i++)
            {
				Vector3 gridKey = gridKeys[i];

				for (int j = 0; j < GridCells[gridKey].Count; j++)
                {
					nearby.Add(GridCells[gridKey][j]);
                }
            }

			// Remove collider to prevent self collision.
			nearby.Remove(collider.ID);

			ID[] result = new ID[nearby.Count];

			nearby.CopyTo(result);

			return result;
        }

        #endregion

        #region PRIVATE_METHODS

		private void Remove(ref Collider collider)
        {
			List<Vector3> gridKeys = IDToGridLookUp[collider.ID];

			for (int i = 0; i < gridKeys.Count; i++)
            {
				GridCells.Remove(gridKeys[i]);
            }
        }

        #endregion
    }
}