using System;
using System.Collections.Generic;
using System.Drawing;

namespace GraphFunc
{
    public class PolygonContainer
    {
        private List<Polygon> _polygons = new List<Polygon>();

        public IReadOnlyList<Polygon> Polygons => _polygons;
        
        private int _selectedPolygon = -1;

        public bool NoPolygons
            => _polygons.Count == 0;

        public void Clear()
        {
            _polygons = new List<Polygon>();
            _selectedPolygon = -1;
        }

        public void AddPolygon()
        {
            _polygons.Add(new Polygon());
            _selectedPolygon = _polygons.Count - 1;
        }

        public void Select(int idxOfSelected)
            => _selectedPolygon = idxOfSelected;
        
        public Polygon Selected => _polygons[_selectedPolygon];
        
        public void PopPolygon()
        {
            if (NoPolygons)
                return;
            _polygons.RemoveAt(_polygons.Count - 1);
            _selectedPolygon = -1;
        }

        public void DeleteSelected()
        {
            if (NoPolygons)
                return;
            _polygons.RemoveAt(_selectedPolygon);
            _selectedPolygon = -1;
        }

        public void AddPoint(Point point)
        {
            if (NoPolygons)
                return;
            _polygons[_selectedPolygon].AddPoint(point);
        }

        public void Draw(Graphics graphics)
        {
            for (var i = 0; i < _polygons.Count; i++)
                _polygons[i].Draw(graphics, i == _selectedPolygon ? Color.Red : Color.Black);
        }

        public void TriangulateSelected()
        {
            if (_selectedPolygon < 0)
                return;
            if (Selected.Points.Count < 3)
                return;
            Triangulate(Selected.ToLinkedList());
        }

        private void AddPolygon(params PointF[] points)
        {
            _polygons.Add(new Polygon());
            foreach (var point in points)
                _polygons[_polygons.Count - 1].AddPoint(point);
        }

        private void AddList(PointNode polygon)
        {
            _polygons.Add(new Polygon());
            _polygons[_polygons.Count - 1].AddPoint(polygon.Point);
            for (var next = polygon.Prev; next != polygon; next = next.Prev)
                _polygons[_polygons.Count - 1].AddPoint(next.Point);
        }

        private void Triangulate(PointNode polygon)
        {
            if (polygon.Next == polygon)
                return;
            var internalPoint = polygon.FindInternalPoint();
            if (internalPoint == null)
            {
                AddPolygon(polygon.Point, polygon.Next.Point, polygon.Prev.Point);
                polygon.Delete();
                Triangulate(polygon.Next);
            }
            else
            {
                var (pointForPrev, pointForNext) = polygon.DivideBy(internalPoint);
                Triangulate(pointForNext);
                Triangulate(pointForPrev);
            }
        }
        
    }
}