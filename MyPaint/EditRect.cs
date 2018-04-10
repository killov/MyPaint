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
        public MovePoint p1, p2, p3, p4;
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
                Point pop = Scaling(po, p.Points[0], p.Points[2], 0);
                p.Points[0] = pop;
                p.Points[1] = new Point(p.Points[1].X, pop.Y);
                p.Points[3] = new Point(pop.X, p.Points[3].Y);
                p1.Move(pop.X, pop.Y);
                p2.Move(p.Points[1].X, pop.Y);
                p4.Move(pop.X, p.Points[3].Y);
                Af(pop);
                if(pop == po)
                {
                    UpdateScale();
                }
                return true;
            });

            p2 = new MovePoint(c, s, p.Points[1], revScale, (po) =>
            {
                Point pop = Scaling(po, p.Points[1], p.Points[3], 1);
                p.Points[1] = pop;
                p.Points[0] = new Point(p.Points[0].X, pop.Y);
                p.Points[2] = new Point(pop.X, p.Points[2].Y);
                p2.Move(pop.X, pop.Y);
                p1.Move(p.Points[0].X, pop.Y);
                p3.Move(pop.X, p.Points[2].Y);
                Bf(pop);
                if (pop == po)
                {
                    UpdateScale();
                }
                return true;
            });

            p3 = new MovePoint(c, s, p.Points[2], revScale, (po) =>
            {
                Point pop = Scaling(po, p.Points[2], p.Points[0], 0);
                p.Points[2] = pop;
                p.Points[1] = new Point(pop.X, p.Points[1].Y);
                p.Points[3] = new Point(p.Points[3].X, pop.Y);
                p3.Move(pop.X, pop.Y);
                p4.Move(p.Points[3].X, pop.Y);
                p2.Move(pop.X, p.Points[1].Y);
                Cf(pop);
                if (pop == po)
                {
                    UpdateScale();
                }
                return true;
            });

            p4 = new MovePoint(c, s, p.Points[3], revScale, (po) =>
            {
                Point pop = Scaling(po, p.Points[3], p.Points[1], 1);
                p.Points[3] = pop;
                p.Points[0] = new Point(pop.X, p.Points[0].Y);
                p.Points[2] = new Point(p.Points[2].X, pop.Y);
                p4.Move(pop.X, pop.Y);
                p3.Move(p.Points[2].X, pop.Y);
                p1.Move(pop.X, p.Points[0].Y);
                Df(pop);
                if (pop == po)
                {
                    UpdateScale();
                }
                return true;
            });
            UpdateScale();
        }

        private void UpdateScale()
        {
            scale = Math.Abs((p.Points[1].Y - p.Points[2].Y) / (p.Points[0].X - p.Points[1].X));
            if(scale == double.NaN)
            {
                scale = 1;
            }
        }

        private Point Scaling(Point m, Point p1, Point p2, int type)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                double x, y;
                switch (type)
                {
                    case 0:
                        if (((p1.X - p2.X) * (m.Y - p2.Y) - (p1.Y - p2.Y) * (m.X - p2.X)) > 0)
                        {
                            x = m.X;
                            y = p2.Y - ((p2.X - x) * scale);
                        }
                        else
                        {
                            y = m.Y;
                            x = p2.X - ((p2.Y - y) / scale);
                        }
                        return new Point(x, y);
                    case 1:
                        if (((p1.X - p2.X) * (m.Y - p2.Y) - (p1.Y - p2.Y) * (m.X - p2.X)) < 0)
                        {
                            x = m.X;
                            y = p2.Y + ((p2.X - x) * scale);
                        }
                        else
                        {
                            y = m.Y;
                            x = p2.X + ((p2.Y - y) / scale);
                        }
                        return new Point(x, y);
                }

            }
            return m;
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
