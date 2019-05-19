using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint.History
{
    public class HistoryShapeThickness : IHistoryNodeSkipped
    {
        public Shapes.Shape shape;
        public double o, n;
        public HistoryShapeThickness(Shapes.Shape s, double oldT, double newT)
        {
            shape = s;
            o = oldT;
            n = newT;
        }

        public void Back()
        {
            shape.SetThickness(o);
        }

        public void Forward()
        {
            shape.SetThickness(n);
        }

        public void Skip(IHistoryNodeSkipped node)
        {
            HistoryShapeThickness n = (HistoryShapeThickness)node;
            this.n = n.n;
        }

        public bool Optimal(IHistoryNodeSkipped node)
        {
            return node is HistoryShapeThickness;
        }
    }
}
