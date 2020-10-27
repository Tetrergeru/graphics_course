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
        
        public List<Point3> PointList;
        
        public readonly List<int> Points = new List<int>();
        public Point3 GetPoint(int i) => PointList[Points[i]];
        
        public Polygon(Color c, List<Point3> pointList)
        {
            Color = c;
            PointList = pointList;
        }

        public Polygon2 Project(IProjection projection)
        {
            var polygon2 = new Polygon2();
            foreach (var projected in Points.Select(i => projection.Project(PointList[i])).Where(projected => projected != null))
                polygon2.AddPoint((PointF)projected);

            return polygon2;
        }
    }
}