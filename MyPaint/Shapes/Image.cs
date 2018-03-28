﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyPaint.Shapes
{
    public class Image : Shape
    {
        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon(), vs;
        MovePoint p1, p2, p3, p4;
        int width, height;
        BitmapSource image;
        public Image(FileControl c, Layer la, BitmapSource bmi, Point start, double w, double h) : base(c, la)
        {
            width = (int)w;
            height = (int)h;
            p.Points.Add(new Point(start.X, start.Y));
            p.Points.Add(new Point(start.X, start.Y + h));
            p.Points.Add(new Point(start.X + w, start.Y + h));
            p.Points.Add(new Point(start.X + w, start.Y));

            CreateVirtualShape();
            image = bmi;
            ImageBrush brush = new ImageBrush(bmi);
            
            p.Fill = brush;
            AddToCanvas(p);
            CreatePoints();
        }

        public Image(FileControl c, Layer la, jsonDeserialize.Shape s) : base(c, la, s)
        {
            width = s.w;
            height = s.h;
            byte[] imageBytes = Convert.FromBase64String(s.b64);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            BitmapImage bmi = new BitmapImage();
            bmi.BeginInit();
            bmi.StreamSource = ms;
            bmi.EndInit();
            ImageBrush brush = new ImageBrush(bmi);
            image = bmi;
           
            p.Points.Add(new Point(s.A.x, s.A.y));
            p.Points.Add(new Point(s.B.x, s.A.y));
            p.Points.Add(new Point(s.B.x, s.B.y));
            p.Points.Add(new Point(s.A.x, s.B.y));
            CreateVirtualShape();
            p.Fill = brush;
            AddToCanvas(p);
            CreatePoints();
            
        }

        override public void SetPrimaryColor(Brush s, bool addHistory = false)
        {

        }

        override public void SetSecondaryColor(Brush s, bool addHistory = false)
        {

        }

        override public void AddToCanvas()
        {
            AddToCanvas(p);
        }

        override public void RemoveFromCanvas()
        {
            RemoveFromCanvas(p);
        }

        override public void SetThickness(double s, bool addHistory = false)
        {

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

        override public void CreateVirtualShape()
        {

            vs = new System.Windows.Shapes.Polygon();
            vs.Points = p.Points;
            vs.Stroke = nullBrush;
            vs.Fill = nullBrush;
            vs.StrokeThickness = p.StrokeThickness;
            vs.Cursor = Cursors.SizeAll;
            vs.MouseDown += delegate (object sender, MouseButtonEventArgs ee)
            {
                virtualShapeCallback(ee.GetPosition(canvas), this);
                hit = true;
            };
            
        }

        override public void ShowVirtualShape(MyOnMouseDown mouseDown)
        {
            base.ShowVirtualShape(mouseDown);
            HideVirtualShape();
            topCanvas.Children.Add(vs);
        }

        override public void HideVirtualShape()
        {
            topCanvas.Children.Remove(vs);
        }

        override public void SetActive()
        {
            base.SetActive();
            p1.Show();
            p2.Show();
            p3.Show();
            p4.Show();
        }

        override public void MoveDrag(Point e)
        {
            base.MoveDrag(e);
            p1.MoveDrag(e);
            p2.MoveDrag(e);
            p3.MoveDrag(e);
            p4.MoveDrag(e);
        }

        override public void StopDrag()
        {
            base.StopDrag();
            p1.StopDrag();
            p2.StopDrag();
            p3.StopDrag();
            p4.StopDrag();
        }

        override public void StopEdit()
        {
            base.StopEdit();
            p1.Hide();
            p2.Hide();
            p3.Hide();
            p4.Hide();
        }

        override public void MoveShape(double x, double y)
        {
            base.MoveShape(x, y);
            p.Points[1] = new Point(p.Points[1].X - p.Points[0].X + x, p.Points[1].Y - p.Points[0].Y + y);
            p.Points[2] = new Point(p.Points[2].X - p.Points[0].X + x, p.Points[2].Y - p.Points[0].Y + y);
            p.Points[3] = new Point(p.Points[3].X - p.Points[0].X + x, p.Points[3].Y - p.Points[0].Y + y);
            p.Points[0] = new Point(x, y);
            p1.Move(x, y);
            p2.Move(p.Points[1].X, p.Points[1].Y);
            p3.Move(p.Points[2].X, p.Points[2].Y);
            p4.Move(p.Points[3].X, p.Points[3].Y);
        }

        override public jsonSerialize.Shape CreateSerializer()
        {
            System.Windows.Shapes.Polygon po = new System.Windows.Shapes.Polygon();
            po.Points.Add(p1.GetPosition());
            po.Points.Add(p2.GetPosition());
            po.Points.Add(p3.GetPosition());
            po.Points.Add(p4.GetPosition());

            po.Fill = new ImageBrush(image);

            ContentControl cc = new ContentControl();
            Rect rect = new Rect(0, 0, width, height);
            cc.Content = po;
            cc.Arrange(rect);
            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
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
            jsonSerialize.Image ret = new jsonSerialize.Image();
            ret.A = new jsonSerialize.Point(p1.GetPosition());
            ret.B = new jsonSerialize.Point(p3.GetPosition());
            ret.b64 = base64String;
            ret.w = width;
            ret.h = height;
            return ret;
        }

        override public Point GetPosition()
        {
            return p.Points[0];
        }

        override public void CreatePoints()
        {
            p1 = new MovePoint(topCanvas, this, p.Points[0], drawControl.revScale, (po) =>
            {
                p.Points[0] = po;
                p.Points[1] = new Point(po.X, p.Points[1].Y);
                p.Points[3] = new Point(p.Points[3].X, po.Y);
                p1.Move(po.X, po.Y);
                p2.Move(po.X, p.Points[1].Y);
                p4.Move(p.Points[3].X, po.Y);
                return true;
            });

            p2 = new MovePoint(topCanvas, this, p.Points[1], drawControl.revScale, (po) =>
            {
                p.Points[1] = po;
                p.Points[0] = new Point(po.X, p.Points[0].Y);
                p.Points[2] = new Point(p.Points[2].X, po.Y);
                p2.Move(po.X, po.Y);
                p1.Move(po.X, p.Points[0].Y);
                p3.Move(p.Points[2].X, po.Y);
                return true;
            });

            p3 = new MovePoint(topCanvas, this, p.Points[2], drawControl.revScale, (po) =>
            {
                p.Points[2] = po;
                p.Points[1] = new Point(p.Points[1].X, po.Y);
                p.Points[3] = new Point(po.X, p.Points[3].Y);
                p3.Move(po.X, po.Y);
                p4.Move(po.X, p.Points[3].Y);
                p2.Move(p.Points[1].X, po.Y);
                return true;
            });

            p4 = new MovePoint(topCanvas, this, p.Points[3], drawControl.revScale, (po) =>
            {
                p.Points[3] = po;
                p.Points[0] = new Point(p.Points[0].X, po.Y);
                p.Points[2] = new Point(po.X, p.Points[2].Y);
                p4.Move(po.X, po.Y);
                p3.Move(po.X, p.Points[2].Y);
                p1.Move(p.Points[0].X, po.Y);
                return true;
            });
        }

        override public void CreateImage(Canvas canvas)
        {
            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();

            p.Points.Add(p1.GetPosition());
            p.Points.Add(p2.GetPosition());
            p.Points.Add(p3.GetPosition());
            p.Points.Add(p4.GetPosition());

            p.Stroke = primaryColor;
            p.Fill = new ImageBrush(image);
            p.StrokeThickness = thickness;
            p.ToolTip = null;

            canvas.Children.Add(p);

        }

    }
}
