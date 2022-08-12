using System;
using System.Collections.Generic;


namespace CMDR.Systems
{
	internal class SpatialIndexer
    {
		#region PUBLIC_MEMBERS

		public readonly Dictionary<Vector3, List<ID>> GridCells;

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
        }

        #region PUBLIC_METHODS

		/// <summary>
		/// Caclculates a new grid position for an object.
		/// </summary>
		/// <param name="collider"> The collider for the object. </param>
		/// <param name="transform"> The Transform for the object. </param>
		public void CalculateGridPos(ref Collider collider, Transform transform)
        {

			if (collider.GridKeys == null)
			{
				collider.GridKeys = new List<Vector3>(4);
			}

			Remove(ref collider);

			BoundingBox boundingBox = new BoundingBox(Vector3.Floor(transform.Position), Vector3.Floor(collider.Size));

			boundingBox /= CellSize;

			Vector3 point1 = boundingBox.Point1;

			Vector3 point2 = boundingBox.Point2;

			// Top left corner converted to grid coordinates
			//Vector3 point1 = new Vector3(MathF.Floor(transform.X / CellSize), MathF.Floor(transform.Y / CellSize), MathF.Floor(transform.Z / CellSize));

			// Bottom right corner converted to grid coordinates
			//Vector3 point2 = new Vector3(MathF.Floor((transform.X + collider.SizeX) / CellSize), MathF.Floor((transform.Y + collider.SizeY) / CellSize), MathF.Floor((transform.Z + collider.SizeZ) / CellSize));

			collider.GridKeys.Clear();

			// Place collider in grid cells that it occupies
			for(float z = point1.Z; z <= point2.Z; z++)
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

						collider.GridKeys.Add(gridKey);
					}
		}

		/// <summary>
		/// Return objects that occupy the same grid cells as the given Collider.
		/// </summary>
		/// <param name="collider"> The Collider that is checked for nearby objects. </param>
		/// <returns> An aray of ID's that reference the nearby objects. </returns>
		public ID[] GetNearby(ref Collider collider)
        {
			if (collider.GridKeys == null)
            {
				collider.GridKeys = new List<Vector3>(4);
            }

			// Using a HashSet prevent duplicate items.
			HashSet<ID> nearby = new HashSet<ID>();

			for (int i = 0; i < collider.GridKeys.Count; i++)
            {
				Vector3 gridKey = collider.GridKeys[i];

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
			for (int i = 0; i < collider.GridKeys.Count; i++)
            {
				GridCells.Remove(collider.GridKeys[i]);
            }
        }

        #endregion
    }

    internal class SpatialIndexer_
	{
        #region MEMBERS

        public int StorageThreshold = 500;

		public int StorageStep = 50;
		
		public Dictionary<(int, int, int), List<int>> GridCells = new Dictionary<(int, int, int), List<int>>();
		
		private int _cellSize = 30;
		
		public int CellSize
		{
			get => _cellSize;
			
			set
			{
				if (value != _cellSize)
				{
					Transform[] transforms = Scene.Active.Components.Get<Transform>();
					
					Collider[] colliders = Scene.Active.Components.Get<Collider>();
					
					foreach (GameObject gameObject in Scene.Active.GameObjects)
					{
						if (gameObject.ContainsComponent<Collider>() && gameObject.ContainsComponent<Transform>() && !colliders[gameObject.Get<Collider>()].Static)
                        {
							CalcGridPos(ref colliders[gameObject.Get<Collider>()], transforms[gameObject.Get<Transform>()]);
                        }
					}
				}
			}
		}

        #endregion

        /// <summary>
        /// Returns a list of GameObject ID's near the provided collider.
        /// </summary>
        protected private int[] GetNearbyColliders(Collider collider)
        {
			// HashSet used to prevent GameObject ID duplication
            HashSet<int> hash = new HashSet<int>();
            
			int[] result;

            for (int i = 0; i < collider.GridKeys.Count; i++)
            {
				foreach (int id in GridCells[collider.GridKeys[i]])
                {
					hash.Add(id);
                }
            }
					
			// Remove colliders parent object to prevent self collision
			hash.Remove(collider.Parent);

            result = new int[hash.Count];

            hash.CopyTo(result);
            
			return result;
        }
		
		protected private void CalcGridPos(ref Collider collider, Transform transform)
		{
			if (collider.GridKeys == null)
            {
				collider.GridKeys = new List<(int X, int Y, int Z)>();
            }

			// Remove the Colliders parent from all grid cells
			foreach ((int, int, int) keys in collider.GridKeys)
            {
				GridCells[keys].Remove(collider.Parent);
            }
			
			// Top left corner converted to grid coordinates
			(int X, int Y, int Z) p1 = ((int)Math.Floor(transform.X / CellSize), (int)Math.Floor(transform.Y / CellSize), (int)Math.Floor(transform.Z / CellSize));
			
			// Bottom right corner converted to grid coordinates
			(int X, int Y, int Z) p2 = ((int)Math.Floor((transform.X + collider.SizeX) / CellSize), (int)Math.Floor((transform.Y + collider.SizeY) / CellSize), (int)Math.Floor((transform.Z + collider.SizeZ) / CellSize));

			// Place gameObject in all it's occupied cells
			collider.GridKeys.Clear();

			for (int z = p1.Z; z <= p2.Z; z++)
				for (int y = p1.Y; y <= p2.Y; y++)
					for (int x = p1.X; x <= p2.X; x++)
					{
						// Create new cells that don't exist
						if(!GridCells.ContainsKey((y, x, z)))
                        {
							GridCells[(y, x, z)] = new List<int>();
                        }
						
						GridCells[(y, x, z)].Add(collider.Parent);

						collider.GridKeys.Add((y, x, z));
					}
				
			// Remove empty grid cells
			if (GridCells.Count >= StorageThreshold)
			{
				int DelCount = 0;
				
				var _temp = GridCells.Keys;
				
				foreach ((int, int, int) key in _temp)// GridCells.Keys)
				{
					if (GridCells[key].Count == 0)
					{
						DelCount++;

						GridCells.Remove(key);
					}
				}
				// More filled cells exist than the StorageThreshold
				// Raise the Threshold to prevent it from being checked every update
				if (DelCount == 0)
                {
					StorageThreshold += StorageStep;
                }
			}
		}
	}
	
	
}