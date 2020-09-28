using System;
using System.Drawing;
using GraphFunc.Tools;

namespace GraphFunc.DrawingTool
{
    public class DrawingToolWrapper : IDrawingTool
    {
        private PolygonContainer _polygonContainer;
        
        private readonly ITool _tool;

        public DrawingToolWrapper(ITool tool)
        {
            _tool = tool;
        }

        public void Init(PolygonContainer polygonContainer)
        {
            _polygonContainer = polygonContainer;
        }
        
        public void OnMouseUp(Point coordinates, Graphics drawer)
        {
            _tool.OnUse(_polygonContainer, coordinates);
            _polygonContainer.Draw(drawer);
        }
        
        public void OnMouseMove(Point coordinates, Graphics drawer)
        {
            _polygonContainer.Draw(drawer);
        }

        public void OnMouseDown(Point coordinates, Graphics drawer)
        {
            _polygonContainer.Draw(drawer);
        }

        public void OnSelect(Graphics drawer)
        {
            _tool.OnSelect(_polygonContainer);
            _polygonContainer.Draw(drawer);
        }

        public bool CanUseInField()
            => _tool.CanUseInField();

        public override string ToString()
            => _tool.ToString();
    }
}