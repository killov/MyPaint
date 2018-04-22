using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.Serializer
{
    class Image : Shape
    {
        public string type = "IMAGE";
        public string b64;
        public Point A = null;
        public int w, h;
    }
}
