using System;
using System.Drawing;

namespace GraphFunc.Geometry
{
    public class Matrix3d
    {
        private readonly float[] _matrix = new float[16];

        public float this[int x, int y]
        {
            get => _matrix[y * 3 + x];
            set => _matrix[y * 3 + x] = value;
        }

        public Point3 Multiply(Point3 point)
            => throw new NotImplementedException();
    }
}