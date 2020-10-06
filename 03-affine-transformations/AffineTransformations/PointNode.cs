using System.Drawing;

namespace GraphFunc
{
    public class PointNode
    {
        public Point Point;
        
        public PointNode Prev, Next;

        public void Delete()
        {
            Next.Prev = Prev;
            Prev.Next = Next;
        }
        
        public PointNode FindInternalPoint()
        {
            var edge = new Edge(Prev.Point, Next.Point);
            var current = Prev.Prev;
            
            PointNode chosen = null;
            var max = double.MinValue;

            for (; current != Next; current = current.Prev)
            {
                var distance = edge.DistanceTo(current.Point);
                if (distance > max)
                {
                    max = distance;
                    chosen = current;
                }
            }

            if (chosen == null)
                return null;
            
            return edge.DistanceTo(chosen.Point) > 0 
                ? chosen 
                : null;
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
    }
}