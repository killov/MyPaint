using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint.History
{
    public class HistorySecondaryColor : IHistoryNodeSkipped
    {
        public Shapes.MyShape shape;
        public Brush o, n;
        public HistorySecondaryColor(Shapes.MyShape s, Brush oldBrush, Brush newBrush)
        {
            shape = s;
            o = oldBrush;
            n = newBrush;
        }

        public void back()
        {
            shape.setSecondaryColor(o);
        }

        public void forward()
        {
            shape.setSecondaryColor(n);
        }

        public void skip(IHistoryNodeSkipped node)
        {
            HistorySecondaryColor n = (HistorySecondaryColor)node;
            this.n = n.n;
        }

        public bool optimal(IHistoryNodeSkipped node)
        {
            return node is HistorySecondaryColor && ((HistorySecondaryColor)node).shape.Equals(shape);
        }
    }
}
