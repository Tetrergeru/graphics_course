using System.Drawing;

namespace GraphFunc
{
    public class Matrix
    {
        private readonly float[] _matrix = new float[9];

        public float this[int x, int y]
        {
            get => _matrix[y * 3 + x];
            set => _matrix[y * 3 + x] = value;
        }

        public PointF Multiply(PointF point)
            => new PointF(
                point.X * this[0, 0] + point.Y * this[1, 0] + this[2, 0],
                point.X * this[0, 1] + point.Y * this[1, 1] + this[2, 1]);

    }
}