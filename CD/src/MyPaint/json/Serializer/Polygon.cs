using System.Collections.Generic;

namespace MyPaint.Serializer
{
    class Polygon : Shape
    {
        public string type = "POLYGON";
        public List<Point> Points;

        public override Shapes.Shape Create(DrawControl c, MyPaint.Layer la)
        {
            return new Shapes.Polygon(c, la, this);
        }
    }
}
