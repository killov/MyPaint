namespace MyPaint.Serializer
{
    class PolyLine : Polygon
    {
        public PolyLine()
        {
            type = "POLYLINE";
        }

        public override Shapes.Shape Create(DrawControl c, MyPaint.Layer la)
        {
            return new Shapes.PolyLine(c, la, this);
        }
    }
}
