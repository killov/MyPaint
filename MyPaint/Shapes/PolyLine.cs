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

        Path path = new Path(), vs = new Path();
        PathFigure pf;
        LineSegment ls;
        bool fclick;

        public PolyLine(DrawControl c, Layer la) : base(c, la)
        {
            MultiDraw = true;
            Element = path;
        }

        public PolyLine(DrawControl c, Layer la, Deserializer.Shape s) : base(c, la, s)
        {
            Element = path;
            PathGeometry p = new PathGeometry();


            bool f = true;
            foreach (var point in s.points)
            {
                if (f)
                {
                    pf = new PathFigure();
                    pf.StartPoint = new Point(point.x, point.y);
                    p.Figures.Add(pf);
                    path.Data = p;
                    f = false;
                    continue;
                }
                pf.Segments.Add(new LineSegment(new Point(point.x, point.y), true));
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

        override public void DrawMouseDown(Point e, MouseButtonEventArgs ee)
        {
            StartDraw();
            path.Stroke = File.GetShapePrimaryColor();
            path.StrokeThickness = File.GetShapeThickness();
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

        override public void DrawMouseMove(Point e)
        {
            if (start)
            {
                ls.Point = e;
            }
        }

        override public void DrawMouseUp(Point e, MouseButtonEventArgs ee)
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

        void CreateVirtualShape()
        {

            vs.Data = path.Data;
            vs.Stroke = nullBrush;
            vs.Fill = nullBrush;
            vs.StrokeThickness = path.StrokeThickness;
            vs.Cursor = Cursors.SizeAll;
            vs.MouseDown += CallBack;
            VirtualElement = vs;
        }

        override public void SetActive()
        {
            base.SetActive();
            File.SetPrimaryColor(path.Stroke);
            File.SetSecondaryColor(path.Fill);
            File.SetThickness(path.StrokeThickness);
            foreach (MovePoint p in movepoints)
            {
                p.Show();
            }
        }

        override public void MoveDrag(Point e)
        {
            base.MoveDrag(e);
            foreach (var p in movepoints)
            {
                p.MoveDrag(e);
            }
        }

        override public void StopDrag()
        {
            base.StopDrag();
            foreach (var p in movepoints)
            {
                p.StopDrag();
            }
        }

        override public void StopEdit()
        {
            base.StopEdit();
            foreach (var p in movepoints)
            {
                p.Hide();
            }
        }

        override public void MoveShape(double x, double y)
        {
            base.MoveShape(x, y);
            for (int i = 0; i < pf.Segments.Count; i++)
            {
                LineSegment ls = ((LineSegment)pf.Segments[i]);
                Point po = new Point(ls.Point.X - pf.StartPoint.X + x, ls.Point.Y - pf.StartPoint.Y + y);

                ls.Point = po;
                movepoints[i + 1].Move(po.X, po.Y);
            }
            pf.StartPoint = new Point(x, y);
            movepoints[0].Move(x, y);
        }

        override public Serializer.Shape CreateSerializer()
        {
            Serializer.PolyLine ret = new Serializer.PolyLine();
            ret.lineWidth = GetThickness();
            ret.stroke = PrimaryBrush;
            ret.fill = SecondaryBrush;
            ret.points = new List<Serializer.Point>();
            foreach (var point in movepoints)
            {
                ret.points.Add(new Serializer.Point(point.GetPosition()));
            }
            return ret;
        }

        override public Point GetPosition()
        {
            return pf.StartPoint;
        }

        void CreatePoints()
        {
            movepoints = new List<MovePoint>();
            MovePoint mp = new MovePoint(File.TopCanvas, this, pf.StartPoint, File.RevScale, (Point po) =>
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
            MovePoint mp = new MovePoint(File.TopCanvas, this, ls.Point, File.RevScale, (Point po) =>
            {
                ls.Point = po;
            });
            movepoints.Add(mp);
        }

        override public void CreateImage(Canvas canvas)
        {
            Path p = new Path();
            PathGeometry pg = new PathGeometry();
            p.DataContext = pg;
            PathFigure pf = new PathFigure();
            pg.Figures.Add(pf);
            pf.StartPoint = movepoints[0].GetPosition();
            for (int i = 1; i < movepoints.Count; i++)
            {
                pf.Segments.Add(new LineSegment(movepoints[i].GetPosition(), true));
            }
            p.Stroke = PrimaryBrush.CreateBrush();
            p.Fill = SecondaryBrush.CreateBrush();
            p.StrokeThickness = thickness;
            p.ToolTip = null;
            canvas.Children.Add(p);
        }
    }
}
