

namespace CMDR
{
    public struct Transform : IComponent<Transform>
    {

        #region PUBLIC_MEMBERS

        public ID ID { get; set; }

        public Scene Scene { get; internal set; }
        
        /// <summary>
        /// Determines if the Model Matrix needs to be recalculated.
        /// </summary>
        public bool ChangeState { get; private set; }

        /// <summary>
        /// Provides an updated Model Matrix that represents this transform.
        /// </summary>
        public Matrix4 Model
        {
            get
            {
                if(ChangeState)
                    GenerateModelMatrix();
                return _model;
            }
        }

        /// <summary>
        /// Determines of this is a Static Transform. If True, this Static Transorm can only be moved via the Teleport method. 
        /// </summary>
        public bool Static
        {
            get => _static == 1;
            set
            {
                _static = value == true ? 0 : 1;
            }
        }

        #region POSITION_PROPERTIES

        public float X
        {
            get => _pos.X;
            set
            {
                (_pos.X, ChangeState) = (value * _static, true);
            }
        }

        public float Y
        {
            get => _pos.Y;
            set
            {
                (_pos.Y, ChangeState) = (value * _static, true);
            }
        }

        public float Z
        {
            get => _pos.Z;
            set
            {
                (_pos.Z, ChangeState) = (value * _static, true);
            }
        }

        public Vector3 Position { get => _pos; }

        #endregion

        #region ROTATION_PROPERTIES

        /// <summary>
        // Get or Set X rotation in degrees.
        /// </summary>
        public float XRot
        {
            get => _rot.X / 0.01745329f;
            set
            {
                (_rot.X, ChangeState) = (value * 0.01745329f, true);
            }
        }

        /// <summary>
        // Get or Set Y rotation in degrees.
        /// </summary>
        public float YRot
        {
            get => _rot.Y / 0.01745329f;
            set
            {
                (_rot.Y, ChangeState) = (value * 0.01745329f, true);
            }
        }

        /// <summary>
        // Get or Set Z rotation in degrees.
        /// </summary>
        public float ZRot
        {
            get => _rot.Z / 0.01745329f;
            set
            {
                (_rot.Z, ChangeState) = (value * 0.01745329f, true);
            }
        }

        public float XRadians { get => _rot.X; }
        public float YRadians { get => _rot.Y; }
        public float ZRadians { get => _rot.Z; }
        public Vector3 Radians { get => _rot; }
        public Vector3 Degrees { get => _rot / 0.01745329f; }

        #endregion

        #region SCALE_PROPERTIES

        public float XScale
        {
            get => _scale.X;
            set
            {
                (_scale.X, ChangeState) = (value, true);
            }
        }

        public float YScale
        {
            get => _scale.Y;
            set
            {
                (_scale.Y, ChangeState) = (value, true);
            }
        }

        public float ZScale
        {
            get => _scale.Z;
            set
            {
                (_scale.Z, ChangeState) = (value, true);
            }
        }

        public Vector3 Scale { get => _scale; }

        #endregion

        #region VELOCITY_PROPERTIES

        public float XVel
        {
            get => _vel.X;
            set
            {
                _vel.X = value * _static;
            }
        }

        public float YVel
        {
            get => _vel.Y;
            set
            {
                _vel.Y = value * _static;
            }
        }

        public float ZVel
        {
            get => _vel.Z;
            set
            {
                _vel.Z = value * _static;
            }
        }

        public Vector3 Velocity { get => _vel; }

        #endregion

        #endregion

        #region PRIVATE_MEMBERS

        private Matrix4 _model;

        private int _static;

        private Vector3 _pos;

		private Vector3 _vel;

		private Vector3 _scale;

		private Vector3 _rot;

        #endregion

        #region PUBLIC_METHODS

        public void Teleport(float x, float y, float z) => Teleport(new Vector3(x, y, z));

        public void Teleport(Vector3 pos) => (_pos, ChangeState) = (pos, true);

        public void Move(float x, float y, float z)
        {
            X += x;
            Y += y;
            Z += z;
        }

        public void Move(Vector3 direction) => Move(direction.X, direction.Y, direction.Z);

        public void SetScale(float scale) => (XScale, YScale, ZScale) = (scale, scale, scale);

        public void SetScale(Vector3 scale) => (XScale, YScale, ZScale) = scale.Tuple();

        #endregion

        #region PRIVATE_METHODS

        public void GenerateModelMatrix()
        {
            _model = Matrix4.CreateScale(XScale, YScale, 1f) * Matrix4.CreateTranslation(_pos) * Matrix4.CreateRotationZ(ZRadians) * Matrix4.CreateRotationX(XRadians) * Matrix4.CreateRotationY(YRadians);
            ChangeState = false;
        }

        #endregion
    }
}