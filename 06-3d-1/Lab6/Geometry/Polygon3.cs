using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GraphFunc.Projections;

namespace GraphFunc.Geometry
{
    public class Polygon
    {
        public Color Color;
        
        public readonly List<Point3> Points = new List<Point3>();

        public Polygon(Color c)
        {
            Color = c;
        }

        public Polygon2 Project(IProjection projection)
        {
            var polygon2 = new Polygon2();
            foreach (var point in Points)
                polygon2.AddPoint(projection.Project(point));
            return polygon2;
        }

    }
}