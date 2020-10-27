using System;
using System.Drawing;
using System.Security.Permissions;
using GraphFunc.Geometry;

namespace GraphFunc.Projections
{
    public class Projector
    {
        private Matrix3d _matrix;

        private Point3 _location;

        private Point3 _transform;

        // If PositiveInfinity => parallel projection
        private float _screenDistance;

        public Point3 Location
        {
            get => _location;
            set
            {
                _location = value;
                MakeMatrix();
            }
        }

        public Point3 Transform
        {
            get => _transform;
            set
            {
                _transform = value;
                MakeMatrix();
            }
        }

        public float ScreenDistance
        {
            get => _screenDistance;
            set
            {
                _screenDistance = value;
                MakeMatrix();
            }
        }

        public Projector(Point3 location, Point3 transform, float screenDistance)
        {
            _location = location;
            _transform = transform;
            _screenDistance = screenDistance;
            MakeMatrix();
        }

        public void Move(Axis axis, float distance)
            => Location = Location.Moved(axis, distance);

        public void Rotate(Axis axis, float angle)
            => Transform = Transform.Moved(axis, angle);
        
        public PointF? Project(Point3 point)
        {
            var point3 = _matrix.Multiply(point);
            if (point.Distance(_transform) < 1)
                return null;
            
            if (float.IsInfinity(ScreenDistance)) 
                return new PointF(point3.X, point3.Y);
            
            var result =  new PointF(
                point3.X * ScreenDistance / Math.Abs(point3.Z + ScreenDistance),
                point3.Y * ScreenDistance / Math.Abs(point3.Z + ScreenDistance));
            
            if (float.IsNaN(result.X) ||
                float.IsNaN(result.X) || 
                float.IsInfinity(result.X) ||
                float.IsInfinity(result.Y))
                return new PointF(point.X, point.Y);
            
            return result;
        }

        private void MakeMatrix()
        {
            _matrix = Matrix3d
                    .One
                    .Rotate(Axis.X, Transform.X)
                    .Rotate(Axis.Y, Transform.Y)
                    .Rotate(Axis.Z, Transform.Z)
                    .Move(Location)
                ;
        }
    }
}