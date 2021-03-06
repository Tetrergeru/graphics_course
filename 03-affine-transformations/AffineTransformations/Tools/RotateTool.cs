﻿using System.Drawing;

namespace GraphFunc.Tools
{
    public class RotateTool : ITool
    {
        public void OnSelect(PolygonContainer polygonContainer)
        {
            polygonContainer.Selected.Rotate(new PointF(100, 100), 90);
        }

        public void OnUse(PolygonContainer polygonContainer, Point point)
        {
        }
        
        public bool CanUseInField()
            => true;
        
        public override string ToString()
            => "Rotate";
    }
}