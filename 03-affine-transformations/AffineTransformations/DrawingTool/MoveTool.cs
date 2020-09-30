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

            const int factor = 1;
            _polygonContainer.Selected.Load();
            var origin = new Point(_from.X - factor, _from.Y - factor);

            Move();

            Polygon.DrawPoint(drawer, origin, Color.Green);
            drawer.DrawLine(new Pen(Color.Cyan), origin, coordinates);
            drawer.DrawRectangle(new Pen(Color.Cyan), new Rectangle(origin.X - factor, origin.Y - factor, factor * 2, factor * 2));
            _polygonContainer.Draw(drawer);
        }

        protected virtual void Move()
        {
            _polygonContainer.Selected.Move(_from);
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