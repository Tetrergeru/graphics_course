using System.Drawing;
using GraphFunc.Geometry;

namespace GraphFunc.Projections
{
    public class ProjectionPerspective : IProjection
    {
        public PointF Project(Point3 point)
        {
            return new PointF(point.X / (1 + point.Z / 50), point.Y / (1 + point.Z / 50));
        }
    }
}