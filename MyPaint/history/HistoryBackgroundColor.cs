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
        public Layer layer;
        public Brush o, n;
        public HistoryBackgroundColor(Layer l, Brush oldBrush, Brush newBrush)
        {
            layer = l;
            o = oldBrush;
            n = newBrush;
        }

        public void Back()
        {
            layer.SetBackground(o);
        }

        public void Forward()
        {
            layer.SetBackground(n);
        }

        public void Skip(IHistoryNodeSkipped node)
        {
            HistoryBackgroundColor n = (HistoryBackgroundColor)node;
            this.n = n.n;
        }

        public bool Optimal(IHistoryNodeSkipped node)
        {
            return (node is HistoryBackgroundColor) && ((HistoryBackgroundColor)node).layer.Equals(layer);
        }
    }
}
