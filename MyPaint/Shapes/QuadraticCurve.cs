using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MyPaint.Shapes
{
    public class QuadraticCurve : Shape
    {
        Path p, vs;
        MovePoint p1, p2, p3;
        PathFigure pf;
        QuadraticBezierSegment qbs;
        System.Windows.Shapes.Line eL1, eL2;

        public QuadraticCurve(DrawControl c, Layer la) : base(c, la)
        {

        }

        public QuadraticCurve(DrawControl c, Layer la, Serializer.Shape s) : base(c, la, s)
        {

        }

        protected override void OnDrawInit()
        {
            p = new Path();
            Element = p;
        }

        protected override void OnCreateInit(Serializer.Shape shape)
        {
            Serializer.QuadraticCurve s = (Serializer.QuadraticCurve)shape;
            p = new Path();
            Element = p;
            PathGeometry pg = new PathGeometry();
            p.Data = pg;
            pf = new PathFigure();
            pg.Figures.Add(pf);
            pf.StartPoint = new Point(s.A.X, s.A.Y);
            qbs = new QuadraticBezierSegment(new Point(s.B.X, s.B.Y), new Point(s.C.X, s.C.Y), true);
            pf.Segments.Add(qbs);

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
                vs.StrokeThickness = Math.Max(3, thickness);
            }
            return true;
        }

        override public void OnDrawMouseDown(Point e, MouseButtonEventArgs ee)
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

        override public void OnDrawMouseMove(Point e)
        {
            Point a = pf.StartPoint;
            qbs.Point1 = new Point((e.X + a.X) / 2, (e.Y + a.Y) / 2);
            qbs.Point2 = e;
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
            eL1 = new System.Windows.Shapes.Line();
            eL2 = new System.Windows.Shapes.Line();
            vs = new Path();
            vs.Data = p.Data;
            vs.Cursor = Cursors.SizeAll;
            vs.Stroke = nullBrush;
            vs.Fill = nullBrush;
            vs.StrokeThickness = Math.Max(3 * DrawControl.RevScale.ScaleX, p.StrokeThickness);
            vs.MouseDown += CallBack;

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
            VirtualElement = vs;
        }

        override protected void OnSetActive()
        {
            DrawControl.SetPrimaryColor(primaryBrush);
            DrawControl.SetSecondaryColor(secondaryBrush);
            DrawControl.SetThickness(thickness);
            DrawControl.TopCanvas.Children.Add(eL1);
            DrawControl.TopCanvas.Children.Add(eL2);
            p1.Show();
            p2.Show();
            p3.Show();

        }

        override protected void OnMoveDrag(Point e)
        {
            p1.MoveDrag(e);
            p2.MoveDrag(e);
            p3.MoveDrag(e);
        }

        override protected void OnStopDrag()
        {
            p1.StopDrag();
            p2.StopDrag();
            p3.StopDrag();
        }

        override protected void OnStopEdit()
        {
            p1.Hide();
            p2.Hide();
            p3.Hide();
            DrawControl.TopCanvas.Children.Remove(eL1);
            DrawControl.TopCanvas.Children.Remove(eL2);
        }

        protected override void OnMoveShape(Point point)
        {
            Vector v = point - p1.Position;
            p1.Move(point);
            p2.Move(p2.Position + v);
            p3.Move(p3.Position + v);
        }

        public override Serializer.Shape CreateSerializer()
        {
            Serializer.QuadraticCurve ret = new Serializer.QuadraticCurve();
            ret.LineWidth = GetThickness();
            ret.Stroke = PrimaryBrush;
            ret.Fill = SecondaryBrush;
            ret.A = new Serializer.Point(p1.Position);
            ret.B = new Serializer.Point(p2.Position);
            ret.C = new Serializer.Point(p3.Position);
            return ret;
        }

        public override Point GetPosition()
        {
            return pf.StartPoint;
        }

        protected override void CreatePoints()
        {
            p1 = new MovePoint(DrawControl.TopCanvas, this, pf.StartPoint, DrawControl.RevScale, (e, mouseDrag) =>
            {
                pf.StartPoint = e;
                eL1.X1 = e.X;
                eL1.Y1 = e.Y;
            });

            p2 = new MovePoint(DrawControl.TopCanvas, this, qbs.Point1, DrawControl.RevScale, (e, mouseDrag) =>
            {
                qbs.Point1 = e;
                eL1.X2 = eL2.X1 = e.X;
                eL1.Y2 = eL2.Y1 = e.Y;
            });

            p3 = new MovePoint(DrawControl.TopCanvas, this, qbs.Point2, DrawControl.RevScale, (e, mouseDrag) =>
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
            p.Data = pg;
            PathFigure pf = new PathFigure();
            pg.Figures.Add(pf);
            pf.StartPoint = p1.Position;

            pf.Segments.Add(new QuadraticBezierSegment(p2.Position, p3.Position, true));

            p.Stroke = PrimaryBrush.CreateBrush();
            p.Fill = SecondaryBrush.CreateBrush();
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
