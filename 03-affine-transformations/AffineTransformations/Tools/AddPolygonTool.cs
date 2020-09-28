using System.Drawing;

namespace GraphFunc.Tools
{
    public class AddPolygonTool : ITool
    {
        public void OnSelect(PolygonContainer polygonContainer)
        {
            polygonContainer.AddPolygon();
        }

        public void OnUse(PolygonContainer polygonContainer, Point point)
        {
        }
        
        public bool CanUseInField()
            => false;
        
        public override string ToString()
            => "Add polygon";
    }
}