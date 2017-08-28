using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.jsonSerialize
{
    public class Layer
    {
        public Brush color;
        public bool visible;
        public string name;
        public List<Shape> shapes;
    }
}
