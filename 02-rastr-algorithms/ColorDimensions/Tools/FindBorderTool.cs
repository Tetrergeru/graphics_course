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
                    return Tuple.Create(xsource - 1, ysource - 1);
                case 1:
                    return Tuple.Create(xsource, ysource - 1);
                case 2:
                    return Tuple.Create(xsource + 1, ysource - 1);
                case 3:
                    return Tuple.Create(xsource + 1, ysource);
                case 4:
                    return Tuple.Create(xsource + 1, ysource + 1);
                case 5:
                    return Tuple.Create(xsource, ysource + 1);
                case 6:
                    return Tuple.Create(xsource - 1, ysource + 1);
                case 7:
                    return Tuple.Create(xsource - 1, ysource);
                default:
                    return Tuple.Create(xsource,  ysource - 1);
            }
        }



        private List<Tuple<int, int>> findBorders(Bitmap image, int x, int y)
        {
            Queue<Tuple<int, int>> points2visit = new Queue<Tuple<int, int>>();
            List<Tuple<int, int>> points = new List<Tuple<int, int>>();
            HashSet<string> visited = new HashSet<string>();
            bool notVisited = true;

            Color c = image.GetPixel(x, y + 10);
            int direction = 5;
            int currX = 0;
            int curY = 0;
            points.Add(Tuple.Create(x, y));
            points2visit.Enqueue(Tuple.Create(x, y));
            visited.Add(x.ToString() + "," + y.ToString());
            while (points2visit.Count != 0)
            {
                currX = points2visit.Peek().Item1;
                curY = points2visit.Peek().Item2;
                points2visit.Dequeue();

                if (notVisited)
                {
                    notVisited = false;
                    for (int i = 0; i < 8; i++)
                    {
                        Tuple<int, int> point = nearPixel(currX, curY, (direction - i + 8) % 8);
                        String str = point.Item1.ToString() + "," + point.Item2.ToString();
                        if (image.GetPixel(point.Item1, point.Item2) != c)
                        {
                            if (!visited.Contains(str))
                            {
                                direction = (direction - i + 10) % 8;
                                points2visit.Enqueue(Tuple.Create(point.Item1, point.Item2));
                                visited.Add(str);
                                points.Add(Tuple.Create(point.Item1, point.Item2));
                                break;
                            }

                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Tuple<int, int> point = nearPixel(currX, curY, ((direction - i) + 8) % 8);
                        String str = point.Item1.ToString() + "," + point.Item2.ToString();
                        if (image.GetPixel(point.Item1, point.Item2) != c)
                        {
                            if (!visited.Contains(str))
                            {
                                direction = (direction - i + 10) % 8;
                                points2visit.Enqueue(Tuple.Create(point.Item1, point.Item2));
                                visited.Add(str);
                                points.Add(Tuple.Create(point.Item1, point.Item2));
                                break;
                            }

                        }
                    }
                }
            }
            return points;
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
                image.SetPixel(point.Item1, point.Item2, Color.IndianRed);
            }
        }
        

        public string Name() => "FindBorders";
    }
}
