namespace GraphFunc.Geometry
{
    public class Point3
    {
        public readonly float X;
        
        public readonly float Y;
        
        public readonly float Z;

        public readonly float W;

        public Point3(float x, float y, float z, float w = 1)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }
}