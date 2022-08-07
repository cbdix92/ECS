using System;

namespace CMDR
{
    public struct Vector2I : IEquatable<Vector2I>
    {
        #region PUBLIC_MEMBERS
        
        public int X;

        public int Y;
        
        #endregion

        #region CONSTRUCTORS

        public Vector2I(int n)
        {
            (X, Y) = (n, n);
        }

        public Vector2I(int x, int y)
        {
            (X, Y) = (x, y);
        }

        #endregion

        #region PUBLIC_METHODS

        public void Invert() => (X, Y) = (-X, -Y);

        public int Magnitude()
        {
            float x = (float)X;

            float y = (float)Y;
            
            return (int)MathF.Sqrt(x * x + y * y);
        }

        public void Normalize() => this /= Magnitude();

        public bool Equals(Vector2I other) => (X == other.X && Y == other.Y);
		
		public float[] ToArray() => new float[] { X, Y };
		
		public override string ToString() => $"X:{X}, Y:{Y}";

        #endregion
		
		#region STATIC_METHODS
		
		public static float Dot(Vector2I v1, Vector2I v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }
		
        public static Vector2I Normalize(Vector2I v)
        {
            return v / v.Magnitude();
        }
		
		public static Vector2I Invert(Vector2I vec)
		{
			return new Vector2I(){ X = -vec.X, Y = -vec.Y };
		}
		
        public static int Distance(Vector2I vec1, Vector2I vec2)
        {
            Vector2I result = vec1 - vec2;

            return result.Magnitude();
        }
		
		#endregion
		
        #region VECTOR_OPERATORS

        public static Vector2I operator +(Vector2I vec1, Vector2I vec2)
        {
            int x = vec1.X + vec2.X;

            int y = vec1.Y + vec2.Y;

            return new Vector2I() { X = x, Y = y };
        }

        public static Vector2I operator -(Vector2I vec1, Vector2I vec2)
        {
            int x = vec1.X - vec2.X;

            int y = vec1.Y - vec2.Y;

            return new Vector2I() { X = x, Y = y };
        }

        public static Vector2I operator *(Vector2I vec1, Vector2I vec2)
        {
            int x = vec1.X * vec2.X;

            int y = vec1.Y * vec2.Y;

            return new Vector2I() { X = x, Y = y };
        }

        public static Vector2I operator /(Vector2I vec1, Vector2I vec2)
        {
            int x = vec1.X / vec2.X;

            int y = vec1.Y / vec2.Y;

            return new Vector2I() { X = x, Y = y };
        }
        
        #endregion

        #region SCALAR_OPERATORS
        
        public static Vector2I operator +(Vector2I vec, int scalar)
        {
            Vector2I other = new Vector2I(scalar);

            return vec + other;
        }
        
        public static Vector2I operator -(Vector2I vec, int scalar)
        {
            Vector2I other = new Vector2I(scalar);

            return vec - other;
        }
        
        public static Vector2I operator *(Vector2I vec, int scalar)
        {
            Vector2I other = new Vector2I(scalar);

            return vec * other;
        }
        
        public static Vector2I operator /(Vector2I vec, int scalar)
        {
            Vector2I other = new Vector2I(scalar);

            return vec / other;
        }
        public static Vector2I operator +(int scalar, Vector2I vec)
        {
            Vector2I other = new Vector2I(scalar);

            return vec + other;
        }

        public static Vector2I operator -(int scalar, Vector2I vec)
        {
            Vector2I other = new Vector2I(scalar);

            return vec - other;
        }

        public static Vector2I operator *(int scalar, Vector2I vec)
        {
            Vector2I other = new Vector2I(scalar);

            return vec * other;
        }

        public static Vector2I operator /(int scalar, Vector2I vec)
        {
            Vector2I other = new Vector2I(scalar);

            return vec / other;
        }
        
        #endregion
    }
}