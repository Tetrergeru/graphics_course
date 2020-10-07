using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GraphFunc.Tools
{
    public static class Globals
    {
        public static LinkedList<Point> POINTLIST = new LinkedList<Point>();
    }


    class DrawPointTool : ITool
    {
        public void Stop()
        {
        }

        public void Draw(Bitmap image, Point coords, Color color)
        {
            Pen EllPen = new Pen(color, 3);

            Graphics.FromImage(image).DrawLine(EllPen, coords, new Point(coords.X+1, coords.Y+1));

            Globals.POINTLIST.AddLast(coords);
            //Console.WriteLine(Globals.POINTLIST.Last());
        }

        public string Name() => "DrawPoint";
    }
}

