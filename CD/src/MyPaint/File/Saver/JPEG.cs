using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace MyPaint.FileSaver
{
    public class JPEG : Raster
    {
        protected override BitmapEncoder GetEncoder()
        {
            return new JpegBitmapEncoder();
        }
    }
}
