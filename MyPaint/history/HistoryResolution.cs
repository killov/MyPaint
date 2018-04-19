using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyPaint.History
{
    public class HistoryResolution : IHistoryNode
    {
        FileControl dc;
        public Point oldR, newR;
        public HistoryResolution(FileControl c, Point o, Point n)
        {
            dc = c;
            oldR = o;
            newR = n;
        }

        public void Back()
        {
            dc.SetResolution(oldR, true);
        }

        public void Forward()
        {
            dc.SetResolution(newR, true);
        }
    }
}
