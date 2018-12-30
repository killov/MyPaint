﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyPaint.Shapes
{
    public class Rectangle : Shape
    {
        System.Windows.Shapes.Polygon p, vs;

        EditRect eR;
        public Rectangle(DrawControl c, Layer la) : base(c, la)
        {

        }

        public Rectangle(DrawControl c, Layer la, Serializer.Shape s) : base(c, la, s)
        {

        }

        protected override void OnDrawInit()
        {
            p = new System.Windows.Shapes.Polygon();
            Element = p;
        }

        protected override void OnCreateInit(Serializer.Shape shape)
        {
            Serializer.Rectangle s = (Serializer.Rectangle)shape;
            p = new System.Windows.Shapes.Polygon();
            Element = p;
            CreateVirtualShape();
            p.Points.Add(new Point(s.A.X, s.A.Y));
            p.Points.Add(new Point(s.B.X, s.A.Y));
            p.Points.Add(new Point(s.B.X, s.B.Y));
            p.Points.Add(new Point(s.A.X, s.B.Y));

            CreatePoints();
        }

        protected override bool OnChangeBrush(BrushEnum brushEnum, Brush brush)
        {
            if (brushEnum == BrushEnum.PRIMARY)
            {
                p.Stroke = brush;
                return true;
            }
            if (brushEnum == BrushEnum.SECONDARY)
            {
                p.Fill = brush;
                return true;
            }
            return false;
        }

        protected override bool OnChangeThickness(double thickness)
        {
            p.StrokeThickness = thickness;
            if (vs != null)
            {
                vs.StrokeThickness = thickness;
            }
            return true;
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

        override protected void CreateVirtualShape()
        {
            vs = new System.Windows.Shapes.Polygon();
            vs.Points = p.Points;
            vs.Stroke = nullBrush;
            vs.Fill = nullBrush;
            vs.Cursor = Cursors.SizeAll;
            vs.MouseDown += CallBack;
            VirtualElement = vs;
        }

        override public void SetActive()
        {
            base.SetActive();
            DrawControl.SetPrimaryColor(p.Stroke);
            DrawControl.SetSecondaryColor(p.Fill);
            DrawControl.SetThickness(p.StrokeThickness);
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
            ret.LineWidth = GetThickness();
            ret.Stroke = PrimaryBrush;
            ret.Fill = SecondaryBrush;
            ret.A = new Serializer.Point(eR.p1.GetPosition());
            ret.B = new Serializer.Point(eR.p3.GetPosition());
            return ret;
        }

        override public Point GetPosition()
        {
            return p.Points[0];
        }

        override protected void CreatePoints()
        {
            eR = new EditRect(DrawControl.TopCanvas, this, p.Points[0], p.Points[2], DrawControl.RevScale,
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
