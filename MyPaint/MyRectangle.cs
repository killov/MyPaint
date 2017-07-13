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
    class MyRectangle : MyShape
    {
        Control control;
        Polygon p;
        MyBrush primaryColor, secondaryColor;
        double thickness;
        bool hit = false;
        public MyRectangle(Control c)
        {
            control = c;
            p = new Polygon();
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

        public void mouseDown(MouseButtonEventArgs e)
        {
            PointCollection points = new PointCollection(4);
            double x = e.GetPosition(control.w.canvas).X;
            double y = e.GetPosition(control.w.canvas).Y;
            points.Add(new Point(x, y));
            points.Add(new Point(x, y));
            points.Add(new Point(x, y));
            points.Add(new Point(x, y));
            p.Stroke = control.color.brush;
            p.Fill = control.fcolor.brush;
            p.Points = points;
            control.w.canvas.Children.Add(p);
            p.ToolTip = null;
            p.Cursor = Cursors.Pen;
            control.draw = true;
        }

        public void mouseMove(MouseEventArgs e)
        {
            double x = e.GetPosition(control.w.canvas).X;
            double y = e.GetPosition(control.w.canvas).Y;

            p.Points[1] = new Point(p.Points[1].X, y);
            p.Points[2] = new Point(x, y);
            p.Points[3] = new Point(x, p.Points[3].Y);
        }

        public void mouseUp(MouseButtonEventArgs e)
        {
            p.Cursor = Cursors.SizeAll;
            control.draw = false;
            createPoints();
            p.MouseDown += delegate (object sender, MouseButtonEventArgs ee)
            {
                hit = true;
                control.startMoveShape(p.Points[0], ee.GetPosition(control.w.canvas));
            };
            
        }

        MovePoint p1, p2, p3, p4;
        void createPoints()
        {
            control.candraw = false;
            p1 = new MovePoint(control, this, p.Points[0], (po) =>
            {
                p.Points[0] = po;
                p.Points[1] = new Point(po.X, p.Points[1].Y);
                p.Points[3] = new Point(p.Points[3].X, po.Y);
                p1.move(po.X, po.Y);
                p2.move(po.X, p.Points[1].Y);
                p4.move(p.Points[3].X, po.Y);
            });

            p2 = new MovePoint(control, this, p.Points[1], (po) =>
            {
                p.Points[1] = po;
                p.Points[0] = new Point(po.X, p.Points[0].Y);
                p.Points[2] = new Point(p.Points[2].X, po.Y);
                p2.move(po.X, po.Y);
                p1.move(po.X, p.Points[0].Y);
                p3.move(p.Points[2].X, po.Y);
            });

            p3 = new MovePoint(control, this, p.Points[2], (po) =>
            {
                p.Points[2] = po;
                p.Points[1] = new Point(p.Points[1].X, po.Y);
                p.Points[3] = new Point(po.X, p.Points[3].Y);
                p3.move(po.X, po.Y);
                p4.move(po.X, p.Points[3].Y);
                p2.move(p.Points[1].X, po.Y);
            });

            p4 = new MovePoint(control, this, p.Points[3], (po) =>
            {
                p.Points[3] = po;
                p.Points[0] = new Point(p.Points[0].X, po.Y);
                p.Points[2] = new Point(po.X, p.Points[2].Y);
                p4.move(po.X, po.Y);
                p3.move(po.X, p.Points[2].Y);
                p1.move(p.Points[0].X, po.Y);
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
            p.Points[1] = new Point(p.Points[1].X - p.Points[0].X + x, p.Points[1].Y - p.Points[0].Y + y);
            p.Points[2] = new Point(p.Points[2].X - p.Points[0].X + x, p.Points[2].Y - p.Points[0].Y + y);
            p.Points[3] = new Point(p.Points[3].X - p.Points[0].X + x, p.Points[3].Y - p.Points[0].Y + y);
            p.Points[0] = new Point(x, y);
            p1.move(x, y);
            p2.move(p.Points[1].X, p.Points[1].Y);
            p3.move(p.Points[2].X, p.Points[2].Y);
            p4.move(p.Points[3].X, p.Points[3].Y);

        }

        public string renderShape()
        {
            StringBuilder stack = new StringBuilder();
            Point fp = p.Points.First();
            stack.Append(String.Format("ctx.moveTo({0},{1});\n", fp.X, fp.Y));

            foreach (var p in p.Points)
            {
                if (fp != p)
                {
                    stack.Append(String.Format("ctx.lineTo({0},{1});\n", p.X, p.Y));
                }
            }
            stack.Append("ctx.closePath();\n");
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
            if (p1 != null)
            {
                stopDraw();
            }
        }
    }
}
