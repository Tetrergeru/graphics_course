using System.Drawing;

namespace GraphFunc.DrawingTool
{
    public class RotateCenterTool : RotateTool
    {
        protected PointF Center;
        
        protected override void Rotate(double angle)
        {
            _polygonContainer.Selected.Rotate(Center, angle);
        }

        public override void OnMouseDown(Point coordinates, Graphics drawer)
        {
            Center = _polygonContainer.Selected.Center();
            base.OnMouseDown(coordinates, drawer);
        }

        public override string ToString()
            => "Rotate center";
    }
}