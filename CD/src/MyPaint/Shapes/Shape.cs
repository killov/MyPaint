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
        public DrawControl DrawControl { get; private set; }
        protected OnMouseDownDelegate virtualElementCallback;
        protected Brush nullBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 255));
        bool inLayer = false;
        protected Serializer.Shape _dShape;
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
            nullBrush.Freeze();
            DrawControl = c;
            layer = la;
            OnDrawInit();
            SetBrush(BrushEnum.PRIMARY, DrawControl.GetShapePrimaryColor());
            SetBrush(BrushEnum.SECONDARY, DrawControl.GetShapeSecondaryColor());
            SetThickness(DrawControl.GetShapeThickness());
            exist = false;
        }

        public Shape(DrawControl c, Layer la, Serializer.Shape s)
        {
            nullBrush.Freeze();
            _dShape = s;
            DrawControl = c;
            layer = la;
            exist = true;
        }

        public void InitDraw()
        {
            OnCreateInit(_dShape);
            if (_dShape != null)
            {
                SetBrush(BrushEnum.PRIMARY, _dShape.Stroke == null ? null : _dShape.Stroke.CreateBrush());
                SetBrush(BrushEnum.SECONDARY, _dShape.Fill == null ? null : _dShape.Fill.CreateBrush());
                SetThickness(_dShape.LineWidth);
            }
            AddToLayer();
        }

        abstract protected void OnDrawInit();

        abstract protected void OnCreateInit(Serializer.Shape s);

        public bool SetBrush(BrushEnum brushEnum, Brush brush)
        {
            if (Element != null && !OnChangeBrush(brushEnum, brush))
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
                if (addHistory) DrawControl.HistoryControl.Add(new History.HistoryShapeChangeLayer(this, layer, newLayer, pos));
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

        public bool SetThickness(double s)
        {
            if (Element != null && !OnChangeThickness(s))
            {
                return false;
            }
            thickness = s;
            return true;
        }

        abstract protected bool OnChangeThickness(double thickness);

        public double GetThickness()
        {
            return thickness;
        }

        public void DrawMouseDown(Point e, MouseButtonEventArgs ee)
        {
            OnDrawMouseDown(e, ee);
        }

        public void DrawMouseMove(Point e)
        {
            OnDrawMouseMove(e);
        }

        public void DrawMouseUp(Point e, MouseButtonEventArgs ee)
        {
            OnDrawMouseUp(e, ee);
        }

        abstract public void OnDrawMouseDown(Point e, MouseButtonEventArgs ee);

        abstract public void OnDrawMouseMove(Point e);

        abstract public void OnDrawMouseUp(Point e, MouseButtonEventArgs ee);

        abstract protected void CreatePoints();

        abstract protected void CreateVirtualShape();

        public void ShowVirtualShape(OnMouseDownDelegate mouseDown)
        {
            virtualElementCallback = mouseDown;
            HideVirtualShape();
            DrawControl.TopCanvas.Children.Add(VirtualElement);
        }

        public void HideVirtualShape()
        {
            DrawControl.TopCanvas.Children.Remove(VirtualElement);
        }

        public void SetSelectable()
        {
            ShowVirtualShape((e, s, m) =>
            {
                SetShapeSelect(e, s, m);
            });
        }

        private void SetShapeSelect(Point e, Shapes.Shape shape, bool enableMoving)
        {
            DrawControl.StopEdit();
            SetSelectable();
            DrawControl.SetShapeActive(shape);
            if (enableMoving)
            {
                shape.StartMove(e);
            }
        }

        public void StartMove(Point e)
        {
            DrawControl.StartMoveShape(GetPosition(), e);
        }

        public void SetActive()
        {
            ShowVirtualShape((e, s, m) =>
            {
                if (m) DrawControl.StartMoveShape(GetPosition(), e);
            });
            DrawControl.StartEdit();
            OnSetActive();
        }

        abstract protected void OnSetActive();

        public void MoveDrag(Point e)
        {
            SetHit(false);
            OnMoveDrag(e);
        }

        abstract protected void OnMoveDrag(Point e);

        public void StopDrag()
        {
            SetHit(false);
            OnStopDrag();
        }

        abstract protected void OnStopDrag();

        public void StopEdit()
        {
            HideVirtualShape();
            OnStopEdit();
        }

        abstract protected void OnStopEdit();

        public void MoveShape(Point point)
        {
            SetHit(true);
            OnMoveShape(point);
        }

        abstract protected void OnMoveShape(Point point);

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
            if (addHistory && pos != i) DrawControl.HistoryControl.Add(new History.HistoryShapePosition(this, pos, i));
        }

        protected void StartDraw()
        {
            DrawControl.StartDraw();
        }

        protected void StopDraw(bool addHistory = true)
        {
            exist = true;
            DrawControl.StopDraw(addHistory);
        }

        abstract public void CreateImage(Canvas canvas);

        virtual public void ChangeZoom()
        {

        }

        protected void CallBack(object sender, MouseButtonEventArgs ee)
        {
            virtualElementCallback(ee.GetPosition(DrawControl.TopCanvas), this);
            hit = true;
        }
    }
}
