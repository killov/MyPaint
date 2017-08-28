using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.jsonSerialize
{
    class Image : Shape
    {
        public string type = "IMAGE";
        public Point A, B;
        public string b64;
    }
}
