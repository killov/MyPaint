using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MyPaint.Shapes
{
    public class Polygon : Shape
    {
        System.Windows.Shapes.Polygon p, vs;
        List<MovePoint> movepoints = new List<MovePoint>();
        bool start = false;
        List<Point> points = new List<Point>();

        Path path = new Path();
        PathFigure pf;
        LineSegment ls;
        public Polygon(DrawControl c, Layer la) : base(c, la)
        {

        }

        public Polygon(DrawControl c, Layer la, Serializer.Shape s) : base(c, la, s)
        {

        }

        protected override void OnDrawInit()
        {
            p = new System.Windows.Shapes.Polygon();
            MultiDraw = true;
            Element = p;
        }

        protected override void OnCreateInit(Serializer.Shape shape)
        {
            Serializer.Polygon s = (Serializer.Polygon)shape;
            p = new System.Windows.Shapes.Polygon();
            Element = p;
            SetThickness(s.LineWidth);

            foreach (var point in s.Points)
            {
                p.Points.Add(new Point(point.X, point.Y));
            }
            p.ToolTip = null;
            p.Cursor = Cursors.SizeAll;


            CreatePoints();
            CreateVirtualShape();
        }

        protected override bool OnChangeBrush(BrushEnum brushEnum, Brush brush)
        {
            if (brushEnum == BrushEnum.PRIMARY)
            {
                p.Stroke = brush;
                if (path != null)
                {
                    path.Stroke = brush;
                }
                return true;
            }
            if (brushEnum == BrushEnum.SECONDARY)
            {
                p.Fill = brush;
                if (path != null)
                {
                    path.Fill = brush;
                }
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

        override public void OnDrawMouseDown(Point e, MouseButtonEventArgs ee)
        {
            StartDraw();
            PathGeometry p = new PathGeometry();
            pf = new PathFigure();
            pf.StartPoint = e;
            p.Figures.Add(pf);
            path.Data = p;
            Element = path;
            AddToLayer();
            ls = new LineSegment();
            ls.Point = e;
            pf.Segments.Add(ls);
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
            start = true;
            ls.Point = e;
            ls = new LineSegment();
            ls.Point = e;
            pf.Segments.Add(ls);

            points.Add(e);

            if (ee.ChangedButton == MouseButton.Right)
            {
                if (start)
                {
                    PointCollection ppoints = new PointCollection();
                    foreach (var p in points)
                    {
                        ppoints.Add(p);
                    }
                    Element = p;
                    p.Points = ppoints;
                    StopDraw();
                    CreatePoints();
                    CreateVirtualShape();
                    SetActive();
                }
            }
        }

        override protected void CreateVirtualShape()
        {
            vs = new System.Windows.Shapes.Polygon();
            vs.Points = p.Points;
            vs.Stroke = nullBrush;
            vs.Fill = nullBrush;
            vs.StrokeThickness = p.StrokeThickness;
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

        override public void MoveShape(Point point)
        {
            base.MoveShape(point);
            MovePoint firstPoint = movepoints[0];
            for (int i = 1; i < p.Points.Count; i++)
            {
                MovePoint p = movepoints[i];
                p.Move(point + (p.GetPosition() - firstPoint.GetPosition()));
            }
            firstPoint.Move(point);
        }

        override public Serializer.Shape CreateSerializer()
        {
            Serializer.Polygon ret = new Serializer.Polygon();
            ret.LineWidth = GetThickness();
            ret.Stroke = PrimaryBrush;
            ret.Fill = SecondaryBrush;
            ret.Points = new List<Serializer.Point>();
            foreach (var point in movepoints)
            {
                ret.Points.Add(new Serializer.Point(point.GetPosition()));
            }
            return ret;
        }

        override public Point GetPosition()
        {
            return p.Points[0];
        }

        override protected void CreatePoints()
        {
            movepoints = new List<MovePoint>();
            for (int i = 0; i < movepoints.Count; i++)
            {
                cp(i);
            }
        }

        void cp(int i)
        {
            MovePoint mp = new MovePoint(DrawControl.TopCanvas, this, p.Points[i], DrawControl.RevScale, (po, mouseDrag) =>
            {
                p.Points[i] = po;
            });
            movepoints.Add(mp);
        }

        override public void CreateImage(Canvas canvas)
        {
            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();

            foreach (var point in movepoints)
            {
                p.Points.Add(point.GetPosition());
            }
            p.Stroke = PrimaryBrush.CreateBrush();
            p.Fill = SecondaryBrush.CreateBrush();
            p.StrokeThickness = GetThickness();
            canvas.Children.Add(p);
        }
    }
}
