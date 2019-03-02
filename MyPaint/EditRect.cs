using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyPaint
{
    public class EditRect
    {
        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
        System.Windows.Shapes.Polygon pv = new System.Windows.Shapes.Polygon();
        public MovePoint p1, p2, p3, p4;
        Canvas canvas;
        double scale;
        bool reversePosition = false;
        ScaleTransform revScale;
        Brush fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 255));
        public EditRect(Canvas c, Shapes.Shape s, Point A, Point B, ScaleTransform revScale, MoveDelegate Af, MoveDelegate Bf, MoveDelegate Cf, MoveDelegate Df)
        {
            this.revScale = revScale;
            p.Points.Add(new Point(A.X, A.Y));
            p.Points.Add(new Point(B.X, A.Y));
            p.Points.Add(new Point(B.X, B.Y));
            p.Points.Add(new Point(A.X, B.Y));
            pv.Points = p.Points;
            p.Stroke = fill;
            p.Fill = fill;
            p.StrokeThickness = revScale.ScaleX * 3;
            p.ToolTip = null;
            p.Cursor = Cursors.SizeAll;
            p.MouseDown += (o, e) =>
            {
                s.SetHit(true);
                s.DrawControl.StartMoveShape(s.GetPosition(), e.GetPosition(canvas));
            };
            DoubleCollection dash = new DoubleCollection
            {
                4,
                6
            };
            pv.StrokeDashArray = dash;
            pv.StrokeThickness = revScale.ScaleX;
            pv.ToolTip = null;
            pv.Cursor = Cursors.SizeAll;
            pv.Stroke = Brushes.Blue;
            canvas = c;
            p1 = new MovePoint(c, s, p.Points[0], revScale, (po, mouseDrag) =>
            {
                Point pop = mouseDrag ? Scaling(po, p.Points[0], p.Points[2], reversePosition ? 1 : 0) : po;
                p.Points[0] = pop;
                Af(pop, mouseDrag);
                if (mouseDrag)
                {
                    p2.Move(new Point(p.Points[1].X, pop.Y));
                    p4.Move(new Point(pop.X, p.Points[3].Y));

                    if (pop == po)
                    {
                        UpdateScale();
                    }
                }
            });

            p2 = new MovePoint(c, s, p.Points[1], revScale, (po, mouseDrag) =>
            {
                Point pop = mouseDrag ? Scaling(po, p.Points[1], p.Points[3], reversePosition ? 0 : 1) : po;
                p.Points[1] = pop;
                Bf(pop, mouseDrag);
                if (mouseDrag)
                {
                    p1.Move(new Point(p.Points[0].X, pop.Y));
                    p3.Move(new Point(pop.X, p.Points[2].Y));

                    if (pop == po)
                    {
                        UpdateScale();
                    }
                }
            });

            p3 = new MovePoint(c, s, p.Points[2], revScale, (po, mouseDrag) =>
            {
                Point pop = mouseDrag ? Scaling(po, p.Points[2], p.Points[0], reversePosition ? 1 : 0) : po;

                p.Points[2] = pop;
                Cf(pop, mouseDrag);
                if (mouseDrag)
                {
                    p4.Move(new Point(p.Points[3].X, pop.Y));
                    p2.Move(new Point(pop.X, p.Points[1].Y));
                    if (pop == po)
                    {
                        UpdateScale();
                    }
                }
            });

            p4 = new MovePoint(c, s, p.Points[3], revScale, (po, mouseDrag) =>
            {
                Point pop = mouseDrag ? Scaling(po, p.Points[3], p.Points[1], reversePosition ? 0 : 1) : po;
                p.Points[3] = pop;
                Df(pop, mouseDrag);
                if (mouseDrag)
                {
                    p4.Move(new Point(pop.X, pop.Y));
                    p3.Move(new Point(p.Points[2].X, pop.Y));
                    p1.Move(new Point(pop.X, p.Points[0].Y));
                    if (pop == po)
                    {
                        UpdateScale();
                    }
                }
            });
            UpdateScale();
        }

        private void UpdateScale()
        {
            scale = Math.Abs((p.Points[1].Y - p.Points[2].Y) / (p.Points[0].X - p.Points[1].X));
            if (scale == double.NaN)
            {
                scale = 1;
            }
            if (!(p.Points[0].X == p.Points[2].X && p.Points[0].Y == p.Points[2].Y))
            {
                reversePosition = !((p.Points[0].X < p.Points[2].X && p.Points[0].Y < p.Points[2].Y) || (p.Points[0].X > p.Points[2].X && p.Points[0].Y > p.Points[2].Y));
            }
        }

        private Point Scaling(Point m, Point p1, Point p2, int type)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                double x, y;
                Vector v1 = p1 - p2, v2 = m - p2;
                double hm = v1.X * v2.Y + v2.X * v2.Y;

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

        public void Move(Point point)
        {
            p2.Move(point + (p2.Position - p1.Position));
            p3.Move(point + (p3.Position - p1.Position));
            p4.Move(point + (p4.Position - p1.Position));
            p1.Move(point);
        }

        public void SetActive()
        {
            canvas.Children.Add(pv);
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
            canvas.Children.Remove(pv);
        }

        public void ChangeZoom()
        {
            p.StrokeThickness = revScale.ScaleX * 3;
            pv.StrokeThickness = revScale.ScaleX;
        }

        public double GetWidth()
        {
            return Math.Abs(p1.Position.X - p3.Position.X);
        }

        public double GetHeight()
        {
            return Math.Abs(p1.Position.Y - p3.Position.Y);
        }

        public Point Position => p1.Position;

        public void SetFill(bool f)
        {
            p.Fill = f ? fill : null;
        }

    }
}
