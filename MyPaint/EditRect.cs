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

namespace MyPaint
{
    public class EditRect
    {
        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon(), vs;
        MovePoint p1, p2, p3, p4;
        Canvas canvas;
        double scale;
        public EditRect(Canvas c, Shapes.Shape s, Point A, Point B, ScaleTransform revScale, posun Af, posun Bf, posun Cf, posun Df)
        {
            p.Points.Add(new Point(A.X, A.Y));
            p.Points.Add(new Point(B.X, A.Y));
            p.Points.Add(new Point(B.X, B.Y));
            p.Points.Add(new Point(A.X, B.Y));
            p.Stroke = Brushes.Black;
            p.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 255));
            p.StrokeThickness = 1;
            p.ToolTip = null;
            p.Cursor = Cursors.SizeAll;
            p.MouseDown +=  (o, e) =>
            {
                s.drawControl.StartMoveShape(s.GetPosition(), e.GetPosition(canvas));
            };
            DoubleCollection dash = new DoubleCollection();
            dash.Add(4);
            dash.Add(6);
            p.StrokeDashArray = dash;
            canvas = c;
            p1 = new MovePoint(c, s, p.Points[0], revScale, (po) =>
            {
                if(Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    double x, y;
                    if(((p.Points[0].X - p.Points[2].X) * (po.Y - p.Points[2].Y) - (p.Points[0].Y - p.Points[2].Y) * (po.X - p.Points[2].X)) > 0)
                    {
                        x = po.X;
                        y = p.Points[3].Y - ((p.Points[1].X - x) * scale);
                    }
                    else
                    {
                        y = po.Y;
                        x = p.Points[1].X - ((p.Points[3].Y - y) / scale);
                    }
   
                    p.Points[0] = new Point(x, y);
                    p.Points[1] = new Point(p.Points[1].X, y);
                    p.Points[3] = new Point(x, p.Points[3].Y);
                    p1.Move(x, y);
                    p2.Move(p.Points[1].X, y);
                    p4.Move(x, p.Points[3].Y);
                    Af(new Point(x, y));
                    return true;
                }
                p.Points[0] = po;
                p.Points[1] = new Point(p.Points[1].X, po.Y);
                p.Points[3] = new Point(po.X, p.Points[3].Y);
                p1.Move(po.X, po.Y);
                p2.Move(p.Points[1].X, po.Y);
                p4.Move(po.X, p.Points[3].Y);
                Af(po);
                UpdateScale();
                return true;
            });

            p2 = new MovePoint(c, s, p.Points[1], revScale, (po) =>
            {
                p.Points[1] = po;
                p.Points[0] = new Point(po.X, p.Points[0].Y);
                p.Points[2] = new Point(p.Points[2].X, po.Y);
                p2.Move(po.X, po.Y);
                p1.Move(po.X, p.Points[0].Y);
                p3.Move(p.Points[2].X, po.Y);
                Bf(po);
                return true;
            });

            p3 = new MovePoint(c, s, p.Points[2], revScale, (po) =>
            {
                p.Points[2] = po;
                p.Points[1] = new Point(p.Points[1].X, po.Y);
                p.Points[3] = new Point(po.X, p.Points[3].Y);
                p3.Move(po.X, po.Y);
                p4.Move(po.X, p.Points[3].Y);
                p2.Move(p.Points[1].X, po.Y);
                Cf(po);
                return true;
            });

            p4 = new MovePoint(c, s, p.Points[3], revScale, (po) =>
            {
                p.Points[3] = po;
                p.Points[0] = new Point(p.Points[0].X, po.Y);
                p.Points[2] = new Point(po.X, p.Points[2].Y);
                p4.Move(po.X, po.Y);
                p3.Move(po.X, p.Points[2].Y);
                p1.Move(p.Points[0].X, po.Y);
                Df(po);
                return true;
            });
        }

        private void UpdateScale()
        {
            scale = Math.Abs((p.Points[1].Y - p.Points[2].Y) / (p.Points[0].X - p.Points[1].X));
            if(scale == double.NaN)
            {
                scale = 1;
            }
        }

        public void Move(double x, double y)
        {
            p1.Move(x, y);
            p2.Move(p.Points[1].X, p.Points[1].Y);
            p3.Move(p.Points[2].X, p.Points[2].Y);
            p4.Move(p.Points[3].X, p.Points[3].Y);
            p.Points[1] = new Point(p.Points[1].X - p.Points[0].X + x, p.Points[1].Y - p.Points[0].Y + y);
            p.Points[2] = new Point(p.Points[2].X - p.Points[0].X + x, p.Points[2].Y - p.Points[0].Y + y);
            p.Points[3] = new Point(p.Points[3].X - p.Points[0].X + x, p.Points[3].Y - p.Points[0].Y + y);
            p.Points[0] = new Point(x, y);
        }

        public void SetActive()
        {
            canvas.Children.Add(p);
            p1.Show();
            p2.Show();
            p3.Show();
            p4.Show();
        }

        public void MoveDrag(Point e)
        {
            p1.MoveDrag(e);
            p2.MoveDrag(e);
            p3.MoveDrag(e);
            p4.MoveDrag(e);
        }

        public void StopDrag()
        {
            p1.StopDrag();
            p2.StopDrag();
            p3.StopDrag();
            p4.StopDrag();
        }

        public void StopEdit()
        {
            p1.Hide();
            p2.Hide();
            p3.Hide();
            p4.Hide();
            canvas.Children.Remove(p);
        }

    }
}
