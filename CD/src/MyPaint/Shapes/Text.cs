using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyPaint.Shapes
{
    public class Text : Shape
    {
        TextBox p, vs;
        double sx, sy, ex, ey;
        EditRect eR;
        FontFamily font;
        double size;
        string text = "";
        public Text(DrawControl c, Layer la) : base(c, la)
        {

        }

        public Text(DrawControl c, Layer la, Serializer.Shape s) : base(c, la, s)
        {

        }

        protected override void OnDrawInit()
        {
            p = new TextBox();
            Element = p;
            p.BorderThickness = new Thickness(0);
            SetFont(DrawControl.GetTextFont());
            SetFontSize(DrawControl.GetTextFontSize());
        }

        protected override void OnCreateInit(Serializer.Shape shape)
        {
            Serializer.Text s = (Serializer.Text)shape;
            p = new TextBox();
            p.AcceptsTab = false;
            Element = p;
            sx = s.A.X;
            sy = s.A.Y;
            p.BorderThickness = new Thickness(0);

            SetFontSize(s.LineWidth);
            SetFont(new FontFamily(s.Font));

            Canvas.SetLeft(p, sx);
            Canvas.SetTop(p, sy);
            moveE(p, s.A.X + s.W, s.A.Y + s.H);
            CreatePoints();
            CreateVirtualShape();
            SetText(s.B64);
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

        override public void OnDrawMouseDown(Point e, MouseButtonEventArgs ee)
        {
            sx = e.X;
            sy = e.Y;


            p.BorderThickness = new Thickness(0);
            p.Foreground = DrawControl.GetShapePrimaryColor();
            p.Background = DrawControl.GetShapeSecondaryColor();

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
            vs = new TextBox();
            vs.AcceptsTab = false;
            vs.Background = nullBrush;
            vs.Foreground = nullBrush;
            vs.CaretBrush = Brushes.Black;
            vs.AcceptsReturn = true;
            vs.AcceptsTab = true;
            vs.FontFamily = font;
            vs.FontSize = size;
            vs.BorderThickness = new Thickness(0);


            moveS(vs, sx, sy);
            moveE(vs, ex, ey);
            vs.GotFocus += (sender, ee) =>
            {
                if (virtualElementCallback != null)
                {
                    virtualElementCallback(new Point(sx, sy), this, false);
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

        override protected void OnSetActive()
        {
            DrawControl.SetPrimaryColor(GetBrush(BrushEnum.PRIMARY));
            DrawControl.SetSecondaryColor(GetBrush(BrushEnum.SECONDARY));
            DrawControl.SetFont(GetFont());
            DrawControl.SetFontSize(GetFontSize());
            DrawControl.ShowWindowFontPanel(true);
            eR.SetActive();
            Keyboard.Focus(vs);
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
            ChangeText();
            Keyboard.Focus(null);
            eR.StopEdit();
        }

        void ChangeText()
        {
            if (vs != null && text != vs.Text)
            {
                SetText(vs.Text, true);
            }
        }

        override protected void OnMoveShape(Point point)
        {
            eR.Move(point);
        }

        override public Serializer.Shape CreateSerializer()
        {
            Serializer.Text ret = new Serializer.Text();
            ret.A = new Serializer.Point(Math.Min(sx, ex), Math.Min(sy, ey));
            ret.W = (int)Math.Abs(sx - ex);
            ret.H = (int)Math.Abs(sy - ey);
            ret.Stroke = PrimaryBrush;
            ret.Fill = SecondaryBrush;
            ret.B64 = GetText();
            ret.Font = font.Source;
            ret.LineWidth = size;
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
                DrawControl.HistoryControl.Add(new History.HistoryShapeText(this, GetText(), t));
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
                    DrawControl.HistoryControl.Add(new History.HistoryShapeTextFont(this, GetFont(), f));
                }
                font = f;
                p.FontFamily = f;
                if (vs != null)
                {
                    vs.FontFamily = f;
                }
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
                    DrawControl.HistoryControl.Add(new History.HistoryShapeTextFontSize(this, GetFontSize(), s));
                }
                size = s;
                p.FontSize = s;
                if (vs != null)
                {
                    vs.FontSize = s;
                }
            }
        }
    }
}
