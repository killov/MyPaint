using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MyPaint
{
    public class MovePoint
    {
        Ellipse el;
        Canvas ca;
        Canvas canvas;
        Shapes.Shape shape;
        Point position;
        bool drag = false;
        Point startPosition;
        MoveDelegate posun;
        Canvas element;
        public MovePoint(Canvas c, Shapes.Shape s, Point p, ScaleTransform revScale, MoveDelegate pos)
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

                shape.SetHit(true);
                StartDrag();
            };
        }

        public void MoveDrag(Point e)
        {
            if (drag)
            {
                Move(e, true);
            }
        }

        public void Move(Point e, bool mouseDrag = false)
        {
            position = e;
            Canvas.SetTop(ca, position.Y);
            Canvas.SetLeft(ca, position.X);

            posun(position, mouseDrag);
        }

        public void Hide()
        {
            canvas.Children.Remove(ca);
        }

        public void Show()
        {
            canvas.Children.Add(ca);
            Canvas.SetTop(el, -5);
            Canvas.SetLeft(el, -5);
            Canvas.SetTop(ca, position.Y);
            Canvas.SetLeft(ca, position.X);
        }

        public void StartDrag()
        {
            drag = true;
        }

        public Point GetPosition()
        {
            return position;
        }

        public void StopDrag()
        {
            if (drag && !startPosition.Equals(position))
            {
                shape.DrawControl.HistoryControl.Add(new History.HistoryMovePoint(this, startPosition, position));
            }
            drag = false;
        }
    }
}
