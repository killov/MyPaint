using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.History
{
    public class HistoryLayerRemove : IHistoryNode
    {
        public Layer layer;
        public int position;
        public HistoryLayerRemove(Layer l, int p)
        {
            layer = l;
            position = p;
        }

        public void Back()
        {
            layer.Add(position);
        }

        public void Forward()
        {
            layer.Remove(false);
        }
    }
}
