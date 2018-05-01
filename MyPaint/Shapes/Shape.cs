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
    public abstract class Shape
    {
        public bool multiDraw = false;
        protected bool hit = false;
        protected Brush primaryColor, secondaryColor;
        protected Serializer.Brush PrimaryColor, SecondaryColor;
        protected double thickness;
        protected bool exist;
        Layer layer;
        public FileControl drawControl;
        protected OnMouseDownDelegate virtualShapeCallback;
        protected Brush nullBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 255));
        protected Canvas topCanvas;
        protected UIElement element;

        public Shape(FileControl c, Layer la)
        {
            drawControl = c;
            layer = la;
            layer.shapes.Add(this);
            SetPrimaryColor(drawControl.GetShapePrimaryColor());
            SetSecondaryColor(drawControl.GetShapeSecondaryColor());
            SetThickness(drawControl.GetShapeThickness());
            topCanvas = drawControl.topCanvas;
            exist = false;
        }

        public Shape(FileControl c, Layer la, Deserializer.Shape s)
        {
            drawControl = c;
            layer = la;
            layer.shapes.Add(this);
            SetThickness(s.lineWidth);
            SetPrimaryColor(s.stroke == null ? null : s.stroke.CreateBrush());
            SetSecondaryColor(s.fill == null ? null : s.fill.CreateBrush());
            topCanvas = drawControl.topCanvas;
            exist = true;
        }

        virtual public void SetPrimaryColor(Brush s, bool addHistory = false)
        {
            if (addHistory && s != primaryColor)
            {
                drawControl.historyControl.Add(new History.HistoryPrimaryColor(this, GetPrimaryColor(), s));
            }
            primaryColor = s;
            PrimaryColor = Serializer.Brush.Create(s);
        }

        virtual public void SetSecondaryColor(Brush s, bool addHistory = false)
        {
            if (addHistory && s != secondaryColor)
            {
                drawControl.historyControl.Add(new History.HistorySecondaryColor(this, GetSecondaryColor(), s));
            }
            secondaryColor = s;
            SecondaryColor = Serializer.Brush.Create(s);
        }

        public Brush GetPrimaryColor()
        {
            return primaryColor;
        }

        public Brush GetSecondaryColor()
        {
            return secondaryColor;
        }


        public void ChangeLayer(Layer newLayer, bool addHistory = false)
        {
            RemoveFromCanvas();
            int pos = layer.shapes.IndexOf(this);
            layer.shapes.Remove(this);   
            if (addHistory) drawControl.historyControl.Add(new History.HistoryShapeChangeLayer(this, layer, newLayer, pos));
            layer = newLayer;
            AddToCanvas();
            layer.shapes.Add(this);
        }

        public void ChangeLayer(Layer newLayer, int pos)
        {
            RemoveFromCanvas();
            layer.shapes.Remove(this);  
            layer = newLayer;
            InsertToCanvas(pos);
            layer.shapes.Insert(pos, this);
        }

        public void AddToCanvas()
        {
            AddToCanvas(element);
        }

        public void InsertToCanvas(int pos)
        {
            InsertToCanvas(pos, element);
        }

        public void RemoveFromCanvas()
        {
            RemoveFromCanvas(element);
        }

        virtual public void SetThickness(double s, bool addHistory = false)
        {
            thickness = s;
            if (addHistory)
            {
                drawControl.historyControl.Add(new History.HistoryShapeThickness(this, GetThickness(), s));
            }
        }

        public double GetThickness()
        {
            return thickness;
        }


        virtual public void DrawMouseDown(Point e, MouseButtonEventArgs ee)
        {

        }

        virtual public void DrawMouseMove(Point e)
        {

        }

        virtual public void DrawMouseUp(Point e, MouseButtonEventArgs ee)
        {

        }

        abstract public void CreateVirtualShape();

        virtual public void ShowVirtualShape(OnMouseDownDelegate mouseDown)
        {
            virtualShapeCallback = mouseDown;
        }

        abstract public void HideVirtualShape();

        public void StartMove(Point e)
        {
            drawControl.StartMoveShape(GetPosition(), e);
        }

        virtual public void SetActive()
        { 
            ShowVirtualShape((e, s, m) =>
            {
                if(m) drawControl.StartMoveShape(GetPosition(), e);
            });
            drawControl.StartEdit();
        }

        virtual public void MoveDrag(Point e)
        {
            hit = false;
        }

        virtual public void StopDrag()
        {
            hit = false;
        }

        virtual public void StopEdit()
        {
            HideVirtualShape();
        }

        virtual public void MoveShape(double x, double y)
        {
            hit = true;
        }

        abstract public Serializer.Shape CreateSerializer();

        public void SetHit(bool h)
        {
            hit = h;
        }

        public bool HitTest()
        {
            return hit;
        }

        public int Delete()
        {
            RemoveFromCanvas();
            int ord = layer.shapes.IndexOf(this);
            layer.shapes.Remove(this);
            if(exist) StopEdit();
            return ord;
        }

        public void Refresh()
        {
            layer.shapes.Add(this);
            AddToCanvas();
        }

        public void Refresh(int pos)
        {
            layer.shapes.Insert(pos, this);
            InsertToCanvas(pos);
        }

        abstract public Point GetPosition();

        abstract public void CreatePoints();

        protected void AddToCanvas(UIElement s)
        {
            layer.canvas.Children.Add(s);
        }

        public void SetPosition(int i, bool addHistory = false)
        {
            int pos = layer.canvas.Children.IndexOf(element);
            RemoveFromCanvas();
            layer.shapes.Remove(this);
            if (i == -1)
            {
                AddToCanvas();
                layer.shapes.Add(this);
            }
            else
            {
                InsertToCanvas(i);
                layer.shapes.Insert(i, this);
            }
            if (addHistory && pos != i) drawControl.historyControl.Add(new History.HistoryShapePosition(this, pos, i));
        }

        protected void InsertToCanvas(int pos, UIElement s)
        {
            layer.canvas.Children.Insert(pos, s);
        }

        protected void RemoveFromCanvas(UIElement s)
        {
            layer.canvas.Children.Remove(s);
        }

        protected void StartDraw()
        {
            drawControl.StartDraw();
        }

        protected void StopDraw()
        {
            exist = true;
            drawControl.StopDraw();
        }

        abstract public void CreateImage(Canvas canvas);

        virtual public void ChangeZoom()
        {

        }
    }
}
