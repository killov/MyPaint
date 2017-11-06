using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.History
{
    public class HistoryShapeDelete : IHistoryNode
    {
        public Shapes.MyShape shape;
        public HistoryShapeDelete(Shapes.MyShape s)
        {
            shape = s;
        }

        public void back()
        {
            shape.refresh();
        }

        public void forward()
        {
            shape.delete();
        }
    }
}
