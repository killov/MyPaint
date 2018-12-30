namespace MyPaint.Serializer
{
    public class Point
    {
        public double X, Y;

        public Point(double xx, double yy)
        {
            X = xx;
            Y = yy;
        }

        public Point()
        {

        }

        public Point(System.Windows.Point p)
        {
            X = p.X;
            Y = p.Y;
        }
    }
}
