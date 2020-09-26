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

        public string Name()
            => "Clear";
        
        public bool CanUseInField()
            => false;
    }
}