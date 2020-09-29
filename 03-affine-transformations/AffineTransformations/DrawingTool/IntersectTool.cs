using System.Collections.Generic;
using System.Drawing;
using GraphFunc.Tools;

namespace GraphFunc.DrawingTool
{
    public class IntersectTool : IDrawingTool
    {
        private PolygonContainer _polygonContainer;

        private Point _from;

        private bool _mouseDown;

        private List<Edge> _edges;
        
        public void Init(PolygonContainer polygonContainer)
        {
            _polygonContainer = polygonContainer;
        }

        public void OnMouseDown(Point coordinates, Graphics drawer)
        {
            _from = coordinates;
            _mouseDown = true;
            _edges = new List<Edge>();
            var points = _polygonContainer.Selected.Points;
            for (var i = 1;i < points.Count;i ++)
                _edges.Add(new Edge(points[i - 1], points[i]));
            _edges.Add(new Edge(points[points.Count - 1], points[0]));
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
            
            drawer.DrawLine(new Pen(Color.Cyan, 2), _from,  coordinates);
            
            var edge = new Edge(new PointF(_from.X, _from.Y), new PointF(coordinates.X, coordinates.Y));
            DrawIntersections(drawer, edge);
            
            _polygonContainer.Draw(drawer);
        }

        public void OnSelect(Graphics drawer)
        {
            _polygonContainer.Draw(drawer);
        }

        public bool CanUseInField()
            => true;

        public override string ToString()
            => "Intersections";

        private void DrawIntersections(Graphics drawer, Edge edge)
        {
            foreach (var polygonEdge in _edges)
            {
                var intersection = polygonEdge.Intersection(edge);
                if (intersection == null)
                    continue;
                var intersect = (PointF) intersection;
                Polygon.DrawPoint(drawer, new Point((int)intersect.X, (int)intersect.Y), Color.Red, 3);
            }
        }
    }
}