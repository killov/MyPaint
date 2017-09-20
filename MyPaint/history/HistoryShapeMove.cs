using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyPaint
{
    public class HistoryShapeMove : IHistoryNode
    {
        public IMyShape shape;
        Point o, n;
        public HistoryShapeMove(IMyShape s, Point oldPosition, Point newPosiotion)
        {
            shape = s;
            o = oldPosition;
            n = newPosiotion;
        }

        public void back()
        {
            shape.moveShape(o.X, o.Y);
        }

        public void forward()
        {
            shape.moveShape(n.X, n.Y);
        }
    }
}
