﻿using System;
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
    public class Line : Shape
    {
        System.Windows.Shapes.Line p = new System.Windows.Shapes.Line(), vs = new System.Windows.Shapes.Line();
        MovePoint p1, p2;
        public Line(FileControl c, Layer la) : base(c, la)
        {
            element = p;
        }

        public Line(FileControl c, Layer la, Deserializer.Shape s) : base(c, la, s)
        {
            element = p;
            p.X1 = s.A.x;
            p.Y1 = s.A.y;
            p.X2 = s.B.x;
            p.Y2 = s.B.y;
            AddToCanvas();
            CreatePoints();
            CreateVirtualShape();
        }

        override public void SetPrimaryColor(Brush s, bool addHistory = false)
        {
            base.SetPrimaryColor(s, addHistory);
            p.Stroke = s;
        }

        override public void SetSecondaryColor(Brush s, bool addHistory = false)
        {

        }

        override public void SetThickness(double s, bool addHistory = false)
        {
            base.SetThickness(s, addHistory);
            p.StrokeThickness = s;
            if (vs != null) vs.StrokeThickness = Math.Max(3, s);
        }

        override public void DrawMouseDown(Point e, MouseButtonEventArgs ee)
        {
            p.X1 = e.X;
            p.Y1 = e.Y;
            p.X2 = e.X;
            p.Y2 = e.Y;
            AddToCanvas();
            StartDraw();
        }

        override public void DrawMouseMove(Point e)
        {
            p.X2 = e.X;
            p.Y2 = e.Y;
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
            vs.X1 = p.X1;
            vs.X2 = p.X2;
            vs.Y1 = p.Y1;
            vs.Y2 = p.Y2;
            vs.Cursor = Cursors.SizeAll;
            vs.Stroke = nullBrush;
            vs.StrokeThickness = Math.Max(3 * drawControl.revScale.ScaleX, p.StrokeThickness);
            vs.MouseDown += delegate (object sender, MouseButtonEventArgs ee)
            {
                virtualShapeCallback(ee.GetPosition(drawControl.canvas), this);
                hit = true;
            };
        }

        override public void ShowVirtualShape(OnMouseDownDelegate mouseDown)
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
            drawControl.SetThickness(p.StrokeThickness);
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

        override public void MoveShape(double x, double y)
        {
            base.MoveShape(x, y);
            vs.X2 = p.X2 = p.X2 - p.X1 + x;
            vs.X1 = p.X1 = x;
            vs.Y2 = p.Y2 = p.Y2 - p.Y1 + y;
            vs.Y1 = p.Y1 = y;

            p1.Move(p.X1, p.Y1);
            p2.Move(p.X2, p.Y2);
        }

        override public Serializer.Shape CreateSerializer()
        {
            Serializer.Line ret = new Serializer.Line();
            ret.lineWidth = GetThickness();
            ret.stroke = PrimaryColor;
            ret.A = new Serializer.Point(p1.GetPosition());
            ret.B = new Serializer.Point(p2.GetPosition());
            return ret;
        }

        override public Point GetPosition()
        {
            return new Point(p.X1, p.Y1);
        }

        override public void CreatePoints()
        {
            p1 = new MovePoint(drawControl.topCanvas, this, new Point(p.X1, p.Y1), drawControl.revScale, (e) =>
            {
                vs.X1 = p.X1 = e.X;
                vs.Y1 = p.Y1 = e.Y;
                return true;
            });

            p2 = new MovePoint(drawControl.topCanvas, this, new Point(p.X2, p.Y2), drawControl.revScale, (e) =>
            {
                vs.X2 = p.X2 = e.X;
                vs.Y2 = p.Y2 = e.Y;
                return true;
            });
        }

        override public void CreateImage(Canvas canvas)
        {
            System.Windows.Shapes.Line p = new System.Windows.Shapes.Line();
            Point a = p1.GetPosition();
            Point b = p2.GetPosition();
            p.X1 = a.X;
            p.Y1 = a.Y;
            p.X2 = b.X;
            p.Y2 = b.Y;
            p.Stroke = primaryColor;
            p.StrokeThickness = thickness;
            p.ToolTip = null;
            canvas.Children.Add(p);
        }

        public override void ChangeZoom()
        {
            vs.StrokeThickness = Math.Max(3 * drawControl.revScale.ScaleX, p.StrokeThickness);
        }
    }
}
