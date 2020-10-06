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
                        Polygon.DrawPoint(drawer, point.ToPoint(), Color.Red, 3);
                        break;
                    case Edge.Position.RIGHT:
                        Polygon.DrawPoint(drawer, point.ToPoint(), Color.Blue, 3);
                        break;
                }
            }

            var (from, to) = Edge.InfiniteLine(_from, coordinates, new Point(500, 500));
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