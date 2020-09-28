using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace GraphFunc.Tools
{
    public class FindBorderTool : ITool
    {
        public void Stop()
        {
        }

        private Tuple<int, int> nearPixel(int xsource, int ysource, int direction)
        {

            switch (direction)
            {
                case 0:
                    return Tuple.Create(xsource + 1, ysource);
                case 1:
                    return Tuple.Create(xsource + 1, ysource - 1);
                case 2:
                    return Tuple.Create(xsource, ysource - 1);
                case 3:
                    return Tuple.Create(xsource - 1, ysource - 1);
                case 4:
                    return Tuple.Create(xsource - 1, ysource);
                case 5:
                    return Tuple.Create(xsource - 1, ysource + 1);
                case 6:
                    return Tuple.Create(xsource, ysource + 1);
                case 7:
                    return Tuple.Create(xsource + 1, ysource + 1);
                default:
                    throw new Exception("wtf?");
            }
        }



        private List<Tuple<int, int>> findBorders(Bitmap image, int x, int y)
        {
            List<Tuple<int, int>> points = new List<Tuple<int, int>>();

            Color c = image.GetPixel(x, y + 1);
            var startPoint = Tuple.Create(x, y);
            int direction = 5;
            points.Add(startPoint);

            while (true)
            {
                Tuple<int, int> point = Tuple.Create(-1, -1); 
                for (int i = 0; i < 8; i++)
                {
                    point = nearPixel(x, y, ((direction - i) + 8) % 8);
                    if (image.GetPixel(point.Item1, point.Item2) != c)
                    {
                        direction = (direction - i + 10) % 8;
                        points.Add(Tuple.Create(point.Item1, point.Item2));
                        x = point.Item1;
                        y = point.Item2;
                        break;
                    }
                }
                if (startPoint.Item1 == point.Item1 && startPoint.Item2 == point.Item2 && (direction - 5 + 8) % 8 < 4)
                    return points;
            }
        }

        public void Draw(Bitmap image, Point coords, Color color)
        {
            var startColor = image.GetPixel(coords.X, coords.Y);
            var nextPixelColor = image.GetPixel(coords.X, coords.Y);
            var nextY = coords.Y;

            while (startColor == nextPixelColor)
            {
                --nextY;
                nextPixelColor = image.GetPixel(coords.X, nextY);
            }
            List<Tuple<int, int>> t = findBorders(image, coords.X, nextY);
            foreach (var point in t)
            {
                image.SetPixel(point.Item1, point.Item2, Color.Red);
            }
        }
        

        public string Name() => "FindBorders";
    }
}
