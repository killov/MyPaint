namespace MyPaint.Serializer
{
    public class Rectangle : Shape
    {
        public string type = "RECTANGLE";
        public Point A, B;

        public override Shapes.Shape Create(DrawControl c, MyPaint.Layer la)
        {
            return new Shapes.Rectangle(c, la, this);
        }
    }
}
