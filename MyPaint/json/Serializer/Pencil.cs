namespace MyPaint.Serializer
{
    class Pencil : Polygon
    {
        public Pencil()
        {
            type = "PENCIL";
        }

        public override Shapes.Shape Create(DrawControl c, MyPaint.Layer la)
        {
            return new Shapes.Pencil(c, la, this);
        }
    }
}
