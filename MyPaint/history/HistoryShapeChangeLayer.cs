using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint.History
{
    public class HistoryShapeChangeLayer : IHistoryNodeSkipped
    {
        public Shapes.Shape shape;
        public Layer o, n;
        public HistoryShapeChangeLayer(Shapes.Shape s, Layer oldL, Layer newL)
        {
            shape = s;
            o = oldL;
            n = newL;
        }

        public void Back()
        {
            shape.ChangeLayer(o);
        }

        public void Forward()
        {
            shape.ChangeLayer(n);
        }

        public void Skip(IHistoryNodeSkipped node)
        {
            HistoryShapeChangeLayer n = (HistoryShapeChangeLayer)node;
            this.n = n.n;
        }

        public bool Optimal(IHistoryNodeSkipped node)
        {
            return (node is HistoryShapeChangeLayer) && ((HistoryShapeChangeLayer)node).shape.Equals(shape);
        }
    }
}
