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
        TextBox p = new TextBox(), vs = new TextBox();
        double sx, sy, ex, ey;
        EditRect eR;
        FontFamily font;
        double size;
        string text = "";
        public Text(FileControl c, Layer la) : base(c, la)
        {
            SetFont(drawControl.GetTextFont());
            SetFontSize(drawControl.GetTextFontSize());
        }

        public Text(FileControl c, Layer la, Deserializer.Shape s) : base(c, la, s)
        {
            sx = s.A.x;
            sy = s.A.y;
            p.BorderThickness = new Thickness(0);
            AddToCanvas(p);
            Canvas.SetLeft(p, sx);
            Canvas.SetTop(p, sy);
            moveE(p, s.B.x, s.B.y);
            CreatePoints();
            CreateVirtualShape();
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
            vs.Background = nullBrush;
            vs.Foreground = nullBrush;
            vs.CaretBrush = Brushes.Black;
            vs.AcceptsReturn = true;
            vs.AcceptsTab = true;

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
                vs.ScrollToVerticalOffset(0);
            };

            vs.MouseWheel += (sender, ee) =>
            {
                vs.ScrollToVerticalOffset(0);
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
            drawControl.SetPrimaryColor(GetPrimaryColor());
            drawControl.SetSecondaryColor(GetSecondaryColor());
            drawControl.SetFont(GetFont());
            drawControl.SetFontSize(GetFontSize());
            drawControl.ShowWindowFontPanel(true);
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
            if(text != vs.Text){
                SetText(vs.Text, true);
            }
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

        override public Serializer.Shape CreateSerializer()
        {
            Serializer.Text ret = new Serializer.Text();
            ret.A = new Serializer.Point(Math.Min(sx, ex), Math.Min(sy, ey));
            ret.w = (int)Math.Abs(sx - ex);
            ret.h = (int)Math.Abs(sy - ey);
            ret.stroke = Serializer.Brush.Create(GetPrimaryColor());
            ret.fill = Serializer.Brush.Create(GetSecondaryColor());
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
            TextBox p = new TextBox();
            p.Foreground = PrimaryColor.CreateBrush();
            p.Background = SecondaryColor.CreateBrush();
            p.Width = Math.Abs(sx - ex);
            p.Height = Math.Abs(sy - ey);
            p.Text = text;
            p.FontFamily = font;
            p.FontSize = size;
            canvas.Children.Add(p);
            Canvas.SetLeft(p, Math.Min(sx,ex));
            Canvas.SetTop(p, Math.Min(sy,ey));
        }

        override public void ChangeZoom()
        {
            eR.ChangeZoom();
        }

        public string GetText()
        {
            return text;
        }

        public void SetText(string t, bool addHistory = false)
        {
            if (addHistory)
            {
                drawControl.historyControl.add(new History.HistoryShapeText(this, GetText(), t));
            }
            text = t;
            p.Text = t;
            vs.Text = t;
        }

        public FontFamily GetFont()
        {
            return font;
        }

        public void SetFont(FontFamily f, bool addHistory = false)
        {
            if (addHistory)
            {
                drawControl.historyControl.add(new History.HistoryShapeTextFont(this, GetFont(), f));
            }
            font = f;
            p.FontFamily = f;
            vs.FontFamily = f;
        }

        public double GetFontSize()
        {
            return size;
        }

        public void SetFontSize(double s, bool addHistory = false)
        {
            if (addHistory)
            {
                drawControl.historyControl.add(new History.HistoryShapeTextFontSize(this, GetFontSize(), s));
            }
            size = s;
            p.FontSize = s;
            vs.FontSize = s;
        }



    }
}
