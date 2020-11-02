using System.Collections.Generic;
using System.Drawing;
using GraphFunc.Geometry;
using GraphFunc.Projections;

namespace GraphFunc.Drawers
{
    public interface IDrawer
    {
        void Draw(Graphics drawer, Point screenSize, IEnumerable<Model> models, IProjection projection);
    }
}