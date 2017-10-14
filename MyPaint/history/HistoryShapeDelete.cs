using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint
{
    public class HistoryShapeDelete : IHistoryNode
    {
        public MyShape shape;
        public HistoryShapeDelete(MyShape s)
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
