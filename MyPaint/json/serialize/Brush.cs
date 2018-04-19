using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.Serializer
{
    abstract public class Brush
    {
        public static Brush Create(System.Windows.Media.Brush brush)
        {
            if(brush is System.Windows.Media.SolidColorBrush)
            {
                return new Color(brush as System.Windows.Media.SolidColorBrush);
            }else if (brush is System.Windows.Media.LinearGradientBrush)
            {
                return new LinearGradient(brush as System.Windows.Media.LinearGradientBrush);
            }
            return new NullBrush();
        }

        abstract public System.Windows.Media.Brush CreateBrush();
    }
}
