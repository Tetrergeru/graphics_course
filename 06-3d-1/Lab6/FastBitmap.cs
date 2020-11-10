using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Dynamic;
using System.Windows.Forms;

namespace GraphFunc
{
    public unsafe class FastBitmap : System.IDisposable
    {
        private readonly Bitmap _source;

        public readonly int Width;

        public readonly int Height;

        private readonly BitmapData _bData;

        private readonly byte* _scan0;

        public int Count => _source.Height * _source.Width;

        public Color GetI(int i)
        {
            var data = _scan0 + i * 4;
            return Color.FromArgb(data[2], data[1], data[0]);
        }

        public void SetI(int i, (byte r, byte g, byte b) cl)
        {
            var data = _scan0 + i * 4;
            (data[2], data[1], data[0]) = cl;
        }

        public void SetPixel(Point p, (byte r, byte g, byte b) cl)
            => SetI(p.X + p.Y * Width, cl);
        
        public Color GetPixel(int x, int y)
            => GetI(x + y * Width);

        public FastBitmap(Bitmap bitmap)
        {
            Width = bitmap.Width;
            Height = bitmap.Height;
            _source = bitmap;
            _bData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite,
                bitmap.PixelFormat);
            _scan0 = (byte*) _bData.Scan0.ToPointer();
        }

        public void Dispose()
        {
            _source.UnlockBits(_bData);
        }
    }
}