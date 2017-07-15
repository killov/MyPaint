using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.json
{
    public class Color : Brush
    {
        public string type = "COLOR";
        public Byte R, G, B, A;

        public Color(Byte RR, Byte GG, Byte BB, Byte AA)
        {
            R = RR;
            G = GG;
            B = BB;
            A = AA;
        }
    }
}
