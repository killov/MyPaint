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
    class MyLine : MyShape
    {
        Control control;
        Line l;

        public MyLine(Control c)
        {
            control = c;
            l = new Line();
            
        }
        public void mouseDown(MouseButtonEventArgs e)
        {
            l.Stroke = control.color;
            l.X1 = e.GetPosition(control.w.canvas).X;
            l.Y1 = e.GetPosition(control.w.canvas).Y;
            l.X2 = e.GetPosition(control.w.canvas).X;
            l.Y2 = e.GetPosition(control.w.canvas).Y;
            l.StrokeThickness = control.StrokeThickness;
            l.ToolTip = null;
            l.Cursor = Cursors.Pen;
            control.w.canvas.Children.Add(l);
            control.draw = true;
        }

        public void mouseMove(MouseEventArgs e)
        {
            l.X2 = e.GetPosition(control.w.canvas).X;
            l.Y2 = e.GetPosition(control.w.canvas).Y;
        }

        public void mouseUp(MouseButtonEventArgs e)
        {
            control.draw = false;
            l.Cursor = Cursors.SizeAll;
            createPoints();
            l.MouseDown += delegate (object sender, MouseButtonEventArgs ee)
            {
                control.startMoveShape(new Point(l.X1, l.Y1), ee.GetPosition(control.w.canvas));
            };
        }

        MovePoint p1, p2;
        void createPoints()
        {
            control.candraw = false;
            p1 = new MovePoint(control.w.canvas, new Point(l.X1, l.Y1), (p) =>
            {
                l.X1 = p.X;
                l.Y1 = p.Y;
            });

            p2 = new MovePoint(control.w.canvas, new Point(l.X2, l.Y2), (p) =>
            {
                l.X2 = p.X;
                l.Y2 = p.Y;
            });
        }

        public void moveDrag(MouseEventArgs e)
        {
            p1.move(e);
            p2.move(e);
        }

        public void stopDrag()
        {
            p1.drag = false;
            p2.drag = false;
        }

        public void stopDraw()
        {
            p1.delete();
            p2.delete();
        }

        public void moveShape(double x, double y)
        {           
            l.X2 = l.X2 - l.X1 + x;
            l.X1 = x;
            l.Y2 = l.Y2 - l.Y1 + y;
            l.Y1 = y;
            p1.move(l.X1, l.Y1);
            p2.move(l.X2, l.Y2);
        }

        public string renderShape()
        {
            return String.Format("ctx.moveTo({0},{1});\n", l.X1, l.Y1) +
                    String.Format("ctx.moveTo({0},{1});\n", l.X2, l.Y2);
        }
    }
}
