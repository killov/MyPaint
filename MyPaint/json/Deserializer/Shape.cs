using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.Deserializer
{
    public class Shape
    {
        public string type;
        public Point A, B;
        public Brush stroke, fill;
        public double lineWidth;
        public Point[] points;
        public string b64;
        public int w, h;

        public Shapes.Shape Create(FileControl c, MyPaint.Layer la)
        {
            switch (type)
            {
                case "LINE":
                    return new Shapes.Line(c, la, this);
                case "POLYLINE":
                    return new Shapes.PolyLine(c, la, this);
                case "RECTANGLE":
                    return new Shapes.Rectangle(c, la, this);
                case "ELLIPSE":
                    return new Shapes.Ellipse(c, la, this);
                case "POLYGON":
                    return new Shapes.Polygon(c, la, this);
                case "IMAGE":
                    return new Shapes.Image(c, la, this);
                case "TEXT":
                    return new Shapes.Text(c, la, this);
            }
            return null;
        }
    }
}
