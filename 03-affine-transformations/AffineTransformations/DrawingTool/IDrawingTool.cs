using System.Drawing;
using GraphFunc.Tools;

namespace GraphFunc.DrawingTool
{
    public interface IDrawingTool
    {
        void Init(PolygonContainer polygonContainer);
        
        void OnMouseDown(Point coordinates, Graphics drawer);

        void OnMouseUp(Point coordinates, Graphics drawer);

        void OnMouseMove(Point coordinates, Graphics drawer);

        void OnSelect(Graphics drawer);

        bool CanUseInField();
    }
}