using System;

namespace GraphFunc.Geometry
{
    public class Point3
    {
        public float X;
        
        public float Y;
        
        public float Z;

        public float W;

        public Point3(float x, float y, float z, float w = 1)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }
}