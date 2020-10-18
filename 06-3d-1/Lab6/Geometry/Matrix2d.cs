using System.Drawing;

namespace GraphFunc.Geometry
{
    public class Matrix2d
    {
        private readonly float[] _matrix = new float[9];

        public float this[int x, int y]
        {
            get => _matrix[y * 3 + x];
            set => _matrix[y * 3 + x] = value;
        }

        public PointF Multiply(PointF point)
            => new PointF(
                point.X * this[0, 0] + point.Y * this[0, 1] + this[0, 2],
                point.X * this[1, 0] + point.Y * this[1, 1] + this[1, 2]);
    }
}