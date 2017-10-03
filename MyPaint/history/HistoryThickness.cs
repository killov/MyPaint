using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint
{
    public class HistoryThickness : IHistoryNode
    {
        public IMyShape shape;
        public double o, n;
        public HistoryThickness(IMyShape s, double oldT, double newT)
        {
            shape = s;
            o = oldT;
            n = newT;
        }

        public void back()
        {
            shape.setThickness(o);
        }

        public void forward()
        {
            shape.setThickness(n);
        }
    }
}
