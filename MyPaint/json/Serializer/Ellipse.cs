namespace MyPaint.Serializer
{
    public class Ellipse : Shape
    {
        public string type = "ELLIPSE";
        public Point A, B;

        public override Shapes.Shape Create(DrawControl c, MyPaint.Layer la)
        {
            return new Shapes.Ellipse(c, la, this);
        }
    }
}
