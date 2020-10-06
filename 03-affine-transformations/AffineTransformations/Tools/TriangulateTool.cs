using System.Drawing;

namespace GraphFunc.Tools
{
    public class TriangulateTool : ITool
    {
        public void OnSelect(PolygonContainer polygonContainer)
        {
            polygonContainer.TriangulateSelected();
        }

        public void OnUse(PolygonContainer polygonContainer, Point point)
        {
        }

        public bool CanUseInField() => false;

        public override string ToString()
            => "Triangulate";
    }
}