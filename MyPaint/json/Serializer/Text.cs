namespace MyPaint.Serializer
{
    class Text : Shape
    {
        public string type = "TEXT";
        public string B64;
        public Point A;
        public int W, H;
        public string Font;

        public override Shapes.Shape Create(DrawControl c, MyPaint.Layer la)
        {
            return new Shapes.Text(c, la, this);
        }
    }
}
