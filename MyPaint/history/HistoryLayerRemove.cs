using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.History
{
    public class HistoryLayerRemove : IHistoryNode
    {
        public MyLayer layer;
        public int position;
        public HistoryLayerRemove(MyLayer l, int p)
        {
            layer = l; //
            position = p;
        }

        public void back()
        {
            layer.add(position);
        }

        public void forward()
        {
            layer.remove(false);
        }
    }
}
