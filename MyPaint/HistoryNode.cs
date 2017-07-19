using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint
{
    public class HistoryNode
    {
        public MyShape shape;
        public HistoryNode(MyShape s)
        {
            shape = s;
        }

        public void back()
        {
            shape.delete();
        }

        public void forward()
        {
            shape.refresh();
        }
    }
}
