using System;
using System.Drawing;

namespace GraphFunc.DrawingTool
{
    public class MoveTool : IDrawingTool
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

            _polygonContainer.Selected.Load();
            _polygonContainer.Selected.Move(new PointF(coordinates.X - _from.X, coordinates.Y - _from.Y));

            Polygon.DrawPoint(drawer, _from, Color.Green);
            drawer.DrawLine(new Pen(Color.Cyan), _from, coordinates);
            _polygonContainer.Draw(drawer);
        }

        public void OnSelect(Graphics drawer)
        {
            _polygonContainer.Draw(drawer);
        }

        public bool CanUseInField()
            => true;

        public override string ToString()
            => "Move";
    }
}