using System;
using System.Drawing;
using GraphFunc.Geometry;

namespace GraphFunc.Projections
{
    public class ProjectionPerspective : IProjection
    {
        public static readonly Projector Projector = new Projector(
            new Point3(0, 0, -100),
            new Point3(0, 0, 0),
            200);

        public Point3 Project3(Point3 point)
            => Projector.Project3(point);
        
        public Point3 ProjectNormal(Point3 normal)
            => Projector.ProjectNormal(normal);
    }
}