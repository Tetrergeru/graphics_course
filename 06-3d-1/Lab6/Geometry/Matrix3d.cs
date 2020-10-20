using System;

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

        public Matrix3d Move(Point3 delta)
            => Multiply(MoveMatrix(delta));
        
        //вращение вокруг прямой, проходящей через центр параллельно одной из осей
        public Matrix3d AxisLineRotate((double, double, double) vec, double angle)
            => Multiply(AxisLineRotationMatrix(vec, angle));
        
        //вращение вокруг произволльной прямой
        public Matrix3d LineRotate(Point3 p1, Point3 p2, double angle)
            => Multiply(LineRotationMatrix(p1, p2, angle));
        
        //масштабирование
        public Matrix3d Scale(float m)
            => Multiply(ScaleMatrix(m));
        
        public Matrix3d ScalePoint(Point3 point,float m)
            => Multiply(ScalePointMatrix(point, m));

        public Matrix3d Set(int x, int y, float value)
        {
            this[x, y] = value;
            return this;
        }

        public Matrix3d ClearAxis(Axis axis)
            => Set((int)axis, (int)axis, 0);

        public static Matrix3d MoveMatrix(Point3 delta)
            => One
                .Set(0, 3, delta.X)
                .Set(1, 3, delta.Y)
                .Set(2, 3, delta.Z);

        public static Matrix3d RotationMatrix(Axis axis, double angle)
        {
            var (axis1, axis2) = NotAxis(axis);
            return new Matrix3d
            {
                [(int)axis, (int)axis] = 1,
                [(int)axis1, (int)axis1] = (float) Math.Cos(angle),
                [(int)axis2, (int)axis2] = (float) Math.Cos(angle),
                [(int)axis1, (int)axis2] = (float) -Math.Sin(angle),
                [(int)axis2, (int)axis1] = (float) Math.Sin(angle),
                [3, 3] = 1,
            };
        }

        public static Matrix3d RotationCenterMatrix(Point3 point, Axis axis, float angle)
            => MoveMatrix(new Point3(-point.X, -point.Y, -point.Z)).Rotate(axis, angle).Move(point);

        public static Matrix3d AxisLineRotationMatrix((double, double, double) vec, double angle)
        {
            return new Matrix3d
            {
                [0, 0] = (float)(vec.Item1 * vec.Item1 + Math.Cos(angle) * (1 - vec.Item1 * vec.Item1)),
                [1, 0] = (float)(vec.Item1 * (1 - Math.Cos(angle)) * vec.Item2 + vec.Item3 * Math.Sin(angle)),
                [2, 0] = (float)(vec.Item1 * (1 - Math.Cos(angle)) * vec.Item3 - vec.Item2 * Math.Sin(angle)),
                [3, 0] = 0,
                [0, 1] = (float)(vec.Item1 * (1 - Math.Cos(angle)) * vec.Item2 - vec.Item3 * Math.Sin(angle)),
                [1, 1] = (float)(vec.Item2 * vec.Item2 + Math.Cos(angle) * (1 - vec.Item2 * vec.Item2)),
                [2, 1] = (float)(vec.Item2 * (1 - Math.Cos(angle)) * vec.Item3 + vec.Item1 * Math.Sin(angle)),
                [3, 1] = 0,
                [0, 2] = (float)(vec.Item1 * (1 - Math.Cos(angle)) * vec.Item3 + vec.Item2 * Math.Sin(angle)),
                [1, 2] = (float)(vec.Item2 * (1 - Math.Cos(angle)) * vec.Item3 - vec.Item1 * Math.Sin(angle)),
                [2, 2] = (float)(vec.Item3 * vec.Item3 + Math.Cos(angle) * (1 - vec.Item3 * vec.Item3)),
                [3, 2] = 0,
                [0, 3] = 0,
                [1, 3] = 0,
                [2, 3] = 0,
                [3, 3] = 1
            };
        }

        public static Matrix3d LineRotationMatrix(Point3 p1, Point3 p2, double angle)
        {
            double v_x = p2.X - p1.X;
            double v_y = p2.Y - p1.Y;
            double v_z = p2.Z - p1.Z;
            double length = Math.Sqrt(v_x * v_x + v_y * v_y + v_z * v_z);
            (double, double, double) vec = (v_x / length, v_y / length, v_z / length);

            return new Matrix3d
            {
                [0, 0] = (float)(vec.Item1 * vec.Item1 + Math.Cos(angle) * (1 - vec.Item1 * vec.Item1)),
                [1, 0] = (float)(vec.Item1 * (1 - Math.Cos(angle)) * vec.Item2 + vec.Item3 * Math.Sin(angle)),
                [2, 0] = (float)(vec.Item1 * (1 - Math.Cos(angle)) * vec.Item3 - vec.Item2 * Math.Sin(angle)),
                [3, 0] = 0,
                [0, 1] = (float)(vec.Item1 * (1 - Math.Cos(angle)) * vec.Item2 - vec.Item3 * Math.Sin(angle)),
                [1, 1] = (float)(vec.Item2 * vec.Item2 + Math.Cos(angle) * (1 - vec.Item2 * vec.Item2)),
                [2, 1] = (float)(vec.Item2 * (1 - Math.Cos(angle)) * vec.Item3 + vec.Item1 * Math.Sin(angle)),
                [3, 1] = 0,
                [0, 2] = (float)(vec.Item1 * (1 - Math.Cos(angle)) * vec.Item3 + vec.Item2 * Math.Sin(angle)),
                [1, 2] = (float)(vec.Item2 * (1 - Math.Cos(angle)) * vec.Item3 - vec.Item1 * Math.Sin(angle)),
                [2, 2] = (float)(vec.Item3 * vec.Item3 + Math.Cos(angle) * (1 - vec.Item3 * vec.Item3)),
                [3, 2] = 0,
                [0, 3] = 0,
                [1, 3] = 0,
                [2, 3] = 0,
                [3, 3] = 1
            };
        }

        public static Matrix3d ScalePointMatrix(Point3 point, float m)
            => MoveMatrix(new Point3(-point.X, -point.Y, -point.Z)).Scale(m).Move(point);

        public static Matrix3d ScaleMatrix(float m)
        {
            return new Matrix3d
            {
                [0, 0] = m,
                [1, 1] = m,
                [2, 2] = m,
                [3, 3] = 1
            };
        }

        public static Matrix3d ReflectionMatrix(Axis axis)
        {
            var (axis1, axis2) = NotAxis(axis);
            return new Matrix3d
            {
                [(int) axis, (int) axis] = -1,
                [(int) axis1, (int) axis1] = 1,
                [(int) axis2, (int) axis2] = 1,
                [3, 3] = 1,
            };
        }

        private static (Axis axis1, Axis axis2) NotAxis(Axis axis)
            => axis switch
            {
                Axis.X => (Axis.Y, Axis.Z),
                Axis.Y => (Axis.X, Axis.Z),
                Axis.Z => (Axis.X, Axis.Y),
                _ => throw new ArgumentException("Wrong value of Axis"),
            };

        public static Matrix3d One => new Matrix3d
        {
            [0, 0] = 1,
            [1, 1] = 1,
            [2, 2] = 1,
            [3, 3] = 1,
        };

        public override string ToString()
            => $"{this[0, 0]:0.000}  {this[0, 1]:0.000}  {this[0, 2]:0.000}  {this[0, 3]:0.000}\n" +
               $"{this[1, 0]:0.000}  {this[1, 1]:0.000}  {this[1, 2]:0.000}  {this[1, 3]:0.000}\n" +
               $"{this[2, 0]:0.000}  {this[2, 1]:0.000}  {this[2, 2]:0.000}  {this[2, 3]:0.000}\n" +
               $"{this[3, 0]:0.000}  {this[3, 1]:0.000}  {this[3, 2]:0.000}  {this[3, 3]:0.000}\n";
    }
}