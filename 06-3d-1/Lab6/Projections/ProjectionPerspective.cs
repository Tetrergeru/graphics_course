using System;
using System.Drawing;
using GraphFunc.Geometry;

namespace GraphFunc.Projections
{
    public class ProjectionPerspective : IProjection
    {
        public static Matrix3d Matrix = Matrix3d
                .One
                .Rotate(Axis.X, Math.PI/2 + Math.PI/12)
                .Rotate(Axis.Y, -Math.PI/12)
            ;
        
        public PointF Project(Point3 point)
        {
            var point3 = Matrix.Multiply(point);
            return new PointF(point3.X / (point3.Z / 10), point3.Y / (point3.Z / 10));
        }
    }
}