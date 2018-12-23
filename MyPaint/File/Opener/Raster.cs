using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace MyPaint.FileOpener
{
    public abstract class Raster : FileOpener
    {
        override protected void Thread_open()
        {
            using (FileStream fs = new FileStream(dc.Path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BitmapSource bmi = GetBitmap(fs);
                ImageBrush brush = new ImageBrush(bmi);
                dc.SetResolution(new System.Windows.Point(bmi.Width, bmi.Height), false, true);
                new Shapes.Image(dc.DrawControl, dc.DrawControl.SelectLayer, bmi, new System.Windows.Point(0, 0), bmi.Width, bmi.Height);
            }
        }

        abstract protected BitmapSource GetBitmap(FileStream fs);
    }
}
