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
    public delegate void MyOnMouseDown(Point e, Shape s);

    public abstract class Shape
    {
        protected bool hit = false;
        protected Brush primaryColor, secondaryColor;
        protected jsonSerialize.Brush sPrimaryColor, sSecondaryColor;
        protected double thickness;
        Layer layer;
        public FileControl drawControl;
        protected MyOnMouseDown virtualShapeCallback;
        protected Brush nullBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 255));
        protected Canvas canvas, topCanvas;

        public Shape(FileControl c, Layer la)
        {
            drawControl = c;
            layer = la;
            layer.shapes.Add(this);
            setPrimaryColor(drawControl.primaryColor);
            setSecondaryColor(drawControl.secondaryColor);
            setThickness(drawControl.thickness);
            canvas = layer.canvas;
            topCanvas = drawControl.topCanvas;
        }

        public Shape(FileControl c, Layer la, jsonDeserialize.Shape s)
        {
            drawControl = c;
            layer = la;
            canvas = layer.canvas;
            topCanvas = drawControl.topCanvas;
        }

        virtual public void setPrimaryColor(Brush s, bool addHistory = false)
        {
            primaryColor = s;
            sPrimaryColor = jsonSerialize.Brush.create(s);
            if (addHistory)
            {
                drawControl.historyControl.add(new History.HistoryPrimaryColor(this, getPrimaryColor(), s));
            }

        }

        virtual public void setSecondaryColor(Brush s, bool addHistory = false)
        {
            secondaryColor = s;
            sSecondaryColor = jsonSerialize.Brush.create(s);
            if (addHistory)
            {
                drawControl.historyControl.add(new History.HistorySecondaryColor(this, getSecondaryColor(), s));
            }
        }

        public Brush getPrimaryColor()
        {
            return primaryColor;
        }

        public Brush getSecondaryColor()
        {
            return secondaryColor;
        }


        public void changeLayer(Layer newLayer)
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
            thickness = s;
            if (addHistory)
            {
                drawControl.historyControl.add(new History.HistoryShapeThickness(this, getThickness(), s));
            }
        }

        public double getThickness()
        {
            return thickness;
        }


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
            drawControl.startEdit();
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

        protected void addToCanvas(System.Windows.Shapes.Shape s)
        {
            layer.canvas.Children.Add(s);
        }

        protected void removeFromCanvas(System.Windows.Shapes.Shape s)
        {
            layer.canvas.Children.Remove(s);
        }

        protected void startDraw()
        {
            drawControl.startDraw();
        }

        protected void stopDraw()
        {
            drawControl.stopDraw();
        }

        abstract public void create(Canvas canvas);
    }
}
