using System;
using System.Collections.Generic;
using CMDR.Systems;

namespace CMDR
{
    public struct Collider : IComponent<Collider>
    {
        #region PUBLIC_MEMBERS

        public ID ID { get; set; }

		public List<(int X, int Y, int Z)> GridKeys;

		public bool Static
		{
			get => _static;
			
			set
			{
				_static = value;
			}
		}
		
		public float ScaleX
		{
			get => _scale.X;
			
			set
			{
				_scale.X = value;

				if(value > SpatialIndexer.CellSize && !_static)
                {
					SpatialIndexer.CellSize = (int)value;
                }
			}
		}
		
		public float ScaleY
		{
			get => _scale.Y;
			
			set
			{
				_scale.Y = value;

				if(value > SpatialIndexer.CellSize)
                {
					SpatialIndexer.CellSize = (int)value;
                }
			}
		}
		
		public float ScaleZ
		{
			get => _scale.Z;

			set
			{
				_scale.Z = value;

				if(value > SpatialIndexer.CellSize)
                {
					SpatialIndexer.CellSize = (int)value;
                }
			}
		}

		public Vector3 Scale => _scale;

		#endregion

		#region PRIVATE_MEMBERS

		private bool _static;

		private Vector3 _scale;

        #endregion

        #region PUBLIC_METHODS

        public void SetBounds(Vector3 scale)
        {
			(ScaleX, ScaleY, ScaleZ) = (scale.X, scale.Y, scale.Z);
        }

        #endregion
    }
}
