using System.Drawing;
using GraphFunc.Tools;

namespace GraphFunc.DrawingTool
{
    public class ScaleCenterTool : ScaleTool
    {
        protected PointF Center;
        
        protected override void Scale((double w, double h) scale)
        {
            _polygonContainer.Selected.Scale(Center, scale);
        }

        public override void OnMouseDown(Point coordinates, Graphics drawer)
        {
            Center = _polygonContainer.Selected.Center();
            base.OnMouseDown(coordinates, drawer);
        }

        public override string ToString()
            => "Scale center";
    }
}