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
            dc.SetResolutionByHistoryControl(oldR);
        }

        public void Forward()
        {
            dc.SetResolutionByHistoryControl(newR);
        }
    }
}
