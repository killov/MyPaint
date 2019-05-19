namespace MyPaint.Serializer
{
    class QuadraticCurve : Line
    {
        public Point C;

        public QuadraticCurve()
        {
            type = "QLINE";
        }

        public override Shapes.Shape Create(DrawControl c, MyPaint.Layer la)
        {
            return new Shapes.QuadraticCurve(c, la, this);
        }
    }
}
