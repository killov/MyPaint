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
        DrawControl drawControl;
        Ellipse p = new Ellipse(), lv;
        Brush primaryColor, secondaryColor;
        bool hit = false;
        double thickness;
        double sx, sy, ex, ey;
        Canvas canvas;

        public MyEllipse(DrawControl c, Canvas ca)
        {
            drawControl = c;
            canvas = ca;
        }

        public MyEllipse(DrawControl c, Canvas ca, jsonDeserialize.Shape s)
        {
            drawControl = c;
            canvas = ca;
            setPrimaryColor(s.stroke == null ? null : s.stroke.createBrush());
            setSecondaryColor(s.fill == null ? null : s.fill.createBrush());
            setThickness(s.lineWidth);

            sx = s.A.x;
            sy = s.A.y;

            canvas.Children.Add(p);
            Canvas.SetLeft(p, sx);
            Canvas.SetTop(p, sy);
            p.ToolTip = null;
            moveE(p, s.B.x, s.B.y);

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
            sx = e.GetPosition(canvas).X;
            sy = e.GetPosition(canvas).Y;

            p.ToolTip = null;
            p.Cursor = Cursors.Pen;
           
            canvas.Children.Add(p);
            Canvas.SetLeft(p, sx);
            Canvas.SetTop(p, sy);
            drawControl.draw = true;
        }

        public void mouseMove(MouseEventArgs e)
        {
            double x = e.GetPosition(canvas).X;
            double y = e.GetPosition(canvas).Y;
            moveE(p, x, y);
        }

        public void mouseUp(MouseButtonEventArgs e)
        {
            drawControl.draw = false;
            createPoints();
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

        MovePoint p1, p2, p3, p4;
        void createPoints()
        {
            createVirtualShape((e, s) =>
            {
                drawControl.startMoveShape(new Point(Canvas.GetLeft(p), Canvas.GetTop(p)), e.GetPosition(canvas));
            });

            drawControl.candraw = false;
            p1 = new MovePoint(drawControl.topCanvas, this, new Point(sx, sy), (po) =>
            {
                moveS(p, po.X, po.Y);
                p1.move(po.X, po.Y);
                p3.move(ex, sy);
                p4.move(sx, ey);
            });

            p2 = new MovePoint(drawControl.topCanvas, this, new Point(ex, ey), (po) =>
            {
                moveE(p, po.X, po.Y);
                p2.move(po.X, po.Y);
                p3.move(ex, sy);
                p4.move(sx, ey);
            });

            p3 = new MovePoint(drawControl.topCanvas, this, new Point(ex, sy), (po) =>
            {
                moveE(p, po.X, ey);
                moveS(p, sx, po.Y);
                p3.move(po.X, po.Y);
                p1.move(sx, sy);
                p2.move(ex, ey);
            });

            p4 = new MovePoint(drawControl.topCanvas, this, new Point(sx, ey), (po) =>
            {
                moveE(p, ex, po.Y);
                moveS(p, po.X, sy);
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
            canvas.Children.Remove(p);
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
            drawControl.shapes.Add(this);
            canvas.Children.Add(p);
            drawControl.lockDraw();
        }
    }
}
