using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GraphFunc.Geometry;
using GraphFunc.Projections;

namespace GraphFunc.Drawers
{
    public class ZBufferDrawer : IDrawer
    {
        public void Draw(Graphics drawer, Point screenSize, IEnumerable<Model> models, IProjection projection)
        {
            var image = new Bitmap(screenSize.X, screenSize.Y);
            var matrix = new float[screenSize.X * screenSize.Y];
            for (var i = 0; i < screenSize.X * screenSize.Y; i++)
                matrix[i] = float.PositiveInfinity;
            foreach (var model in models)
            {
                var projected = model.Applied(projection);
                foreach (var polygon in projected.Polygons)
                {
                    var (a, b, c) = (
                        polygon.GetPoint(0, projected.Points),
                        polygon.GetPoint(1, projected.Points),
                        polygon.GetPoint(2, projected.Points));
                    foreach (var (x, y, z) in Rasterize(a, b, c))
                    {
                        var newX = x + screenSize.X / 2;
                        var newY = y + screenSize.Y / 2;
                        if (newX <= 0 || newX >= screenSize.X ||
                            newY <= 0 || newY >= screenSize.Y)
                            continue;

                        var idx = newX * screenSize.X + newY;
                        if (matrix[idx] > z)
                            matrix[idx] = z;
                    }
                }
            }

            for (var x = 0; x < screenSize.X; x++)
            for (var y = 0; y < screenSize.Y; y++)
            {
                var idx = x * screenSize.X + y;

                var z = matrix[idx];
                if (float.IsPositiveInfinity(z))
                    continue;
                if (z < 0)
                    z = 0;
                if (z > 255)
                    z = 255;
                image.SetPixel(x, y, Color.FromArgb((int) z, (int) z, (int) z));
            }

            drawer.DrawImage(image, 0, 0, screenSize.X, screenSize.Y);
        }

        private IEnumerable<(int, int, float)> Rasterize(Point3 a, Point3 b, Point3 c)
        {
            if (Math.Abs(a.Y - b.Y) < 0.0001 && Math.Abs(b.Y - c.Y) < 0.0001)
                yield break;
            if (a.Y < b.Y)
                (a, b) = (b, a);
            if (a.Y < c.Y)
                (a, c) = (c, a);
            if (b.Y < c.Y)
                (b, c) = (c, b);

            var diff = (a.Y - b.Y) / (a.Y - c.Y);
            var m = new Point3(
                a.X - (a.X - c.X) * diff,
                b.Y,
                a.Z - (a.Z - c.Z) * diff);
            var (ml, mr) = b.X < c.X
                ? (b, m)
                : (m, b);

            foreach (var p in TopTriangle(a, ml, mr))
                yield return p;
            foreach (var p in BottomTriangle(ml, mr, c))
                yield return p;
        }

        private IEnumerable<(int, int, float)> TopTriangle(Point3 t, Point3 bl, Point3 br)
        {
            t.Y = (float) Math.Floor(t.Y);
            bl.Y = (float) Math.Ceiling(bl.Y);
            var height = (float) Math.Ceiling (t.Y - bl.Y);
            var width = (float) Math.Ceiling(br.X - bl.X);
            var leftStep = (bl.X - t.X) / height;
            var rightStep = (br.X - t.X) / height;
            for (var i = 0; i < height; i++)
            for (var j = (int) Math.Floor(t.X + i * leftStep); j < (int) Math.Ceiling(t.X + i * rightStep + 1); j++)
            {
                var depthHor = (br.Z - bl.Z) * (j - bl.X) / width;
                yield return (j, (int) t.Y - i,  float.PositiveInfinity);//(depthHor - t.Z) * (t.Y - i) / height
            }
        }

        private IEnumerable<(int, int, float)> BottomTriangle(Point3 tl, Point3 tr, Point3 b)
        {
            tl.Y = (float) Math.Ceiling(tl.Y);
            b.Y = (float) Math.Ceiling(b.Y);
            var height = (float) Math.Ceiling(tl.Y - b.Y);
            var width = (float) Math.Ceiling(tr.X - tl.X);
            var leftStep = (b.X - tl.X) / height;
            var rightStep = (b.X - tr.X) / height;
            for (var i = 0; i < (int) height; i++)
            for (var j = (int) Math.Floor(tl.X + i * leftStep); j < (int) Math.Ceiling(tr.X + i * rightStep + 1); j++)
            {
                var depthHor = (tr.Z - tl.Z) * (j - tl.X) / width;
                yield return (j, (int) tl.Y - i, i);//(depthHor - b.Z) * (b.Y - i) / height
            }
        }
    }
}