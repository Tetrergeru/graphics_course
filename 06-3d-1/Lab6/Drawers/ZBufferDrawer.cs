using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using GraphFunc.Geometry;
using GraphFunc.Projections;

namespace GraphFunc.Drawers
{
    public class ZBufferDrawer : IDrawer
    {
        public static Point3 Light = new Point3(0, 0, -1);
        private const float Ka = 0.3f;

        private const float Ia = 0.1f;
        
        private const float Ks = 0.3f;
        
        private const float Kd = 0.3f;
        
        public void Draw(Graphics drawer, Point screenSize, IEnumerable<Model> models, IProjection projection)
        {
            var image = new Bitmap(screenSize.X, screenSize.Y);
            var matrix = new float[screenSize.X * screenSize.Y];
            var shades = new float[screenSize.X * screenSize.Y];
            var colors = new Color[screenSize.X * screenSize.Y];
            for (var i = 0; i < screenSize.X * screenSize.Y; i++)
            {
                matrix[i] = float.PositiveInfinity;
                shades[i] = float.PositiveInfinity;
            }

            foreach (var model in models)
            {
                var projected = model.Applied(projection);
                var (w, h) = (projected.Texture.Width, projected.Texture.Height);
                using (var texture = new FastBitmap(projected.Texture.Clone(new Rectangle(0, 0, projected.Texture.Width, projected.Texture.Height), PixelFormat.Format32bppArgb)))
                {
                    projected.Polygons.AsParallel().ForAll(polygon =>
                    {
                        for (var j = 1; j < polygon.Points.Count - 1; j++)
                        {
                            var (a, b, c) = (
                                polygon.GetPoint(0, projected.Points),
                                polygon.GetPoint(j, projected.Points),
                                polygon.GetPoint(j + 1, projected.Points));
                            var (aData, bData, cData) = (
                                new Data(a.Z, polygon.GetNormal(0, projected.Normals),
                                    polygon.GetTexture(0, projected.TextureCoords)),
                                new Data(b.Z, polygon.GetNormal(j, projected.Normals),
                                    polygon.GetTexture(j, projected.TextureCoords)),
                                new Data(c.Z, polygon.GetNormal(j + 1, projected.Normals),
                                    polygon.GetTexture(j + 1, projected.TextureCoords))
                            );
                            foreach (var (oldX, oldY, data) in Rasterize(a, b, c, aData, bData, cData))
                            {
                                var x = oldX + screenSize.X / 2;
                                var y = oldY + screenSize.Y / 2;
                                if (x <= 0 || x >= screenSize.X ||
                                    y <= 0 || y >= screenSize.Y)
                                    continue;
                                var z = data.Z;
                                var idx = x * screenSize.X + y;

                                if (matrix[idx] <= z)
                                    continue;

                                matrix[idx] = z;
                                var normal = data.Normal * (1 / data.Normal.Length());
                                var shade = Ka * Ia + Kd * (normal * Light) +
                                            Ks * Math.Pow(normal * (normal * (2 * (Light * normal)) - Light), 80);
                                shades[idx] = (float) shade;
                                colors[idx] = texture.GetPixel(
                                    (int) (data.TextureCoordinate.X * w),
                                    (int) ((1 - data.TextureCoordinate.Y) * h));
                                //Console.WriteLine(colors[idx]);
                            }
                        }
                    });
                }
            }

            var maxShade = float.MinValue;
            var minShade = float.MaxValue;
            for (var x = 0; x < screenSize.X; x++)
            for (var y = 0; y < screenSize.Y; y++)
            {
                var idx = x * screenSize.X + y;
                if (float.IsPositiveInfinity(shades[idx]))
                    continue;
                if (shades[idx] > maxShade)
                    maxShade = shades[idx];
                if (shades[idx] < minShade)
                    minShade = shades[idx];
            }

            for (var x = 0; x < screenSize.X; x++)
            for (var y = 0; y < screenSize.Y; y++)
            {
                var idx = x * screenSize.X + y;

                var shade = shades[idx] * 1.5f;
                if (float.IsPositiveInfinity(shade))
                    continue;
                //shade -= minShade;
                //shade /= maxShade - minShade;
                var color = colors[idx];
                var r = Bytize(color.R * shade);
                var g = Bytize(color.G * shade);
                var b = Bytize(color.B * shade );// * shade
                //r = Bytize(shade * 255);
                //g = r;
                //b = r;
                image.SetPixel(x, y, Color.FromArgb(r, g, b));
            }
            drawer.DrawImage(image, 0, 0, screenSize.X, screenSize.Y);
            Console.WriteLine("1");
        }

