using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyPaint.Shapes
{
    public class Image : Shape
    {
        System.Windows.Shapes.Polygon p, vs;
        EditRect eR;
        BitmapSource image;

        public Image(DrawControl c, Layer la, BitmapSource bmi, Point start, double w, double h) : base(c, la, null)
        {
            image = bmi;
        }

        public Image(DrawControl c, Layer la, Serializer.Shape s) : base(c, la, s)
        {

        }

        protected override void OnDrawInit()
        {

        }

        protected override void OnCreateInit(Serializer.Shape shape)
        {
            Serializer.Image s = (Serializer.Image)shape;
            p = new System.Windows.Shapes.Polygon();
            Element = p;

            if (s != null)
            {
                byte[] imageBytes = Convert.FromBase64String(s.B64);
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                BitmapImage bmi = new BitmapImage();
                bmi.BeginInit();
                bmi.StreamSource = ms;
                bmi.EndInit();
                image = bmi;

                p.Points.Add(new Point(s.A.X, s.A.Y));
                p.Points.Add(new Point(s.A.X + s.W, s.A.Y));
                p.Points.Add(new Point(s.A.X + s.W, s.A.Y + s.H));
                p.Points.Add(new Point(s.A.X, s.A.Y + s.H));
            }
            else
            {
                p.Points.Add(new Point(0, 0));
                p.Points.Add(new Point(image.Width, 0));
                p.Points.Add(new Point(image.Width, image.Height));
                p.Points.Add(new Point(0, image.Height));
            }
            CreateVirtualShape();
            p.Fill = new ImageBrush(image);

            CreatePoints();
        }

        protected override bool OnChangeBrush(BrushEnum brushEnum, Brush brush)
        {
            return false;
        }

        protected override bool OnChangeThickness(double thickness)
        {
            return false;
        }

        override public void OnDrawMouseDown(Point e, MouseButtonEventArgs ee)
        {

        }

        override public void OnDrawMouseMove(Point e)
        {

        }

        override public void OnDrawMouseUp(Point e, MouseButtonEventArgs ee)
        {

        }

        override protected void CreateVirtualShape()
        {
            vs = new System.Windows.Shapes.Polygon();
            vs.Points = p.Points;
            vs.Stroke = nullBrush;
            vs.Fill = nullBrush;
            vs.StrokeThickness = p.StrokeThickness;
            vs.Cursor = Cursors.SizeAll;
            vs.MouseDown += CallBack;
            VirtualElement = vs;
        }

        override public void SetActive()
        {
            base.SetActive();
            eR.SetActive();
        }

        override public void MoveDrag(Point e)
        {
            base.MoveDrag(e);
            eR.MoveDrag(e);
        }

        override public void StopDrag()
        {
            base.StopDrag();
            eR.StopDrag();
        }

        override public void StopEdit()
        {
            base.StopEdit();
            eR.StopEdit();
        }

        override public void MoveShape(Point point)
        {
            base.MoveShape(point);

            eR.Move(point);
        }

        override public Serializer.Shape CreateSerializer()
        {
            System.Windows.Shapes.Rectangle po = new System.Windows.Shapes.Rectangle();
            po.Width = eR.GetWidth();
            po.Height = eR.GetHeight();
            po.Fill = new ImageBrush(image);

            ContentControl cc = new ContentControl();
            Rect rect = new Rect(0, 0, eR.GetWidth(), eR.GetHeight());
            cc.Content = po;
            cc.Arrange(rect);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)eR.GetWidth(), (int)eR.GetHeight(), 96, 96, PixelFormats.Default);
            rtb.Render(po);

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            byte[] f = null;
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                f = stream.ToArray();
            }

            string base64String = Convert.ToBase64String(f);
            Serializer.Image ret = new Serializer.Image();
            ret.B64 = base64String;

            ret.A = new Serializer.Point(eR.Position);
            ret.W = (int)eR.GetWidth();
            ret.H = (int)eR.GetHeight();
            return ret;
        }

        override public Point GetPosition()
        {
            return p.Points[0];
        }

        override protected void CreatePoints()
        {
            eR = new EditRect(DrawControl.TopCanvas, this, p.Points[0], p.Points[2], DrawControl.RevScale, (po, mouseDrag) =>
            {
                p.Points[0] = po;
            }, (po, mouseDrag) =>
            {
                p.Points[1] = po;
            }, (po, mouseDrag) =>
            {
                p.Points[2] = po;
            }, (po, mouseDrag) =>
            {
                p.Points[3] = po;
            });
        }

        override public void CreateImage(Canvas canvas)
        {
            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();

            p.Points.Add(eR.p1.Position);
            p.Points.Add(eR.p2.Position);
            p.Points.Add(eR.p3.Position);
            p.Points.Add(eR.p4.Position);

            p.Stroke = primaryBrush;
            p.Fill = new ImageBrush(image);
            p.StrokeThickness = thickness;
            p.ToolTip = null;

            canvas.Children.Add(p);

        }

        override public void ChangeZoom()
        {
            eR.ChangeZoom();
        }

        public BitmapSource CreateBitmap()
        {
            System.Windows.Shapes.Rectangle po = new System.Windows.Shapes.Rectangle();
            po.Width = eR.GetWidth();
            po.Height = eR.GetHeight();
            po.Fill = new ImageBrush(image);

            ContentControl cc = new ContentControl();
            Rect rect = new Rect(0, 0, eR.GetWidth(), eR.GetHeight());
            cc.Content = po;
            cc.Arrange(rect);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)eR.GetWidth(), (int)eR.GetHeight(), 96, 96, PixelFormats.Default);
            rtb.Render(po);

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            byte[] f = null;
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                f = stream.ToArray();
            }

            MemoryStream ms = new MemoryStream(f, 0, f.Length);
            BitmapImage bmi = new BitmapImage();
            bmi.BeginInit();
            bmi.StreamSource = ms;
            bmi.EndInit();
            return bmi;
        }

    }
}
