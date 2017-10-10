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
    class MyEllipse : IMyShape
    {
        DrawControl drawControl;
        Ellipse p = new Ellipse(), lv;
        Brush primaryColor, secondaryColor;
        bool hit = false;
        double thickness;
        double sx, sy, ex, ey;
        MyLayer layer;

        public MyEllipse(DrawControl c, MyLayer la)
        {
            drawControl = c;
            layer = la;
            layer.shapes.Add(this);
        }

        public MyEllipse(DrawControl c, MyLayer la, jsonDeserialize.Shape s)
        {
            drawControl = c;
            layer = la;
            setPrimaryColor(s.stroke == null ? null : s.stroke.createBrush());
            setSecondaryColor(s.fill == null ? null : s.fill.createBrush());
            setThickness(s.lineWidth);

            sx = s.A.x;
            sy = s.A.y;

            layer.canvas.Children.Add(p);
            Canvas.SetLeft(p, sx);
            Canvas.SetTop(p, sy);
            p.ToolTip = null;
            moveE(p, s.B.x, s.B.y);
        }

        public void setPrimaryColor(Brush s, bool addHistory = false)
        {
            if (addHistory)
            {
                drawControl.control.addHistory(new HistoryPrimaryColor(this, primaryColor, s));
            }
            primaryColor = s;
            p.Stroke = s;
        }

        public void setSecondaryColor(Brush s, bool addHistory = false)
        {
            if (addHistory)
            {
                drawControl.control.addHistory(new HistorySecondaryColor(this, secondaryColor, s));
            }
            secondaryColor = s;
            p.Fill = s;
        }

        public void changeLayer(MyLayer newLayer)
        {
            if(layer != null)
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

        public void setThickness(double s, bool addHistory = false)
        {
            if (addHistory)
            {
                drawControl.control.addHistory(new HistoryShapeThickness(this, thickness, s));
            }
            p.StrokeThickness = s;
            if (lv != null) lv.StrokeThickness = s;
            thickness = s;
        }

        void moveS(Ellipse p, double x, double y)
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

        void moveE(Ellipse p, double x, double y)
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
            sx = e.GetPosition(layer.canvas).X;
            sy = e.GetPosition(layer.canvas).Y;

            p.ToolTip = null;
            p.Cursor = Cursors.Pen;

            layer.canvas.Children.Add(p);
            Canvas.SetLeft(p, sx);
            Canvas.SetTop(p, sy);
            drawControl.draw = true;
        }

        public void mouseMove(MouseEventArgs e)
        {
            double x = e.GetPosition(layer.canvas).X;
            double y = e.GetPosition(layer.canvas).Y;
            moveE(p, x, y);
        }

        public void mouseUp(MouseButtonEventArgs e)
        {
            drawControl.draw = false;
            setActive();
            drawControl.lockDraw();
        }

        public void createVirtualShape(MyOnMouseDown mouseDown)
        {
            lv = new Ellipse();
            moveS(lv, sx, sy);
            moveE(lv, ex, ey);
            lv.Cursor = Cursors.SizeAll;
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
            drawControl.startMoveShape(new Point(Canvas.GetLeft(p), Canvas.GetTop(p)), e.GetPosition(layer.canvas));
        }

        MovePoint p1, p2, p3, p4;
        public void setActive()
        {
            drawControl.setPrimaryColor(primaryColor);
            drawControl.setSecondaryColor(secondaryColor);
            drawControl.setThickness(thickness);
            createVirtualShape((e, s) =>
            {
                drawControl.startMoveShape(new Point(Canvas.GetLeft(p), Canvas.GetTop(p)), e.GetPosition(layer.canvas));
            });

            drawControl.candraw = false;
            p1 = new MovePoint(drawControl.topCanvas, this, new Point(sx, sy), drawControl.revScale, (po) =>
            {
                moveS(p, po.X, po.Y);
                moveS(lv, po.X, po.Y);
                p1.move(po.X, po.Y);
                p3.move(ex, sy);
                p4.move(sx, ey);
            });

            p2 = new MovePoint(drawControl.topCanvas, this, new Point(ex, ey), drawControl.revScale, (po) =>
            {
                moveE(p, po.X, po.Y);
                moveE(lv, po.X, po.Y);
                p2.move(po.X, po.Y);
                p3.move(ex, sy);
                p4.move(sx, ey);
            });

            p3 = new MovePoint(drawControl.topCanvas, this, new Point(ex, sy), drawControl.revScale, (po) =>
            {
                moveE(p, po.X, ey);
                moveS(p, sx, po.Y);
                moveE(lv, po.X, ey);
                moveS(lv, sx, po.Y);
                p3.move(po.X, po.Y);
                p1.move(sx, sy);
                p2.move(ex, ey);
            });

            p4 = new MovePoint(drawControl.topCanvas, this, new Point(sx, ey), drawControl.revScale, (po) =>
            {
                moveE(p, ex, po.Y);
                moveS(p, po.X, sy);
                moveE(lv, ex, po.Y);
                moveS(lv, po.X, sy);
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
            p1.stopDrag();
            p2.stopDrag();
            p3.stopDrag();
            p4.stopDrag();
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
            hit = true;
            sx += x-Canvas.GetLeft(p);
            ex += x-Canvas.GetLeft(p);
            sy += y-Canvas.GetTop(p);
            ey += y-Canvas.GetTop(p);
            Canvas.SetLeft(p, x);
            Canvas.SetTop(p, y);
            if (lv != null)
            {
                Canvas.SetLeft(lv, x);
                Canvas.SetTop(lv, y);
            }

            p1.move(sx, sy);
            p2.move(ex, ey);
            p3.move(ex, sy);
            p4.move(sx, ey);
        }

        public jsonSerialize.Shape renderShape()
        {
            jsonSerialize.Ellipse ret = new jsonSerialize.Ellipse();
            ret.lineWidth = thickness;
            ret.stroke = Utils.BrushToCanvas(primaryColor);
            ret.fill = Utils.BrushToCanvas(secondaryColor);
            ret.A = new jsonSerialize.Point(sx, sy);
            ret.B = new jsonSerialize.Point(ex, ey);
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
            layer.canvas.Children.Remove(p);
            layer.shapes.Remove(this);
            if(p1 != null)
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
            return new Point(Canvas.GetLeft(p), Canvas.GetTop(p));
        }

    }
}
