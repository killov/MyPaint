using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.jsonDeserialize
{
    public class Shape
    {
        public string type;
        public Point A, B;
        public Brush stroke, fill;
        public double lineWidth;
        public Point[] points;
        public string b64;
    }
}
