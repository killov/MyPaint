﻿using System;
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
    class MyPolygon : MyShape
    {
        Control control;
        Polygon p;
        MyBrush primaryColor, secondaryColor;
        List<Point> points = new List<Point>();
        List<Line> lines = new List<Line>();
        Line l;
        double thickness;
        bool hit = false;
        bool start = false;
        public MyPolygon(Control c)
        {
            control = c;
            p = new Polygon();
            p.Stroke = control.color.brush;
            p.Fill = control.fcolor.brush;
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
            control.draw = true;
        }

        public void mouseMove(MouseEventArgs e)
        {
            if (start)
            {
                double x = e.GetPosition(control.w.canvas).X;
                double y = e.GetPosition(control.w.canvas).Y;

                l.X2 = x;
                l.Y2 = y;               
            }
        }

        public void mouseUp(MouseButtonEventArgs e)
        {
            
            start = true;
            l = new Line();
            
            l.Stroke = control.color.brush;
            l.StrokeThickness = thickness;
            l.ToolTip = null;
            l.Cursor = Cursors.Pen;
            l.X1 = e.GetPosition(control.w.canvas).X;
            l.X2 = e.GetPosition(control.w.canvas).X;
            l.Y1 = e.GetPosition(control.w.canvas).Y;
            l.Y2 = e.GetPosition(control.w.canvas).Y;
            control.w.canvas.Children.Add(l);
            points.Add(e.GetPosition(control.w.canvas));
            lines.Add(l);
            if (e.ChangedButton == MouseButton.Right)
            {
                if (start)
                {
                    PointCollection ppoints = new PointCollection();
                    foreach(var p in points)
                    {
                        ppoints.Add(p);
                    }
                    foreach(var l in lines)
                    {
                        control.w.canvas.Children.Remove(l);
                    }
                    p = new Polygon();
                    p.Stroke = control.color.brush;
                    p.Fill = control.fcolor.brush;
                    p.StrokeThickness = thickness;
                    p.Points = ppoints;
                    control.w.canvas.Children.Add(p);
                    p.ToolTip = null;
                    p.Cursor = Cursors.SizeAll;
                    control.draw = false;
                    createPoints();
                    p.MouseDown += delegate (object sender, MouseButtonEventArgs ee)
                    {
                        hit = true;
                        control.startMoveShape(p.Points[0], ee.GetPosition(control.w.canvas));
                    };
                }
            }
        }

        List<MovePoint> movepoints = new List<MovePoint>();
        void createPoints()
        {
            control.candraw = false;
            for (int i = 0; i < points.Count; i++)
            {
                cp(i);
            }
        }

        void cp(int i)
        {
            MovePoint mp = new MovePoint(control, this, p.Points[i], (Point po) =>
            {
                if (i < points.Count) p.Points[i] = po;
            });
            movepoints.Add(mp);
        }

        public void moveDrag(MouseEventArgs e)
        {
            foreach(var p in movepoints)
            {
                p.move(e);
            }
        }

        public void stopDrag()
        {
            hit = false;
            foreach (var p in movepoints)
            {
                p.drag = false;
            }
        }

        public void stopDraw()
        {
            foreach (var p in movepoints)
            {
                p.delete();
            }
        }

        public void moveShape(double x, double y)
        {  
            for(int i = 1; i < p.Points.Count; i++)
            {
                p.Points[i] = new Point(p.Points[i].X - p.Points[0].X + x, p.Points[i].Y - p.Points[0].Y + y);
                movepoints[i].move(p.Points[i].X - p.Points[0].X + x, p.Points[i].Y - p.Points[0].Y + y);
            }
            p.Points[0] = new Point(x, y);
            movepoints[0].move(x,y);
        }

        public string renderShape()
        {
            StringBuilder stack = new StringBuilder();
            Point fp = p.Points.First();
            stack.Append(String.Format("ctx.moveTo({0},{1});\n", fp.X, fp.Y));
            
            foreach (var p in p.Points)
            {
                if(fp != p)
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
            if (p.Points.Count > 0)
            {
                stopDraw();
            }
        }
    }
}
