using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint.Serializer
{
    public class NullBrush : Brush
    {
        public string type = "NULL";

        public override System.Windows.Media.Brush CreateBrush()
        {
            return null;
        }
    }
}
