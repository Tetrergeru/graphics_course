using System.Drawing;

namespace GraphFunc.Tools
{
    public class FillColTool : ITool
    {
        public Pen FillPen = new Pen(Color.Black, 1);
        public Color _background;
        public bool flag = true;

        public void Stop()
        {
            flag = true;
        }

        public void Draw(Bitmap image, Point coords, Color color)
        {
            int start_X = coords.X;
            int start_Y = coords.Y;
            int left_board = 0;
            int right_board = 0;
            
            FillPen.Color = color;

            if (flag)
            {
                flag = false;

                _background = image.GetPixel(start_X, start_Y);
                ;
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
                    Graphics.FromImage(image).DrawLine(FillPen, new Point(left_board, start_Y), new Point(right_board, start_Y));

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

        public string Name() => "FillColor";
    }
}