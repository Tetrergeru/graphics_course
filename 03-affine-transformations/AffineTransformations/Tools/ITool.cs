using System.ComponentModel;
using System.Drawing;

namespace GraphFunc.Tools
{
    public interface ITool
    {
        void OnSelect(PolygonContainer polygonContainer);
        
        void OnUse(PolygonContainer polygonContainer, Point point);

        string Name();

        bool CanUseInField();
    }
}