using System;
using System.Diagnostics;

namespace GraphFunc.Geometry
{
    public class Point3
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

        public override string ToString()
            => $"({X:0.000}, {Y:0.000}, {Z:0.000}, {W:0.000})";
    }
}