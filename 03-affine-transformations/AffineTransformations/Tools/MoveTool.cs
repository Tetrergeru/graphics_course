using System.Drawing;

namespace GraphFunc.Tools
{
    public class MoveTool : ITool
    {
        public void OnSelect(PolygonContainer polygonContainer)
        {
            polygonContainer.Selected.Move(new PointF(2, 2));
        }

        public void OnUse(PolygonContainer polygonContainer, Point point)
        {
        }

        public bool CanUseInField()
            => true;

        public override string ToString()
            => "Move";
    }
}