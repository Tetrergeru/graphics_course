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
        public Point3 GetPoint(int i, IList<Point3> pointList) => pointList[Points[i]];
        
        public Polygon(Color c)
        {
            Color = c;
        }

        public Polygon2 Project(IProjection projection, IList<Point3> pointList)
        {
            var polygon2 = new Polygon2();
            foreach (var projected in Points.Select(i => projection.Project(pointList[i])).Where(projected => projected != null))
                polygon2.AddPoint((PointF)projected);

            return polygon2;
        }
    }
}