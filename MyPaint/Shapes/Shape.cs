using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyPaint.Shapes
{
    public abstract class Shape
    {
        public bool MultiDraw { get; protected set; }
        protected bool hit = false;
        protected Brush primaryBrush, secondaryBrush;
        protected Serializer.Brush PrimaryBrush, SecondaryBrush;
        protected double thickness;
        protected bool exist;
        Layer layer;
        public DrawControl File { get; private set; }
        protected OnMouseDownDelegate virtualShapeCallback;
        protected Brush nullBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 255));
        protected Canvas topCanvas;
        bool inLayer = false;

        UIElement _element = null;

        public UIElement Element
        {
            get
            {
                return _element;
            }
            protected set
            {
                if (_element != null)
                {
                    _element = value;
                    ChangeElement();
                }
                else
                {
                    _element = value;
                }
            }
        }

        public UIElement VirtualElement
        {
            get;
            protected set;
        }

        public Shape(DrawControl c, Layer la)
        {
            File = c;
            layer = la;
            SetBrush(BrushEnum.PRIMARY, File.GetShapePrimaryColor());
            SetBrush(BrushEnum.SECONDARY, File.GetShapeSecondaryColor());
            SetThickness(File.GetShapeThickness());
            topCanvas = File.TopCanvas;
            exist = false;
        }

        public Shape(DrawControl c, Layer la, Deserializer.Shape s)
        {
            File = c;
            layer = la;
            SetBrush(BrushEnum.PRIMARY, s.stroke == null ? null : s.stroke.CreateBrush());
            SetBrush(BrushEnum.SECONDARY, s.fill == null ? null : s.fill.CreateBrush());
            SetThickness(s.lineWidth);
            topCanvas = File.TopCanvas;
            exist = true;
        }

        public bool SetBrush(BrushEnum brushEnum, Brush brush)
        {
            if (!OnChangeBrush(brushEnum, brush))
            {
                return false;
            }
            switch (brushEnum)
            {
                case BrushEnum.PRIMARY:
                    primaryBrush = brush;
                    PrimaryBrush = Serializer.Brush.Create(brush);
                    break;
                case BrushEnum.SECONDARY:
                    secondaryBrush = brush;
                    SecondaryBrush = Serializer.Brush.Create(brush);
                    break;
                default:
                    throw new Exception();
            }
            return true;
        }

        abstract protected bool OnChangeBrush(BrushEnum brushEnum, Brush brush);

        public Brush GetBrush(BrushEnum brushEnum)
        {
            switch (brushEnum)
            {
                case BrushEnum.PRIMARY:
                    return primaryBrush;
                case BrushEnum.SECONDARY:
                    return secondaryBrush;
                default:
                    throw new Exception();
            }
        }

        public void ChangeLayer(Layer newLayer, bool addHistory = false)
        {
            if (inLayer)
            {
                int pos = RemoveFromLayer();
                if (addHistory) File.HistoryControl.Add(new History.HistoryShapeChangeLayer(this, layer, newLayer, pos));
                layer = newLayer;
                AddToLayer();
            }
        }

        public void ChangeLayer(Layer newLayer, int pos)
        {
            RemoveFromLayer();
            layer = newLayer;
            InsertToLayer(pos);
        }

        protected void AddToLayer()
        {
            layer.AddShape(this);
            inLayer = true;
        }

        protected void InsertToLayer(int pos)
        {
            layer.InsertShape(pos, this);
            inLayer = true;
        }

        protected int RemoveFromLayer()
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

        public void SetThickness(double s, bool addHistory = false)
        {
            if (!OnChangeThickness(s))
            {
                return;
            }
            thickness = s;
            if (addHistory)
            {
                File.HistoryControl.Add(new History.HistoryShapeThickness(this, GetThickness(), s));
            }
        }

        abstract protected bool OnChangeThickness(double thickness);

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

        public void ShowVirtualShape(OnMouseDownDelegate mouseDown)
        {
            virtualShapeCallback = mouseDown;
            HideVirtualShape();
            File.TopCanvas.Children.Add(VirtualElement);
        }

        public void HideVirtualShape()
        {
            File.TopCanvas.Children.Remove(VirtualElement);
        }

        public void StartMove(Point e)
        {
            File.StartMoveShape(GetPosition(), e);
        }

        virtual public void SetActive()
        {
            ShowVirtualShape((e, s, m) =>
            {
                if (m) File.StartMoveShape(GetPosition(), e);
            });
            File.StartEdit();
        }

        virtual public void MoveDrag(Point e)
        {
            SetHit(false);
        }

        virtual public void StopDrag()
        {
            SetHit(false);
        }

        virtual public void StopEdit()
        {
            HideVirtualShape();
        }

        virtual public void MoveShape(double x, double y)
        {
            SetHit(true);
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

        public void Refresh(int pos = -1)
        {
            if (pos == -1)
            {
                AddToLayer();
                return;
            }
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

        protected void CallBack(object sender, MouseButtonEventArgs ee)
        {
            virtualShapeCallback(ee.GetPosition(File.TopCanvas), this);
            hit = true;
        }
    }
}
