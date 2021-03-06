﻿using System;
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
        public Point start_point;

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
            int pic_x, pic_y;

            if (y >= start_point.Y)
                pic_y = (y - start_point.Y) % _fill_image.Height;
            else
                pic_y = _fill_image.Height - ((start_point.Y - y) % _fill_image.Height);

            for (int i = start; i < fin; i++)
            {
                if (i >= start_point.X)
                    pic_x = (i - start_point.X) % _fill_image.Width;
                else
                    pic_x = _fill_image.Width - ((start_point.X - i) % _fill_image.Width);
                
                image.SetPixel(i, y, _fill_image.GetPixel(pic_x, pic_y));
            }
        }
        public void Stop()
        {
            flag = true;
            pic_flag = true;
        }
        public void Draw(Bitmap image, Point coords, Color color)
        {
            if (pic_flag)
            {
                pic_flag = false;
                start_point = coords;
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
