using System;
using System.Diagnostics;
using System.Drawing;
using GraphFunc.Geometry;

namespace GraphFunc.Projections
{
    public class ProjectionOrthographic : IProjection
    {
        private readonly Matrix3d _projectionMatrix;

        public ProjectionOrthographic(Axis ignoredAxis)
        {
            _projectionMatrix = Matrix3d
                .One
                //.ClearAxis(ignoredAxis)
                .Rotate(ignoredAxis, Math.PI / 2);
        }

        public PointF Project(Point3 point)
        {
            var point3 = _projectionMatrix.Multiply(point);
            return new PointF(point3.X, point3.Y);
        }
    }
}