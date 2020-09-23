using System;
using System.Drawing;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GraphFunc.Tools
{
    public class LineTool : ITool
    {
        private readonly bool Wu;
        public LineTool(bool wu)
        {
            Wu = wu;
        }

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

        public static void Swap<T>(ref T a, ref T b)
        {
            var t = a;
            a = b;
            b = t;
        }

        private static double Fpart(double y)
            => y - Math.Truncate(y);

        private static double RFpart(double y)
            => 1 - Fpart(y);
        
        private static (byte r, byte g, byte b) AdjustIntensity((byte r, byte g, byte b) cl, (byte r, byte g, byte b) bc, double y)
            => (
                (byte) (cl.r * (1 - y) + bc.r * y), 
                (byte) (cl.g * (1 - y) + bc.g * y), 
                (byte) (cl.b * (1 - y) + bc.b * y)
                );


        private static void SetPixel(FastBitmap bitmap, Point point, (byte r, byte g, byte b) color, double y)
        {
            var cl = bitmap.GetPixel(point);
            var res = AdjustIntensity(color, cl, y);
            bitmap.SetPixel(point, res);
        }

        public static void LineWu(Bitmap image, Color color, (int X, int Y) from, (int X, int Y) to)
        {
            using (var bitmap = new FastBitmap(image))
            {
                var cl = (color.R, color.G, color.B);
                var steep = Math.Abs(to.Y - from.Y) > Math.Abs(to.X - from.X);
                if (steep)
                {
                    Swap(ref from.X, ref from.Y);
                    Swap(ref to.X, ref to.Y);
                }

                if (from.X > to.X)
                {
                    Swap(ref from.X, ref to.X);
                    Swap(ref from.Y, ref to.Y);
                }

                var dx = to.X - from.X;
                var dy = to.Y - from.Y;
                var gradient = dx == 0 ? 1.0 : dy * 1.0 / dx;
                var y = from.Y + gradient;
                for (var x = from.X; x != to.X; x += 1)
                {
                    if (steep)
                    {
                        SetPixel(bitmap, new Point((int) y, x), cl, Fpart(y));
                        SetPixel(bitmap, new Point((int) y + 1, x), cl, RFpart(y));
                    }
                    else
                    {
                        SetPixel( bitmap, new Point(x, (int) y), cl, Fpart(y));
                        SetPixel( bitmap, new Point(x, (int) y + 1), cl, RFpart(y));
                    }

                    y += gradient;
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
            var lp = (Point) _coordinates;
            Console.WriteLine($"from: {lp}, to: {coords}");
            if (Wu)
                LineWu(image, color, (lp.X, lp.Y), (coords.X, coords.Y));
            else
                LineBresenham(image, color, (Point)_coordinates, coords);
            _coordinates = coords;
        }

        public string Name() => Wu ? "Wu Line" : "Br Line";
    }
}