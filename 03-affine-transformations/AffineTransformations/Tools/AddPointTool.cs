using System.Drawing;

namespace GraphFunc.Tools
{
    public class AddPointTool : ITool
    {
        public void OnSelect(PolygonContainer polygonContainer)
        {
        }

        public void OnUse(PolygonContainer polygonContainer, Point point)
        {
            polygonContainer.AddPoint(point);
        }

        public string Name()
            => "Add vertex";

        public bool CanUseInField()
            => true;
    }
}