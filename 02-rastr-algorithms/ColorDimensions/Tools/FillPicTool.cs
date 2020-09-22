using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Web;

namespace GraphFunc.Tools
{
    public class FillPicTool : ITool
    {
        public Pen FillPen = new Pen(Color.Black, 1);
        public Color _background;
        public Bitmap _fill_image;
        public bool flag = true;
        public bool pic_flag = true;

        public void LoadImage()
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                RestoreDirectory = true,
                ShowHelp = true,
            };
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK || dialog.SafeFileName == null)
                return;
            _fill_image = new Bitmap(dialog.FileName);
        }
        public void CopyLine(Bitmap image, int start, int fin, int y)
        {
            for (int i = start; i < fin; i++)
                image.SetPixel(i, y, _fill_image.GetPixel(i % _fill_image.Width, y % _fill_image.Height));
        }
        public void Stop()
        {
            flag = true;
        }
        public void Draw(Bitmap image, Point coords, Color color)
        {
            if (pic_flag)
            {
                pic_flag = false;
                LoadImage();
            }

            int start_X = coords.X;
            int start_Y = coords.Y;
            int left_board = 0;
            int right_board = 0;

            if (flag)
            {
                flag = false;
                _background = image.GetPixel(start_X, start_Y);
            }
            if (start_X >= 0 && start_X < image.Width && start_Y >= 0 && start_Y < image.Height)
            {
                if (image.GetPixel(start_X, start_Y) == _background)
                {
                    while (image.GetPixel(start_X, start_Y) == _background && start_X > 1)
                    {
                        start_X -= 1;
                    }
                    left_board = start_X + 1;
                    start_X = coords.X;
                    while (image.GetPixel(start_X, start_Y) == _background && start_X < image.Width - 1)
                    {
                        start_X += 1;
                    }
                    right_board = start_X - 1;

                    CopyLine(image, left_board, right_board, start_Y);

                    for (int i = left_board + 1; i < right_board; i++)
                    {
                        Draw(image, new Point(i, start_Y + 1), color);
                    }
                    for (int i = left_board + 1; i < right_board; i++)
                    {
                        Draw(image, new Point(i, start_Y - 1), color);
                    }
                }
            }
        }

        public string Name() => "FillPicture";
    }
}
