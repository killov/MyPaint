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
        Canvas canvas;
        double x, y;
        public bool drag = false;
        int id;
        posun posun;
        public MovePoint(Canvas c, Point p, posun pos, int index = -1)
        {
            id = index;
            posun = pos;
            x = p.X;
            y = p.Y;
            el = new Ellipse();
            el.Fill = Brushes.White;
            el.StrokeThickness = 1;
            el.Stroke = Brushes.Black;
            el.Width = 10;
            el.Height = 10;
            canvas = c;
            c.Children.Add(el);
            Canvas.SetTop(el, y-5);
            Canvas.SetLeft(el, x-5);

            el.MouseDown += delegate (object sender, MouseButtonEventArgs e)
            {
                drag = true;
            };
        }

        public void move(MouseEventArgs e)
        {
            if (drag)
            {
                x = e.GetPosition(canvas).X;
                y = e.GetPosition(canvas).Y;
                Canvas.SetTop(el, y-5);
                Canvas.SetLeft(el, x-5);

                posun(new Point(x, y));
            }
        }

        public void move(double x, double y)
        {
            Canvas.SetTop(el, y - 5);
            Canvas.SetLeft(el, x - 5);
        }

        public void delete()
        {
            canvas.Children.Remove(el);
        }
    }
}
