using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using GraphFunc.Projections;

namespace GraphFunc.Geometry
{
    public class Model
    {
        public List<Point3> Points = new List<Point3>();

        public List<Point3> Normals = new List<Point3>();

        public string Name = "";

        public List<Polygon> Polygons = new List<Polygon>();

        public Point3 Center
        {
            get
            {
                var sum = new Point3(0, 0, 0);
                foreach (var point in Points)
                {
                    sum.X += point.X;
                    sum.Y += point.Y;
                    sum.Z += point.Z;
                }

                sum.X /= Points.Count;
                sum.Y /= Points.Count;
                sum.Z /= Points.Count;
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
            for (var i = 0; i < Points.Count; i++)
            {
                Points[i] = matrix.Multiply(Points[i]);
                Normals[i] = matrix.Multiply(Normals[i]);
            }
        }

        public Model Applied(IProjection projection)
        {
            var model = new Model
            {
                Points = Points.Select(projection.Project3).Select(p => p ?? new Point3(0, 0, 0)).ToList(),
                Normals = Normals.Select(projection.Project3).Select(p => p ?? new Point3(0, 0, 0)).ToList(),
                Polygons = Polygons
            };
            return model;
        }

        public static Model MakeGraphic(Func<float, float, float> f, float x0, float y0, float x1, float y1, float step)
        {
            var pts = new List<Point3>();
            Model result = new Model {Name = "Graphic"};
            for (var x = x0; x < x1 - step; x += step)
            for (var y = y0; y < y1 - step; y += step)
            {
                pts.Add(new Point3(x, y, f(x, y)));
                pts.Add(new Point3(x + step, y, f(x + step, y)));
                pts.Add(new Point3(x, y + step, f(x, y + step)));

                var poly = new Polygon(Color.Black);

                poly.Points.Add(pts.Count - 1);
                poly.Points.Add(pts.Count - 2);
                poly.Points.Add(pts.Count - 3);

                result.Polygons.Add(poly);
            }

            result.Points = pts;
            return result;
        }

        public IEnumerable<string> SaveToObj()
        {
            foreach (var point in Points)
                yield return $"v {point.X} {point.Y} {point.Z}".Replace(',', '.');
            
            foreach (var point in Normals)
                yield return $"vn {point.X} {point.Y} {point.Z}".Replace(',', '.');

            foreach (var polygon in Polygons)
                yield return
                    $"f {string.Join(" ", polygon.Points.Zip(polygon.Normals, (p, n) => (p, n)).Select(pn => $"{pn.p + 1}//{pn.n + 1}"))}";
        }

        public static Model LoadFromObj(IEnumerable<string> file, string name)
        {
            var model = new Model {Name = name};
            foreach (var line in file)
            {
                if (line.Length == 0)
                    continue;
                if (line[0] == '#')
                    continue;
                var split = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                switch (split[0])
                {
                    case "v":
                        model.Points.Add(ParsePoint(split));
                        break;
                    case "vn":
                        model.Normals.Add(ParsePoint(split));
                        break;
                    case "f":
                        model.Polygons.Add(ParsePolygon(split));
                        break;
                }
            }

            return model;
        }

        private static Point3 ParsePoint(IReadOnlyList<string> line)
            => new Point3(ParseFloat(line[1]), ParseFloat(line[2]), ParseFloat(line[3]));

        private static Polygon ParsePolygon(IEnumerable<string> line)
        {
            var polygon = new Polygon(Color.Red);
            foreach (var str in line.Skip(1))
            {
                var split = str.Split('/');
                polygon.Points.Add(ToObjFormat(int.Parse(split[0])));
                polygon.Normals.Add(ToObjFormat(int.Parse(split[2])));
            }

            return polygon;
        }

        private static int ToObjFormat(int idx)
        {
            if (idx < 0)
                idx = -idx;
            idx -= 1;
            return idx;
        }

        public bool IsNewPolygon(List<Point3> used_points, Polygon p)
        {
            Point3 temp;
            foreach (var point in p.Points.Select(pointIdx => used_points[pointIdx]))
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
            var polygon = new Polygon(Color.Black);
            polygon.Points.Add(index1);
            polygon.Points.Add(index2);
            polygon.Points.Add(indexes[indexes.Count - count - 1]);
            res.Polygons.Add(polygon);

            polygon = new Polygon(Color.Black);
            polygon.Points.Add(index1);
            polygon.Points.Add(indexes[indexes.Count - count - 1]);
            polygon.Points.Add(indexes[indexes.Count - count - 2]);
            res.Polygons.Add(polygon);
        }

        public Model MakeSpinObj(Model base_model, Axis axis, int segments)
        {
            var p1 = new Point3(0, 0, 0);
            var p2 = axis switch
            {
                Axis.X => new Point3(1, 0, 0),
                Axis.Y => new Point3(0, 1, 0),
                Axis.Z => new Point3(0, 0, 1),
            };

            Polygon foundation = base_model.Polygons[0];
            double angle = 2 * Math.PI / segments;
            Model result = new Model {Name = base_model.Name + " Spin"};
            var points = new List<Point3>();
            if (foundation.Points.Count > 3)
            {
                points.AddRange(foundation.Points.Select(t => base_model.Points[t].Moved(0, 0)));

                int index1, index2, first_point_index = 0;
                Point3 first_point;
                var indexes = new List<int> {0, 1, 2, 3};
                for (int i = 0; i < segments; i++)
                {
                    base_model.RotateLine(p1, p2, angle);
                    first_point = base_model.Points[foundation.Points[0]];
                    index1 = points.FindIndex(x => x == first_point);
                    if (index1 == -1)
                    {
                        points.Add(foundation.GetPoint(0, base_model.Points).Moved(0, 0));
                        index1 = points.Count - 1;
                    }

                    first_point_index = index1;
                    indexes.Add(index1);
                    for (int j = 1; j < foundation.Points.Count; j++)
                    {
                        index2 = points.FindIndex(x => x == foundation.GetPoint(j, base_model.Points));
                        if (index2 == -1)
                        {
                            points.Add(foundation.GetPoint(j, base_model.Points).Moved(0, 0));
                            index2 = points.Count - 1;
                        }

                        indexes.Add(index2);

                        AddTriangles(ref result, points, indexes, index1, index2, foundation.Points.Count);
                        index1 = index2;
                    }

                    var poly = new Polygon(Color.Black);
                    poly.Points.Add(index1);
                    poly.Points.Add(first_point_index);
                    poly.Points.Add(indexes[indexes.Count - 2 * foundation.Points.Count]);
                    result.Polygons.Add(poly);

                    poly = new Polygon(Color.Black);
                    poly.Points.Add(index1);
                    poly.Points.Add(indexes[indexes.Count - foundation.Points.Count - 1]);
                    poly.Points.Add(indexes[indexes.Count - foundation.Points.Count - 1]);
                    result.Polygons.Add(poly);
                }

                result.Points = points;
                return result;
            }
            else
                return result;
        }


        private static float ParseFloat(string str)
            => float.Parse(str, CultureInfo.InvariantCulture);
    }
}