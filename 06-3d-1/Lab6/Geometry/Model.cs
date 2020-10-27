using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

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
                var sum = new Point3(0, 0, 0);
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
            foreach (var point in _points)
                point.Apply(matrix);
        }

        public IEnumerable<string> SaveToObj()
        {
            foreach (var point in _points)
                yield return $"v {point.X} {point.Y} {point.Z}".Replace(',', '.');

            foreach (var polygon in Polygons)
                yield return $"f {string.Join(" ", polygon.Points.Select(p => $"{p + 1}/"))}";
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
                var split = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
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
            var polygon = new Polygon(Color.Black, points);
            foreach (var str in line.Skip(1))
            {
                var pointIdx = int.Parse(str.Substring(0, str.IndexOf('/')));
                if (pointIdx < 0)
                    pointIdx = -pointIdx;
                pointIdx -= 1;
                polygon.Points.Add(pointIdx);
            }

            return polygon;
        }

        public bool IsNewPolygon(List<Point3> used_points, Polygon p)
        {
            Point3 temp;
            foreach (var point in p.Points.Select(pointIdx => p.PointList[pointIdx]))
            {
                temp = new Point3(point.X, point.Y, point.Z, point.W);
                if (used_points.FindIndex(x => x == temp) == -1)
                    return true;
            }

            return false;
        }

        public void AddTriangles(ref Model res, List<Point3> points, List<int> indexes, int index1, int index2,
            int count)
        {
            var polygon = new Polygon(Color.Black, points);
            polygon.Points.Add(index1);
            polygon.Points.Add(index2);
            polygon.Points.Add(indexes[indexes.Count - count - 1]);
            res.Polygons.Add(polygon);

            polygon = new Polygon(Color.Black, points);
            polygon.Points.Add(index1);
            polygon.Points.Add(indexes[indexes.Count - count - 1]);
            polygon.Points.Add(indexes[indexes.Count - count - 2]);
            res.Polygons.Add(polygon);
        }

        public Model MakeSpinObj(Model base_model, string axis, int segments)
        {
            Point3 p1 = new Point3(0, 0, 0);
            Point3 p2 = new Point3(0, 0, 1);
            switch (axis)
            {
                case "X":
                    p2 = new Point3(1, 0, 0);
                    break;
                case "Y":
                    p2 = new Point3(0, 1, 0);
                    break;
                case "Z":
                    p2 = new Point3(0, 0, 1);
                    break;
            }

            Polygon foundation = base_model.Polygons[0];
            double angle = 2 * Math.PI / segments;
            Model result = new Model {Name = base_model.Name + " Spin"};
            var points = new List<Point3>();
            if (foundation.Points.Count > 3)
            {
                points.AddRange(foundation.Points.Select(t => foundation.PointList[t].Moved(0, 0)));

                int index1, index2, first_point_index = 0;
                Point3 first_point;
                var indexes = new List<int> {0, 1, 2, 3};
                for (int i = 0; i < segments; i++)
                {
                    base_model.RotateLine(p1, p2, angle);
                    first_point = foundation.PointList[foundation.Points[0]];
                    index1 = points.FindIndex(x => x == first_point);
                    if (index1 == -1)
                    {
                        points.Add(foundation.GetPoint(0).Moved(0, 0));
                        index1 = points.Count - 1;
                    }

                    first_point_index = index1;
                    indexes.Add(index1);
                    for (int j = 1; j < foundation.Points.Count; j++)
                    {
                        index2 = points.FindIndex(x => x == foundation.GetPoint(j));
                        if (index2 == -1)
                        {
                            points.Add(foundation.GetPoint(j).Moved(0,0));
                            index2 = points.Count - 1;
                        }

                        indexes.Add(index2);

                        AddTriangles(ref result, points, indexes, index1, index2, foundation.Points.Count);
                        index1 = index2;
                    }

                    var poly = new Polygon(Color.Black, points);
                    poly.Points.Add(index1);
                    poly.Points.Add(first_point_index);
                    poly.Points.Add(indexes[indexes.Count - 2 * foundation.Points.Count]);
                    result.Polygons.Add(poly);

                    poly = new Polygon(Color.Black, points);
                    poly.Points.Add(index1);
                    poly.Points.Add(indexes[indexes.Count - foundation.Points.Count - 1]);
                    poly.Points.Add(indexes[indexes.Count - foundation.Points.Count - 1]);
                    result.Polygons.Add(poly);
                }

                result._points = points;
                return result;
            }
            else
                return result;
        }


        private static float ParseFloat(string str)
            => float.Parse(str, CultureInfo.InvariantCulture);
    }
}