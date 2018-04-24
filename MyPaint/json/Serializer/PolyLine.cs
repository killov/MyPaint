using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.Serializer
{
    class PolyLine : Shape
    {
        public string type = "POLYLINE";

        public Brush stroke, fill;
        public double lineWidth;
        public List<Point> points;
    }
}
