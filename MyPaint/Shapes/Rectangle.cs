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

        }

        public Rectangle(FileControl c, Layer la, Deserializer.Shape s) : base(c, la, s)
        {
            CreateVirtualShape();
            p.Points.Add(new Point(s.A.x, s.A.y));
            p.Points.Add(new Point(s.A.x, s.B.y));
            p.Points.Add(new Point(s.B.x, s.B.y));
            p.Points.Add(new Point(s.B.x, s.A.y));
            
            AddToCanvas(p);
            CreatePoints();
        }

        override public void SetPrimaryColor(Brush s, bool addHistory = false)
        {
            base.SetPrimaryColor(s, addHistory);
            p.Stroke = s;
        }

        override public void SetSecondaryColor(Brush s, bool addHistory = false)
        {
            base.SetSecondaryColor(s, addHistory);
            p.Fill = s;
        }

        override public void AddToCanvas()
        {
            AddToCanvas(p);
        }

        override public void InsertToCanvas(int pos)
        {
            InsertToCanvas(pos, p);
        }

        override public void RemoveFromCanvas()
        {
            RemoveFromCanvas(p);
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
            AddToCanvas(p);
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

        override public void CreateVirtualShape()
        {
            vs.Fill = Brushes.Aqua;
            vs.Points = p.Points;
            vs.Stroke = nullBrush;
            //vs.Fill = nullBrush;
            vs.Cursor = Cursors.SizeAll;
            vs.MouseDown += delegate (object sender, MouseButtonEventArgs ee)
            {
                virtualShapeCallback(ee.GetPosition(drawControl.canvas), this);
                hit = true;
            };           
        }

        override public void ShowVirtualShape(MyOnMouseDown mouseDown)
        {
            base.ShowVirtualShape(mouseDown);
            HideVirtualShape();
            drawControl.topCanvas.Children.Add(vs);
        }

        override public void HideVirtualShape()
        {
            drawControl.topCanvas.Children.Remove(vs);
        }

        override public void SetActive()
        {
            base.SetActive();
            drawControl.SetPrimaryColor(p.Stroke);
            drawControl.SetSecondaryColor(p.Fill);
            drawControl.SetThickness(p.StrokeThickness);
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
            ret.stroke = PrimaryColor;
            ret.fill = SecondaryColor;
            ret.A = new Serializer.Point(eR.p1.GetPosition());
            ret.B = new Serializer.Point(eR.p3.GetPosition());
            return ret;
        }

        override public Point GetPosition()
        {
            return p.Points[0];
        }

        override public void CreatePoints()
        {
            eR = new EditRect(drawControl.topCanvas, this, p.Points[0], p.Points[2], drawControl.revScale,
            (po) =>
            {
                p.Points[0] = po;
                p.Points[3] = new Point(po.X, p.Points[3].Y);
                p.Points[1] = new Point(p.Points[1].X, po.Y);
                return true;
            },

            (po) =>
            {
                p.Points[1] = po;
                p.Points[2] = new Point(po.X, p.Points[2].Y);
                p.Points[0] = new Point(p.Points[0].X, po.Y);
                return true;
            },

            (po) =>
            {
                p.Points[2] = po;
                p.Points[3] = new Point(p.Points[3].X, po.Y);
                p.Points[1] = new Point(po.X, p.Points[1].Y);
                return true;
            },

            (po) =>
            {
                p.Points[3] = po;
                p.Points[2] = new Point(p.Points[2].X, po.Y);
                p.Points[0] = new Point(po.X, p.Points[0].Y);
                return true;
            });
        }

        override public void CreateImage(Canvas canvas)
        {
            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();

            p.Points.Add(eR.p1.GetPosition());
            p.Points.Add(eR.p2.GetPosition());
            p.Points.Add(eR.p3.GetPosition());
            p.Points.Add(eR.p4.GetPosition());
            p.Stroke = PrimaryColor == null ? null : PrimaryColor.CreateBrush();
            p.Fill = SecondaryColor == null ? null : SecondaryColor.CreateBrush();
            p.StrokeThickness = GetThickness();
            canvas.Children.Add(p);
        }

        override public void ChangeZoom()
        {
            eR.ChangeZoom();
        }
    }
}
