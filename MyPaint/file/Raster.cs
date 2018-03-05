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
    abstract class Raster
    {
        public DrawControl dc;

        public void open(DrawControl dc, string filename)
        {
            this.dc = dc;
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BitmapSource bmi = getBitmap(fs);
                ImageBrush brush = new ImageBrush(bmi);
                dc.setResolution(new System.Windows.Point(bmi.Width, bmi.Height));
                dc.selectLayer.shapes.Add(new Shapes.MyImage(dc, dc.selectLayer, bmi, new System.Windows.Point(0, 0), bmi.Width, bmi.Height));
            }
        }

        abstract protected BitmapSource getBitmap(FileStream fs);

        public void save(DrawControl dc)
        {
            JPEG info = new JPEG();
            info.dc = dc;
            Thread t = new Thread(thread_save);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void thread_save()
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
