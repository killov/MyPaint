using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint
{
    public class HistorySecondaryColor : IHistoryNodeSkipped
    {
        public IMyShape shape;
        public Brush o, n;
        public HistorySecondaryColor(IMyShape s, Brush oldBrush, Brush newBrush)
        {
            shape = s;
            o = oldBrush;
            n = newBrush;
        }

        public void back()
        {
            shape.setSecondaryColor(o);
        }

        public void forward()
        {
            shape.setSecondaryColor(n);
        }

        public bool optimal(IHistoryNodeSkipped node)
        {
            return (node is HistorySecondaryColor) && ((HistorySecondaryColor)node).shape == shape;
        }

        public void skip(IHistoryNodeSkipped node)
        {
            HistorySecondaryColor nodee = (HistorySecondaryColor)node;
            n = nodee.n;
        }
    }
}
