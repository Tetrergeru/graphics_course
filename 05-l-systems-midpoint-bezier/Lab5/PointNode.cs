using System;
using System.Drawing;

namespace GraphFunc
{
    public class PointNode
    {
        public PointF Point;
        
        public PointNode Prev, Next;

        public void Delete()
        {
            Next.Prev = Prev;
            Prev.Next = Next;
        }
        
        public PointNode FindInternalPoint()
        {
            var edge = new Edge(Prev.Point, Next.Point);
            var right = new Edge(Point, Next.Point);
            var left = new Edge(Prev.Point, Point);
            var current = Prev.Prev;
            
            PointNode chosen = null;
            var min = double.MaxValue;

            for (; current != Next; current = current.Prev)
            {
                var distance = edge.DistanceTo(current.Point);
                if (edge.Classify(current.Point) == Edge.Position.RIGHT 
                    && right.Classify(current.Point) == Edge.Position.LEFT
                    && left.Classify(current.Point) == Edge.Position.RIGHT
                    && distance < min)
                {
                    min = distance;
                    chosen = current;
                }
            }

            return chosen;
        }
        
        public (PointNode, PointNode) DivideBy(PointNode internalPoint)
        {
            var internalForPrev = new PointNode
            {
                Point = internalPoint.Point,
                Next = internalPoint.Next,
            };
            var pointForPrev = new PointNode
            {
                Point = Point,
                Prev = Prev,
                Next = internalForPrev,
            };
            internalForPrev.Prev = pointForPrev;
            internalForPrev.Next.Prev = internalForPrev;
            pointForPrev.Prev.Next = pointForPrev;
                
            var pointForNext = this;
            var internalForNext = internalPoint;
            internalForNext.Next = pointForNext;
            pointForNext.Prev = internalForNext;

            return (pointForPrev, pointForNext);
        }

        public PointNode SelectFirst()
        {
            var polygon = this;
            while (true)
            {
                var edge = new Edge(polygon.Prev.Point, polygon.Next.Point);
                if (edge.Classify(Point) == Edge.Position.RIGHT)
                    return polygon;
                polygon = polygon.Next;
            }
        }
    }
}