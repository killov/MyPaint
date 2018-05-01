using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyPaint.History
{
    public class HistoryShapePosition : IHistoryNode
    {
        public Shapes.Shape shape;
        int o, n;
        public HistoryShapePosition(Shapes.Shape s, int oldPosition, int newPosiotion)
        {
            shape = s;
            o = oldPosition;
            n = newPosiotion;
        }

        public void Back()
        {
            shape.SetPosition(o);
        }

        public void Forward()
        {
            shape.SetPosition(n);
        }
    }
}
