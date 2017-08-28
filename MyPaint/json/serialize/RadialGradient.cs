using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.jsonSerialize
{
    public class RadialGradient : Brush
    {
        public string type = "RG";
        public Point S, E, RA;
        public List<GradientStop> stops;
    }
}
