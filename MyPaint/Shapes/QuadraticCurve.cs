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
    public class QuadraticCurve : Shape
    {
        Path p = new Path(), vs = new Path();
        MovePoint p1, p2, p3;
        PathFigure pf;
        QuadraticBezierSegment qbs;
        System.Windows.Shapes.Line eL1 = new System.Windows.Shapes.Line(), eL2 = new System.Windows.Shapes.Line();

        public QuadraticCurve(FileControl c, Layer la) : base(c, la)
        {
            Element = p;
        }

        public QuadraticCurve(FileControl c, Layer la, Deserializer.Shape s) : base(c, la, s)
        {
            Element = p;
            PathGeometry pg = new PathGeometry();
            p.Data = pg;
            pf = new PathFigure();
            pg.Figures.Add(pf);
            pf.StartPoint = new Point(s.A.x, s.A.y);
            qbs = new QuadraticBezierSegment(new Point(s.B.x, s.B.y), new Point(s.C.x, s.C.y), true);
            pf.Segments.Add(qbs);
            AddToLayer();
            CreatePoints();
            CreateVirtualShape();
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
            if (vs != null) vs.StrokeThickness = Math.Max(3, s);
        }

        override public void DrawMouseDown(Point e, MouseButtonEventArgs ee)
        {
            StartDraw();
            PathGeometry pg = new PathGeometry();
            pf = new PathFigure();
            pf.StartPoint = e;
            pg.Figures.Add(pf);
            p.Data = pg;
            AddToLayer();
            qbs = new QuadraticBezierSegment();
            qbs.Point1 = e;
            qbs.Point2 = e;
            pf.Segments.Add(qbs);
        }

        override public void DrawMouseMove(Point e)
        {
            Point a = pf.StartPoint;
            qbs.Point1 = new Point((e.X + a.X) / 2, (e.Y + a.Y) / 2);
            qbs.Point2 = e;
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
            vs.Data = p.Data;
            vs.Cursor = Cursors.SizeAll;
            vs.Stroke = nullBrush;
            vs.Fill = nullBrush;
            vs.StrokeThickness = Math.Max(3 * File.RevScale.ScaleX, p.StrokeThickness);
            vs.MouseDown += delegate (object sender, MouseButtonEventArgs ee)
            {
                virtualShapeCallback(ee.GetPosition(File.Canvas), this);
                hit = true;
            };
            
            DoubleCollection dash = new DoubleCollection();
            dash.Add(4);
            dash.Add(6);
            eL1.StrokeDashArray = dash;
            eL2.StrokeDashArray = dash;
            eL1.StrokeThickness = 1;
            eL2.StrokeThickness = 1;
            eL1.Stroke = Brushes.Blue;
            eL2.Stroke = Brushes.Blue;
            eL1.X1 = pf.StartPoint.X;
            eL1.Y1 = pf.StartPoint.Y;
            eL1.X2 = qbs.Point1.X;
            eL1.Y2 = qbs.Point1.Y;
            eL2.X1 = qbs.Point1.X;
            eL2.Y1 = qbs.Point1.Y;
            eL2.X2 = qbs.Point2.X;
            eL2.Y2 = qbs.Point2.Y;

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
            File.SetPrimaryColor(primaryBrush);
            File.SetSecondaryColor(secondaryBrush);
            File.SetThickness(thickness);
            File.TopCanvas.Children.Add(eL1);
            File.TopCanvas.Children.Add(eL2);
            p1.Show();
            p2.Show();
            p3.Show();
            
        }

        override public void MoveDrag(Point e)
        {
            base.MoveDrag(e);
            p1.MoveDrag(e);
            p2.MoveDrag(e);
            p3.MoveDrag(e);
        }

        override public void StopDrag()
        {
            base.StopDrag();
            p1.StopDrag();
            p2.StopDrag();
            p3.StopDrag();
        }

        override public void StopEdit()
        {
            base.StopEdit();
            p1.Hide();
            p2.Hide();
            p3.Hide();
            File.TopCanvas.Children.Remove(eL1);
            File.TopCanvas.Children.Remove(eL2);
        }

        override public void MoveShape(double x, double y)
        {
            base.MoveShape(x, y);
            qbs.Point1 = new Point(qbs.Point1.X - pf.StartPoint.X + x, qbs.Point1.Y - pf.StartPoint.Y + y);
            qbs.Point2 = new Point(qbs.Point2.X - pf.StartPoint.X + x, qbs.Point2.Y - pf.StartPoint.Y + y);
            p1.Move(x, y);
            p2.Move(qbs.Point1.X, qbs.Point1.Y);
            p3.Move(qbs.Point2.X, qbs.Point2.Y);
            pf.StartPoint = new Point(x, y);
            eL1.X1 = x;
            eL1.Y1 = y;
            eL2.X1 = eL1.X2 = qbs.Point1.X;
            eL2.Y1 = eL1.Y2 = qbs.Point1.Y;
            eL2.X2 = qbs.Point2.X;
            eL2.Y2 = qbs.Point2.Y;
        }

        override public Serializer.Shape CreateSerializer()
        {
            Serializer.QuadraticCurve ret = new Serializer.QuadraticCurve();
            ret.lineWidth = GetThickness();
            ret.stroke = PrimaryBrush;
            ret.A = new Serializer.Point(p1.GetPosition());
            ret.B = new Serializer.Point(p2.GetPosition());
            ret.C = new Serializer.Point(p3.GetPosition());
            return ret;
        }

        override public Point GetPosition()
        {
            return pf.StartPoint;
        }

        void CreatePoints()
        {
            p1 = new MovePoint(File.TopCanvas, this, pf.StartPoint, File.RevScale, (e) =>
            {
                pf.StartPoint = e;
                eL1.X1 = e.X;
                eL1.Y1 = e.Y;
            });

            p2 = new MovePoint(File.TopCanvas, this, qbs.Point1, File.RevScale, (e) =>
            {
                qbs.Point1 = e;
                eL1.X2 = eL2.X1 = e.X;
                eL1.Y2 = eL2.Y1 = e.Y;
            });

            p3 = new MovePoint(File.TopCanvas, this, qbs.Point2, File.RevScale, (e) =>
            {
                qbs.Point2 = e;
                eL2.X2 = e.X;
                eL2.Y2 = e.Y;
            });
        }

        override public void CreateImage(Canvas canvas)
        {
            Path p = new Path();
            PathGeometry pg = new PathGeometry();
            p.DataContext = pg;
            PathFigure pf = new PathFigure();
            pg.Figures.Add(pf);
            pf.StartPoint = p1.GetPosition();

            pf.Segments.Add(new QuadraticBezierSegment(p2.GetPosition(), p3.GetPosition(), true));

            p.Stroke = PrimaryBrush.CreateBrush();
            p.Fill = SecondaryBrush.CreateBrush();
            p.StrokeThickness = thickness;
            p.ToolTip = null;
            canvas.Children.Add(p);
        }

        public override void ChangeZoom()
        {
            vs.StrokeThickness = Math.Max(3 * File.RevScale.ScaleX, p.StrokeThickness);
        }
    }
}
