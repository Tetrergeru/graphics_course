using System.Drawing;

namespace GraphFunc.Tools
{
    public interface ITool
    {
        void Stop();
        
        void Draw(Bitmap image, Point coords, Color color);

        string Name();
    }
}