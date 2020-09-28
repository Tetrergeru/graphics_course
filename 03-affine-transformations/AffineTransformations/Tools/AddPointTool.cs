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

        public bool CanUseInField()
            => true;
        
        public override string ToString()
            => "Add point";
    }
}