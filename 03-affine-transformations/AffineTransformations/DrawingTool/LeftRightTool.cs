using System;
using System.Drawing;

namespace GraphFunc.DrawingTool
{
    public class LeftRightTool : IDrawingTool
    {
        private PolygonContainer _polygonContainer;

        private Point _from;

        private bool _mouseDown;
        
        public void Init(PolygonContainer polygonContainer)
        {
            _polygonContainer = polygonContainer;
        }

        public void OnMouseDown(Point coordinates, Graphics drawer)
        {
            _from = coordinates;
            _mouseDown = true;
            _polygonContainer.Draw(drawer);
        }

        public void OnMouseUp(Point coordinates, Graphics drawer)
        {
            _from = new Point();
            _mouseDown = false;
            _polygonContainer.Draw(drawer);
        }

        private (Point, Point) InfiniteLine(Point p0, Point p1, Point Size)
        {
            if (p1.X == p0.X)
                return (new Point(p0.X, 0), new Point(p1.X, Size.Y));
            var a = (double) (p1.Y - p0.Y) / (p1.X - p0.X);
            var b = p0.Y  - a * p0.X;
            var from = b > 0 
                ? new Point(0, (int)b)
                : new Point((int)(-b/a), 0);
            var to = a * Size.X + b > Size.Y
                ? new Point((int) ((Size.Y - b) / a), Size.Y)
                : new Point(Size.X, (int) (a * Size.X + b)); 
            return (from, to);
        }

        public void OnMouseMove(Point coordinates, Graphics drawer)
        {
            if (!_mouseDown)
            {
                _polygonContainer.Draw(drawer);
                return;
            }

            var edge = new Edge(new PointF(_from.X, _from.Y), new PointF(coordinates.X, coordinates.Y));
            foreach (var polygon in _polygonContainer.Polygons)
            foreach (var point in polygon.Points)
            {
                switch (edge.Classify(point))
                {
                    case Edge.Position.LEFT:
                        Polygon.DrawPoint(drawer, point, Color.Red, 3);
                        break;
                    case Edge.Position.RIGHT:
                        Polygon.DrawPoint(drawer, point, Color.Blue, 3);
                        break;
                }
            }

            var (from, to) = InfiniteLine(_from, coordinates, new Point(500, 500));
            drawer.DrawLine(new Pen(Color.Cyan, 2), from,  to);
            _polygonContainer.Draw(drawer);
        }

        public void OnSelect(Graphics drawer)
        {
            _polygonContainer.Draw(drawer);
        }

        public bool CanUseInField()
            => true;

        public override string ToString()
            => "Left/right";
    }
}