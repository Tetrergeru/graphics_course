using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GraphFunc
{
    public class Polygon
    {
        private List<Point> _points = new List<Point>();

        private List<Point> _copy;
        
        public IReadOnlyList<Point> Points => _points;

        public void AddPoint(Point point)
            => _points.Add(point);

        public void Draw(Graphics graphics, Color color)
        {
            if (_points.Count == 0)
                return;
            if (_points.Count == 1)
                DrawPoint(graphics, _points[0], color);
            else
                graphics.DrawPolygon(new Pen(color), _points.ToArray());
        }

        public bool HasPoint(Point p)
        {
            PointF a = new PointF(p.X, p.Y);
            PointF edge_start = _points[_points.Count - 1];
            PointF edge_fin;
            var parity = 0;
            foreach (var point in _points)
            {
                edge_fin = point;
                var e = new Edge(edge_start, edge_fin);
                switch (e.edgeType(a))
                {
                    case Edge.Edge_Instance.TOUCHING:
                        return true;
                    case Edge.Edge_Instance.CROSSING:
                        parity = 1 - parity;
                        break;
                }

                edge_start = edge_fin;
            }

            return parity == 1;
        }

        public void Rotate(PointF origin, double angle)
        {
            angle = (float)(angle / 180 * Math.PI);
            var matrix = new Matrix
            {
                [0, 0] = (float)Math.Cos(angle),
                [1, 1] = (float)Math.Cos(angle),
                [2, 2] = 1,
                [0, 1] = -(float)Math.Sin(angle),
                [1, 0] = (float)Math.Sin(angle),
                [0, 2] = (float)(origin.X * Math.Cos(angle) - origin.Y * Math.Sin(angle)),
                [1, 2] = (float)(origin.X * Math.Sin(angle) + origin.Y * Math.Cos(angle))
            };
            for (var i = 0; i < _points.Count; i++)
                _points[i] = matrix.Multiply(_points[i]).ToPoint();
        }

        public void Move(PointF delta)
        {
            var matrix = new Matrix
            {
                [0, 0] = 1,
                [1, 1] = 1,
                [2, 2] = 1,
                [0, 2] = delta.X,
                [1, 2] = delta.Y
            };
            for (var i = 0; i < _points.Count; i++)
                _points[i] = matrix.Multiply(_points[i]).ToPoint();
        }

        public void Save()
        {
            _copy = _points.ToList();
        }

        public void Load()
        {
            _points = _copy.ToList();
        }

        public PointF Center()
        {
            var (x, y) = (0.0f, 0.0f);
            foreach (var point in Points)
            {
                x += point.X;
                y += point.Y;
            }
            return new PointF(x / Points.Count, y / Points.Count);
        }

        public void Scale(PointF origin, (double w, double h) scale)
        {
            var (w, h) = scale;
            var matrix = new Matrix
            {
                [0, 0] = (float) w,
                [1, 1] = (float) h,
                [2, 2] = 1,
                [0, 2] = (float) ((1 - w) * origin.X),
                [1, 2] = (float) ((1 - h) * origin.Y),
            };
            for (var i = 0; i < _points.Count; i++)
                _points[i] = matrix.Multiply(_points[i]).ToPoint();
        }

        public static void DrawPoint(Graphics graphics, Point point, Color color, int radius = 2)
            => graphics.FillEllipse(new SolidBrush(color),
                new Rectangle(
                    point.X - radius,
                    point.Y - radius,
                    radius * 2,
                    radius * 2));
    }
}