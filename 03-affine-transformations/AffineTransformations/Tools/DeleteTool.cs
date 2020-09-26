using System.Drawing;

namespace GraphFunc.Tools
{
    public class DeleteTool : ITool
    {
        public void OnSelect(PolygonContainer polygonContainer)
        {
            polygonContainer.DeleteSelected();
        }

        public void OnUse(PolygonContainer polygonContainer, Point point)
        {
        }

        public string Name() => "Delete";
        
        public bool CanUseInField()
            => false;
    }
}