        private static byte Bytize(float number)
        {
            if (number <= 0)
                number = 0;
            if (number >= 255)
                number = 255;
            return (byte)number;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<(int, int, Data)> Rasterize(Point3 a, Point3 b, Point3 c,  Data aData, Data bData, Data cData)
        {
            if (Math.Abs(a.Y - b.Y) < 0.000001 && Math.Abs(b.Y - c.Y) < 0.000001)
                return new(int, int, Data)[]{};
            
            if (a.Y < b.Y)
                (a, b, aData, bData) = (b, a, bData, aData);
            if (a.Y < c.Y)
                (a, c, aData, cData) = (c, a, cData, aData);
            if (b.Y < c.Y)
                (b, c, bData, cData) = (c, b, cData, bData);

            var diff = (a.Y - b.Y) / (a.Y - c.Y);
            var m = new Point3(a.X - (a.X - c.X) * diff, b.Y, a.Z - (a.Z - c.Z) * diff);
            var mData = aData - (aData - cData) * diff;
            var (ml, mr, mlData, mrData) = b.X < m.X
                ? (b, m, bData, mData)
                : (m, b, mData, bData);

            return TopTriangle(a, ml, mr, aData, mlData, mrData)
                .Concat(BottomTriangle(ml, mr, c, mlData, mrData, cData));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<(int, int, Data)> TopTriangle(Point3 t, Point3 bl, Point3 br, Data tData, Data blData, Data brData)
            => Trapezoid(t, t, bl, br, tData, tData, blData, brData);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<(int, int, Data)> BottomTriangle(Point3 tl, Point3 tr, Point3 b, Data tlData, Data trData, Data bData)
            => Trapezoid(tl, tr, b, b, tlData, trData, bData, bData);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<(int, int, Data)> Trapezoid(Point3 tl, Point3 tr, Point3 bl, Point3 br, Data tlData, Data trData, Data blData, Data brData)
        {
            tl.Y = (float) Math.Ceiling(tl.Y);
            bl.Y = (float) Math.Floor(bl.Y);
            
            tl.X = (float) Math.Floor(tl.X);
            bl.X = (float) Math.Floor(bl.X);
            tr.X = (float) Math.Ceiling(tr.X);
            br.X = (float) Math.Ceiling(br.X);

            var height = (int) (tl.Y - bl.Y);

            var leftDataStep = Step(tlData, blData, height);
            var rightDataStep = Step(trData, brData, height);
            var leftData = tlData;
            var rightData = trData;

            var xLeftStep = Step(tl.X, bl.X, height);
            var xRightStep = Step(tr.X, br.X, height);

            var xLeft = tl.X;
            var xRight = tr.X;
            for (var i = 0; i < height; i++)
            {
                var y = (int) tl.Y - i;

                var xStart = (int) Math.Floor(xLeft);
                var xFinish = (int) Math.Ceiling(xRight);

                var zStep = Step(leftData, rightData, xFinish - xStart);
                var data = leftData;
                for (var x = xStart; x <= xFinish; x++)
                {
                    yield return (x, y, data);
                    data += zStep;
                }

                leftData += leftDataStep;
                rightData += rightDataStep;

                xLeft += xLeftStep;
                xRight += xRightStep;
            }
        }

        private static float Step(float begin, float end, int steps)
            => (end - begin) / steps;
        
        private static Data Step(Data begin, Data end, int steps)
            => (end - begin) / steps;
    }

    struct Data
    {
        public float Z;
        
        public Point3 Normal;

        public PointF TextureCoordinate;

        public Data(float z, Point3 normal, PointF textureCoordinate)
        {
            Z = z;
            Normal = normal;
            TextureCoordinate = textureCoordinate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Data operator +(Data a, Data b)
            => new Data(a.Z + b.Z, a.Normal + b.Normal, AddPointF(a.TextureCoordinate, b.TextureCoordinate));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Data operator -(Data a, Data b)
            => new Data(a.Z - b.Z, a.Normal - b.Normal, SubPointF(a.TextureCoordinate, b.TextureCoordinate));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Data operator *(Data a, float b)
            => new Data(a.Z * b, a.Normal * b, new PointF(a.TextureCoordinate.X * b, a.TextureCoordinate.Y * b));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Data operator /(Data a, float b)
            => a * (1 / b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static PointF AddPointF(PointF a, PointF b)
            => new PointF(a.X + b.X, a.Y + b.Y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static PointF SubPointF(PointF a, PointF b)
            => new PointF(a.X - b.X, a.Y - b.Y);
    }
}