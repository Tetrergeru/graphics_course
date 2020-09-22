using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace GraphFunc.Tools
{
    public class LineTool : ITool
    {
        public static void LineBresenham(Bitmap image, Color color, Point from, Point to)
        {
            using (var bitmap = new FastBitmap(image))
            {
                var delta = new Point(Math.Abs(to.X - from.X), Math.Abs(to.Y - from.Y));
                var sign = new Point(Math.Sign(to.X - from.X), Math.Sign(to.Y - from.Y));
                var error = delta.X - delta.Y;
                for (var (x, y) = (from.X, from.Y); x != to.X || y != to.Y; )
                {
                    bitmap.SetPixel(new Point(x, y), (color.R, color.G, color.B));
                    var error2 = error * 2;
                    if (error2 > -delta.Y)
                    {
                        error -= delta.Y;
                        x += sign.X;
                    }
                    if (error2 < delta.X)
                    {
                        error += delta.X;
                        y += sign.Y;
                    }
                }
            }
        }

        private Point? _coordinates = null;
        
        public void Stop()
        {
            _coordinates = null;
        }

        public void Draw(Bitmap image, Point coords, Color color)
        {
            if (_coordinates == null)
            {
                Console.WriteLine("123");
                _coordinates = coords;
                return;
            }
            LineBresenham(image, color, (Point)_coordinates, coords);
            _coordinates = coords;
        }

        public string Name() => "Line";
    }
}