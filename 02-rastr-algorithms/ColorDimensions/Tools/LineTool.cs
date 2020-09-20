using System;
using System.Drawing;

namespace GraphFunc.Tools
{
    public class LineTool : ITool
    {
        private Point? _coordinates = null;
        
        public void Stop()
        {
            _coordinates = null;
        }

        public void Draw(Bitmap image, Point coords, Color color)
        {
            if (_coordinates == null)
            {
                _coordinates = coords;
                return;
            }

            using (var drawer = Graphics.FromImage(image))
                drawer.DrawLine(new Pen(color), (Point)_coordinates, coords);

            _coordinates = coords;
        }

        public string Name() => "Line";
    }
}