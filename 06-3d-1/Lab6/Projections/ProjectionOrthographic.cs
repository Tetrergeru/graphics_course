using System;
using System.Diagnostics;
using System.Drawing;
using GraphFunc.Geometry;

namespace GraphFunc.Projections
{
    public class ProjectionOrthographic : IProjection
    {
        private readonly Projector _projector;

        public ProjectionOrthographic(Axis ignoredAxis)
        {
            _projector = new Projector(
                new Point3(0, 0, 0),
                ignoredAxis switch
                {
                    Axis.X => new Point3(0, (float) Math.PI / 2, 0),
                    Axis.Y => new Point3((float) Math.PI / 2, 0, 0),
                    Axis.Z => new Point3(0, 0, 0),
                },
                float.PositiveInfinity);
        }

        public PointF? Project(Point3 point)
            => _projector.Project(point);

        public Matrix3d GetMatrix()
            => _projector.Matrix;
    }
}