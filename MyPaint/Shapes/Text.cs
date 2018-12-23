using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
        public Text(DrawControl c, Layer la) : base(c, la)
        {
            Element = p;
            SetFont(File.GetTextFont());
            SetFontSize(File.GetTextFontSize());
        }

        public Text(DrawControl c, Layer la, Deserializer.Shape s) : base(c, la, s)
        {
            p.AcceptsTab = false;
            vs.AcceptsTab = false;
            Element = p;
            sx = s.A.x;
            sy = s.A.y;
            p.BorderThickness = new Thickness(0);
            SetText(s.b64);
            SetFontSize(s.lineWidth);
            SetFont(new FontFamily(s.font));
            AddToLayer();
            Canvas.SetLeft(p, sx);
            Canvas.SetTop(p, sy);
            moveE(p, s.A.x + s.w, s.A.y + s.h);
            CreatePoints();
            CreateVirtualShape();
        }

        protected override bool OnChangeBrush(BrushEnum brushEnum, Brush brush)
        {
            if (brushEnum == BrushEnum.PRIMARY)
            {
                p.Foreground = brush;
                return true;
            }
            if (brushEnum == BrushEnum.SECONDARY)
            {
                p.Background = brush;
                return true;
            }
            return false;
        }

        protected override bool OnChangeThickness(double thickness)
        {
            return false;
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
            p.Foreground = File.GetShapePrimaryColor();
            p.Background = File.GetShapeSecondaryColor();

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
            VirtualElement = vs;
        }

        override public void SetActive()
        {
            base.SetActive();
            File.SetPrimaryColor(GetBrush(BrushEnum.PRIMARY));
            File.SetSecondaryColor(GetBrush(BrushEnum.SECONDARY));
            File.SetFont(GetFont());
            File.SetFontSize(GetFontSize());
            File.ShowWindowFontPanel(true);
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
            ChangeText();
            Keyboard.Focus(null);
            eR.StopEdit();
        }

        void ChangeText()
        {
            if (text != vs.Text)
            {
                SetText(vs.Text, true);
            }
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
            ret.stroke = PrimaryBrush;
            ret.fill = SecondaryBrush;
            ret.b64 = GetText();
            ret.font = font.Source;
            ret.lineWidth = size;
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
            eR.SetFill(false);
        }

        override public void CreateImage(Canvas canvas)
        {
            TextBox p = new TextBox();
            p.Foreground = PrimaryBrush.CreateBrush();
            p.Background = SecondaryBrush.CreateBrush();
            p.BorderThickness = new Thickness(0);
            p.Width = Math.Abs(sx - ex);
            p.Height = Math.Abs(sy - ey);
            p.Text = text;
            p.FontFamily = font;
            p.FontSize = size;
            canvas.Children.Add(p);
            Canvas.SetLeft(p, Math.Min(sx, ex));
            Canvas.SetTop(p, Math.Min(sy, ey));
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
                File.HistoryControl.Add(new History.HistoryShapeText(this, GetText(), t));
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
            ChangeText();
            if (f != null)
            {
                if (addHistory)
                {
                    File.HistoryControl.Add(new History.HistoryShapeTextFont(this, GetFont(), f));
                }
                font = f;
                p.FontFamily = f;
                vs.FontFamily = f;
            }
        }

        public double GetFontSize()
        {
            return size;
        }

        public void SetFontSize(double s, bool addHistory = false)
        {
            ChangeText();
            if (s > 0)
            {
                if (addHistory)
                {
                    File.HistoryControl.Add(new History.HistoryShapeTextFontSize(this, GetFontSize(), s));
                }
                size = s;
                p.FontSize = s;
                vs.FontSize = s;
            }
        }
    }
}
