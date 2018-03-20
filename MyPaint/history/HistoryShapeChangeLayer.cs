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

        public void back()
        {
            shape.changeLayer(o);
        }

        public void forward()
        {
            shape.changeLayer(n);
        }

        public void skip(IHistoryNodeSkipped node)
        {
            HistoryShapeChangeLayer n = (HistoryShapeChangeLayer)node;
            this.n = n.n;
        }

        public bool optimal(IHistoryNodeSkipped node)
        {
            return (node is HistoryShapeChangeLayer) && ((HistoryShapeChangeLayer)node).shape.Equals(shape);
        }
    }
}
