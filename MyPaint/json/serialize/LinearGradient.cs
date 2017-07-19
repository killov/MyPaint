using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.jsonSerialize
{
    public class LinearGradient : Brush
    {
        public string type = "LG";
        public Point S, E;
        public List<LinearGradientStop> stops;
    }
}
