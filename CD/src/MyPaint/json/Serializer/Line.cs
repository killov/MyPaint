namespace MyPaint.Serializer
{
    public class Line : Shape
    {
        public string type = "LINE";
        public Point A, B;

        public override Shapes.Shape Create(DrawControl c, MyPaint.Layer la)
        {
            return new Shapes.Line(c, la, this);
        }
    }
}
