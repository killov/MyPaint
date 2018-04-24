using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.Serializer
{
    public class Ellipse : Shape
    {
        public string type = "ELLIPSE";
        public Brush stroke, fill;
        public double lineWidth;
        public Point A, B;
    }
}
