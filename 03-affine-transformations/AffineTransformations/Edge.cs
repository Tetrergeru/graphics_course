using System;
using System.Drawing;

namespace GraphFunc
{
    class Edge
    {
        private PointF _source;

        private PointF _dest;
        
        public Edge(PointF s, PointF d) 
        {
            _source = s;
            _dest = d;
        }
        
        public Edge() 
        {
            _source = new PointF(0, 0);
            _dest = new PointF(1, 0);
        }
        
        public PointF Point(float t)
        {
            return new PointF(_source.X + t * (_dest.X - _source.X), _source.Y + t * (_dest.Y - _source.Y));
        }

        private static double Length(PointF p)
        {
            return Math.Sqrt(p.X * p.X + p.Y * p.Y);
        }
        
        public enum Position {LEFT, RIGHT, BEYOND, BEHIND, BETWEEN, ORIGIN, DESTINATION }
        
        public enum Line_Instance { COLLINEAR, PARALLEL, SKEW, SKEW_CROSS, SKEW_NO_CROSS }
        
        public enum Edge_Instance { TOUCHING, CROSSING, INESSENTIAL }
        
        public Position Classify(PointF p)
        {
            var a = new PointF(_dest.X - _source.X, _dest.Y - _source.Y);
            var b = new PointF(p.X - _source.X, p.Y - _source.Y);
            var sa = a.X * b.Y - b.X * a.Y;
            if (sa > 0.0)
                return Position.LEFT;
            if (sa < 0.0)
                return Position.RIGHT;
            if ((a.X * b.X < 0.0) || (a.Y * b.Y < 0.0))
                return Position.BEHIND;
            if (Length(a) < Length(b))
                return Position.BEYOND;
            if (_source == p)
                return Position.ORIGIN;
            if (_dest == p)
                return Position.DESTINATION;
            return Position.BETWEEN;
        }

        public static float DotProduct(PointF a, PointF b)
            => a.X * b.X + a.Y * b.Y;

        public (Line_Instance, float) Intersect(Edge e)
        {
            var a = _source;
            var b = _dest;
            var c = e._source;
            var d = e._dest;
            var n = new PointF((d.Y - c.Y), (c.X - d.X));
            var denom = DotProduct(n, new PointF(b.X - a.X, b.Y - a.Y));
            if (Math.Abs(denom) < 0.0001)
            {
                var aclass = Classify(_source);
                if (aclass == Position.LEFT || aclass == Position.RIGHT)
                    return (Line_Instance.PARALLEL, float.NaN);
                return (Line_Instance.COLLINEAR, float.NaN);
            }
            var num = DotProduct(n, new PointF(a.X - c.X, a.Y - c.Y));
            return (Line_Instance.SKEW, -num / denom);
        }

        public PointF? Intersection(Edge other)
        {
            var (type, offset) = Cross(other);
            if (type != Line_Instance.SKEW_CROSS)
                return null;
            Console.WriteLine($"{offset}: ({_source.X + offset * (_dest.X - _source.X)}, {_source.Y + offset * (_dest.Y - _source.Y)}), ({_source.X}, {_source.Y})");
            var intersection = new PointF( _source.X + offset * (_dest.X - _source.X), _source.Y + offset * (_dest.Y - _source.Y));
            return intersection;
        }

        public (Line_Instance, float) Cross(Edge e)
        {
            var (crossType, t) = Intersect(e);
            if (crossType == Line_Instance.COLLINEAR || crossType == Line_Instance.PARALLEL)
                return (crossType, t);
            if (0.0 <= t && t <= 1.0)
            {
                return (Line_Instance.SKEW_CROSS, t);
            }

            return (Line_Instance.SKEW_NO_CROSS, t);
        }
        
        public Edge_Instance edgeType(PointF a)
        {
            var v = _source;
            var w = _dest;
            switch (Classify(a))
            {
                case Position.LEFT:
                    return v.Y < a.Y && a.Y <= w.Y ? Edge_Instance.CROSSING : Edge_Instance.INESSENTIAL;
                case Position.RIGHT:
                    return w.Y < a.Y && a.Y <= v.Y ? Edge_Instance.CROSSING : Edge_Instance.INESSENTIAL;
                case Position.BETWEEN:
                case Position.ORIGIN:
                case Position.DESTINATION:
                    return Edge_Instance.TOUCHING;
                default:
                    return Edge_Instance.INESSENTIAL;
            }
        }
        
        public static (Point, Point) InfiniteLine(Point p0, Point p1, Point size)
        {
            if (p1.X == p0.X)
                return (new Point(p0.X, 0), new Point(p1.X, size.Y));
            var a = (double) (p1.Y - p0.Y) / (p1.X - p0.X);
            var b = p0.Y  - a * p0.X;
            var from = b > 0 
                ? new Point(0, (int)b)
                : new Point((int)(-b/a), 0);
            var to = a * size.X + b > size.Y
                ? new Point((int) ((size.Y - b) / a), size.Y)
                : new Point(size.X, (int) (a * size.X + b)); 
            return (from, to);
        }
    }
}