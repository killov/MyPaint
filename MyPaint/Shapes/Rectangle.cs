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
        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon(), vs;
        MovePoint p1, p2, p3, p4;
        public Rectangle(FileControl c, Layer la) : base(c, la)
        {

        }

        public Rectangle(FileControl c, Layer la, jsonDeserialize.Shape s) : base(c, la, s)
        {
            CreateVirtualShape();
            SetPrimaryColor(s.stroke == null ? null : s.stroke.CreateBrush());
            SetThickness(s.lineWidth);
            SetPrimaryColor(s.stroke == null ? null : s.stroke.CreateBrush());
            SetSecondaryColor(s.fill == null ? null : s.fill.CreateBrush());
            SetThickness(s.lineWidth);

            p.Points.Add(new Point(s.A.x, s.A.y));
            p.Points.Add(new Point(s.A.x, s.B.y));
            p.Points.Add(new Point(s.B.x, s.B.y));
            p.Points.Add(new Point(s.B.x, s.A.y));
            
            

            p.ToolTip = null;
            p.Cursor = Cursors.SizeAll;

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
            p.Stroke = drawControl.GetShapePrimaryColor();
            p.Fill = drawControl.GetShapeSecondaryColor();
            p.StrokeThickness = drawControl.GetShapeThickness();
            p.Points = points;
            p.ToolTip = null;
            p.Cursor = Cursors.Pen;
            AddToCanvas(p);
            StartDraw();
        }

        override public void DrawMouseMove(Point e)
        {
            p.Points[1] = new Point(p.Points[1].X, e.Y);
            p.Points[2] = e;
            p.Points[3] = new Point(e.X, p.Points[3].Y);
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
            vs = new System.Windows.Shapes.Polygon();
            vs.Points = p.Points;
            vs.Stroke = nullBrush;
            vs.Fill = nullBrush;
            vs.StrokeThickness = p.StrokeThickness;
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
            jsonSerialize.Rectangle ret = new jsonSerialize.Rectangle();
            ret.lineWidth = GetThickness();
            ret.stroke = PrimaryColor;
            ret.fill = SecondaryColor;
            ret.A = new jsonSerialize.Point(p1.GetPosition());
            ret.B = new jsonSerialize.Point(p3.GetPosition());
            return ret;
        }

        override public Point GetPosition()
        {
            return p.Points[0];
        }

        override public void CreatePoints()
        {
            p1 = new MovePoint(drawControl.topCanvas, this, p.Points[0], drawControl.revScale, (po) =>
            {
                p.Points[0] = po;
                p.Points[1] = new Point(po.X, p.Points[1].Y);
                p.Points[3] = new Point(p.Points[3].X, po.Y);
                p1.Move(po.X, po.Y);
                p2.Move(po.X, p.Points[1].Y);
                p4.Move(p.Points[3].X, po.Y);
                return true;
            });

            p2 = new MovePoint(drawControl.topCanvas, this, p.Points[1], drawControl.revScale, (po) =>
            {
                p.Points[1] = po;
                p.Points[0] = new Point(po.X, p.Points[0].Y);
                p.Points[2] = new Point(p.Points[2].X, po.Y);
                p2.Move(po.X, po.Y);
                p1.Move(po.X, p.Points[0].Y);
                p3.Move(p.Points[2].X, po.Y);
                return true;
            });

            p3 = new MovePoint(drawControl.topCanvas, this, p.Points[2], drawControl.revScale, (po) =>
            {
                p.Points[2] = po;
                p.Points[1] = new Point(p.Points[1].X, po.Y);
                p.Points[3] = new Point(po.X, p.Points[3].Y);
                p3.Move(po.X, po.Y);
                p4.Move(po.X, p.Points[3].Y);
                p2.Move(p.Points[1].X, po.Y);
                return true;
            });

            p4 = new MovePoint(drawControl.topCanvas, this, p.Points[3], drawControl.revScale, (po) =>
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
            p.Fill = secondaryColor;
            p.StrokeThickness = thickness;
            p.ToolTip = null;
            canvas.Children.Add(p);
        }

    }
}
