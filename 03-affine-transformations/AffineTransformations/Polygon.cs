using System.Collections.Generic;
using System.Drawing;

namespace GraphFunc
{
   
    class Edge
    {
        public PointF source;
        public PointF dest;
        public Edge(PointF s, PointF d) 
        {
            source = s;
            dest = d;
        }
        public Edge() 
        {
            source = new PointF(0, 0);
            dest = new PointF(1, 0);
        }
        public PointF point(float t)
        {
            return new PointF(source.X + t * (dest.X - source.X), source.Y + t * (dest.Y - source.Y));
        }
        public double length(PointF p)
        {
            return System.Math.Sqrt(p.X * p.X + p.Y * p.Y);
        }
        public enum Position {LEFT, RIGHT, BEYOND, BEHIND, BETWEEN, ORIGIN, DESTINATION };
        public enum Line_Instance { COLLINEAR, PARALLEL, SKEW, SKEW_CROSS, SKEW_NO_CROSS };
        public enum Edge_Instance { TOUCHING, CROSSING, INESSENTIAL };
        public Position classify(PointF p)
        {
            PointF a = new PointF(dest.X - source.X, dest.Y - source.Y);
            PointF b = new PointF(p.X - source.X, p.Y - source.Y);
            float sa = a.X * b.Y - b.X * a.Y;
            if (sa > 0.0)
                return Position.LEFT;
            if (sa < 0.0)
                return Position.RIGHT;
            if ((a.X * b.X < 0.0) || (a.Y * b.Y < 0.0))
                return Position.BEHIND;
            if (length(a) < length(b))
                return Position.BEYOND;
            if (source == p)
                return Position.ORIGIN;
            if (dest == p)
                return Position.DESTINATION;
            return Position.BETWEEN;
        }
        public float dot_product(PointF a, PointF b)
        {
            return (a.X * b.X + a.Y * b.Y);
        }
        public Line_Instance intersect(Edge e, ref float t)
        {
            PointF a = source;
            PointF b = dest;
            PointF c = e.source;
            PointF d = e.dest;
            PointF n = new PointF((d.Y - c.Y), (c.X - d.X));
            float denom = dot_product(n, new PointF(b.X - a.X, b.Y - a.Y));
            if (denom == 0.0)
            {
                Position aclass = classify(source);
                if ((aclass == Position.LEFT) || (aclass == Position.RIGHT))
                    return Line_Instance.PARALLEL;
                else return Line_Instance.COLLINEAR;
            }
            float num = dot_product(n, new PointF(a.X - c.X, a.Y - c.Y));
            t = -num / denom;
            return Line_Instance.SKEW;
        }
        public Line_Instance cross(Edge e, ref float t)
        {
            Line_Instance crossType = intersect(e, ref t);
            if ((crossType == Line_Instance.COLLINEAR) || (crossType == Line_Instance.PARALLEL))
                return crossType;
            if ((t < 0.0) || (t > 1.0))
                return Line_Instance.SKEW_NO_CROSS;
            if ((0.0 <= t) && (t <= 1.0))
                return Line_Instance.SKEW_CROSS;
            else
                return Line_Instance.SKEW_NO_CROSS;
        }
        public Edge_Instance edgeType(PointF a)
        {
            PointF v = source;
            PointF w = dest;
            switch (classify(a))
            {
                case Position.LEFT:
                    return ((v.Y < a.Y) && (a.Y <= w.Y)) ? Edge_Instance.CROSSING : Edge_Instance.INESSENTIAL;
                case Position.RIGHT:
                    return ((w.Y < a.Y) && (a.Y <= v.Y)) ? Edge_Instance.CROSSING : Edge_Instance.INESSENTIAL;
                case Position.BETWEEN:
                case Position.ORIGIN:
                case Position.DESTINATION:
                    return Edge_Instance.TOUCHING;
                default:
                    return Edge_Instance.INESSENTIAL;
            }
        }
    };

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