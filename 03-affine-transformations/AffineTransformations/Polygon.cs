using System.Collections.Generic;
using System.Drawing;

namespace GraphFunc
{
    public class Polygon
    {
        private readonly List<Point> points = new List<Point>();

        public void AddPoint(Point point) 
            => points.Add(point);

        public void Draw(Graphics graphics, Color color)
        {
            if (points.Count == 0)
                return;
            if (points.Count == 1)
                DrawPoint(graphics, points[0], color);
            else
                graphics.DrawPolygon(new Pen(color), points.ToArray());
        }

        public bool HasPoint(Point p)
        {
            PointF a = new PointF(p.X, p.Y);
            PointF edge_start = points[points.Count - 1];
            PointF edge_fin;
            int parity = 0;
            for (int i = 0; i < points.Count; i++)
            {
                edge_fin = points[i];
                Edge e = new Edge(edge_start, edge_fin);
                switch (e.edgeType(a))
                {
                    case Edge.Edge_Instance.TOUCHING:
                        return true;
                    case Edge.Edge_Instance.CROSSING:
                        parity = 1 - parity;
                        break;
                    default: break;
                }
                edge_start = edge_fin;
            }
            return (parity == 1 ? true : false);
        }
        private void DrawPoint(Graphics graphics, Point point, Color color)
        {
            graphics.FillEllipse(new SolidBrush(color), new Rectangle(point.X - 2, point.Y - 2, 4, 4));
        }
    }
}