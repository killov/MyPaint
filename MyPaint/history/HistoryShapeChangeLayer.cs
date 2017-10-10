using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint
{
    public class HistoryShapeChangeLayer : IHistoryNode
    {
        public IMyShape shape;
        public MyLayer o, n;
        public HistoryShapeChangeLayer(IMyShape s, MyLayer oldL, MyLayer newL)
        {
            shape = s;
            o = oldL;
            n = newL;
        }

        public void back()
        {
            shape.changeLayer(o);
        }

        public void forward()
        {
            shape.changeLayer(n);
        }
    }
}
