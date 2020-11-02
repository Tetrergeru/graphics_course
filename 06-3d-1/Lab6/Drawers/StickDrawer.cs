using System.Collections.Generic;
using System.Drawing;
using GraphFunc.Geometry;
using GraphFunc.Projections;

namespace GraphFunc.Drawers
{
    public class StickDrawer : IDrawer
    {
        public void Draw(Graphics drawer, Point screenSize, IEnumerable<Model> models, IProjection projection)
        {
            foreach (var model in models)
            foreach (var polygon in model.Polygons)
                polygon
                    .Project(projection)
                    .Scale(new PointF(0, 0), (20, 20))
                    .Move(new PointF(screenSize.X / 2f, screenSize.Y / 2f))
                    .Draw(drawer, polygon.Color);
        }
    }
}