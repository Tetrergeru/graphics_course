using System.Drawing;

namespace GraphFunc.Tools
{
    public class ClearTool : ITool
    {
        public void OnSelect(PolygonContainer polygonContainer)
        {
            polygonContainer.Clear();
        }

        public void OnUse(PolygonContainer polygonContainer, Point point)
        {
        }
        
        public bool CanUseInField()
            => false;
        
        public override string ToString()
            => "Clear";
    }
}