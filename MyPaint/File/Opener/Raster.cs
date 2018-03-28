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


namespace MyPaint.FileOpener
{
    public abstract class Raster : FileOpener
    {
        override protected void Thread_open()
        {
            using (FileStream fs = new FileStream(dc.path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BitmapSource bmi = GetBitmap(fs);
                ImageBrush brush = new ImageBrush(bmi);
                dc.SetResolution(new System.Windows.Point(bmi.Width, bmi.Height), false);
                dc.selectLayer.shapes.Add(new Shapes.Image(dc, dc.selectLayer, bmi, new System.Windows.Point(0, 0), bmi.Width, bmi.Height));
            }
        }

        abstract protected BitmapSource GetBitmap(FileStream fs);
    }
}
