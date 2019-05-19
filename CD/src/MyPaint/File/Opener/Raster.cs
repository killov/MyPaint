using System.Collections.Generic;
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
                dc.Resolution = new System.Windows.Point(bmi.Width, bmi.Height);

                var l = new Serializer.Layer();
                l.Visible = true;
                l.Name = "IMAGE";
                l.Shapes = new List<Serializer.Shape>();
                var layer = new Layer(dc, l);
                var image = new Shapes.Image(dc.DrawControl, layer, bmi, new System.Windows.Point(0, 0), bmi.Width, bmi.Height);
                layer._shapes.Add(image);

                dc.layers.Add(layer);
            }
        }

        abstract protected BitmapSource GetBitmap(FileStream fs);
    }
}
