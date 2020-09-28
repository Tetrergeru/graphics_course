using System.Drawing;

namespace GraphFunc.Tools
{
    public class RotateTool : ITool
    {
        public void OnSelect(PolygonContainer polygonContainer)
        {
            polygonContainer.Selected.Rotate(new PointF(0, 0), 11);
        }

        public void OnUse(PolygonContainer polygonContainer, Point point)
        {
        }

        public string Name()
            => "Rotate";

        public bool CanUseInField()
            => false;
    }
}