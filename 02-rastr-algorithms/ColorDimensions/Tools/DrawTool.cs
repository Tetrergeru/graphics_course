using System.Drawing;
using System.Drawing.Drawing2D;

namespace GraphFunc.Tools
{
    public class DrawTool : ITool
    {
        public void Stop()
        {
        }

        public void Draw(Bitmap image, Point coords, Color color)
        {
            // Create graphics path object and add ellipse.
            GraphicsPath graphPath = new GraphicsPath();
            graphPath.AddEllipse(coords.X - 100, coords.Y - 50, 200, 100);

            // Create pen.
            Pen EllPen = new Pen(color, 3);

            // Draw graphics path to screen.
            Graphics.FromImage(image).DrawPath(EllPen, graphPath);
        }

        public string Name() => "DrawEllipse";
    }
}
