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
    class MyLine : MyShape
    {
        DrawControl drawControl;
        Line l = new Line(), lv;
        bool hit = false;
        Brush primaryColor;
        double thickness;
        MyLayer layer;

        public MyLine(DrawControl c, MyLayer la)
        {
            drawControl = c;
            layer = la;
            layer.shapes.Add(this);
        }

        public MyLine(DrawControl c, MyLayer la, jsonDeserialize.Shape s)
        {
            drawControl = c;
            layer = la;
            setPrimaryColor(s.stroke == null ? null : s.stroke.createBrush());
            setThickness(s.lineWidth);
            l.X1 = s.A.x;
            l.Y1 = s.A.y;
            l.X2 = s.B.x;
            l.Y2 = s.B.y;
            l.ToolTip = null;
            l.Cursor = Cursors.SizeAll;
            layer.canvas.Children.Add(l);
            layer.shapes.Add(this);
        }

        public void setPrimaryColor(Brush s)
        {
            primaryColor = s;
            l.Stroke = s;
        }

        public void setSecondaryColor(Brush s)
        {

        }

        public void changeLayer(MyLayer newLayer)
        {
            if (layer != null)
            {
                layer.canvas.Children.Remove(l);
                layer.shapes.Remove(this);
            }
            layer = newLayer;
            if (layer != null)
            {
                layer.canvas.Children.Add(l);
                layer.shapes.Add(this);
            }
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
            l.X1 = e.GetPosition(layer.canvas).X;
            l.Y1 = e.GetPosition(layer.canvas).Y;
            l.X2 = e.GetPosition(layer.canvas).X;
            l.Y2 = e.GetPosition(layer.canvas).Y;
            l.ToolTip = null;
            l.Cursor = Cursors.Pen;
            layer.canvas.Children.Add(l);
            drawControl.draw = true;
        }

        public void mouseMove(MouseEventArgs e)
        {
            l.X2 = e.GetPosition(layer.canvas).X;
            l.Y2 = e.GetPosition(layer.canvas).Y;
        }

        public void mouseUp(MouseButtonEventArgs e)
        {
            drawControl.draw = false;           
            setActive();
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

        public void startMove(MouseButtonEventArgs e)
        {
            drawControl.startMoveShape(new Point(l.X1, l.Y1), e.GetPosition(layer.canvas));
        }

        MovePoint p1, p2;
        public void setActive()
        {
            createVirtualShape((e, s) =>
            {
                drawControl.startMoveShape(new Point(l.X1, l.Y1), e.GetPosition(layer.canvas));
            });

            drawControl.candraw = false;
            p1 = new MovePoint(drawControl.topCanvas, this, new Point(l.X1, l.Y1), drawControl.revScale, (p) =>
            {
                lv.X1 = l.X1 = p.X;
                lv.Y1 = l.Y1 = p.Y;
            });

            p2 = new MovePoint(drawControl.topCanvas, this,  new Point(l.X2, l.Y2), drawControl.revScale, (p) =>
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
            layer.canvas.Children.Remove(l);
            layer.shapes.Remove(this);
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
            layer.canvas.Children.Add(l);
            drawControl.lockDraw();
        }
    }
}
