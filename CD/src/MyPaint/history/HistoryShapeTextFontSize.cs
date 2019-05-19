using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint.History
{
    public class HistoryShapeTextFontSize : IHistoryNodeSkipped
    {
        public Shapes.Text shape;
        public double o, n;
        public HistoryShapeTextFontSize(Shapes.Text s, double oldT, double newT)
        {
            shape = s;
            o = oldT;
            n = newT;
        }

        public void Back()
        {
            shape.SetFontSize(o);
        }

        public void Forward()
        {
            shape.SetFontSize(n);
        }

        public void Skip(IHistoryNodeSkipped node)
        {
            HistoryShapeTextFontSize n = (HistoryShapeTextFontSize)node;
            this.n = n.n;
        }

        public bool Optimal(IHistoryNodeSkipped node)
        {
            return (node is HistoryShapeTextFontSize) && ((HistoryShapeTextFontSize)node).shape.Equals(shape);
        }
    }
}
