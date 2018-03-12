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

namespace MyPaint.Shapes
{
    public class MyEllipse : MyShape
    {
        Ellipse p = new Ellipse(), vs;
        double sx, sy, ex, ey;
        MovePoint p1, p2, p3, p4;
        double left, top, width, height;
        public MyEllipse(DrawControl c, MyLayer la) : base(c, la)
        {

        }

        public MyEllipse(DrawControl c, MyLayer la, jsonDeserialize.Shape s) : base(c, la, s)
        {
            
            setPrimaryColor(s.stroke == null ? null : s.stroke.createBrush());
            setThickness(s.lineWidth);
            setPrimaryColor(s.stroke == null ? null : s.stroke.createBrush());
            setSecondaryColor(s.fill == null ? null : s.fill.createBrush());
            setThickness(s.lineWidth);

            sx = s.A.x;
            sy = s.A.y;

            p.ToolTip = null;

            addToCanvas(p);
            left = sx;
            top = sy;
            Canvas.SetLeft(p, sx);
            Canvas.SetTop(p, sy);
            moveE(p, s.B.x, s.B.y);
            createVirtualShape();
            createPoints();
            
        }

        override public void setPrimaryColor(Brush s, bool addHistory = false)
        {
            base.setPrimaryColor(s, addHistory);
            p.Stroke = s;
        }

        override public void setSecondaryColor(Brush s, bool addHistory = false)
        {
            base.setSecondaryColor(s, addHistory);
            p.Fill = s;
        }

        override public void addToCanvas()
        {
            addToCanvas(p);
        }

        override public void removeFromCanvas()
        {
            removeFromCanvas(p);
        }

        override public void setThickness(double s, bool addHistory = false)
        {
            base.setThickness(s, addHistory);
            p.StrokeThickness = s;
            if(vs != null) vs.StrokeThickness = s;
        }

        void moveS(Ellipse p, double x, double y)
        {
            if (x > ex)
            {
                width = x - ex;
                p.Width = width;
            }
            else
            {
                left = x;
                Canvas.SetLeft(p, x);
                p.Width = ex - x;
            }
            if (y > ey)
            {
                height = y - ey;
                p.Height = height;
            }
            else
            {
                top = y;
                Canvas.SetTop(p, y);
                height = ey - y;
                p.Height = height;
            }
            sx = x;
            sy = y;
        }

        void moveE(Ellipse p, double x, double y)
        {
            if (x > sx)
            {
                width = x - sx;
                p.Width = width;
            }
            else
            {
                left = x;
                Canvas.SetLeft(p, x);
                width = sx - x;
                p.Width = width;
            }
            if (y > sy)
            {
                height = y - sy;
                p.Height = height;
            }
            else
            {
                top = y;
                Canvas.SetTop(p, y);
                height = sy - y;
                p.Height = height;
            }
            ex = x;
            ey = y;
        }

        override public void drawMouseDown(Point e, MouseButtonEventArgs ee)
        {
            sx = e.X;
            sy = e.Y;

            p.ToolTip = null;
            p.Cursor = Cursors.Pen;
            p.Stroke = drawControl.getShapePrimaryColor();
            p.Fill = drawControl.getShapeSecondaryColor();
            p.StrokeThickness = drawControl.getShapeThickness();

            addToCanvas(p);
            Canvas.SetLeft(p, sx);
            Canvas.SetTop(p, sy);
            
            startDraw();
        }

        override public void drawMouseMove(Point e)
        {
            moveE(p, e.X, e.Y);
        }

        override public void drawMouseUp(Point e, MouseButtonEventArgs ee)
        {
            stopDraw();
            createPoints();
            createVirtualShape();
            setActive();
        }

        override public void createVirtualShape()
        {
            vs = new Ellipse();
            moveS(vs, sx, sy);
            moveE(vs, ex, ey);
            vs.Cursor = Cursors.SizeAll;
            vs.Stroke = drawControl.nullBrush;
            vs.Fill = drawControl.nullBrush;
            vs.StrokeThickness = p.StrokeThickness;
            vs.Cursor = Cursors.SizeAll;
            vs.MouseDown += delegate (object sender, MouseButtonEventArgs ee)
            {
                virtualShapeCallback(ee.GetPosition(drawControl.canvas), this);
                hit = true;
            };            
        }

