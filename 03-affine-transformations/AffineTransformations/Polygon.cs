using System.Collections.Generic;
using System.Drawing;

namespace GraphFunc
{
    public class Polygon
    {
        private readonly List<Point> _points = new List<Point>();

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
            // TODO
        }

        public void Move(PointF delta)
        {
            // TODO
        }

        public void Scale(PointF origin, double scale)
        {
            // TODO
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