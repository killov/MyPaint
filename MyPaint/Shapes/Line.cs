﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyPaint.Shapes
{
    public class Line : Shape
    {
        System.Windows.Shapes.Line p, vs;
        MovePoint p1, p2;
        public Line(DrawControl c, Layer la) : base(c, la)
        {

        }

        public Line(DrawControl c, Layer la, Serializer.Shape s) : base(c, la, s)
        {

        }

        protected override void OnDrawInit()
        {
            p = new System.Windows.Shapes.Line();
            Element = p;
        }

        protected override void OnCreateInit(Serializer.Shape shape)
        {
            Serializer.Line s = (Serializer.Line)shape;
            p = new System.Windows.Shapes.Line();
            Element = p;
            p.X1 = s.A.X;
            p.Y1 = s.A.Y;
            p.X2 = s.B.X;
            p.Y2 = s.B.Y;
            CreatePoints();
            CreateVirtualShape();
        }

        protected override bool OnChangeBrush(BrushEnum brushEnum, Brush brush)
        {
            if (brushEnum == BrushEnum.PRIMARY)
            {
                p.Stroke = brush;
                return true;
            }
            return false;
        }

        protected override bool OnChangeThickness(double thickness)
        {
            p.StrokeThickness = thickness;
            if (vs != null)
            {
                vs.StrokeThickness = Math.Max(3, thickness);
            }
            return true;
        }

        override public void OnDrawMouseDown(Point e, MouseButtonEventArgs ee)
        {
            p.X1 = e.X;
            p.Y1 = e.Y;
            p.X2 = e.X;
            p.Y2 = e.Y;
            AddToLayer();
            StartDraw();
        }

        override public void OnDrawMouseMove(Point e)
        {
            p.X2 = e.X;
            p.Y2 = e.Y;
        }

        override public void OnDrawMouseUp(Point e, MouseButtonEventArgs ee)
        {
            StopDraw();
            CreatePoints();
            CreateVirtualShape();
            SetActive();
        }

        override protected void CreateVirtualShape()
        {
            vs = new System.Windows.Shapes.Line();
            vs.X1 = p.X1;
            vs.X2 = p.X2;
            vs.Y1 = p.Y1;
            vs.Y2 = p.Y2;
            vs.Cursor = Cursors.SizeAll;
            vs.Stroke = nullBrush;
            vs.StrokeThickness = Math.Max(3 * DrawControl.RevScale.ScaleX, p.StrokeThickness);
            vs.MouseDown += CallBack;
            VirtualElement = vs;
        }

        override public void SetActive()
        {
            base.SetActive();
            DrawControl.SetPrimaryColor(p.Stroke);
            DrawControl.SetThickness(p.StrokeThickness);
            p1.Show();
            p2.Show();
        }

        override public void MoveDrag(Point e)
        {
            base.MoveDrag(e);
            p1.MoveDrag(e);
            p2.MoveDrag(e);
        }

        override public void StopDrag()
        {
            base.StopDrag();
            p1.StopDrag();
            p2.StopDrag();
        }

        override public void StopEdit()
        {
            base.StopEdit();
            p1.Hide();
            p2.Hide();
        }

        public override void MoveShape(Point point)
        {
            base.MoveShape(point);

            p2.Move(point + (p2.Position - p1.Position));
            p1.Move(point);
        }

        public override Serializer.Shape CreateSerializer()
        {
            Serializer.Line ret = new Serializer.Line();
            ret.LineWidth = GetThickness();
            ret.Stroke = PrimaryBrush;
            ret.A = new Serializer.Point(p1.Position);
            ret.B = new Serializer.Point(p2.Position);
            return ret;
        }

        public override Point GetPosition()
        {
            return new Point(p.X1, p.Y1);
        }

        override protected void CreatePoints()
        {
            p1 = new MovePoint(DrawControl.TopCanvas, this, new Point(p.X1, p.Y1), DrawControl.RevScale, (e, mouseDrag) =>
            {
                vs.X1 = p.X1 = e.X;
                vs.Y1 = p.Y1 = e.Y;
            });

            p2 = new MovePoint(DrawControl.TopCanvas, this, new Point(p.X2, p.Y2), DrawControl.RevScale, (e, mouseDrag) =>
            {
                vs.X2 = p.X2 = e.X;
                vs.Y2 = p.Y2 = e.Y;
            });
        }

        override public void CreateImage(Canvas canvas)
        {
            System.Windows.Shapes.Line p = new System.Windows.Shapes.Line();
            Point a = p1.Position;
            Point b = p2.Position;
            p.X1 = a.X;
            p.Y1 = a.Y;
            p.X2 = b.X;
            p.Y2 = b.Y;
            p.Stroke = primaryBrush;
            p.StrokeThickness = thickness;
            p.ToolTip = null;
            canvas.Children.Add(p);
        }

        public override void ChangeZoom()
        {
            vs.StrokeThickness = Math.Max(3 * DrawControl.RevScale.ScaleX, p.StrokeThickness);
        }
    }
}
