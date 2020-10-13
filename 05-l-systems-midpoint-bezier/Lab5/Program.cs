using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GraphFunc.Tools;

namespace GraphFunc
{
    public static class Program
    {
        public static Point ToPoint(this PointF point)
            => new Point((int) point.X, (int) point.Y);

            [STAThread]
        private static void Main(string[] args)
        {
            var form = new Form(new List<ITool>
            {
                new MidpointTool(),
                new BezierTool(),
            });
            Application.Run(form);
        }
    }
}
