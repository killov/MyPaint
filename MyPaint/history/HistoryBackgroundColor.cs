using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint.History
{
    public class HistoryBackgroundColor : IHistoryNodeSkipped
    {
        public MyLayer layer;
        public Brush o, n;
        public HistoryBackgroundColor(MyLayer l, Brush oldBrush, Brush newBrush)
        {
            layer = l;
            o = oldBrush;
            n = newBrush;
        }

        public void back()
        {
            layer.setColor(o);
        }

        public void forward()
        {
            layer.setColor(n);
        }

        public void skip(IHistoryNodeSkipped node)
        {
            HistoryBackgroundColor n = (HistoryBackgroundColor)node;
            this.n = n.n;
        }

        public bool optimal(IHistoryNodeSkipped node)
        {
            return (node is HistoryBackgroundColor) && ((HistoryBackgroundColor)node).layer.Equals(layer);
        }
    }
}
