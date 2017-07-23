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
    class MyPolygon : MyShape
    {
        DrawControl drawControl;
        Polygon p = new Polygon(), lv;
        Brush primaryColor, secondaryColor;
        List<Point> points = new List<Point>();
        List<Line> lines = new List<Line>();
        Line l;
        double thickness;
        bool hit = false;
        bool start = false;
        MyLayer layer;

        public MyPolygon(DrawControl c, MyLayer la)
        {
            drawControl = c;
            layer = la;
            layer.shapes.Add(this);
        }

        public MyPolygon(DrawControl c, MyLayer la, jsonDeserialize.Shape s)
        {
            drawControl = c;
            layer = la;
            setPrimaryColor(s.stroke == null ? null : s.stroke.createBrush());
            setSecondaryColor(s.fill == null ? null : s.fill.createBrush());
            setThickness(s.lineWidth);
            
            foreach(var point in s.points)
            {
                p.Points.Add(new Point(point.x, point.y));
            }

            layer.canvas.Children.Add(p);

            p.ToolTip = null;

            p.Cursor = Cursors.SizeAll;
            layer.shapes.Add(this);
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
            if (lv != null) lv.StrokeThickness = s;
            thickness = s;
        }

        public void mouseDown(MouseButtonEventArgs e)
        {
            drawControl.draw = true;
        }

        public void mouseMove(MouseEventArgs e)
        {
            if (start)
            {
                double x = e.GetPosition(layer.canvas).X;
                double y = e.GetPosition(layer.canvas).Y;

                l.X2 = x;
                l.Y2 = y;               
            }
        }

        public void mouseUp(MouseButtonEventArgs e)
        {
            
            start = true;
            l = new Line();

            l.Stroke = primaryColor;
            l.StrokeThickness = thickness;
            l.ToolTip = null;
            l.Cursor = Cursors.Pen;
            l.X1 = e.GetPosition(layer.canvas).X;
            l.X2 = e.GetPosition(layer.canvas).X;
            l.Y1 = e.GetPosition(layer.canvas).Y;
            l.Y2 = e.GetPosition(layer.canvas).Y;
            layer.canvas.Children.Add(l);
            points.Add(e.GetPosition(layer.canvas));
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
                        layer.canvas.Children.Remove(l);
                    }
                    p = new Polygon();
                    p.Stroke = primaryColor;
                    p.Fill = secondaryColor;
                    p.StrokeThickness = thickness;
                    p.Points = ppoints;
                    layer.canvas.Children.Add(p);
                    p.ToolTip = null;
                    p.Cursor = Cursors.SizeAll;
                    drawControl.draw = false;
                    createPoints();
                    drawControl.lockDraw();
                }
            }
        }

        public void createVirtualShape(MyOnMouseDown mouseDown)
        {
            lv = new Polygon();
            lv.Points = p.Points;
            lv.Stroke = drawControl.nullBrush;
            lv.Fill = drawControl.nullBrush;
            lv.StrokeThickness = thickness;
            lv.Cursor = Cursors.SizeAll;
            lv.MouseDown += delegate (object sender, MouseButtonEventArgs ee)
            {
                mouseDown(ee, this);
                hit = true;
            };
            drawControl.topCanvas.Children.Add(lv);
        }

        public void deleteVirtualShape()
        {
            drawControl.topCanvas.Children.Remove(lv);
            lv = null;
        }

        List<MovePoint> movepoints;
        void createPoints()
        {
            createVirtualShape((e, s) =>
            {
                drawControl.startMoveShape(p.Points[0], e.GetPosition(layer.canvas));
            });

            movepoints = new List<MovePoint>();
            drawControl.candraw = false;
            for (int i = 0; i < p.Points.Count; i++)
            {
                cp(i);
            }
        }

        void cp(int i)
        {
            MovePoint mp = new MovePoint(drawControl.topCanvas, this, p.Points[i], (Point po) =>
            {
                if (i < p.Points.Count) p.Points[i] = po;
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
            deleteVirtualShape();
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

        public jsonSerialize.Shape renderShape()
        {
            jsonSerialize.Polygon ret = new jsonSerialize.Polygon();
            ret.lineWidth = thickness;
            ret.stroke = Utils.BrushToCanvas(primaryColor);
            ret.fill = Utils.BrushToCanvas(secondaryColor);
            ret.points = new List<jsonSerialize.Point>();
            foreach(var point in p.Points)
            {
                ret.points.Add(new jsonSerialize.Point(point.X, point.Y));
            }
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
            layer.shapes.Remove(this);
            layer.canvas.Children.Remove(p);
            if (p.Points.Count > 0)
            {
                stopDraw();
            }
            else
            {
                deleteVirtualShape();
            }
        }

        public void refresh()
        {
            layer.shapes.Add(this);
            layer.canvas.Children.Add(p);
            drawControl.lockDraw();
        }
    }
}
