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
        DrawControl drawControl;
        Line l = new Line(), lv;
        bool hit = false;
        Brush primaryColor;
        double thickness;
        Canvas canvas;

        public MyLine(DrawControl c, Canvas ca)
        {
            drawControl = c;
            canvas = ca;
        }

        public MyLine(DrawControl c, Canvas ca, jsonDeserialize.Shape s)
        {
            drawControl = c;
            canvas = ca;
            setPrimaryColor(s.stroke == null ? null : s.stroke.createBrush());
            setThickness(s.lineWidth);
            l.X1 = s.A.x;
            l.Y1 = s.A.y;
            l.X2 = s.B.x;
            l.Y2 = s.B.y;
            l.ToolTip = null;
            l.Cursor = Cursors.SizeAll;
            canvas.Children.Add(l);
        }

        public void setPrimaryColor(Brush s)
        {
            primaryColor = s;
            l.Stroke = s;
        }

        public void setSecondaryColor(Brush s)
        {

        }

        public void setThickness(double s)
        {
            l.StrokeThickness = s;
            if(lv != null) lv.StrokeThickness = s;
            thickness = s;
        }

        public void mouseDown(MouseButtonEventArgs e)
        {
            l.Stroke = drawControl.getPrimaryColor();
            l.X1 = e.GetPosition(canvas).X;
            l.Y1 = e.GetPosition(canvas).Y;
            l.X2 = e.GetPosition(canvas).X;
            l.Y2 = e.GetPosition(canvas).Y;
            l.ToolTip = null;
            l.Cursor = Cursors.Pen;
            canvas.Children.Add(l);
            drawControl.draw = true;
        }

        public void mouseMove(MouseEventArgs e)
        {
            l.X2 = e.GetPosition(canvas).X;
            l.Y2 = e.GetPosition(canvas).Y;
        }

        public void mouseUp(MouseButtonEventArgs e)
        {
            drawControl.draw = false;           
            createPoints();
            drawControl.lockDraw();
            
        }

        public void createVirtualShape(MyOnMouseDown mouseDown)
        {
            lv = new Line();
            lv.X1 = l.X1;
            lv.X2 = l.X2;
            lv.Y1 = l.Y1;
            lv.Y2 = l.Y2;
            lv.Cursor = Cursors.SizeAll;
            lv.Stroke = drawControl.nullBrush;
            lv.StrokeThickness = thickness;
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

        MovePoint p1, p2;
        void createPoints()
        {
            createVirtualShape((e, s) =>
            {
                drawControl.startMoveShape(new Point(l.X1, l.Y1), e.GetPosition(canvas));
            });

            drawControl.candraw = false;
            p1 = new MovePoint(drawControl.topCanvas, this, new Point(l.X1, l.Y1), (p) =>
            {
                lv.X1 = l.X1 = p.X;
                lv.Y1 = l.Y1 = p.Y;
            });

            p2 = new MovePoint(drawControl.topCanvas, this,  new Point(l.X2, l.Y2), (p) =>
            {
                lv.X2 = l.X2 = p.X;
                lv.Y2 = l.Y2 = p.Y;
            });
        }

        public void moveDrag(MouseEventArgs e)
        {
            hit = false;
            p1.move(e);
            p2.move(e);
        }

        public void stopDrag()
        {
            hit = false;
            p1.drag = false;
            p2.drag = false;
        }

        public void stopDraw()
        {
            deleteVirtualShape();
            p1.delete();
            p2.delete();
        }

        public void moveShape(double x, double y)
        {           
            lv.X2 = l.X2 = l.X2 - l.X1 + x;
            lv.X1 = l.X1 = x;
            lv.Y2 = l.Y2 = l.Y2 - l.Y1 + y;
            lv.Y1 = l.Y1 = y;
            p1.move(l.X1, l.Y1);
            p2.move(l.X2, l.Y2);
        }

        public jsonSerialize.Shape renderShape()
        {
            jsonSerialize.Line ret = new jsonSerialize.Line();
            ret.lineWidth = thickness;
            ret.stroke = Utils.BrushToCanvas(primaryColor);
            ret.A = new jsonSerialize.Point(l.X1, l.Y1);
            ret.B = new jsonSerialize.Point(l.X2, l.Y2);
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
            canvas.Children.Remove(l);
            drawControl.shapes.Remove(this);
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
            drawControl.shapes.Add(this);
            canvas.Children.Add(l);
            drawControl.lockDraw();
        }
    }
}
