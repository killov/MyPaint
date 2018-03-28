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
        public HistoryShapeDelete(Shapes.Shape s)
        {
            shape = s;
        }

        public void Back()
        {
            shape.Refresh();
        }

        public void Forward()
        {
            shape.Delete();
        }
    }
}
