using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Controls;

namespace MyPaint
{
    delegate void posun(Point b);
    delegate void posuni(Point b, int i);
    class MovePoint
    {
        Ellipse el;
        Canvas ca;
        Canvas canvas;
        IMyShape shape;
        double x, y;
        public bool drag = false;
 
        posun posun;
        public MovePoint(Canvas c, IMyShape s, Point p, ScaleTransform revScale, posun pos)
        {
            ca = new Canvas();
            TransformGroup g = new TransformGroup();
            g.Children.Add(revScale);
            ca.LayoutTransform = g;
            posun = pos;
            x = p.X;
            y = p.Y;
            el = new Ellipse();
            el.Fill = Brushes.White;
            el.StrokeThickness = 1;
            el.Stroke = Brushes.Black;
            el.Width = 10;
            el.Height = 10;
            ca.Children.Add(el);
            shape = s;
            canvas = c;
            canvas.Children.Add(ca);
            Canvas.SetTop(el,-5);
            Canvas.SetLeft(el,-5);
            Canvas.SetTop(ca, y);
            Canvas.SetLeft(ca, x);

            el.MouseDown += delegate (object sender, MouseButtonEventArgs e) 
            {
                shape.setHit(true);
                drag = true;
            };
        }

        public void move(MouseEventArgs e)
        {
            if (drag)
            {
                x = e.GetPosition(canvas).X;
                y = e.GetPosition(canvas).Y;
                Canvas.SetTop(ca, y);
                Canvas.SetLeft(ca, x);

                posun(new Point(x, y));
            }
        }

        public void move(double x, double y)
        {
            Canvas.SetTop(ca, y);
            Canvas.SetLeft(ca, x);
        }

        public void delete()
        {
            canvas.Children.Remove(ca);
        }
    }
}
