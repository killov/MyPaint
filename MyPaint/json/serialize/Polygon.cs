using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.jsonSerialize
{
    class Polygon : Shape
    {
        public string type = "POLYGON";

        public Brush stroke, fill;
        public double lineWidth;
        public List<Point> points;
    }
}
