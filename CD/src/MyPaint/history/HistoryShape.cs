using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.History
{
    public class HistoryShape : IHistoryNode
    {
        public Shapes.Shape shape;
        public HistoryShape(Shapes.Shape s)
        {
            shape = s;
        }

        public void Back()
        {
            shape.Delete();
        }

        public void Forward()
        {
            shape.Refresh();
        }
    }
}
