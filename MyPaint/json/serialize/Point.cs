using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.jsonSerialize
{
    public class Point
    {
        public double x, y;

        public Point(double xx, double yy)
        {
            x = xx;
            y = yy;
        }

        public Point(System.Windows.Point p)
        {
            x = p.X;
            y = p.Y;
        }
    }
}
