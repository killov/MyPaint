using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.Serializer
{
    class Text : Shape
    {
        public string type = "TEXT";
        public Brush stroke, fill;
        public string b64;
        public Point A;
        public int w, h;
        public string font;
        public double lineWidth;
    }
}
