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

        public Point3 Project3(Point3 point)
        {
            var point3 = _matrix.Multiply(point);

            if (float.IsInfinity(ScreenDistance))
                return new Point3(point3.X, point3.Y, point3.Z);

            var result = new Point3(
                point3.X * ScreenDistance / (point3.Z + ScreenDistance),
                point3.Y * ScreenDistance / (point3.Z + ScreenDistance),
                point3.Z);

            return result;
        }

        public Point3 ProjectNormal(Point3 normal)
        {
            var point3 = _matrix.Multiply(normal) - _matrix.Multiply(new Point3(0,0,0));
            point3 *= 1 / point3.Length();
            
            return point3;
        }
        
        private void MakeMatrix()
        {
            _matrix = Matrix3d
                    .One
                    .Move(new Point3(-Location.X, -Location.Y, -Location.Z))
                    .Move(new Point3(0, 0, _screenDistance))
                    .Rotate(Axis.X, Transform.X)
                    .Rotate(Axis.Y, Transform.Y)
                    //.Rotate(Axis.Z, Transform.Z)
                    .Move(new Point3(0, 0, -_screenDistance))
                ;
        }
    }
}