using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace MyPaint.Shapes
{
    public class Rectangle : Shape
    {
        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon(), vs = new System.Windows.Shapes.Polygon();

        EditRect eR;
        public Rectangle(FileControl c, Layer la) : base(c, la)
        {
            Element = p;
        }

        public Rectangle(FileControl c, Layer la, Deserializer.Shape s) : base(c, la, s)
        {
            Element = p;
            CreateVirtualShape();
            p.Points.Add(new Point(s.A.x, s.A.y));
            p.Points.Add(new Point(s.B.x, s.A.y));
            p.Points.Add(new Point(s.B.x, s.B.y));
            p.Points.Add(new Point(s.A.x, s.B.y));
            AddToLayer();
            CreatePoints();
        }

        override public void SetPrimaryBrush(Brush s, bool addHistory = false)
        {
            base.SetPrimaryBrush(s, addHistory);
            p.Stroke = s;
        }

        override public void SetSecondaryBrush(Brush s, bool addHistory = false)
        {
            base.SetSecondaryBrush(s, addHistory);
            p.Fill = s;
        }

        override public void SetThickness(double s, bool addHistory = false)
        {
            base.SetThickness(s, addHistory);
            p.StrokeThickness = s;
            if(vs != null) vs.StrokeThickness = s;
        }

        override public void DrawMouseDown(Point e, MouseButtonEventArgs ee)
        {
            PointCollection points = new PointCollection(4);
            points.Add(e);
            points.Add(e);
            points.Add(e);
            points.Add(e);
            p.Points = points;
            AddToLayer();
            StartDraw();
        }

        override public void DrawMouseMove(Point e)
        {
            p.Points[3] = new Point(p.Points[3].X, e.Y);
            p.Points[2] = e;
            p.Points[1] = new Point(e.X, p.Points[1].Y);
        }

        override public void DrawMouseUp(Point e, MouseButtonEventArgs ee)
        {
            StopDraw();
            CreatePoints();
            CreateVirtualShape();
            SetActive();
        }

        void CreateVirtualShape()
        {
            vs.Points = p.Points;
            vs.Stroke = nullBrush;
            vs.Fill = nullBrush;
            vs.Cursor = Cursors.SizeAll;
            vs.MouseDown += delegate (object sender, MouseButtonEventArgs ee)
            {
                virtualShapeCallback(ee.GetPosition(File.Canvas), this);
                hit = true;
            };           
        }

        override public void ShowVirtualShape(OnMouseDownDelegate mouseDown)
        {
            base.ShowVirtualShape(mouseDown);
            HideVirtualShape();
            File.TopCanvas.Children.Add(vs);
        }

        override public void HideVirtualShape()
        {
            File.TopCanvas.Children.Remove(vs);
        }

        override public void SetActive()
        {
            base.SetActive();
            File.SetPrimaryColor(p.Stroke);
            File.SetSecondaryColor(p.Fill);
            File.SetThickness(p.StrokeThickness);
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
            Serializer.Rectangle ret = new Serializer.Rectangle();
            ret.lineWidth = GetThickness();
            ret.stroke = PrimaryBrush;
            ret.fill = SecondaryBrush;
            ret.A = new Serializer.Point(eR.p1.GetPosition());
            ret.B = new Serializer.Point(eR.p3.GetPosition());
            return ret;
        }

        override public Point GetPosition()
        {
            return p.Points[0];
        }

        void CreatePoints()
        {
            eR = new EditRect(File.TopCanvas, this, p.Points[0], p.Points[2], File.RevScale,
            (po) =>
            {
                p.Points[0] = po;
                p.Points[3] = new Point(po.X, p.Points[3].Y);
                p.Points[1] = new Point(p.Points[1].X, po.Y);
            },

            (po) =>
            {
                p.Points[1] = po;
                p.Points[2] = new Point(po.X, p.Points[2].Y);
                p.Points[0] = new Point(p.Points[0].X, po.Y);
            },

            (po) =>
            {
                p.Points[2] = po;
                p.Points[3] = new Point(p.Points[3].X, po.Y);
                p.Points[1] = new Point(po.X, p.Points[1].Y);
            },

            (po) =>
            {
                p.Points[3] = po;
                p.Points[2] = new Point(p.Points[2].X, po.Y);
                p.Points[0] = new Point(po.X, p.Points[0].Y);
            });
        }

        override public void CreateImage(Canvas canvas)
        {
            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();

            p.Points.Add(eR.p1.GetPosition());
            p.Points.Add(eR.p2.GetPosition());
            p.Points.Add(eR.p3.GetPosition());
            p.Points.Add(eR.p4.GetPosition());
            p.Stroke = PrimaryBrush == null ? null : PrimaryBrush.CreateBrush();
            p.Fill = SecondaryBrush == null ? null : SecondaryBrush.CreateBrush();
            p.StrokeThickness = GetThickness();
            canvas.Children.Add(p);
        }

        override public void ChangeZoom()
        {
            eR.ChangeZoom();
        }
    }
}
