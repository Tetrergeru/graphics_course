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
    }
}