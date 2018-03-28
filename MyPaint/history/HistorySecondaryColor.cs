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
        public Shapes.Shape shape;
        public Brush o, n;
        public HistorySecondaryColor(Shapes.Shape s, Brush oldBrush, Brush newBrush)
        {
            shape = s;
            o = oldBrush;
            n = newBrush;
        }

        public void Back()
        {
            shape.SetSecondaryColor(o);
        }

        public void Forward()
        {
            shape.SetSecondaryColor(n);
        }

        public void Skip(IHistoryNodeSkipped node)
        {
            HistorySecondaryColor n = (HistorySecondaryColor)node;
            this.n = n.n;
        }

        public bool Optimal(IHistoryNodeSkipped node)
        {
            return node is HistorySecondaryColor && ((HistorySecondaryColor)node).shape.Equals(shape);
        }
    }
}
