using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyPaint.Shapes
{
    public class Ellipse : Shape
    {
        System.Windows.Shapes.Ellipse p = new System.Windows.Shapes.Ellipse(), vs = new System.Windows.Shapes.Ellipse();
        double sx, sy, ex, ey;
        EditRect eR;

        public Ellipse(DrawControl c, Layer la) : base(c, la)
        {
            Element = p;
        }

        public Ellipse(DrawControl c, Layer la, Deserializer.Shape s) : base(c, la, s)
        {
            Element = p;
            sx = s.A.x;
            sy = s.A.y;
            AddToLayer();
            Canvas.SetLeft(p, sx);
            Canvas.SetTop(p, sy);
            moveE(p, s.B.x, s.B.y);
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

        override public void DrawMouseDown(Point e, MouseButtonEventArgs ee)
        {
            sx = e.X;
            sy = e.Y;
            AddToLayer();
            Canvas.SetLeft(p, sx);
            Canvas.SetTop(p, sy);
            StartDraw();
        }

        override public void DrawMouseMove(Point e)
        {
            moveE(p, e.X, e.Y);
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
            moveS(vs, sx, sy);
            moveE(vs, ex, ey);
            vs.Cursor = Cursors.SizeAll;
            vs.Stroke = nullBrush;
            vs.Fill = nullBrush;
            vs.Cursor = Cursors.SizeAll;
            vs.MouseDown += CallBack;
            VirtualElement = vs;
        }

        override public void SetActive()
        {
            base.SetActive();
            File.SetPrimaryColor(p.Stroke);
            File.SetSecondaryColor(p.Fill);
            File.SetThickness(p.StrokeThickness);
            eR.SetActive();
        }

        override public void MoveDrag(Point e)
        {
            base.MoveDrag(e);
            eR.MoveDrag(e);
        }

        override public void StopDrag()
        {
            base.StopDrag();
            eR.StopDrag();
        }

        override public void StopEdit()
        {
            base.StopEdit();
            eR.StopEdit();
        }

        override public void MoveShape(double x, double y)
        {
            base.MoveShape(x, y);


            double zx = x - sx + ex;
            double zy = y - sy + ey;


            moveE(p, zx, zy);
            moveE(vs, zx, zy);
            moveS(p, x, y);
            moveS(vs, x, y);
            eR.Move(x, y);
        }

        override public Serializer.Shape CreateSerializer()
        {
            Serializer.Ellipse ret = new Serializer.Ellipse();
            ret.lineWidth = GetThickness();
            ret.stroke = PrimaryBrush;
            ret.fill = SecondaryBrush;
            ret.A = new Serializer.Point(sx, sy);
            ret.B = new Serializer.Point(ex, ey);
            return ret;
        }

        override public Point GetPosition()
        {
            return new Point(sx, sy);
        }

        void CreatePoints()
        {
            eR = new EditRect(File.TopCanvas, this, new Point(sx, sy), new Point(ex, ey), File.RevScale,
            (po) =>
            {
                moveS(p, po.X, po.Y);
                moveS(vs, po.X, po.Y);
            },
            (po) =>
            {
                moveE(p, po.X, ey);
                moveE(vs, po.X, ey);
                moveS(p, sx, po.Y);
                moveS(vs, sx, po.Y);
            },
            (po) =>
            {
                moveE(p, po.X, po.Y);
                moveE(vs, po.X, po.Y);
            },
            (po) =>
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
