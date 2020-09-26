using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GraphFunc.Tools;

namespace GraphFunc
{
    public static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var form = new Form(new List<ITool>
            {
                new AddPolygonTool(),
                new ClearTool(),
                new AddPointTool(),
                new SelectTool(),
            });
            Application.Run(form);
        }
    }
}
