using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyPaint.file
{
    class JPEG
    {
        public static void open(DrawControl dc, string filename)
        {
  
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                JpegBitmapDecoder decoder = new JpegBitmapDecoder(fs, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                BitmapSource bmi = decoder.Frames[0];
                

                ImageBrush brush = new ImageBrush(bmi);
                dc.setResolution(new System.Windows.Point(bmi.Width, bmi.Height));
                dc.selectLayer.shapes.Add(new Shapes.MyImage(dc, dc.selectLayer, brush, new System.Windows.Point(0, 0), bmi.Width, bmi.Height));

            }
        }

        public static void save(DrawControl dc, string filename)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)dc.resolution.X,
                (int)dc.resolution.Y, 96, 96, PixelFormats.Default);
            rtb.Render(dc.canvas);
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            using (var fs = File.OpenWrite(@filename))
            {
                encoder.Save(fs);
            }
        }
    }
}
