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

        public bool HasPoint(Point point) 
            // TODO: good algorithm
            => true;
        private void DrawPoint(Graphics graphics, Point point, Color color)
        {
            graphics.FillEllipse(new SolidBrush(color), new Rectangle(point.X - 2, point.Y - 2, 4, 4));
        }
    }
}