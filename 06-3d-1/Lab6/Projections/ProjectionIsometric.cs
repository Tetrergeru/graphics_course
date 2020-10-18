using System;
using System.Drawing;
using GraphFunc.Geometry;

namespace GraphFunc.Projections
{
    public class ProjectionIsometric : IProjection
    {
        public static readonly Matrix3d Matrix = Matrix3d
            .One
            .Rotate(Axis.X, Math.PI/2 + Math.PI/12)
            .Rotate(Axis.Y, -Math.PI/12)
            .Set(0, 2, 0)
            .ClearAxis(Axis.Z)
            ;
        
        public PointF Project(Point3 point)
        {
            var point3 = Matrix.Multiply(point);
            return new PointF(point3.X, point3.Y);
        }
    }
}