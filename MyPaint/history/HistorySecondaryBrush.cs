using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint.History
{
    public class HistorySecondaryBrush : IHistoryNodeSkipped
    {
        public Shapes.Shape shape;
        public Brush o, n;
        public HistorySecondaryBrush(Shapes.Shape s, Brush oldBrush, Brush newBrush)
        {
            shape = s;
            o = oldBrush;
            n = newBrush;
        }

        public void Back()
        {
            shape.SetSecondaryBrush(o);
        }

        public void Forward()
        {
            shape.SetSecondaryBrush(n);
        }

        public void Skip(IHistoryNodeSkipped node)
        {
            HistorySecondaryBrush n = (HistorySecondaryBrush)node;
            this.n = n.n;
        }

        public bool Optimal(IHistoryNodeSkipped node)
        {
            return node is HistorySecondaryBrush && ((HistorySecondaryBrush)node).shape.Equals(shape);
        }
    }
}
