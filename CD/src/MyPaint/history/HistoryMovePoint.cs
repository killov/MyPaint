using System.Windows;

namespace MyPaint.History
{
    public class HistoryMovePoint : IHistoryNode
    {
        MovePoint point;
        public Point oldP, newP;
        public HistoryMovePoint(MovePoint c, Point o, Point n)
        {
            point = c;
            oldP = o;
            newP = n;
        }

        public void Back()
        {
            point.Move(oldP, true);
        }

        public void Forward()
        {
            point.Move(newP, true);
        }
    }
}
