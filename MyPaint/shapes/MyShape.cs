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
    public delegate void MyOnMouseDown(Point e, MyShape s);

    public abstract class MyShape
    {
        protected bool hit = false;
        MyLayer layer;
        public DrawControl drawControl;
        protected MyOnMouseDown virtualShapeCallback;

        public MyShape(DrawControl c, MyLayer la)
        {
            drawControl = c;
            layer = la;
            layer.shapes.Add(this);
        }

        public MyShape(DrawControl c, MyLayer la, jsonDeserialize.Shape s)
        {
            drawControl = c;
            layer = la;
        }

        virtual public void setPrimaryColor(Brush s, bool addHistory = false)
        {
            if (addHistory)
            {
                drawControl.control.addHistory(new HistoryPrimaryColor(this, getPrimaryColor(), s));
            }

        }

        virtual public void setSecondaryColor(Brush s, bool addHistory = false)
        {
            if (addHistory)
            {
                drawControl.control.addHistory(new HistorySecondaryColor(this, getSecondaryColor(), s));
            }
        }

        abstract public Brush getPrimaryColor();

        abstract public Brush getSecondaryColor();


        public void changeLayer(MyLayer newLayer)
        {
            if (layer != null)
            {
                removeFromCanvas();
                layer.shapes.Remove(this);
            }
            layer = newLayer;
            if (layer != null)
            {
                addToCanvas();
                layer.shapes.Add(this);
            }
        }

        abstract public void addToCanvas();

        abstract public void removeFromCanvas();

        virtual public void setThickness(double s, bool addHistory = false)
        {
            if (addHistory)
            {
                drawControl.control.addHistory(new HistoryShapeThickness(this, getThickness(), s));
            }
        }

        public abstract double getThickness();


        virtual public void drawMouseDown(Point e, MouseButtonEventArgs ee)
        {

        }

        virtual public void drawMouseMove(Point e)
        {

        }

        virtual public void drawMouseUp(Point e, MouseButtonEventArgs ee)
        {

        }

        abstract public void createVirtualShape();

        virtual public void showVirtualShape(MyOnMouseDown mouseDown)
        {
            virtualShapeCallback = mouseDown;
        }

        abstract public void hideVirtualShape();

        public void startMove(Point e)
        {
            drawControl.startMoveShape(getPosition(), e);
        }

        virtual public void setActive()
        { 
            showVirtualShape((e, s) =>
            {
                drawControl.startMoveShape(getPosition(), e);
            });
            drawControl.candraw = false;
        }

        virtual public void moveDrag(Point e)
        {
            hit = false;
        }

        virtual public void stopDrag()
        {
            hit = false;
        }

        virtual public void stopEdit()
        {
            hideVirtualShape();
        }

        virtual public void moveShape(double x, double y)
        {
            hit = true;
        }

        abstract public jsonSerialize.Shape renderShape();

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
            removeFromCanvas();
            layer.shapes.Remove(this);
            stopEdit();
            hideVirtualShape();
        }

        public void refresh()
        {
            layer.shapes.Add(this);
            addToCanvas();
            drawControl.lockDraw();
        }

        abstract public Point getPosition();

        abstract public void createPoints();

        protected void addToCanvas(Shape s)
        {
            layer.canvas.Children.Add(s);
        }

        protected void removeFromCanvas(Shape s)
        {
            layer.canvas.Children.Remove(s);
        }

        protected void startDraw()
        {
            drawControl.draw = true;
        }

        protected void stopDraw()
        {
            drawControl.draw = false;
        }
    }
}
