namespace MyPaint.Serializer
{

    abstract public class Shape
    {
        public Brush Stroke, Fill;
        public double LineWidth;

        abstract public Shapes.Shape Create(DrawControl c, MyPaint.Layer la);
    }
}
