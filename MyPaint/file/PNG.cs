using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyPaint.file
{
    class PNG
    {
        public static void open(DrawControl dc, string filename)
        {
            MemoryStream memoStream = new MemoryStream();
            using (FileStream fs = File.OpenRead(@filename))
            {
                fs.CopyTo(memoStream);
                BitmapImage bmi = new BitmapImage();
                bmi.BeginInit();
                bmi.StreamSource = memoStream;
                bmi.EndInit();
                ImageBrush brush = new ImageBrush(bmi);
                dc.setResolution(new System.Windows.Point(bmi.Width, bmi.Height));
                dc.selectLayer.shapes.Add(new Shapes.MyImage(dc, dc.selectLayer, brush, new System.Windows.Point(0, 0), bmi.Width, bmi.Height));
                fs.Close();
            }
        }

        public static void save(DrawControl dc, string filename)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)dc.resolution.X,
                (int)dc.resolution.Y, 96, 96, PixelFormats.Default);
            rtb.Render(dc.canvas);
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            using (var fs = File.OpenWrite(@filename))
            {
                encoder.Save(fs);
            }
        }
    }
}
