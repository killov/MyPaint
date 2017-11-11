using System;
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
    public class MyImage : MyShape
    {
        Polygon p = new Polygon(), vs;
        MovePoint p1, p2, p3, p4;
        public MyImage(DrawControl c, MyLayer la, ImageBrush im, Point start, double w, double h) : base(c, la)
        {
            p.Points.Add(new Point(start.X, start.Y));
            p.Points.Add(new Point(start.X, start.Y + h));
            p.Points.Add(new Point(start.X + w, start.Y + h));
            p.Points.Add(new Point(start.X + w, start.Y));

            createVirtualShape();

            p.Fill = im;
            addToCanvas(p);
            createPoints();
        }

        public MyImage(DrawControl c, MyLayer la, jsonDeserialize.Shape s) : base(c, la, s)
        {
            byte[] imageBytes = Convert.FromBase64String(s.b64);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            BitmapImage bmi = new BitmapImage();
            bmi.BeginInit();
            bmi.StreamSource = ms;
            bmi.EndInit();
            ImageBrush brush = new ImageBrush(bmi);
           
            p.Points.Add(new Point(s.A.x, s.A.y));
            p.Points.Add(new Point(s.B.x, s.A.y));
            p.Points.Add(new Point(s.B.x, s.B.y));
            p.Points.Add(new Point(s.A.x, s.B.y));
            createVirtualShape();
            p.Fill = brush;
            addToCanvas(p);
            createPoints();
            
        }

        override public void setPrimaryColor(Brush s, bool addHistory = false)
        {

        }

        override public void setSecondaryColor(Brush s, bool addHistory = false)
        {

        }

        override public Brush getPrimaryColor()
        {
            return null;
        }

        override public Brush getSecondaryColor()
        {
            return null;
        }

        override public void addToCanvas()
        {
            addToCanvas(p);
        }

        override public void removeFromCanvas()
        {
            removeFromCanvas(p);
        }

        override public void setThickness(double s, bool addHistory = false)
        {

        }

        override public double getThickness()
        {
            return 0;
        }


        override public void drawMouseDown(Point e, MouseButtonEventArgs ee)
        {

        }

        override public void drawMouseMove(Point e)
        {

        }

        override public void drawMouseUp(Point e, MouseButtonEventArgs ee)
        {

        }

        override public void createVirtualShape()
        {

            vs = new Polygon();
            vs.Points = p.Points;
            vs.Stroke = drawControl.nullBrush;
            vs.Fill = drawControl.nullBrush;
            vs.StrokeThickness = p.StrokeThickness;
            vs.Cursor = Cursors.SizeAll;
            vs.MouseDown += delegate (object sender, MouseButtonEventArgs ee)
            {
                virtualShapeCallback(ee.GetPosition(drawControl.canvas), this);
                hit = true;
            };
            
        }

        override public void showVirtualShape(MyOnMouseDown mouseDown)
        {
            base.showVirtualShape(mouseDown);
            hideVirtualShape();
            drawControl.topCanvas.Children.Add(vs);
        }

        override public void hideVirtualShape()
        {
            drawControl.topCanvas.Children.Remove(vs);
        }

        override public void setActive()
        {
            base.setActive();
            drawControl.setPrimaryColor(p.Stroke);
            p1.show();
            p2.show();
            p3.show();
            p4.show();
        }

        override public void moveDrag(Point e)
        {
            base.moveDrag(e);
            p1.move(e);
            p2.move(e);
            p3.move(e);
            p4.move(e);
        }

        override public void stopDrag()
        {
            base.stopDrag();
            p1.stopDrag();
            p2.stopDrag();
            p3.stopDrag();
            p4.stopDrag();
        }

        override public void stopEdit()
        {
            base.stopEdit();
            p1.hide();
            p2.hide();
            p3.hide();
            p4.hide();
        }

        override public void moveShape(double x, double y)
        {
            base.moveShape(x, y);
            p.Points[1] = new Point(p.Points[1].X - p.Points[0].X + x, p.Points[1].Y - p.Points[0].Y + y);
            p.Points[2] = new Point(p.Points[2].X - p.Points[0].X + x, p.Points[2].Y - p.Points[0].Y + y);
            p.Points[3] = new Point(p.Points[3].X - p.Points[0].X + x, p.Points[3].Y - p.Points[0].Y + y);
            p.Points[0] = new Point(x, y);
            p1.move(x, y);
            p2.move(p.Points[1].X, p.Points[1].Y);
            p3.move(p.Points[2].X, p.Points[2].Y);
            p4.move(p.Points[3].X, p.Points[3].Y);
        }

        override public jsonSerialize.Shape renderShape()
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)p.RenderSize.Width,
            (int)p.RenderSize.Height, 96, 96, PixelFormats.Default);
            rtb.Render(p);


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
            ret.A = new jsonSerialize.Point(p.Points[0].X, p.Points[0].Y);
            ret.B = new jsonSerialize.Point(p.Points[2].X, p.Points[2].Y);
            ret.b64 = base64String;
            return ret;
        }

        override public Point getPosition()
        {
            return p.Points[0];
        }

        override public void createPoints()
        {
            p1 = new MovePoint(drawControl.topCanvas, this, p.Points[0], drawControl.revScale, (po) =>
            {
                p.Points[0] = po;
                p.Points[1] = new Point(po.X, p.Points[1].Y);
                p.Points[3] = new Point(p.Points[3].X, po.Y);
                p1.move(po.X, po.Y);
                p2.move(po.X, p.Points[1].Y);
                p4.move(p.Points[3].X, po.Y);
            });

            p2 = new MovePoint(drawControl.topCanvas, this, p.Points[1], drawControl.revScale, (po) =>
            {
                p.Points[1] = po;
                p.Points[0] = new Point(po.X, p.Points[0].Y);
                p.Points[2] = new Point(p.Points[2].X, po.Y);
                p2.move(po.X, po.Y);
                p1.move(po.X, p.Points[0].Y);
                p3.move(p.Points[2].X, po.Y);
            });

            p3 = new MovePoint(drawControl.topCanvas, this, p.Points[2], drawControl.revScale, (po) =>
            {
                p.Points[2] = po;
                p.Points[1] = new Point(p.Points[1].X, po.Y);
                p.Points[3] = new Point(po.X, p.Points[3].Y);
                p3.move(po.X, po.Y);
                p4.move(po.X, p.Points[3].Y);
                p2.move(p.Points[1].X, po.Y);
            });

            p4 = new MovePoint(drawControl.topCanvas, this, p.Points[3], drawControl.revScale, (po) =>
            {
                p.Points[3] = po;
                p.Points[0] = new Point(p.Points[0].X, po.Y);
                p.Points[2] = new Point(po.X, p.Points[2].Y);
                p4.move(po.X, po.Y);
                p3.move(po.X, p.Points[2].Y);
                p1.move(p.Points[0].X, po.Y);
            });
        }

    }
}
