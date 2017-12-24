using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.jsonSerialize
{
    abstract public class Brush
    {
        public static Brush create(System.Windows.Media.Brush brush)
        {
            if(brush is System.Windows.Media.SolidColorBrush)
            {
                return new Color(brush as System.Windows.Media.SolidColorBrush);
            }else if (brush is System.Windows.Media.LinearGradientBrush)
            {
                return new LinearGradient(brush as System.Windows.Media.LinearGradientBrush);
            }
            return null;
        }

        abstract public System.Windows.Media.Brush createBrush();
    }
}
