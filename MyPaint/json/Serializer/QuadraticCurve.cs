using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.Serializer
{
    class QuadraticCurve : Line
    { 
        public Point C;
        public QuadraticCurve()
        {
            type = "QLINE";
        }
    }
}
