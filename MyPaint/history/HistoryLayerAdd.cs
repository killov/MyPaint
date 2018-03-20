using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.History
{
    public class HistoryLayerAdd : IHistoryNode
    {
        public Layer layer;
        public HistoryLayerAdd(Layer l)
        {
            layer = l;//
        }

        public void back()
        {
            layer.Remove(false);
        }

        public void forward()
        {
            layer.Add();
        }
    }
}
