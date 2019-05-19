using System.Collections.Generic;

namespace MyPaint.Serializer
{
    public class Layer
    {
        public Brush Color;
        public bool Visible;
        public string Name;
        public List<Shape> Shapes;
    }
}
