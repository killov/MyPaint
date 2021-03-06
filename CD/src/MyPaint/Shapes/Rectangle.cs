using System.Windows;
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

        override public void OnDrawMouseDown(Point e, MouseButtonEventArgs ee)
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

        override public void OnDrawMouseMove(Point e)
        {
            p.Points[3] = new Point(p.Points[3].X, e.Y);
            p.Points[2] = e;
            p.Points[1] = new Point(e.X, p.Points[1].Y);
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
            vs = new System.Windows.Shapes.Polygon();
            vs.Points = p.Points;
            vs.Stroke = nullBrush;
            vs.Fill = nullBrush;
            vs.Cursor = Cursors.SizeAll;
            vs.MouseDown += CallBack;
            VirtualElement = vs;
        }

        override protected void OnSetActive()
        {
            DrawControl.SetPrimaryColor(p.Stroke);
            DrawControl.SetSecondaryColor(p.Fill);
            DrawControl.SetThickness(p.StrokeThickness);
            eR.SetActive();
        }

        override protected void OnMoveDrag(Point e)
        {
            eR.MoveDrag(e);
        }

        override protected void OnStopDrag()
        {
            eR.StopDrag();
        }

        override protected void OnStopEdit()
        {
            eR.StopEdit();
        }

        override protected void OnMoveShape(Point point)
        {
            eR.Move(point);
        }

        override public Serializer.Shape CreateSerializer()
        {
            Serializer.Rectangle ret = new Serializer.Rectangle();
            ret.LineWidth = GetThickness();
            ret.Stroke = PrimaryBrush;
            ret.Fill = SecondaryBrush;
            ret.A = new Serializer.Point(eR.p1.Position);
            ret.B = new Serializer.Point(eR.p3.Position);
            return ret;
        }

        override public Point GetPosition()
        {
            return p.Points[0];
        }

        override protected void CreatePoints()
        {
            eR = new EditRect(DrawControl.TopCanvas, this, p.Points[0], p.Points[2], DrawControl.RevScale,
            (po, mouseDrag) =>
            {
                p.Points[0] = po;
            },

            (po, mouseDrag) =>
            {
                p.Points[1] = po;
            },

            (po, mouseDrag) =>
            {
                p.Points[2] = po;
            },

            (po, mouseDrag) =>
            {
                p.Points[3] = po;
            });
        }

        override public void CreateImage(Canvas canvas)
        {
            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();

            p.Points.Add(eR.p1.Position);
            p.Points.Add(eR.p2.Position);
            p.Points.Add(eR.p3.Position);
            p.Points.Add(eR.p4.Position);
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
