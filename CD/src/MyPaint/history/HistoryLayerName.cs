using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.History
{
    public class HistoryLayerName : IHistoryNode
    {
        Layer layer;
        string oldP, newP;
        public HistoryLayerName(Layer l, string o, string n)
        {
            layer = l;
            oldP = o;
            newP = n;
        }

        public void Back()
        {
            layer.SetName(oldP);
        }

        public void Forward()
        {
            layer.SetName(newP);
        }
    }
}
