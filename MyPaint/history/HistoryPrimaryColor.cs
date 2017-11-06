using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint.History
{
    public class HistoryPrimaryColor : IHistoryNodeSkipped
    {
        public Shapes.MyShape shape;
        public Brush o, n;
        public HistoryPrimaryColor(Shapes.MyShape s, Brush oldBrush, Brush newBrush)
        {
            shape = s;
            o = oldBrush;
            n = newBrush;
        }

        public void back()
        {
            shape.setPrimaryColor(o);
        }

        public void forward()
        {
            shape.setPrimaryColor(n);
        }

        public void skip(IHistoryNodeSkipped node)
        {
            HistoryPrimaryColor n = (HistoryPrimaryColor)node;
            this.n = n.n;
        }

        public bool optimal(IHistoryNodeSkipped node)
        {
            return (node is HistoryPrimaryColor) && ((HistoryPrimaryColor)node).shape.Equals(shape);
        }
    }
}
