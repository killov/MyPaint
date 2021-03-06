using System;
using System.Windows.Media;

namespace MyPaint.Serializer
{
    public class Color : Brush
    {
        public string type = "COLOR";
        public Byte R, G, B, A;

        public Color()
        {

        }

        public Color(Byte RR, Byte GG, Byte BB, Byte AA)
        {
            R = RR;
            G = GG;
            B = BB;
            A = AA;
        }

        public Color(System.Windows.Media.SolidColorBrush brush)
        {
            var color = brush.Color;
            R = color.R;
            G = color.G;
            B = color.B;
            A = color.A;
        }

        public override System.Windows.Media.Brush CreateBrush()
        {
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb(A, R, G, B));
        }

        public System.Windows.Media.Color createColor()
        {
            return System.Windows.Media.Color.FromArgb(A, R, G, B);
        }
    }
}
