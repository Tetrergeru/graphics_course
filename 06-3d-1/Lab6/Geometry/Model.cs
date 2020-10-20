using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GraphFunc.Geometry
{
    public class Model
    {
        private List<Point3> _points;

        public string Name = "";

        public readonly List<Polygon> Polygons = new List<Polygon>();

        public Point3 Center
        {
            get
            {
                var sum = new Point3(0 ,0, 0);
                foreach (var point in _points)
                {
                    sum.X += point.X;
                    sum.Y += point.Y;
                    sum.Z += point.Z;
                }

                sum.X /= _points.Count;
                sum.Y /= _points.Count;
                sum.Z /= _points.Count;
                Console.WriteLine(sum);
                return sum;
            }
        }

        public void Scale(float m)
            => Apply(Matrix3d.ScaleMatrix(m));
        
        public void ScaleCenter(float m)
            => Apply(Matrix3d.ScalePointMatrix(Center, m));
        
        public void Move(Point3 delta)
            => Apply(Matrix3d.MoveMatrix(delta));

        public void Rotate(Axis axis, float angle)
            => Apply(Matrix3d.RotationMatrix(axis, angle));
        
        public void RotateCenter(Axis axis, float angle)
            => Apply(Matrix3d.RotationCenterMatrix(Center, axis, angle));

        public void RotateLine(Point3 p1, Point3 p2, double angle)
            => Apply(Matrix3d.LineRotationMatrix(p1, p2, angle));

        public void Reflect(Axis axis)
            => Apply(Matrix3d.ReflectionMatrix(axis));
        
        private void Apply(Matrix3d matrix)
        {
            foreach(var point in _points)
                point.Apply(matrix);
        }

        public static Model LoadFromObj(IEnumerable<string> file, string name)
        {
            var model = new Model {Name = name};
            var points = new List<Point3>();
            foreach (var line in file)
            {
                if (line.Length == 0)
                    continue;
                if (line[0] == '#')
                    continue;
                var split = line.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);
                if (split[0] == "v")
                    points.Add(ParsePoint(split));
                else if (split[0] == "f")
                    model.Polygons.Add(ParsePolygon(split, points));
            }

            model._points = points;
            return model;
        }

        private static Point3 ParsePoint(IReadOnlyList<string> line)
            => new Point3(ParseFloat(line[1]), ParseFloat(line[2]), ParseFloat(line[3]));

        private static Polygon ParsePolygon(string[] line, List<Point3> points)
        {
            var polygon = new Polygon(Color.Black);
            foreach (var str in line.Skip(1))
            {
                var pointIdx = int.Parse(str.Substring(0, str.IndexOf('/')));
                if (pointIdx < 0)
                    pointIdx = -pointIdx;
                pointIdx -= 1;
                polygon.Points.Add(points[pointIdx]);
            }
            return polygon;
        }

        private static float ParseFloat(string str)
            => float.Parse(str, CultureInfo.InvariantCulture);
    }
}