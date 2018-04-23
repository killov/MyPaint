using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.History
{
    public class HistoryShapeDelete : IHistoryNode
    {
        public Shapes.Shape shape;
        int pos;
        public HistoryShapeDelete(Shapes.Shape s, int p)
        {
            shape = s;
            pos = p;
        }

        public void Back()
        {
            shape.Refresh(pos);
        }

        public void Forward()
        {
            shape.Delete();
        }
    }
}
