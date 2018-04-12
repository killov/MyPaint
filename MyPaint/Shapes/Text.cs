using System;
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
    public class Text : Shape
    {
        TextBox p = new TextBox(), vs;
        double sx, sy, ex, ey;
        EditRect eR;
        double left, top, width, height;
        public Text(FileControl c, Layer la) : base(c, la)
        {

        }

        public Text(FileControl c, Layer la, jsonDeserialize.Shape s) : base(c, la, s)
        {
            SetPrimaryColor(s.stroke == null ? null : s.stroke.CreateBrush());
            SetThickness(s.lineWidth);
            SetPrimaryColor(s.stroke == null ? null : s.stroke.CreateBrush());
            SetSecondaryColor(s.fill == null ? null : s.fill.CreateBrush());
            SetThickness(s.lineWidth);

            sx = s.A.x;
            sy = s.A.y;

            p.ToolTip = null;
            p.BorderThickness = new Thickness(0);
            AddToCanvas(p);
            left = sx;
            top = sy;
            Canvas.SetLeft(p, sx);
            Canvas.SetTop(p, sy);
            moveE(p, s.B.x, s.B.y);
            CreateVirtualShape();
            CreatePoints();

        }

        override public void SetPrimaryColor(Brush s, bool addHistory = false)
        {
            base.SetPrimaryColor(s, addHistory);
            p.Foreground = s;
        }

        override public void SetSecondaryColor(Brush s, bool addHistory = false)
        {
            base.SetSecondaryColor(s, addHistory);
            p.Background = s;
        }

        override public void AddToCanvas()
        {
            AddToCanvas(p);
        }

        override public void RemoveFromCanvas()
        {
            RemoveFromCanvas(p);
        }

        override public void SetThickness(double s, bool addHistory = false)
        {

        }

        void moveS(TextBox p, double x, double y)
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

        void moveE(TextBox p, double x, double y)
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


            p.BorderThickness = new Thickness(1);
            p.Foreground = drawControl.GetShapePrimaryColor();
            p.Background = drawControl.GetShapeSecondaryColor();
            
            AddToCanvas(p);
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

        override public void CreateVirtualShape()
        {
            vs = new TextBox();
            vs.Background = nullBrush;
            vs.Foreground = nullBrush;

            vs.CaretBrush = Brushes.Black;
            vs.AcceptsReturn = true;
            vs.BorderThickness = new Thickness(0);
            p.BorderThickness = new Thickness(0);
            moveS(vs, sx, sy);
            moveE(vs, ex, ey);

            vs.GotFocus += (sender, ee) =>
            {
                if (virtualShapeCallback != null)
                {
                    virtualShapeCallback(new Point(sx, sy), this, false);
                    hit = true;
                }
            };
           
            vs.TextChanged += (sender, ee) =>
            {
                p.Text = vs.Text;
            };

        }

        override public void ShowVirtualShape(MyOnMouseDown mouseDown)
        {
            base.ShowVirtualShape(mouseDown);
            HideVirtualShape();
            drawControl.topCanvas.Children.Add(vs);
        }

        override public void HideVirtualShape()
        {
            drawControl.topCanvas.Children.Remove(vs);
        }

        override public void SetActive()
        {
            base.SetActive();
            //drawControl.SetPrimaryColor(p.Stroke);
            //drawControl.SetSecondaryColor(p.Fill);
            //drawControl.SetThickness(p.StrokeThickness);
            eR.SetActive();
            Keyboard.Focus(vs);
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
            Keyboard.Focus(null);
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

        override public jsonSerialize.Shape CreateSerializer()
        {
            jsonSerialize.Ellipse ret = new jsonSerialize.Ellipse();
            ret.lineWidth = GetThickness();
            ret.stroke = PrimaryColor;
            ret.fill = SecondaryColor;
            ret.A = new jsonSerialize.Point(sx, sy);
            ret.B = new jsonSerialize.Point(ex, ey);
            return ret;
        }

        override public Point GetPosition()
        {
            return new Point(sx, sy);
        }

        override public void CreatePoints()
        {
            eR = new EditRect(drawControl.topCanvas, this, new Point(sx, sy), new Point(ex, ey), drawControl.revScale,
            (po) =>
            {
                moveS(p, po.X, po.Y);
                moveS(vs, po.X, po.Y);
                return true;
            },
            (po) =>
            {
                moveE(p, po.X, ey);
                moveE(vs, po.X, ey);
                moveS(p, sx, po.Y);
                moveS(vs, sx, po.Y);
                return true;
            },
            (po) =>
            {
                moveE(p, po.X, po.Y);
                moveE(vs, po.X, po.Y);
                return true;
            },
            (po) =>
            {
                moveE(p, ex, po.Y);
                moveE(vs, ex, po.Y);
                moveS(p, po.X, sy);
                moveS(vs, po.X, sy);
                return true;
            });
        }

        override public void CreateImage(Canvas canvas)
        {
            System.Windows.Shapes.Ellipse p = new System.Windows.Shapes.Ellipse();

            p.Stroke = primaryColor;
            p.Fill = secondaryColor;
            p.StrokeThickness = thickness;
            p.ToolTip = null;
            p.Width = width;
            p.Height = height;
            canvas.Children.Add(p);
            Canvas.SetLeft(p, left);
            Canvas.SetTop(p, top);

        }

        override public void ChangeZoom()
        {
            eR.ChangeZoom();
        }

    }
}
