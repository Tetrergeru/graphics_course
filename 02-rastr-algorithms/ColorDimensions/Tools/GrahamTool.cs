using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphFunc.Tools
{
    class GrahamTool : ITool
    {
        public void Stop()
        {
        }

        const int TURN_LEFT = 1;
        const int TURN_RIGHT = -1;
        const int TURN_NONE = 0;

        public int turn(Point p, Point q, Point r)
        {
            return ((q.X - p.X) * (r.Y - p.Y) - (r.X - p.X) * (q.Y - p.Y)).CompareTo(0);
        }

        public void keepLeft(List<Point> hull, Point r)
        {
            while (hull.Count > 1 && turn(hull[hull.Count - 2], hull[hull.Count - 1], r) != TURN_LEFT)
            {
                hull.RemoveAt(hull.Count - 1);
            }
            if (hull.Count == 0 || hull[hull.Count - 1] != r)
            {
                hull.Add(r);
            }
        }
        public Point GetLowestPoint()
        {
            Point p0 = Globals.POINTLIST.First.Value;
            foreach (Point p in Globals.POINTLIST)
            {
                if (p0.Y > p.Y)
                    p0 = p;
            }
            return p0;
        }

        public double getAngle(Point p1, Point p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
        }

        

        public List<Point> WithoutP0List(Point p0)
        {
            List<Point> order = new List<Point>();
            foreach (Point value in Globals.POINTLIST)
            {
                if (p0 != value)
                    order.Add(value);
            }
            return order;
        }

        public List<Point> DSort(Point p0, List<Point> arrPoint)
        {
            if (arrPoint.Count == 1)
            {
                return arrPoint;
            }
            List<Point> arrSortedInt = new List<Point>();
            int middle = (int)arrPoint.Count / 2;
            List<Point> leftArray = arrPoint.GetRange(0, middle);
            List<Point> rightArray = arrPoint.GetRange(middle, arrPoint.Count - middle);
            leftArray = DSort(p0, leftArray);
            rightArray = DSort(p0, rightArray);
            int leftptr = 0;
            int rightptr = 0;
            for (int i = 0; i < leftArray.Count + rightArray.Count; i++)
            {
                if (leftptr == leftArray.Count)
                {
                    arrSortedInt.Add(rightArray[rightptr]);
                    rightptr++;
                }
                else if (rightptr == rightArray.Count)
                {
                    arrSortedInt.Add(leftArray[leftptr]);
                    leftptr++;
                }
                else if (getAngle(p0, leftArray[leftptr]) < getAngle(p0, rightArray[rightptr]))
                {
                    arrSortedInt.Add(leftArray[leftptr]);
                    leftptr++;
                }
                else
                {
                    arrSortedInt.Add(rightArray[rightptr]);
                    rightptr++;
                }
            }
            return arrSortedInt;
        }

        public void Draw(Bitmap image, Point coords, Color color)
        {
            Point p0 = GetLowestPoint();

            var ordered = WithoutP0List(p0);

            ordered = DSort(p0, ordered);

            var res = new List<Point>();
            res.Add(p0);
            res.Add(ordered[0]);
            res.Add(ordered[1]);
            ordered.RemoveAt(0);
            ordered.RemoveAt(0);

            foreach (Point p in ordered)
            {
                keepLeft(res, p);
            }

            Pen pen = new Pen(color, 3);
            for (var i = 0; i < res.Count-1; i++)
                Graphics.FromImage(image).DrawLine(pen, res[i+1], res[i]);
            Graphics.FromImage(image).DrawLine(pen, res.First(), res.Last());
        }

        public string Name() => "Graham";
    }
}
