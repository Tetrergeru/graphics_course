using System.Drawing;
using GraphFunc.Geometry;

namespace GraphFunc.Projections
{
    public interface IProjection
    {
        Point3 Project3(Point3 point);

        Point3 ProjectNormal(Point3 normal);
    }
}