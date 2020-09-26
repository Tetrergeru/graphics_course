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

        public string Name()
            => "Add polygon";
        
        public bool CanUseInField()
            => false;
    }
}