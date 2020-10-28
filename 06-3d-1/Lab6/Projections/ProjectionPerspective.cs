using System;
using System.Drawing;
using GraphFunc.Geometry;

namespace GraphFunc.Projections
{
    public class ProjectionPerspective : IProjection
    {
        public static readonly Projector Projector = new Projector(
            new Point3(0, 0, -10),
            new Point3(0, 0, 0),
            20);

        public PointF? Project(Point3 point)
            => Projector.Project(point);
    }
}