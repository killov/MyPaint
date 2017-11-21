using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.History
{
    public class HistoryLayerAdd : IHistoryNode
    {
        public MyLayer layer;
        public HistoryLayerAdd(MyLayer l)
        {
            layer = l;//
        }

        public void back()
        {
            layer.remove(false);
        }

        public void forward()
        {
            layer.add();
        }
    }
}
