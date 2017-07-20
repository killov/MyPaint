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
    class MyRectangle : MyShape
    {
        DrawControl drawControl;
        Polygon p = new Polygon();
        Brush primaryColor, secondaryColor;
        double thickness;
        bool hit = false;
        Canvas canvas;

        public MyRectangle(DrawControl c, Canvas ca)
        {
            drawControl = c;
            canvas = ca;
        }

        public MyRectangle(DrawControl c, Canvas ca, jsonDeserialize.Shape s)
        {
            drawControl = c;
            canvas = ca;
            setPrimaryColor(s.stroke == null ? null : s.stroke.createBrush());
            setSecondaryColor(s.fill == null ? null : s.fill.createBrush());
            setThickness(s.lineWidth);


            p.Points.Add(new Point(s.A.x, s.A.y));
            p.Points.Add(new Point(s.B.x, s.A.y));
            p.Points.Add(new Point(s.B.x, s.B.y));
            p.Points.Add(new Point(s.A.x, s.B.y));

            canvas.Children.Add(p);

            p.ToolTip = null;
            p.Cursor = Cursors.SizeAll;
            p.MouseDown += delegate (object sender, MouseButtonEventArgs ee)
            {
                hit = true;
                drawControl.startMoveShape(new Point(Canvas.GetLeft(p), Canvas.GetTop(p)), ee.GetPosition(canvas));
            };
        }

        public void setPrimaryColor(Brush s)
        {
            primaryColor = s;
            p.Stroke = s;
        }

        public void setSecondaryColor(Brush s)
        {
            secondaryColor = s;
            p.Fill = s;
        }

        public void setThickness(double s)
        {
            p.StrokeThickness = s;
            thickness = s;
        }

        public void mouseDown(MouseButtonEventArgs e)
        {
            PointCollection points = new PointCollection(4);
            double x = e.GetPosition(canvas).X;
            double y = e.GetPosition(canvas).Y;
            points.Add(new Point(x, y));
            points.Add(new Point(x, y));
            points.Add(new Point(x, y));
            points.Add(new Point(x, y));
            p.Stroke = primaryColor;
            p.Fill = secondaryColor;
            p.Points = points;
            canvas.Children.Add(p);
            p.ToolTip = null;
            p.Cursor = Cursors.Pen;
            drawControl.draw = true;
        }

        public void mouseMove(MouseEventArgs e)
        {
            double x = e.GetPosition(canvas).X;
            double y = e.GetPosition(canvas).Y;

            p.Points[1] = new Point(p.Points[1].X, y);
            p.Points[2] = new Point(x, y);
            p.Points[3] = new Point(x, p.Points[3].Y);
        }

        public void mouseUp(MouseButtonEventArgs e)
        {
            p.Cursor = Cursors.SizeAll;
            drawControl.draw = false;
            createPoints();
            p.MouseDown += delegate (object sender, MouseButtonEventArgs ee)
            {
                hit = true;
                drawControl.startMoveShape(p.Points[0], ee.GetPosition(canvas));
            };
            
        }

        MovePoint p1, p2, p3, p4;
        void createPoints()
        {
            drawControl.candraw = false;
            p1 = new MovePoint(canvas, this, p.Points[0], (po) =>
            {
                p.Points[0] = po;
                p.Points[1] = new Point(po.X, p.Points[1].Y);
                p.Points[3] = new Point(p.Points[3].X, po.Y);
                p1.move(po.X, po.Y);
                p2.move(po.X, p.Points[1].Y);
                p4.move(p.Points[3].X, po.Y);
            });

            p2 = new MovePoint(canvas, this, p.Points[1], (po) =>
            {
                p.Points[1] = po;
                p.Points[0] = new Point(po.X, p.Points[0].Y);
                p.Points[2] = new Point(p.Points[2].X, po.Y);
                p2.move(po.X, po.Y);
                p1.move(po.X, p.Points[0].Y);
                p3.move(p.Points[2].X, po.Y);
            });

            p3 = new MovePoint(canvas, this, p.Points[2], (po) =>
            {
                p.Points[2] = po;
                p.Points[1] = new Point(p.Points[1].X, po.Y);
                p.Points[3] = new Point(po.X, p.Points[3].Y);
                p3.move(po.X, po.Y);
                p4.move(po.X, p.Points[3].Y);
                p2.move(p.Points[1].X, po.Y);
            });

            p4 = new MovePoint(canvas, this, p.Points[3], (po) =>
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

        public jsonSerialize.Shape renderShape()
        {
            jsonSerialize.Rectangle ret = new jsonSerialize.Rectangle();
            ret.lineWidth = thickness;
            ret.stroke = Utils.BrushToCanvas(primaryColor);
            ret.fill = Utils.BrushToCanvas(secondaryColor);
            ret.A = new jsonSerialize.Point(p.Points[0].X, p.Points[0].Y);
            ret.B = new jsonSerialize.Point(p.Points[2].X, p.Points[2].Y);
            return ret;
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
            canvas.Children.Remove(p);
            if (p1 != null)
            {
                stopDraw();
            }
        }

        public void refresh()
        {
            drawControl.shapes.Add(this);
            canvas.Children.Add(p);
            drawControl.lockDraw();
        }
    }
}