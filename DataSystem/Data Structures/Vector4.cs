using System;

namespace CMDR
{
    public struct Vector4 : IEquatable<Vector4>
    {
        #region PUBLIC_MEMBERS

        public float X;
        
        public float Y;
        
        public float Z;
        
        public float W;

        #endregion

        #region CONSTRUCTORS

        public Vector4(float n) => (X, Y, Z, W) = (n, n, n, n);

        public Vector4(float x, float y, float z, float w) => (X, Y, Z, W) = (x, y, z, w);

        #endregion

        #region PUBLIC_METHODS

        public void Invert()
        {
            (X, Y, Z, W) = (-X, -Y, -Z, -W);
        }

        /// <summary>
        /// Calculate the Magnitude of this vector.
        /// </summary>
        /// <returns> The magnitude of this vector. </returns>
        public float Magnitude()
        {
            return MathF.Sqrt(X * X + Y * Y + Z * Z + W * W);
        }

        /// <summary>
        /// Normalize this vector. 
        /// </summary>
        public void Normalize()
        {
            float magnitude = Magnitude();

            X /= magnitude;
            
            Y /= magnitude;
            
            Z /= magnitude;
            
            W /= magnitude;
        }

        public static float Dot(Vector4 v1, Vector4 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z + v1.W * v2.W;
        }

        public static Vector4 Normalize(Vector4 v)
        {
            return v / v.Magnitude();
        }

        public static Vector4 Invert(Vector4 vec)
        {
            return new Vector4() { X = -vec.X, Y = -vec.Y, Z = -vec.Z, W = -vec.W };
        }

        public static float Distance(Vector4 vec1, Vector4 vec2)
        {
            Vector4 result = vec1 - vec2;

            return result.Magnitude();
        }

        public static Vector4 Floor(Vector4 vector)
        {
            return new Vector4(MathF.Floor(vector.X), MathF.Floor(vector.Y), MathF.Floor(vector.Z), MathF.Floor(vector.W));
        }

        public float[] ToArray()
        {
            return new float[] { X, Y, Z, W };
        }

        public override string ToString()
        {
            return $"X:{X}, Y:{Y}, Z:{Z}, W:{W}";
        }

        public bool Equals(Vector4 other)
        {
            return (X == other.X && Y == other.Y && Z == other.Z && W == other.W);
        }

        #endregion

        #region VECTOR_OPERATORS

        public static Vector4 operator +(Vector4 vec1, Vector4 vec2)
        {
            return new Vector4(vec1.X + vec2.X,
                               vec1.Y + vec2.Y,
                               vec1.Z + vec2.Z,
                               vec1.W + vec2.W);
        }

        public static Vector4 operator -(Vector4 vec1, Vector4 vec2)
        {
            return new Vector4(vec1.X - vec2.X,
                               vec1.Y - vec2.Y,
                               vec1.Z - vec2.Z,
                               vec1.W - vec2.W);
        }

        public static Vector4 operator *(Vector4 vec1, Vector4 vec2)
        {
            return new Vector4(vec1.X * vec2.X,
                               vec1.Y * vec2.Y,
                               vec1.Z * vec2.Z,
                               vec1.W * vec2.W);
        }

        public static Vector4 operator /(Vector4 vec1, Vector4 vec2)
        {
            return new Vector4(vec1.X / vec2.X,
                               vec1.Y / vec2.Y,
                               vec1.Z / vec2.Z,
                               vec1.W / vec2.W);
        }

        #endregion

        #region SCALAR_OPERATORS

        public static Vector4 operator +(Vector4 vec, float scalar) => vec + new Vector4(scalar);

        public static Vector4 operator -(Vector4 vec, float scalar) => vec - new Vector4(scalar);

        public static Vector4 operator *(Vector4 vec, float scalar) => vec * new Vector4(scalar);

        public static Vector4 operator /(Vector4 vec, float scalar) => vec / new Vector4(scalar);

        public static Vector4 operator +(float scalar, Vector4 vec) => new Vector4(scalar) + vec;

        public static Vector4 operator -(float scalar, Vector4 vec) => new Vector4(scalar) - vec;

        public static Vector4 operator *(float scalar, Vector4 vec) => new Vector4(scalar) * vec;

        public static Vector4 operator /(float scalar, Vector4 vec) => new Vector4(scalar) /  vec;

        #endregion
    }
}
