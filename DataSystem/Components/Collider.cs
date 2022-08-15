using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CMDR.Systems;

namespace CMDR
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Collider : IComponent<Collider>
    {
        #region PUBLIC_MEMBERS

        public ID ID { get; set; }

		//public List<Vector3> GridKeys;

		public bool Static
		{
			get => _static;
			
			set
			{
				_static = value;
			}
		}
		
		public float SizeX
		{
			get => _size.X;
			
			set
			{
				_size.X = value;
			}
		}
		
		public float SizeY
		{
			get => _size.Y;
			
			set
			{
				_size.Y = value;
			}
		}
		
		public float SizeZ
		{
			get => _size.Z;

			set
			{
				_size.Z = value;
			}
		}

		public Vector3 Size => _size;

		#endregion

		#region PRIVATE_MEMBERS

		private bool _static;

		private Vector3 _size;

        #endregion

        #region PUBLIC_METHODS

        public void SetBounds(Vector3 scale)
        {
			(SizeX, SizeY, SizeZ) = (scale.X, scale.Y, scale.Z);
        }

        #endregion
    }
}
