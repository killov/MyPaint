using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyPaint.Shapes
{
    public class Ellipse : Shape
    {
        System.Windows.Shapes.Ellipse p, vs;
        double sx, sy, ex, ey;
        EditRect eR;

        public Ellipse(DrawControl c, Layer la) : base(c, la)
        {

        }

        public Ellipse(DrawControl c, Layer la, Serializer.Shape s) : base(c, la, s)
        {

        }

        protected override void OnDrawInit()
        {
            p = new System.Windows.Shapes.Ellipse();
            Element = p;
        }

        protected override void OnCreateInit(Serializer.Shape shape)
        {
            Serializer.Ellipse s = (Serializer.Ellipse)shape;
            p = new System.Windows.Shapes.Ellipse();
            Element = p;
            sx = s.A.X;
            sy = s.A.Y;
            Canvas.SetLeft(p, sx);
            Canvas.SetTop(p, sy);
            moveE(p, s.B.X, s.B.Y);
            CreateVirtualShape();
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

        void moveS(System.Windows.Shapes.Ellipse p, double x, double y)
        {
            if (x > ex)
            {
                Canvas.SetLeft(p, ex);
                p.Width = x - ex;
            }
            else
            {
                Canvas.SetLeft(p, x);
                p.Width = ex - x;
            }
            if (y > ey)
            {
                Canvas.SetTop(p, ey);
                p.Height = y - ey;
            }
            else
            {
                Canvas.SetTop(p, y);
                p.Height = ey - y;
            }
            sx = x;
            sy = y;
        }

        void moveE(System.Windows.Shapes.Ellipse p, double x, double y)
        {
            if (x > sx)
            {
                Canvas.SetLeft(p, sx);
                p.Width = x - sx;
            }
            else
            {
                Canvas.SetLeft(p, x);
                p.Width = sx - x;
            }
            if (y > sy)
            {
                Canvas.SetTop(p, sy);
                p.Height = y - sy;
            }
            else
            {
                Canvas.SetTop(p, y);
                p.Height = sy - y;
            }
            ex = x;
            ey = y;
        }

        override public void OnDrawMouseDown(Point e, MouseButtonEventArgs ee)
        {
            sx = e.X;
            sy = e.Y;
            AddToLayer();
            Canvas.SetLeft(p, sx);
            Canvas.SetTop(p, sy);
            StartDraw();
        }

        override public void OnDrawMouseMove(Point e)
        {
            moveE(p, e.X, e.Y);
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
            vs = new System.Windows.Shapes.Ellipse();
            moveS(vs, sx, sy);
            moveE(vs, ex, ey);
            vs.Cursor = Cursors.SizeAll;
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
            Serializer.Ellipse ret = new Serializer.Ellipse();
            ret.LineWidth = GetThickness();
            ret.Stroke = PrimaryBrush;
            ret.Fill = SecondaryBrush;
            ret.A = new Serializer.Point(sx, sy);
            ret.B = new Serializer.Point(ex, ey);
            return ret;
        }

        override public Point GetPosition()
        {
            return new Point(sx, sy);
        }

        override protected void CreatePoints()
        {
            eR = new EditRect(DrawControl.TopCanvas, this, new Point(sx, sy), new Point(ex, ey), DrawControl.RevScale,
            (po, mouseDrag) =>
            {
                moveS(p, po.X, po.Y);
                moveS(vs, po.X, po.Y);
            },
            (po, mouseDrag) =>
            {
                moveE(p, po.X, ey);
                moveE(vs, po.X, ey);
                moveS(p, sx, po.Y);
                moveS(vs, sx, po.Y);
            },
            (po, mouseDrag) =>
            {
                moveE(p, po.X, po.Y);
                moveE(vs, po.X, po.Y);
            },
            (po, mouseDrag) =>
            {
                moveE(p, ex, po.Y);
                moveE(vs, ex, po.Y);
                moveS(p, po.X, sy);
                moveS(vs, po.X, sy);
            });
        }

        override public void CreateImage(Canvas canvas)
        {
            System.Windows.Shapes.Ellipse p = new System.Windows.Shapes.Ellipse();
            p.Stroke = PrimaryBrush.CreateBrush();
            p.Fill = SecondaryBrush.CreateBrush();
            p.StrokeThickness = GetThickness();
            p.ToolTip = null;
            p.Width = Math.Abs(sx - ex);
            p.Height = Math.Abs(sy - ey);
            canvas.Children.Add(p);
            Canvas.SetLeft(p, Math.Min(sx, ex));
            Canvas.SetTop(p, Math.Min(sy, ey));
        }

        override public void ChangeZoom()
        {
            eR.ChangeZoom();
        }

    }
}
