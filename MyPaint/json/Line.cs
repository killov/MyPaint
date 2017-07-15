using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.json
{
    public class Line : Shape
    {
        public string type = "LINE";
        public Brush stroke;
        public double lineWidth;
        public Point A, B;
    }
}
