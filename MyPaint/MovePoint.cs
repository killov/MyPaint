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
    public delegate void posun(Point b);
    public class MovePoint
    {
        Ellipse el;
        Canvas ca;
        Canvas canvas;
        Shapes.MyShape shape;
        Point position;
        public bool drag = false;
        Point startPosition;
        posun posun;
        Canvas element;
        public MovePoint(Canvas c, Shapes.MyShape s, Point p, ScaleTransform revScale, posun pos)
        {
            ca = new Canvas();
            TransformGroup g = new TransformGroup();
            g.Children.Add(revScale);
            ca.LayoutTransform = g;
            posun = pos;
            position = p;
            el = new Ellipse();
            el.Fill = Brushes.White;
            el.StrokeThickness = 1;
            el.Stroke = Brushes.Black;
            el.Width = 10;
            el.Height = 10;
            ca.Children.Add(el);
            shape = s;
            canvas = c;
            element = ca;
            el.MouseDown += delegate (object sender, MouseButtonEventArgs e) 
            {
                startPosition = position;
                shape.setHit(true);
                startDrag();
            };
        }

        public void move(Point e)
        {
            if (drag)
            {
                movee(e);
            }
        }

        public void movee(Point e)
        {
            position = e;
            Canvas.SetTop(ca, position.Y);
            Canvas.SetLeft(ca, position.X);

            posun(position);
        }

        public void move(double x, double y)
        {
            position = new Point(x, y);
            Canvas.SetTop(ca, y);
            Canvas.SetLeft(ca, x);
        }

        public void hide()
        {
            canvas.Children.Remove(ca);
        }

        public void show()
        {
            canvas.Children.Add(ca);
            Canvas.SetTop(el, -5);
            Canvas.SetLeft(el, -5);
            Canvas.SetTop(ca, position.Y);
            Canvas.SetLeft(ca, position.X);
        }

        public void startDrag()
        {
            drag = true;
        }

        public void stopDrag()
        {
            if (drag && !startPosition.Equals(position))
            {
                shape.drawControl.control.addHistory(new History.HistoryMovePoint(this, startPosition, position));
            }
            drag = false;
        }
    }
}
