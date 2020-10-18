using System;
using System.Diagnostics;
using System.Drawing;
using GraphFunc.Geometry;

namespace GraphFunc.Projections
{
    public class ProjectionOrthographic : IProjection
    {
        private Func<Point3, PointF> _projector;
        
        public ProjectionOrthographic(Axis ignoredAxis)
        {
            _projector = ignoredAxis switch
            {
                Axis.X => p3 => new PointF(p3.Y, p3.Z),
                Axis.Y => p3 => new PointF(p3.X, p3.Z),
                Axis.Z => p3 => new PointF(p3.X, p3.Y),
                _ => throw new ArgumentException($"Invalid value of Axis: {ignoredAxis}"),
            };
        }

        public PointF Project(Point3 point)
            => _projector(point);
    }
}