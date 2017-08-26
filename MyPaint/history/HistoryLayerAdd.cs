using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint
{
    public class HistoryLayerAdd : IHistoryNode
    {
        public MyLayer layer;
        public HistoryLayerAdd(MyLayer l)
        {
            layer = l;
        }

        public void back()
        {
            layer.remove();
        }

        public void forward()
        {
            layer.add();
        }
    }
}
