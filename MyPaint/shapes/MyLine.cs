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
    public class MyLine : MyShape
    {
        Line p = new Line(), vs;
        MovePoint p1, p2;
        public MyLine(DrawControl c, MyLayer la) : base(c, la)
        {

        }

        public MyLine(DrawControl c, MyLayer la, jsonDeserialize.Shape s) : base(c, la, s)
        {
            setPrimaryColor(s.stroke == null ? null : s.stroke.createBrush());
            setThickness(s.lineWidth);
            p.X1 = s.A.x;
            p.Y1 = s.A.y;
            p.X2 = s.B.x;
            p.Y2 = s.B.y;
            p.ToolTip = null;
            p.Cursor = Cursors.SizeAll;
            addToCanvas(p);
            createPoints();
            createVirtualShape();
        }

        override public void setPrimaryColor(Brush s, bool addHistory = false)
        {
            base.setPrimaryColor(s, addHistory);
            p.Stroke = s;
        }

        override public void setSecondaryColor(Brush s, bool addHistory = false)
        {

        }

        override public Brush getPrimaryColor()
        {
            return p.Stroke;
        }

        override public Brush getSecondaryColor()
        {
            return null;
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

        override public double getThickness()
        {
            return p.StrokeThickness;
        }


        override public void drawMouseDown(Point e, MouseButtonEventArgs ee)
        {
            p.Stroke = drawControl.getShapePrimaryColor();
            p.StrokeThickness = drawControl.getShapeThickness();
            p.X1 = e.X;
            p.Y1 = e.Y;
            p.X2 = e.X;
            p.Y2 = e.Y;
            p.ToolTip = null;
            p.Cursor = Cursors.Pen;
            addToCanvas(p);
            startDraw();
        }

        override public void drawMouseMove(Point e)
        {
            p.X2 = e.X;
            p.Y2 = e.Y;
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
            vs = new Line();
            vs.X1 = p.X1;
            vs.X2 = p.X2;
            vs.Y1 = p.Y1;
            vs.Y2 = p.Y2;
            vs.Cursor = Cursors.SizeAll;
            vs.Stroke = drawControl.nullBrush;
            vs.StrokeThickness = p.StrokeThickness;
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
            drawControl.setThickness(p.StrokeThickness);
            p1.show();
            p2.show();
        }

        override public void moveDrag(Point e)
        {
            base.moveDrag(e);
            p1.move(e);
            p2.move(e);
        }

        override public void stopDrag()
        {
            base.stopDrag();
            p1.stopDrag();
            p2.stopDrag();
        }

        override public void stopEdit()
        {
            base.stopEdit();
            p1.hide();
            p2.hide();
        }

        override public void moveShape(double x, double y)
        {
            base.moveShape(x, y);
            vs.X2 = p.X2 = p.X2 - p.X1 + x;
            vs.X1 = p.X1 = x;
            vs.Y2 = p.Y2 = p.Y2 - p.Y1 + y;
            vs.Y1 = p.Y1 = y;

            p1.move(p.X1, p.Y1);
            p2.move(p.X2, p.Y2);
        }

        override public jsonSerialize.Shape renderShape()
        {
            jsonSerialize.Line ret = new jsonSerialize.Line();
            ret.lineWidth = p.StrokeThickness;
            ret.stroke = Utils.BrushToCanvas(p.Stroke);
            ret.A = new jsonSerialize.Point(p.X1, p.Y1);
            ret.B = new jsonSerialize.Point(p.X2, p.Y2);
            return ret;
        }

        override public Point getPosition()
        {
            return new Point(p.X1, p.Y1);
        }

        override public void createPoints()
        {
            p1 = new MovePoint(drawControl.topCanvas, this, new Point(p.X1, p.Y1), drawControl.revScale, (e) =>
            {
                vs.X1 = p.X1 = e.X;
                vs.Y1 = p.Y1 = e.Y;
            });

            p2 = new MovePoint(drawControl.topCanvas, this, new Point(p.X2, p.Y2), drawControl.revScale, (e) =>
            {
                vs.X2 = p.X2 = e.X;
                vs.Y2 = p.Y2 = e.Y;
            });
        }

    }
}
