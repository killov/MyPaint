using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint
{
    public class HistoryPrimaryColor : IHistoryNodeSkipped
    {
        public IMyShape shape;
        public Brush o, n;
        public HistoryPrimaryColor(IMyShape s, Brush oldBrush, Brush newBrush)
        {
            shape = s;
            o = oldBrush;
            n = newBrush;
        }

        public void back()
        {
            shape.delete();
        }

        public void forward()
        {
            shape.refresh();
        }

        public bool optimal(IHistoryNodeSkipped node)
        {
            return (node is HistoryPrimaryColor) && ((HistoryPrimaryColor)node).shape == shape;
        }

        public void skip(IHistoryNodeSkipped node)
        {
            HistoryPrimaryColor nodee = (HistoryPrimaryColor)node;
            n = nodee.n;
        }
    }
}
