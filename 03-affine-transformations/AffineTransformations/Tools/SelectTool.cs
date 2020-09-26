using System.Drawing;

namespace GraphFunc.Tools
{
    public class SelectTool : ITool
    {
        public void OnSelect(PolygonContainer polygonContainer)
        {
        }

        public void OnUse(PolygonContainer polygonContainer, Point point)
        {
            var i = 0;
            foreach (var polygon in polygonContainer.Polygons)
            {
                if (polygon.HasPoint(point))
                {
                    polygonContainer.Select(i);
                    return;
                }
                i++;
            }
        }
        
        public string Name()
            => "Select";
        
        public bool CanUseInField()
            => true;
    }
}