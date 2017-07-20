using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace MyPaint
{
    class MyImage : MyShape
    {
        DrawControl drawControl;
        Rectangle r = new Rectangle();
        ImageBrush image;
        Canvas canvas;

        public MyImage(DrawControl c, Canvas ca, ImageBrush im, double w, double h)
        {
            drawControl = c;
            canvas = ca;
            r.Width = w;
            r.Height = h;
            r.Fill = im;
            image = im;
            canvas.Children.Add(r);
        }

        public MyImage(DrawControl c, Canvas ca, jsonDeserialize.Shape s)
        {
            drawControl = c;
            canvas = ca;
            byte[] imageBytes = Convert.FromBase64String(s.b64);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            BitmapImage bmi = new BitmapImage();
            bmi.BeginInit();
            bmi.StreamSource = ms;
            bmi.EndInit();
            ImageBrush brush = new ImageBrush(bmi);
            r.Width = bmi.Width;
            r.Height = bmi.Height;
            r.Fill = brush;
            canvas.Children.Add(r);
        }

        public void delete()
        {
            throw new NotImplementedException();
        }

        public bool hitTest()
        {
            throw new NotImplementedException();
        }

        public void mouseDown(MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void mouseMove(MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void mouseUp(MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void moveDrag(MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void moveShape(double x, double y)
        {
            throw new NotImplementedException();
        }

        public void setHit(bool hit)
        {
            throw new NotImplementedException();
        }

        public void setPrimaryColor(System.Windows.Media.Brush b)
        {
            throw new NotImplementedException();
        }

        public void setSecondaryColor(System.Windows.Media.Brush b)
        {
            throw new NotImplementedException();
        }

        public void setThickness(double b)
        {
            throw new NotImplementedException();
        }

        public void stopDrag()
        {
            throw new NotImplementedException();
        }

        public void stopDraw()
        {
            throw new NotImplementedException();
        }

        jsonSerialize.Shape MyShape.renderShape()
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)r.RenderSize.Width,
            (int)r.RenderSize.Height, 96, 96, PixelFormats.Default);
            rtb.Render(r);
            

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            byte[] f = null;
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                f = stream.ToArray();
            }
            
            string base64String = Convert.ToBase64String(f);
            jsonSerialize.Image ret = new jsonSerialize.Image();
            ret.A = new jsonSerialize.Point(0, 0);
            ret.b64 = base64String;
            return ret;
        }

        public void refresh()
        {
            throw new NotImplementedException();
        }
    }
}
