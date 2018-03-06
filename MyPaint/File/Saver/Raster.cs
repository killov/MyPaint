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
    public abstract class Raster : FileSaver
    {
        override protected void thread_save()
        {
            ContentControl cc = new ContentControl();
            Rect rect = new Rect(0, 0, dc.resolution.X, dc.resolution.Y);
            cc.Content = dc.create();
            cc.Arrange(rect);

            string filename = dc.path;
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)dc.resolution.X,
                (int)dc.resolution.Y, 96, 96, PixelFormats.Default);
            rtb.Render(cc);
            BitmapEncoder encoder = getEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            using (var fs = File.OpenWrite(@filename))
            {
                encoder.Save(fs);
            }
        }

        abstract protected BitmapEncoder getEncoder();
    }
}
