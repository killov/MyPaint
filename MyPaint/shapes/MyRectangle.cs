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
        DrawControl drawControl;
        Polygon p = new Polygon(), lv;
        Brush primaryColor, secondaryColor;
        double thickness;
        bool hit = false;
        MyLayer layer;

        public MyRectangle(DrawControl c, MyLayer la)
        {
            drawControl = c;
            layer = la;
            layer.shapes.Add(this);
        }

        public MyRectangle(DrawControl c, MyLayer la, jsonDeserialize.Shape s)
        {
            drawControl = c;
            layer = la;
            setPrimaryColor(s.stroke == null ? null : s.stroke.createBrush());
            setSecondaryColor(s.fill == null ? null : s.fill.createBrush());
            setThickness(s.lineWidth);


            p.Points.Add(new Point(s.A.x, s.A.y));
            p.Points.Add(new Point(s.B.x, s.A.y));
            p.Points.Add(new Point(s.B.x, s.B.y));
            p.Points.Add(new Point(s.A.x, s.B.y));

            layer.canvas.Children.Add(p);

            p.ToolTip = null;
            p.Cursor = Cursors.SizeAll;
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

        public void changeLayer(MyLayer newLayer)
        {
            if (layer != null)
            {
                layer.canvas.Children.Remove(p);
                layer.shapes.Remove(this);
            }
            layer = newLayer;
            if (layer != null)
            {
                layer.canvas.Children.Add(p);
                layer.shapes.Add(this);
            }
        }

        public void setThickness(double s)
        {
            p.StrokeThickness = s;
            if (lv != null) lv.StrokeThickness = s;
            thickness = s;
        }

        public void mouseDown(MouseButtonEventArgs e)
        {
            PointCollection points = new PointCollection(4);
            double x = e.GetPosition(layer.canvas).X;
            double y = e.GetPosition(layer.canvas).Y;
            points.Add(new Point(x, y));
            points.Add(new Point(x, y));
            points.Add(new Point(x, y));
            points.Add(new Point(x, y));
            p.Stroke = primaryColor;
            p.Fill = secondaryColor;
            p.Points = points;
            layer.canvas.Children.Add(p);
            p.ToolTip = null;
            p.Cursor = Cursors.Pen;
            drawControl.draw = true;
        }

        public void mouseMove(MouseEventArgs e)
        {
            double x = e.GetPosition(layer.canvas).X;
            double y = e.GetPosition(layer.canvas).Y;

            p.Points[1] = new Point(p.Points[1].X, y);
            p.Points[2] = new Point(x, y);
            p.Points[3] = new Point(x, p.Points[3].Y);
        }

        public void mouseUp(MouseButtonEventArgs e)
        {
            p.Cursor = Cursors.SizeAll;
            drawControl.draw = false;
            setActive();
            drawControl.lockDraw();           
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

        public void startMove(MouseButtonEventArgs e)
        {
            drawControl.startMoveShape(p.Points[0], e.GetPosition(layer.canvas));
        }

        MovePoint p1, p2, p3, p4;
        public void setActive()
        {
            drawControl.control.setPrimaryColor(primaryColor);
            drawControl.control.setSecondaryColor(secondaryColor);
            drawControl.control.setThickness(thickness);
            createVirtualShape((e, s) =>
            {
                drawControl.startMoveShape(p.Points[0], e.GetPosition(layer.canvas));
            });
            drawControl.candraw = false;
            p1 = new MovePoint(drawControl.topCanvas, this, p.Points[0], drawControl.revScale, (po) =>
            {
                p.Points[0] = po;
                p.Points[1] = new Point(po.X, p.Points[1].Y);
                p.Points[3] = new Point(p.Points[3].X, po.Y);
                p1.move(po.X, po.Y);
                p2.move(po.X, p.Points[1].Y);
                p4.move(p.Points[3].X, po.Y);
            });

            p2 = new MovePoint(drawControl.topCanvas, this, p.Points[1], drawControl.revScale, (po) =>
            {
                p.Points[1] = po;
                p.Points[0] = new Point(po.X, p.Points[0].Y);
                p.Points[2] = new Point(p.Points[2].X, po.Y);
                p2.move(po.X, po.Y);
                p1.move(po.X, p.Points[0].Y);
                p3.move(p.Points[2].X, po.Y);
            });

            p3 = new MovePoint(drawControl.topCanvas, this, p.Points[2], drawControl.revScale, (po) =>
            {
                p.Points[2] = po;
                p.Points[1] = new Point(p.Points[1].X, po.Y);
                p.Points[3] = new Point(po.X, p.Points[3].Y);
                p3.move(po.X, po.Y);
                p4.move(po.X, p.Points[3].Y);
                p2.move(p.Points[1].X, po.Y);
            });

            p4 = new MovePoint(drawControl.topCanvas, this, p.Points[3], drawControl.revScale, (po) =>
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
            deleteVirtualShape();
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
            layer.shapes.Remove(this);
            layer.canvas.Children.Remove(p);
            if (p1 != null)
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

        public Point getPosition()
        {
            return p.Points[0];
        }
    }
}
