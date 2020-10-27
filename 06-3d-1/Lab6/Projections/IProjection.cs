using System.Drawing;
using GraphFunc.Geometry;

namespace GraphFunc.Projections
{
    public interface IProjection
    {
        PointF Project(Point3 point);
    }
}