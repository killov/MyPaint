﻿using System;
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
        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon(), vs = new System.Windows.Shapes.Polygon();
        EditRect eR;
        BitmapSource image;
        Point start;
        double w, h;
        public Image(DrawControl c, Layer la, BitmapSource bmi, Point start, double w, double h) : base(c, la)
        {
            image = bmi;
            this.start = start;
            this.w = w;
            this.h = h;
        }

        public Image(DrawControl c, Layer la, Deserializer.Shape s) : base(c, la, s)
        {

        }

        protected override void OnDrawInit()
        {
            Element = p;
            p.Points.Add(new Point(start.X, start.Y));
            p.Points.Add(new Point(start.X + w, start.Y));
            p.Points.Add(new Point(start.X + w, start.Y + h));
            p.Points.Add(new Point(start.X, start.Y + h));
            CreateVirtualShape();

            ImageBrush brush = new ImageBrush(image);
            p.Fill = brush;
            AddToLayer();
            CreatePoints();
            exist = true;
        }

        protected override void OnCreateInit(Deserializer.Shape s)
        {
            Element = p;
            byte[] imageBytes = Convert.FromBase64String(s.b64);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            BitmapImage bmi = new BitmapImage();
            bmi.BeginInit();
            bmi.StreamSource = ms;
            bmi.EndInit();
            ImageBrush brush = new ImageBrush(bmi);
            image = bmi;

            p.Points.Add(new Point(s.A.x, s.A.y));
            p.Points.Add(new Point(s.A.x + s.w, s.A.y));
            p.Points.Add(new Point(s.A.x + s.w, s.A.y + s.h));
            p.Points.Add(new Point(s.A.x, s.A.y + s.h));
            CreateVirtualShape();
            p.Fill = brush;
            AddToLayer();
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

        override public void DrawMouseDown(Point e, MouseButtonEventArgs ee)
        {

        }

        override public void DrawMouseMove(Point e)
        {

        }

        override public void DrawMouseUp(Point e, MouseButtonEventArgs ee)
        {

        }

        void CreateVirtualShape()
        {
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

        override public void MoveShape(double x, double y)
        {
            base.MoveShape(x, y);
            p.Points[1] = new Point(p.Points[1].X - p.Points[0].X + x, p.Points[1].Y - p.Points[0].Y + y);
            p.Points[2] = new Point(p.Points[2].X - p.Points[0].X + x, p.Points[2].Y - p.Points[0].Y + y);
            p.Points[3] = new Point(p.Points[3].X - p.Points[0].X + x, p.Points[3].Y - p.Points[0].Y + y);
            p.Points[0] = new Point(x, y);
            eR.Move(x, y);
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
            ret.b64 = base64String;

            ret.A = new Serializer.Point(eR.GetPoint());
            ret.w = (int)eR.GetWidth();
            ret.h = (int)eR.GetHeight();
            return ret;
        }

        override public Point GetPosition()
        {
            return p.Points[0];
        }

        void CreatePoints()
        {
            eR = new EditRect(DrawControl.TopCanvas, this, p.Points[0], p.Points[2], DrawControl.RevScale, (po) =>
            {
                p.Points[0] = po;
                p.Points[1] = new Point(p.Points[1].X, po.Y);
                p.Points[3] = new Point(po.X, p.Points[3].Y);
            }, (po) =>
            {
                p.Points[1] = po;
                p.Points[0] = new Point(p.Points[0].X, po.Y);
                p.Points[2] = new Point(po.X, p.Points[2].Y);
            }, (po) =>
            {
                p.Points[2] = po;
                p.Points[1] = new Point(po.X, p.Points[1].Y);
                p.Points[3] = new Point(p.Points[3].X, po.Y);
            }, (po) =>
            {
                p.Points[3] = po;
                p.Points[0] = new Point(po.X, p.Points[0].Y);
                p.Points[2] = new Point(p.Points[2].X, po.Y);
            });
        }

        override public void CreateImage(Canvas canvas)
        {
            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();

            p.Points.Add(eR.p1.GetPosition());
            p.Points.Add(eR.p2.GetPosition());
            p.Points.Add(eR.p3.GetPosition());
            p.Points.Add(eR.p4.GetPosition());

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
