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


namespace MyPaint.file
{
    class JPEG : Raster
    {
        protected override BitmapSource getBitmap(FileStream fs)
        {
            BitmapDecoder decoder = new JpegBitmapDecoder(fs, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            return decoder.Frames[0];
        }

        protected override BitmapEncoder getEncoder()
        {
            return new JpegBitmapEncoder();
        }
    }
}
