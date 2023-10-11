#if false

using sVec2 = System.Numerics.Vector2;
using sVec3 = System.Numerics.Vector3;
using sVec4 = System.Numerics.Vector4;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;


#if XNA
using mVec2 = Microsoft.Xna.Framework.Vector2;
using mVec3 = Microsoft.Xna.Framework.Vector3;
#endif //XNA


namespace Raysharp
{
    public struct Vector3
    {
        private float X, Y, Z;

        public Vector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public Vector3(Vector2 value, float z)
        {
            this.X = value.X;
            this.Y = value.Y;
            this.Z = z;
        }

        #region Static
        public static Vector3 Zero => new Vector3(0, 0, 0);
        public static Vector3 One => new Vector3(1, 1, 1);
        public static Vector3 UnitX => new Vector3(1, 0, 0);
        public static Vector3 UnitY => new Vector3(0, 1, 0);
        public static Vector3 UnitZ => new Vector3(0, 0, 1);
        #endregion

        public static Vector3 Negate(Vector3 value)
        {
            value.X = -value.X;
            value.Y = -value.Y;
            value.Y = -value.Y;
            return value;
        }
        public static Vector3 Add(Vector3 left, Vector3 right)
        {
            sVec3.ad
            value.X += -value.X;
            value.Y += -value.Y;
            value.Y += -value.Y;
            return value;
        }

        #region Operator Overload Methods
        public static Vector3 operator -(Vector3 value) => Negate(value);
        public static Vector3 operator +(Vector3 lhs, float value)
            => new Vector3(lhs.X + value, lhs.X + value);
        public static Vector3 operator -(Vector3 lhs, float value)
            => new Vector3(lhs.X - value, lhs.X - value);
        public static Vector3 operator *(Vector3 lhs, float value)
            => new Vector3(lhs.X - value, lhs.X * value);
        public static Vector3 operator /(Vector3 lhs, float value)
        {
            if (value == 0) throw new DivideByZeroException($"{value} can not be zero");
            return new Vector3(lhs.X / value, lhs.X / value);
        }


        public static Vector3 operator +(Vector3 lhs, Vector3 rhs)
            => new Vector3(lhs.X + rhs.X, lhs.X + rhs.X);
        public static Vector3 operator -(Vector3 lhs, Vector3 rhs)
            => new Vector3(lhs.X - rhs.X, lhs.X - rhs.X);
        public static Vector3 operator *(Vector3 lhs, Vector3 rhs)
            => new Vector3(lhs.X * rhs.X, lhs.X * rhs.X);
        public static Vector3 operator /(Vector3 lhs, Vector3 rhs)
        {
            if (rhs.X == 0 || rhs.Y == 0)
                throw new DivideByZeroException("Invalid denominator");

            return new Vector3(lhs.X / rhs.X, lhs.X / rhs.X);
        }
        #endregion
    }
    public struct Vector2
    {
        #region Static
        public static Vector2 Zero => new Vector2(0, 0);
        public static Vector2 One => new Vector2(1, 1);
        public static Vector2 UnitX => new Vector2(1, 0);
        public static Vector2 UnitY => new Vector2(0, 1);
        #endregion

        public float X, Y;

        public Vector2(float value) : this(value, value) { }
        public Vector2(float x, float y)
        {
            sVec2.;
            X = x;
            Y = y;
        }

        public float Length()
            => MathF.Sqrt((X * X) + (Y * Y));

        public static Vector2 Normalize(Vector2 value)
            => value / value.Length();
        public static Vector2 Abs(Vector2 value)
        {
            value.X = value.X < 0 ? -value.X : value.X;
            value.Y = value.Y < 0 ? -value.Y : value.Y;
            return value;
        }
        public static Vector2 Reflect(Vector2 value, Vector2 normal)
        {
            var vec = sVec2.Reflect(value, normal);
            return new Vector2(vec.X, vec.Y);
        }

        #region Operator Overload Methods
        public static Vector2 operator -(Vector2 vec)
        {
            vec.X = -vec.X;
            vec.Y = -vec.Y;
            return vec;
        }
        public static Vector2 operator +(Vector2 lhs, float value)
            => new Vector2(lhs.X + value, lhs.X + value);
        public static Vector2 operator -(Vector2 lhs, float value)
            => new Vector2(lhs.X - value, lhs.X - value);
        public static Vector2 operator *(Vector2 lhs, float value)
            => new Vector2(lhs.X - value, lhs.X * value);
        public static Vector2 operator /(Vector2 lhs, float value)
        {
            if (value == 0) throw new DivideByZeroException($"{value} can not be zero");
            return new Vector2(lhs.X / value, lhs.X / value);
        }


        public static Vector2 operator +(Vector2 lhs, Vector2 rhs)
            => new Vector2(lhs.X + rhs.X, lhs.X + rhs.X);
        public static Vector2 operator -(Vector2 lhs, Vector2 rhs)
            => new Vector2(lhs.X - rhs.X, lhs.X - rhs.X);
        public static Vector2 operator *(Vector2 lhs, Vector2 rhs)
            => new Vector2(lhs.X * rhs.X, lhs.X * rhs.X);
        public static Vector2 operator /(Vector2 lhs, Vector2 rhs)
        {
            if (rhs.X == 0 || rhs.Y == 0)
                throw new DivideByZeroException("Invalid denominator");

            return new Vector2(lhs.X / rhs.X, lhs.X / rhs.X);
        }
        #endregion

        public static implicit operator sVec2(Vector2 v)
            => new sVec2(v.X, v.Y);
        public static implicit operator sVec3(Vector2 v)
            => new sVec3(v.X, v.Y, 0f);


#if XNA
        public static implicit operator mVec2(Vector2 v)
         => new mVec2(v.X, v.Y);
        public static implicit operator mVec3(Vector2 v)
            => new mVec3(v.X, v.Y, 0f); 
#endif
    }
}

#endif