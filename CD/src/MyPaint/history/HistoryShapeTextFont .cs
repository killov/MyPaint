using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint.History
{
    public class HistoryShapeTextFont : IHistoryNodeSkipped
    {
        public Shapes.Text shape;
        public FontFamily o, n;
        public HistoryShapeTextFont(Shapes.Text s, FontFamily oldT, FontFamily newT)
        {
            shape = s;
            o = oldT;
            n = newT;
        }

        public void Back()
        {
            shape.SetFont(o);
        }

        public void Forward()
        {
            shape.SetFont(n);
        }

        public void Skip(IHistoryNodeSkipped node)
        {
            HistoryShapeTextFont n = (HistoryShapeTextFont)node;
            this.n = n.n;
        }

        public bool Optimal(IHistoryNodeSkipped node)
        {
            return (node is HistoryShapeTextFont) && ((HistoryShapeTextFont)node).shape.Equals(shape);
        }
    }
}
