﻿using System;
using System.Drawing;
using System.Windows.Forms;
using GraphFunc.Projections;

namespace GraphFunc
{
    public static class Program
    {
        public static Point ToPoint(this PointF point)
            => new Point((int) point.X, (int) point.Y);

        [STAThread]
        private static void Main(string[] args)
        {
            Console.WriteLine(ProjectionIsometric.Matrix);
            var form = new Form();
            Application.Run(form);
        }
    }
}
