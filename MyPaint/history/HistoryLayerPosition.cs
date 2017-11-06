using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.History
{
    public class HistoryLayerPosition : IHistoryNode
    {
        MyLayer layer;
        int oldP, newP;
        public HistoryLayerPosition(MyLayer l, int o, int n)
        {
            layer = l;
            oldP = o;
            newP = n;
        }

        public void back()
        {
            layer.setPosition(oldP);
        }

        public void forward()
        {
            layer.setPosition(newP);
        }
    }
}
