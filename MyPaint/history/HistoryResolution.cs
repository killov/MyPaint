using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyPaint
{
    public class HistoryResolution : IHistoryNode
    {
        MainControl control;
        public Point oldR, newR;
        public HistoryResolution(MainControl c, Point o, Point n)
        {
            control = c;
            oldR = o;
            newR = n;
        }

        public void back()
        {
            control.setResolution(oldR.X, oldR.Y);
        }

        public void forward()
        {
            control.setResolution(newR.X, newR.Y);
        }
    }
}
