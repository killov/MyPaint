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
        public Shapes.Shape shape;
        public Brush o, n;
        public HistoryPrimaryColor(Shapes.Shape s, Brush oldBrush, Brush newBrush)
        {
            shape = s;
            o = oldBrush;
            n = newBrush;
        }

        public void Back()
        {
            shape.SetPrimaryBrush(o);
        }

        public void Forward()
        {
            shape.SetPrimaryBrush(n);
        }

        public void Skip(IHistoryNodeSkipped node)
        {
            HistoryPrimaryColor n = (HistoryPrimaryColor)node;
            this.n = n.n;
        }

        public bool Optimal(IHistoryNodeSkipped node)
        {
            return (node is HistoryPrimaryColor) && ((HistoryPrimaryColor)node).shape.Equals(shape);
        }
    }
}
