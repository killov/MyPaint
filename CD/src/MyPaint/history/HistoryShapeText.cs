using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint.History
{
    public class HistoryShapeText : IHistoryNodeSkipped
    {
        public Shapes.Text shape;
        public string o, n;
        public HistoryShapeText(Shapes.Text s, string oldT, string newT)
        {
            shape = s;
            o = oldT;
            n = newT;
        }

        public void Back()
        {
            shape.SetText(o);
        }

        public void Forward()
        {
            shape.SetText(n);
        }

        public void Skip(IHistoryNodeSkipped node)
        {
            HistoryShapeText n = (HistoryShapeText)node;
            this.n = n.n;
        }

        public bool Optimal(IHistoryNodeSkipped node)
        {
            return (node is HistoryShapeText) && ((HistoryShapeText)node).shape.Equals(shape);
        }
    }
}
