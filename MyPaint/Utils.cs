using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint
{
    public class Utils
    {
        static public json.Brush BrushToCanvas(Brush b)
        {
            if(b is SolidColorBrush)
            {
                Color c = ((SolidColorBrush)b).Color;
                return new json.Color(c.R, c.G, c.B, c.A);
            }
            return null;
        }
    }
}
