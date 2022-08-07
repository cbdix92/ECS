using System;

namespace CMDR
{
    public struct Vector2 : IEquatable<Vector2>
    {
        #region PUBLIC_MEMBERS
        
        public float X;

        public float Y;
        
        #endregion

        #region CONSTRUCTORS

        public Vector2(float n)
        {
            (X, Y) = (n, n);
        }

        public Vector2(float x, float y)
        {
            (X, Y) = (x, y);
        }

        #endregion

        #region PUBLIC_METHODS

        public void Invert()
        {
            (X, Y) = (-X, -Y);
        }

        public float Magnitude() => MathF.Sqrt(X * X + Y * Y);

        public void Normalize() => this /= Magnitude();

        public bool Equals(Vector2 other) => (X == other.X && Y == other.Y);
		
		public float[] ToArray() => new float[] { X, Y};
		
		public override string ToString() => $"X:{X}, Y:{Y}";

        #endregion
		
		#region STATIC_METHODS
		
		public static float Dot(Vector2 v1, Vector2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }
		
        public static Vector2 Normalize(Vector2 v)
        {
            return v / v.Magnitude();
        }
		
		public static Vector2 Invert(Vector2 vec)
		{
			return new Vector2(){ X = -vec.X, Y = -vec.Y };
		}
		
        public static float Distance(Vector2 vec1, Vector2 vec2)
        {
            Vector2 result = vec1 - vec2;

            return result.Magnitude();
        }
		
		#endregion
		
        #region VECTOR_OPERATORS

        public static Vector2 operator +(Vector2 vec1, Vector2 vec2)
        {
            float x = vec1.X + vec2.X;

            float y = vec1.Y + vec2.Y;

            return new Vector2() { X = x, Y = y };
        }

        public static Vector2 operator -(Vector2 vec1, Vector2 vec2)
        {
            float x = vec1.X - vec2.X;

            float y = vec1.Y - vec2.Y;

            return new Vector2() { X = x, Y = y };
        }

        public static Vector2 operator *(Vector2 vec1, Vector2 vec2)
        {
            float x = vec1.X * vec2.X;

            float y = vec1.Y * vec2.Y;

            return new Vector2() { X = x, Y = y };
        }

        public static Vector2 operator /(Vector2 vec1, Vector2 vec2)
        {
            float x = vec1.X / vec2.X;

            float y = vec1.Y / vec2.Y;

            return new Vector2() { X = x, Y = y };
        }
        
        #endregion

        #region SCALAR_OPERATORS
        
        public static Vector2 operator +(Vector2 vec, float scalar)
        {
            Vector2 other = new Vector2(scalar);

            return vec + other;
        }
        
        public static Vector2 operator -(Vector2 vec, float scalar)
        {
            Vector2 other = new Vector2(scalar);

            return vec - other;
        }
        
        public static Vector2 operator *(Vector2 vec, float scalar)
        {
            Vector2 other = new Vector2(scalar);

            return vec * other;
        }
        
        public static Vector2 operator /(Vector2 vec, float scalar)
        {
            Vector2 other = new Vector2(scalar);

            return vec / other;
        }
        public static Vector2 operator +(float scalar, Vector2 vec)
        {
            Vector2 other = new Vector2(scalar);

            return vec + other;
        }

        public static Vector2 operator -(float scalar, Vector2 vec)
        {
            Vector2 other = new Vector2(scalar);

            return vec - other;
        }

        public static Vector2 operator *(float scalar, Vector2 vec)
        {
            Vector2 other = new Vector2(scalar);

            return vec * other;
        }

        public static Vector2 operator /(float scalar, Vector2 vec)
        {
            Vector2 other = new Vector2(scalar);

            return vec / other;
        }
        
        #endregion
    }
}