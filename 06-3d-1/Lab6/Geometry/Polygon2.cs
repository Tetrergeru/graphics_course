using System;
using System.Collections.Generic;
using System.Drawing;

namespace GraphFunc.Geometry
{
    public class Polygon2
    {
        private List<PointF> _points = new List<PointF>();
        
        public IReadOnlyList<PointF> Points => _points;

        public void AddPoint(PointF point)
            => _points.Add(point);
        
        public void Draw(Graphics graphics, Color color)
        {
            if (_points.Count == 0)
                return;
            if (_points.Count == 1)
                DrawPoint(graphics, _points[0].ToPoint(), color);
            else
                graphics.DrawPolygon(new Pen(color), _points.ToArray());
        }
        
        private static void DrawPoint(Graphics graphics, Point point, Color color, int radius = 2)
            => graphics.FillEllipse(new SolidBrush(color),
                new Rectangle(
                    point.X - radius,
                    point.Y - radius,
                    radius * 2,
                    radius * 2));

        public Polygon2 Move(PointF delta)
        {
            if (_points.Count == 0)
                return this;
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
            return this;
        }
        
        public Polygon2 Scale(PointF origin, (double w, double h) scale)
        {
            if (_points.Count == 0)
                return this;
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
            return this;
        }

    }
}