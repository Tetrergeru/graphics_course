using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GraphFunc.DrawingTool;
using GraphFunc.Tools;

namespace GraphFunc
{
    public static class Program
    {
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
            });
            Application.Run(form);
        }
    }
}
