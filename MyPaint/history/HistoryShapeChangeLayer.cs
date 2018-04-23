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
        int pos;
        public HistoryShapeChangeLayer(Shapes.Shape s, Layer oldL, Layer newL, int p)
        {
            shape = s;
            o = oldL;
            n = newL;
            pos = p;
        }

        public void Back()
        {
            shape.StopEdit();
            shape.ChangeLayer(o, pos);
        }

        public void Forward()
        {
            shape.StopEdit();
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
