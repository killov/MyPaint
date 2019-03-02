using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MyPaint.Shapes
{
    public class Pencil : Shape
    {

        List<MovePoint> movepoints = new List<MovePoint>();


        Path path, vs;
        PathFigure pf;
        LineSegment ls;
        EditRect area;
        List<Point> points = new List<Point>();
        double width, height;

        public Pencil(DrawControl c, Layer la) : base(c, la)
        {

        }

        public Pencil(DrawControl c, Layer la, Serializer.Shape s) : base(c, la, s)
        {

        }

        protected override void OnDrawInit()
        {
            path = new Path();
            Element = path;
        }

        protected override void OnCreateInit(Serializer.Shape shape)
        {
            Serializer.Pencil s = (Serializer.Pencil)shape;
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
        }

        override public void OnDrawMouseMove(Point e)
        {
            ls = new LineSegment();
            ls.Point = e;
            pf.Segments.Add(ls);

        }

        override public void OnDrawMouseUp(Point e, MouseButtonEventArgs ee)
        {
            path.ToolTip = null;
            path.Cursor = Cursors.SizeAll;

            StopDraw();
            CreatePoints();
            CreateVirtualShape();
            SetActive();
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

        override public void SetActive()
        {
            base.SetActive();
            DrawControl.SetPrimaryColor(path.Stroke);
            DrawControl.SetSecondaryColor(path.Fill);
            DrawControl.SetThickness(path.StrokeThickness);
            area.SetActive();
        }

        override public void MoveDrag(Point e)
        {
            base.MoveDrag(e);
            area.MoveDrag(e);
        }

        override public void StopDrag()
        {
            base.StopDrag();
            area.StopDrag();
        }

        override public void StopEdit()
        {
            base.StopEdit();
            area.StopEdit();
        }

        override public void MoveShape(Point point)
        {
            base.MoveShape(point);
            area.Move(point);
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
            return area.Position;
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

            createArea();
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

        void createArea()
        {
            double left, right, top, bottom;

            left = double.MaxValue;
            right = double.MinValue;
            top = double.MaxValue;
            bottom = double.MinValue;

            movepoints.ForEach((point) =>
            {
                Point position = point.Position;
                if (position.X < left)
                {
                    left = position.X;
                }
                if (position.X > right)
                {
                    right = position.X;
                }
                if (position.Y < top)
                {
                    top = position.Y;
                }
                if (position.Y > bottom)
                {
                    bottom = position.Y;
                }
            });

            width = right - left;
            height = bottom - top;

            movepoints.ForEach((point) =>
            {
                Point position = point.Position;
                points.Add(new Point(position.X - left, position.Y - top));
            });

            area = new EditRect(DrawControl.TopCanvas, this, new Point(left, top), new Point(right, bottom), DrawControl.RevScale,
            (po, mouseDrag) =>
            {
                this.recalculatePoints();
            },
            (po, mouseDrag) =>
            {
                this.recalculatePoints();
            },
            (po, mouseDrag) =>
            {
                this.recalculatePoints();
            },
            (po, mouseDrag) =>
            {
                this.recalculatePoints();
            });
        }

        void recalculatePoints()
        {
            Point p1 = area.p1.Position;
            Point p3 = area.p3.Position;

            double left = p1.X;
            double top = p1.Y;
            double w = p3.X - p1.X;
            double h = p3.Y - p1.Y;

            double wScale = w / width;
            double hScale = h / height;

            for (int i = 0; i < points.Count; i++)
            {
                movepoints[i].Move(new Point(left + points[i].X * wScale, top + points[i].Y * hScale));
            }
        }

        override public void CreateImage(Canvas canvas)
        {
            Path p = new Path();
            PathGeometry pg = new PathGeometry();
            p.DataContext = pg;
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
