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
        public bool MultiDraw { get; protected set; }
        protected bool hit = false;
        protected Brush primaryColor, secondaryColor;
        protected Serializer.Brush PrimaryColor, SecondaryColor;
        protected double thickness;
        protected bool exist;
        Layer layer;
        public FileControl File { get; private set; }
        protected OnMouseDownDelegate virtualShapeCallback;
        protected Brush nullBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 255));
        protected Canvas topCanvas;
        bool inLayer = false;
        
        UIElement element = null;

        public UIElement Element {
            get {
                return element;
            }
            protected set {
                if(element != null)
                {
                    element = value;
                    ChangeElement();
                }
                else
                {
                    element = value;
                }
            }
        }

        public Shape(FileControl c, Layer la)
        {
            File = c;
            layer = la;
            SetPrimaryColor(File.GetShapePrimaryColor());
            SetSecondaryColor(File.GetShapeSecondaryColor());
            SetThickness(File.GetShapeThickness());
            topCanvas = File.TopCanvas;
            exist = false;
        }

        public Shape(FileControl c, Layer la, Deserializer.Shape s)
        {
            File = c;
            layer = la;
            SetThickness(s.lineWidth);
            SetPrimaryColor(s.stroke == null ? null : s.stroke.CreateBrush());
            SetSecondaryColor(s.fill == null ? null : s.fill.CreateBrush());
            topCanvas = File.TopCanvas;
            exist = true;
        }

        virtual public void SetPrimaryColor(Brush s, bool addHistory = false)
        {
            if (addHistory && s != primaryColor)
            {
                File.HistoryControl.Add(new History.HistoryPrimaryColor(this, GetPrimaryColor(), s));
            }
            primaryColor = s;
            PrimaryColor = Serializer.Brush.Create(s);
        }

        virtual public void SetSecondaryColor(Brush s, bool addHistory = false)
        {
            if (addHistory && s != secondaryColor)
            {
                File.HistoryControl.Add(new History.HistorySecondaryColor(this, GetSecondaryColor(), s));
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
            RemoveFromLayer();
            int pos = RemoveFromLayer();   
            if (addHistory) File.HistoryControl.Add(new History.HistoryShapeChangeLayer(this, layer, newLayer, pos));
            layer = newLayer;
            AddToLayer();
        }

        public void ChangeLayer(Layer newLayer, int pos)
        {
            RemoveFromLayer();
            layer = newLayer;
            InsertToLayer(pos);
        }

        public void AddToLayer()
        {
            layer.AddShape(this);
            inLayer = true;
        }

        public void InsertToLayer(int pos)
        {
            layer.InsertShape(pos, this);
            inLayer = true;
        }

        public int RemoveFromLayer()
        {
            return layer.RemoveShape(this);
        }

        void ChangeElement()
        {
            if (inLayer)
            {
                layer.ShapeElementChanged(this);
            }
        }

        virtual public void SetThickness(double s, bool addHistory = false)
        {
            thickness = s;
            if (addHistory)
            {
                File.HistoryControl.Add(new History.HistoryShapeThickness(this, GetThickness(), s));
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

        virtual public void ShowVirtualShape(OnMouseDownDelegate mouseDown)
        {
            virtualShapeCallback = mouseDown;
        }

        abstract public void HideVirtualShape();

        public void StartMove(Point e)
        {
            File.StartMoveShape(GetPosition(), e);
        }

        virtual public void SetActive()
        { 
            ShowVirtualShape((e, s, m) =>
            {
                if(m) File.StartMoveShape(GetPosition(), e);
            });
            File.StartEdit();
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
            if (exist) StopEdit();
            HideVirtualShape();
            return RemoveFromLayer();
        }

        public void Refresh()
        {
            AddToLayer();
        }

        public void Refresh(int pos)
        {
            InsertToLayer(pos);
        }

        abstract public Point GetPosition();

        public void SetPosition(int i, bool addHistory = false)
        {
            int pos = RemoveFromLayer();
            if (i == -1)
            {
                AddToLayer();
            }
            else
            {
                InsertToLayer(i);
            }
            if (addHistory && pos != i) File.HistoryControl.Add(new History.HistoryShapePosition(this, pos, i));
        }

        protected void StartDraw()
        {
            File.StartDraw();
        }

        protected void StopDraw(bool addHistory = true)
        {
            exist = true;
            File.StopDraw(addHistory);
        }

        abstract public void CreateImage(Canvas canvas);

        virtual public void ChangeZoom()
        {

        }
    }
}
