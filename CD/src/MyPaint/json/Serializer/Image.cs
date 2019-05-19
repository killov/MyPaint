namespace MyPaint.Serializer
{
    class Image : Shape
    {
        public string type = "IMAGE";
        public string B64;
        public Point A = null;
        public int W, H;

        public override Shapes.Shape Create(DrawControl c, MyPaint.Layer la)
        {
            return new Shapes.Image(c, la, this);
        }
    }
}
