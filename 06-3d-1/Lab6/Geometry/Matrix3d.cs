using System;
using System.Drawing;

namespace GraphFunc.Geometry
{
    public class Matrix3d
    {
        private readonly float[] _matrix = new float[16];

        public float this[int x, int y]
        {
            get => _matrix[y * 4 + x];
            set => _matrix[y * 4 + x] = value;
        }

        public Point3 Multiply(Point3 point)
            => new Point3(
                point.X * this[0, 0] + point.Y * this[0, 1] + point.Z * this[0, 2] + point.W * this[0, 3],
                point.X * this[1, 0] + point.Y * this[1, 1] + point.Z * this[1, 2] + point.W * this[1, 3],
                point.X * this[2, 0] + point.Y * this[2, 1] + point.Z * this[2, 2] + point.W * this[2, 3],
                point.X * this[3, 0] + point.Y * this[3, 1] + point.Z * this[3, 2] + point.W * this[3, 3]);

        public Matrix3d Multiply(Matrix3d other)
        {
            var result = new Matrix3d();
            for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
            for (var k = 0; k < 4; k++)
                result[i, j] += this[k, j] * other[i, k];
            return result;
        }

        public Matrix3d Rotate(Axis axis, double angle)
            => Multiply(RotationMatrix(axis, angle));


        public Matrix3d Set(int x, int y, float value)
        {
            this[x, y] = value;
            return this;
        }

        public Matrix3d ClearAxis(Axis axis)
        {
            switch (axis) {
                case Axis.X:
                    this[0, 0] = 0;
                    break;
                case Axis.Y:
                    this[1, 1] = 0;
                    break;
                case Axis.Z:
                    this[2, 2] = 0;
                    break;
                default:
                    throw new ArgumentException("Wrong value of Axis");
            }
            return this;
        }
        
        public static Matrix3d RotationMatrix(Axis axis, double angle)
            => axis switch
            {
                Axis.X => new Matrix3d
                {
                    [0, 0] = 1,
                    [1, 1] = (float) Math.Cos(angle),
                    [2, 2] = (float) Math.Cos(angle),
                    [1, 2] = (float) -Math.Sin(angle),
                    [2, 1] = (float) Math.Sin(angle),
                    [3, 3] = 1,
                },
                Axis.Y => new Matrix3d
                {
                    [1, 1] = 1,
                    [0, 0] = (float) Math.Cos(angle),
                    [2, 2] = (float) Math.Cos(angle),
                    [0, 2] = (float) -Math.Sin(angle),
                    [2, 0] = (float) Math.Sin(angle),
                    [3, 3] = 1,
                },
                Axis.Z => new Matrix3d
                {
                    [2, 2] = 1,
                    [0, 0] = (float) Math.Cos(angle),
                    [1, 1] = (float) Math.Cos(angle),
                    [0, 1] = (float) -Math.Sin(angle),
                    [1, 0] = (float) Math.Sin(angle),
                    [3, 3] = 1,
                },
                _ => throw new ArgumentException("Wrong value of Axis")
            };
        
        public static Matrix3d One => new Matrix3d
        {
            [0, 0] = 1,
            [1, 1] = 1,
            [2, 2] = 1,
            [3, 3] = 1,
        };

        public override string ToString()
        {
            return $"{this[0, 0]:0.000}  {this[0, 1]:0.000}  {this[0, 2]:0.000}  {this[0, 3]:0.000}\n"
                   + $"{this[1, 0]:0.000}  {this[1, 1]:0.000}  {this[1, 2]:0.000}  {this[1, 3]:0.000}\n"
                   + $"{this[2, 0]:0.000}  {this[2, 1]:0.000}  {this[2, 2]:0.000}  {this[2, 3]:0.000}\n"
                   + $"{this[3, 0]:0.000}  {this[3, 1]:0.000}  {this[3, 2]:0.000}  {this[3, 3]:0.000}\n"
                ;
        }
    }
}