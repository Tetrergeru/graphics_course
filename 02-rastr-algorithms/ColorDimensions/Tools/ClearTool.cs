using System.Drawing;

namespace GraphFunc.Tools
{
    public class ClearTool : ITool
    {
        public void Stop()
        {
        }

        public void Draw(Bitmap image, Point coords, Color color)
        {
            Globals.POINTLIST = new System.Collections.Generic.LinkedList<Point>();
            using (var drawer = Graphics.FromImage(image))
                drawer.Clear(color);
        }

        public string Name() => "Clear";
    }
}