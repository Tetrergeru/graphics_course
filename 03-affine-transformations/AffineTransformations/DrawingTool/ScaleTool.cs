using System;
using System.Drawing;

namespace GraphFunc.DrawingTool
{
    public class ScaleTool : IDrawingTool
    {
        protected PolygonContainer _polygonContainer;
        
        protected Point _from;

        private bool _mouseDown;
        
        public void Init(PolygonContainer polygonContainer)
        {
            _polygonContainer = polygonContainer;
        }

        public virtual void OnMouseDown(Point coordinates, Graphics drawer)
        {
            _from = coordinates;
            _mouseDown = true;
            _polygonContainer.Selected.Save();
            _polygonContainer.Draw(drawer);
        }

        public void OnMouseUp(Point coordinates, Graphics drawer)
        {
            _from = coordinates;
            _mouseDown = false;
            _polygonContainer.Draw(drawer);
        }

        public void OnMouseMove(Point coordinates, Graphics drawer)
        {
            if (!_mouseDown)
            {
                _polygonContainer.Draw(drawer);
                return;
            }

            const int factor = 100;
            _polygonContainer.Selected.Load();
            var origin = new Point(_from.X - factor, _from.Y - factor);
            var scale = (
                (coordinates.X - origin.X) / (factor * 1.0),
                (coordinates.Y - origin.Y) / (factor * 1.0));

            Scale(scale);
            
            Polygon.DrawPoint(drawer, origin, Color.Green);
            drawer.DrawLine(new Pen(Color.Cyan), origin, coordinates);
            drawer.DrawRectangle(new Pen(Color.Cyan), new Rectangle(origin.X - factor, origin.Y - factor, factor * 2, factor * 2));
            _polygonContainer.Draw(drawer);
        }

        protected virtual void Scale((double w, double h) scale)
        {
            _polygonContainer.Selected.Scale(_from, scale);
        }

        public void OnSelect(Graphics drawer)
        {
            _polygonContainer.Draw(drawer);
        }

        public bool CanUseInField()
            => true;

        public override string ToString()
            => "Scale";
    }
}