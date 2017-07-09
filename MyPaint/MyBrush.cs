using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Composition;

namespace MyPaint
{
    enum MyBrushType
    {
        COLOR, GRADIENT
    }

    class MyBrush
    {
        public Brush brush;
        MyBrushType type;

        public MyBrush(int r, int g, int b)
        {
            type = MyBrushType.COLOR;
            brush = new SolidColorBrush(Color.FromArgb(255, (byte)r, (byte)g, (byte)b));
        }

        public MyBrush(Brush b)
        {
            brush = b;
            type = MyBrushType.COLOR;
        }
    }
}
