using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace MyPaint.FileSaver
{
    public abstract class Raster : FileSaver
    {
        override protected void SaveImage()
        {
            ContentControl cc = new ContentControl();
            Rect rect = new Rect(0, 0, dc.Resolution.X, dc.Resolution.Y);
            cc.Content = dc.CreateImage();
            cc.Arrange(rect);

            string filename = dc.Path;
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)dc.Resolution.X,
                (int)dc.Resolution.Y, 96, 96, PixelFormats.Default);
            rtb.Render(cc);
            BitmapEncoder encoder = GetEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            using (var fs = File.OpenWrite(@filename))
            {
                encoder.Save(fs);
            }
        }

        abstract protected BitmapEncoder GetEncoder();
    }
}
