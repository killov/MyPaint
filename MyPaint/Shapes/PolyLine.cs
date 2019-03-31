using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MyPaint.Shapes
{
    public class PolyLine : Shape
    {

        List<MovePoint> movepoints = new List<MovePoint>();
        bool start = false;

        Path path, vs;
        PathFigure pf;
        LineSegment ls;
        bool fclick;

        public PolyLine(DrawControl c, Layer la) : base(c, la)
        {

        }

        public PolyLine(DrawControl c, Layer la, Serializer.Shape s) : base(c, la, s)
        {

        }

        protected override void OnDrawInit()
        {
            path = new Path();
            MultiDraw = true;
            Element = path;
        }

        protected override void OnCreateInit(Serializer.Shape shape)
        {
            Serializer.PolyLine s = (Serializer.PolyLine)shape;
            path = new Path();
            Element = path;
            PathGeometry p = new PathGeometry();

            bool f = true;
            foreach (var point in s.Points)
            {
                if (f)
                {
                    pf = new PathFigure();
                    pf.StartPoint = new Point(point.X, point.Y);
                    p.Figures.Add(pf);
                    path.Data = p;
                    f = false;
                    continue;
                }
                pf.Segments.Add(new LineSegment(new Point(point.X, point.Y), true));
            }
            CreatePoints();
            CreateVirtualShape();
        }

        protected override bool OnChangeBrush(BrushEnum brushEnum, Brush brush)
        {
            if (brushEnum == BrushEnum.PRIMARY)
            {
                path.Stroke = brush;
                return true;
            }
            if (brushEnum == BrushEnum.SECONDARY)
            {
                path.Fill = brush;
                return true;
            }
            return false;
        }

        protected override bool OnChangeThickness(double thickness)
        {
            path.StrokeThickness = thickness;
            if (vs != null)
            {
                vs.StrokeThickness = Math.Max(3, thickness);
            }
            return true;
        }

        override public void OnDrawMouseDown(Point e, MouseButtonEventArgs ee)
        {
            StartDraw();
            path.Stroke = DrawControl.GetShapePrimaryColor();
            path.StrokeThickness = DrawControl.GetShapeThickness();
            path.ToolTip = null;
            PathGeometry p = new PathGeometry();
            pf = new PathFigure();
            pf.StartPoint = e;
            p.Figures.Add(pf);
            path.Data = p;
            AddToLayer();
            ls = new LineSegment();
            ls.Point = e;
            pf.Segments.Add(ls);
            fclick = true;
        }

        override public void OnDrawMouseMove(Point e)
        {
            if (start)
            {
                ls.Point = e;
            }
        }

        override public void OnDrawMouseUp(Point e, MouseButtonEventArgs ee)
        {
            if (fclick)
            {
                fclick = false;
                start = true;
                return;
            }
            if (ee.ChangedButton == MouseButton.Right)
            {
                if (start)
                {

                    path.ToolTip = null;
                    path.Cursor = Cursors.SizeAll;


                    StopDraw();
                    CreatePoints();
                    CreateVirtualShape();
                    SetActive();
                }
            }
            else
            {
                ls.Point = e;
                ls = new LineSegment();
                ls.Point = e;
                pf.Segments.Add(ls);
            }
        }

        override protected void CreateVirtualShape()
        {
            vs = new Path();
            vs.Data = path.Data;
            vs.Stroke = nullBrush;
            vs.Fill = nullBrush;
            vs.StrokeThickness = path.StrokeThickness;
            vs.Cursor = Cursors.SizeAll;
            vs.MouseDown += CallBack;
            VirtualElement = vs;
        }

        override protected void OnSetActive()
        {
            DrawControl.SetPrimaryColor(path.Stroke);
            DrawControl.SetSecondaryColor(path.Fill);
            DrawControl.SetThickness(path.StrokeThickness);
            foreach (MovePoint p in movepoints)
            {
                p.Show();
            }
        }

        override protected void OnMoveDrag(Point e)
        {
            foreach (var p in movepoints)
            {
                p.MoveDrag(e);
            }
        }

        override protected void OnStopDrag()
        {
            foreach (var p in movepoints)
            {
                p.StopDrag();
            }
        }

        override protected void OnStopEdit()
        {
            foreach (var p in movepoints)
            {
                p.Hide();
            }
        }

        override protected void OnMoveShape(Point point)
        {
            MovePoint firstPoint = movepoints[0];
            for (int i = 1; i < movepoints.Count; i++)
            {
                MovePoint p = movepoints[i];
                p.Move(point + (p.Position - firstPoint.Position));
            }
            firstPoint.Move(point);
        }

        override public Serializer.Shape CreateSerializer()
        {
            Serializer.PolyLine ret = new Serializer.PolyLine();
            ret.LineWidth = GetThickness();
            ret.Stroke = PrimaryBrush;
            ret.Fill = SecondaryBrush;
            ret.Points = new List<Serializer.Point>();
            foreach (var point in movepoints)
            {
                ret.Points.Add(new Serializer.Point(point.Position));
            }
            return ret;
        }

        override public Point GetPosition()
        {
            return pf.StartPoint;
        }

        override protected void CreatePoints()
        {
            movepoints = new List<MovePoint>();
            MovePoint mp = new MovePoint(DrawControl.TopCanvas, this, pf.StartPoint, DrawControl.RevScale, (po, mouseDrag) =>
            {
                pf.StartPoint = po;
            });
            movepoints.Add(mp);
            for (int i = 0; i < pf.Segments.Count; i++)
            {
                cp(i);
            }
        }

        void cp(int i)
        {
            LineSegment ls = (LineSegment)pf.Segments[i];
            MovePoint mp = new MovePoint(DrawControl.TopCanvas, this, ls.Point, DrawControl.RevScale, (po, mouseDrag) =>
            {
                ls.Point = po;
            });
            movepoints.Add(mp);
        }

        override public void CreateImage(Canvas canvas)
        {
            Path p = new Path();
            PathGeometry pg = new PathGeometry();
            p.Data = pg;
            PathFigure pf = new PathFigure();
            pg.Figures.Add(pf);
            pf.StartPoint = movepoints[0].Position;
            for (int i = 1; i < movepoints.Count; i++)
            {
                pf.Segments.Add(new LineSegment(movepoints[i].Position, true));
            }
            p.Stroke = PrimaryBrush.CreateBrush();
            p.Fill = SecondaryBrush.CreateBrush();
            p.StrokeThickness = thickness;
            p.ToolTip = null;
            canvas.Children.Add(p);
        }
    }
}
