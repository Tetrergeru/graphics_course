using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

namespace GraphFunc.Geometry
{
    public struct Point3
    {
        public float X;

        public float Y;

        public float Z;

        public float W;

        public Point3(float x, float y, float z, float w = 1)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public void Apply(Matrix3d matrix)
        {
            var point = matrix.Multiply(this);
            X = point.X;
            Y = point.Y;
            Z = point.Z;
            W = point.W;
        }
        
        public Point3 Moved(Axis axis, float distance)
            => axis switch
            {
                Axis.X => new Point3(X + distance, Y, Z, W),
                Axis.Y => new Point3(X, Y + distance, Z, W),
                Axis.Z => new Point3(X, Y, Z + distance, W),
            };

        public readonly float Distance(Point3 other)
        {
            return (float)Math.Sqrt(Math.Pow(other.X - this.X, 2) + Math.Pow(other.Y - this.Y, 2) + Math.Pow(other.Z- this.Z, 2));
        }

        public override string ToString()
            => $"({X:0.000}, {Y:0.000}, {Z:0.000}, {W:0.000})";

        private const float Tolerance = 0.0001f;

        public override bool Equals(object obj)
        {
            if (obj is Point3 point3) 
                return this == point3;
            return false;
        }
        
        public static bool operator ==(Point3 self, Point3 other)
            => Math.Abs(self.X - other.X) < Tolerance && Math.Abs(self.Y - other.Y) < Tolerance && Math.Abs(self.Z - other.Z) < Tolerance;

        public static bool operator !=(Point3 self, Point3 other) 
            => !(self == other);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3 operator +(Point3 self, Point3 other) 
            => new Point3(self.X + other.X, self.Y + other.Y, self.Z + other.Z);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3 operator -(Point3 self, Point3 other) 
            => new Point3(self.X - other.X, self.Y - other.Y, self.Z - other.Z);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3 operator *(Point3 self, float b) 
            => new Point3(self.X * b, self.Y * b, self.Z * b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator *(Point3 self, Point3 other) 
            => self.X * other.X + self.Y * other.Y + self.Z * other.Z;
    }
}