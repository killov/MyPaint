using System.Windows;

namespace MyPaint.History
{
    public class HistoryShapeMove : IHistoryNode
    {
        public Shapes.Shape shape;
        Point o, n;
        public HistoryShapeMove(Shapes.Shape s, Point oldPosition, Point newPosiotion)
        {
            shape = s;
            o = oldPosition;
            n = newPosiotion;
        }

        public void Back()
        {
            shape.MoveShape(o);
        }

        public void Forward()
        {
            shape.MoveShape(n);
        }
    }
}
