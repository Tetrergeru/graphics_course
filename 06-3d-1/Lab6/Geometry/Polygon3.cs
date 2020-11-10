using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GraphFunc.Projections;

namespace GraphFunc.Geometry
{
    public class Polygon
    {
        public Color Color;
        
        public readonly List<int> Points = new List<int>();
        
        public readonly List<int> Normals = new List<int>();
        
        public readonly List<int> Textures = new List<int>();
        
        public Point3 GetPoint(int i, IList<Point3> pointList) => pointList[Points[i]];

        public Point3 GetNormal(int i, IList<Point3> normalList) => normalList[Normals[i]];
        
        public PointF GetTexture(int i, IList<PointF> textureList) => textureList[Textures[i]];
        
        public Polygon(Color c)
        {
            Color = c;
        }

        public Polygon2 Project(IProjection projection, IList<Point3> pointList)
        {
            var polygon2 = new Polygon2();
            foreach (var projected in Points
                .Select(i => projection.Project3(pointList[i]))
                .Where(projected => projected.Z > 0)
                .Select(p3 => new PointF(p3.X, p3.Y))
            )
                polygon2.AddPoint(projected);

            return polygon2;
        }
    }
}