using System;
using System.Collections.Generic;
using System.Drawing;
using GraphFunc.Geometry;
using GraphFunc.Projections;

namespace GraphFunc.Drawers
{
    public class ZBufferDrawer : IDrawer
    {
        public void Draw(Graphics drawer, Point screenSize, IEnumerable<Model> models, IProjection projection)
        {
            //foreach (var model in models)
        }

        private Model Project(Model model, IProjection projection)
        {
            throw new Exception();
        }
    }
}