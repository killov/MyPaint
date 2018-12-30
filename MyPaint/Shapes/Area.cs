using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyPaint.Shapes
{
    public class Area : Shape
    {
        System.Windows.Shapes.Polygon vs = new System.Windows.Shapes.Polygon();

        public Area(DrawControl c, Layer la) : base(c, la)
        {

        }

        public Area(DrawControl c, Layer la, Serializer.Shape s) : base(c, la, s)
        {

        }

        protected override void OnDrawInit()
        {

        }

        protected override void OnCreateInit(Serializer.Shape s)
        {

        }

        protected override bool OnChangeBrush(BrushEnum brushEnum, Brush brush)
        {
            return false;
        }

        protected override bool OnChangeThickness(double thickness)
        {
            return false;
        }

        override public void DrawMouseDown(Point e, MouseButtonEventArgs ee)
        {
            StartDraw();
            PointCollection points = new PointCollection(4);
            points.Add(e);
            points.Add(e);
            points.Add(e);
            points.Add(e);
            DoubleCollection dash = new DoubleCollection();
            dash.Add(4);
            dash.Add(6);
            vs.StrokeThickness = DrawControl.RevScale.ScaleX * 2;
            vs.StrokeDashArray = dash;
            vs.Points = points;
            vs.Stroke = Brushes.Blue;
            vs.Fill = nullBrush;
            DrawControl.TopCanvas.Children.Add(vs);
            VirtualElement = vs;
        }

        override public void DrawMouseMove(Point e)
        {
            vs.Points[3] = new Point(vs.Points[3].X, e.Y);
            vs.Points[2] = e;
            vs.Points[1] = new Point(e.X, vs.Points[1].Y);
        }

        override public void DrawMouseUp(Point e, MouseButtonEventArgs ee)
        {
            StopDraw(false);
            vs.Cursor = Cursors.SizeAll;
            vs.MouseDown += CallBack;
            SetActive();
        }

        override public void SetActive()
        {
            base.SetActive();
        }

        override public void MoveDrag(Point e)
        {
            base.MoveDrag(e);
        }

        override public void StopDrag()
        {
            base.StopDrag();
        }

        override public void StopEdit()
        {
            base.StopEdit();
        }

        override public void MoveShape(double x, double y)
        {
            base.MoveShape(x, y);
            vs.Points[1] = new Point(vs.Points[1].X - vs.Points[0].X + x, vs.Points[1].Y - vs.Points[0].Y + y);
            vs.Points[2] = new Point(vs.Points[2].X - vs.Points[0].X + x, vs.Points[2].Y - vs.Points[0].Y + y);
            vs.Points[3] = new Point(vs.Points[3].X - vs.Points[0].X + x, vs.Points[3].Y - vs.Points[0].Y + y);
            vs.Points[0] = new Point(x, y);
        }

        override public Serializer.Shape CreateSerializer()
        {
            return null;
        }

        override public Point GetPosition()
        {
            return vs.Points[0];
        }

        override public void CreateImage(Canvas canvas)
        {

        }

        override public void ChangeZoom()
        {
            vs.StrokeThickness = DrawControl.RevScale.ScaleX * 2;
        }

        public BitmapSource CreateBitmap()
        {
            return DrawControl.CreateBitmap(Math.Min(vs.Points[0].X, vs.Points[2].X), Math.Min(vs.Points[0].Y, vs.Points[2].Y), Math.Abs(vs.Points[0].X - vs.Points[2].X), Math.Abs(vs.Points[0].Y - vs.Points[2].Y));
        }

        protected override void CreatePoints()
        {
            throw new NotImplementedException();
        }

        protected override void CreateVirtualShape()
        {
            throw new NotImplementedException();
        }
    }
}
