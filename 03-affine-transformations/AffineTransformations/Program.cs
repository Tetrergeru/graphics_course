using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GraphFunc.DrawingTool;
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
            var form = new Form(new List<IDrawingTool>
            {
                new DrawingToolWrapper(new AddPolygonTool()),
                new DrawingToolWrapper(new ClearTool()),
                new DrawingToolWrapper(new AddPointTool()),
                new DrawingToolWrapper(new SelectTool()),
                new LeftRightTool(),
                new IntersectTool(),
                new ScaleTool(),
                new ScaleCenterTool(),
                new DrawingTool.RotateTool(),
                new RotateCenterTool(),
                new DrawingTool.MoveTool(),
                new DrawingToolWrapper(new DeleteTool()),
            });
            Application.Run(form);
        }
    }
}
