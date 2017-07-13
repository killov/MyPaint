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
    class MyEllipse : MyShape
    {
        Control control;
        Ellipse p;
        MyBrush primaryColor, secondaryColor;
        bool hit = false;
        double thickness;
        double sx, sy, ex, ey;
        public MyEllipse(Control c)
        {
            control = c;
            p = new Ellipse();
        }

        public void setPrimaryColor(MyBrush s)
        {
            primaryColor = s;
            p.Stroke = s.brush;
        }

        public void setSecondaryColor(MyBrush s)
        {
            secondaryColor = s;
            p.Fill = s.brush;
        }

        public void setThickness(double s)
        {
            p.StrokeThickness = s;
            thickness = s;
        }

        void moveS(double x, double y)
        {
            if (x > ex)
            {
                p.Width = x - ex;
            }
            else
            {
                Canvas.SetLeft(p, x);
                p.Width = ex - x;
            }
            if (y > ey)
            {
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

        void moveE(double x, double y)
        {
            if (x > sx)
            {
                p.Width = x - sx;
            }
            else
            {
                Canvas.SetLeft(p, x);
                p.Width = sx - x;
            }
            if (y > sy)
            {
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

        public void mouseDown(MouseButtonEventArgs e)
        {
            sx = e.GetPosition(control.w.canvas).X;
            sy = e.GetPosition(control.w.canvas).Y;

            p.ToolTip = null;
            p.Cursor = Cursors.Pen;
           
            control.w.canvas.Children.Add(p);
            Canvas.SetLeft(p, sx);
            Canvas.SetTop(p, sy);
            control.draw = true;
        }

        public void mouseMove(MouseEventArgs e)
        {
            double x = e.GetPosition(control.w.canvas).X;
            double y = e.GetPosition(control.w.canvas).Y;
            moveE(x, y);
        }

        public void mouseUp(MouseButtonEventArgs e)
        {
            p.Cursor = Cursors.SizeAll;
            control.draw = false;
            createPoints();
            p.MouseDown += delegate (object sender, MouseButtonEventArgs ee)
            {
                hit = true;
                control.startMoveShape(new Point(Canvas.GetLeft(p),Canvas.GetTop(p)), ee.GetPosition(control.w.canvas));
            };  
        }

        MovePoint p1, p2, p3, p4;
        void createPoints()
        {
            control.candraw = false;
            p1 = new MovePoint(control, this, new Point(sx, sy), (po) =>
            {
                moveS(po.X, po.Y);
                p1.move(po.X, po.Y);
                p3.move(ex, sy);
                p4.move(sx, ey);
            });

            p2 = new MovePoint(control, this, new Point(ex, ey), (po) =>
            {
                moveE(po.X, po.Y);
                p2.move(po.X, po.Y);
                p3.move(ex, sy);
                p4.move(sx, ey);
            });

            p3 = new MovePoint(control, this, new Point(ex, sy), (po) =>
            {
                moveE(po.X, ey);
                moveS(sx, po.Y);
                p3.move(po.X, po.Y);
                p1.move(sx, sy);
                p2.move(ex, ey);
            });

            p4 = new MovePoint(control, this, new Point(sx, ey), (po) =>
            {
                moveE(ex, po.Y);
                moveS(po.X, sy);
                p4.move(po.X, po.Y);
                p1.move(sx, sy);
                p2.move(ex, ey);
            });
        }

        public void moveDrag(MouseEventArgs e)
        {
            p1.move(e);
            p2.move(e);
            p3.move(e);
            p4.move(e);
        }

        public void stopDrag()
        {
            hit = false;
            p1.drag = false;
            p2.drag = false;
            p3.drag = false;
            p4.drag = false;
        }

        public void stopDraw()
        {
            p1.delete();
            p2.delete();
            p3.delete();
            p4.delete();
        }

        public void moveShape(double x, double y)
        {
            hit = true;
            sx += x-Canvas.GetLeft(p);
            ex += x-Canvas.GetLeft(p);
            sy += y-Canvas.GetTop(p);
            ey += y-Canvas.GetTop(p);
            Canvas.SetLeft(p, x);
            Canvas.SetTop(p, y);

            p1.move(sx, sy);
            p2.move(ex, ey);
            p3.move(ex, sy);
            p4.move(sx, ey);
        }

        public string renderShape()
        {
            StringBuilder stack = new StringBuilder();
            stack.Append("ctx.beginPath();\n");
            stack.Append(String.Format("ctx.ellipse({0},{1},{2},{3},0,0,2*Math.PI);\n", (int)(sx + ex)/2, (int)(sy + ey)/2, (int)Math.Abs(sx - ex)/2, (int)Math.Abs(sy - ey)/2));  
            stack.Append("ctx.stroke();\n");
            return stack.ToString();
        }

        public void setHit(bool h)
        {
            hit = h;
        }

        public bool hitTest()
        {
            return hit;
        }

        public void delete()
        {
            control.w.canvas.Children.Remove(p);
            if(p1 != null)
            {
                stopDraw();
            }
        }
    }
}