        override public void showVirtualShape(MyOnMouseDown mouseDown)
        {
            base.showVirtualShape(mouseDown);
            hideVirtualShape();
            drawControl.topCanvas.Children.Add(vs);
        }

        override public void hideVirtualShape()
        {
            drawControl.topCanvas.Children.Remove(vs);
        }

        override public void setActive()
        {
            base.setActive();
            drawControl.setPrimaryColor(p.Stroke);
            drawControl.setSecondaryColor(p.Fill);
            drawControl.setThickness(p.StrokeThickness);
            p1.show();
            p2.show();
            p3.show();
            p4.show();
        }

        override public void moveDrag(Point e)
        {
            base.moveDrag(e);
            p1.move(e);
            p2.move(e);
            p3.move(e);
            p4.move(e);
        }

        override public void stopDrag()
        {
            base.stopDrag();
            p1.stopDrag();
            p2.stopDrag();
            p3.stopDrag();
            p4.stopDrag();
        }

        override public void stopEdit()
        {
            base.stopEdit();
            p1.hide();
            p2.hide();
            p3.hide();
            p4.hide();
        }

        override public void moveShape(double x, double y)
        {
            base.moveShape(x, y);
            sx += x - Canvas.GetLeft(p);
            ex += x - Canvas.GetLeft(p);
            sy += y - Canvas.GetTop(p);
            ey += y - Canvas.GetTop(p);
            Canvas.SetLeft(p, x);
            Canvas.SetTop(p, y);
            Canvas.SetLeft(vs, x);
            Canvas.SetTop(vs, y);

            p1.move(sx, sy);
            p2.move(ex, ey);
            p3.move(ex, sy);
            p4.move(sx, ey);
        }

        override public jsonSerialize.Shape renderShape()
        {
            jsonSerialize.Ellipse ret = new jsonSerialize.Ellipse();
            ret.lineWidth = getThickness();
            ret.stroke = sPrimaryColor;
            ret.fill = sSecondaryColor;
            ret.A = new jsonSerialize.Point(sx, sy);
            ret.B = new jsonSerialize.Point(ex, ey);
            return ret;
        }

        override public Point getPosition()
        {
            return new Point(Canvas.GetLeft(p), Canvas.GetTop(p));
        }

        override public void createPoints()
        {
            p1 = new MovePoint(drawControl.topCanvas, this, new Point(sx, sy), drawControl.revScale, (po) =>
            {
                moveS(p, po.X, po.Y);
                moveS(vs, po.X, po.Y);
                p1.move(po.X, po.Y);
                p3.move(ex, sy);
                p4.move(sx, ey);
            });

            p2 = new MovePoint(drawControl.topCanvas, this, new Point(ex, ey), drawControl.revScale, (po) =>
            {
                moveE(p, po.X, po.Y);
                moveE(vs, po.X, po.Y);
                p2.move(po.X, po.Y);
                p3.move(ex, sy);
                p4.move(sx, ey);
            });

            p3 = new MovePoint(drawControl.topCanvas, this, new Point(ex, sy), drawControl.revScale, (po) =>
            {
                moveE(p, po.X, ey);
                moveS(p, sx, po.Y);
                moveE(vs, po.X, ey);
                moveS(vs, sx, po.Y);
                p3.move(po.X, po.Y);
                p1.move(sx, sy);
                p2.move(ex, ey);
            });

            p4 = new MovePoint(drawControl.topCanvas, this, new Point(sx, ey), drawControl.revScale, (po) =>
            {
                moveE(p, ex, po.Y);
                moveS(p, po.X, sy);
                moveE(vs, ex, po.Y);
                moveS(vs, po.X, sy);
                p4.move(po.X, po.Y);
                p1.move(sx, sy);
                p2.move(ex, ey);
            });
        }

        override public void create(Canvas canvas)
        {
            Ellipse p = new Ellipse();
  
            p.Stroke = primaryColor;
            p.Fill = secondaryColor;
            p.StrokeThickness = thickness;
            p.ToolTip = null;
            p.Width = width;
            p.Height = height;
            canvas.Children.Add(p);
            Canvas.SetLeft(p, left);
            Canvas.SetTop(p, top);
            
        }

    }
}
