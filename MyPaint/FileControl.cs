using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MyPaint.History;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;

namespace MyPaint
{
    public class FileControl
    {
        public string path = "";
        public string name;
        public TabItem tabItem;
        public MainControl control;
        public HistoryControl historyControl;
        public Canvas canvas;
        public Canvas topCanvas;
        public Brush primaryColor, secondaryColor;
        public double thickness;
        public Shapes.Shape shape;
        Point posunStart = new Point();
        Point posunStartMys = new Point();
        public Point resolution;
        Point startDraw;
        ToolEnum activeShape;
        public ObservableCollection<Layer> layers = new ObservableCollection<Layer>();
        public Layer selectLayer;
        public ScaleTransform revScale;
        private DrawEnum state = DrawEnum.DRAW;

        private int layerCounter = 1;

        public FileControl(MainControl c, ScaleTransform revScale, TabItem ti, Canvas tC)
        {
            control = c;
            canvas = new Canvas();
            topCanvas = tC;
            historyControl = new HistoryControl(c);
            ResetLayers();
            historyControl.clear();
            this.revScale = revScale;
            tabItem = ti;
        }

        public void AddLayer()
        {
            Layer layer = new Layer(canvas, this) { Name = "layer_" + layerCounter, visible = true };
            layers.Add(layer);
            layerCounter++;
            SetActiveLayer(layers.Count - 1, false);
            historyControl.add(new HistoryLayerAdd(layer));
        }

        public void ResetLayers()
        {
            layerCounter = 1;
            DeleteLayers();
            AddLayer();
        }

        public void DeleteLayers()
        {
            foreach (var l in layers)
            {
                l.Delete();
            }
            layers.Clear();
        }

        public void SetActiveLayer(int i, bool history = true)
        {
            if (i == -1) return;
            foreach (Layer l in layers)
            {
                l.SetActive(false);
            }
            if (activeShape == ToolEnum.SELECT)
            {
                if (selectLayer != null) selectLayer.UnsetSelectable();
                layers[i].SetSelectable();
            }
            if (shape != null)
            {
                shape.ChangeLayer(null);
                if (history) historyControl.add(new HistoryShapeChangeLayer(shape, selectLayer, layers[i]));
            }
            selectLayer = layers[i];
            selectLayer.SetActive(true);
            control.w.setBackgroundBrush(selectLayer.color);
            if (shape != null)
            {
                shape.ChangeLayer(selectLayer);
                shape.StopEdit();
                shape.SetActive();
            }
                    
        }

        public void Activate()
        {
            if (selectLayer != null)
            {
                if (activeShape == ToolEnum.SELECT)
                {
                    selectLayer.UnsetSelectable();
                    selectLayer.SetSelectable();
                }
                else
                {
                    selectLayer.UnsetSelectable();
                }
            }
        }

        public void Deactivate()
        {
            if (selectLayer != null)
            {
                if (activeShape == ToolEnum.SELECT)
                { 
                    selectLayer.UnsetSelectable();
                }
            }
            StopEdit();
        }

        public void SetResolution(Point res, bool back = false, bool history = false)
        {
            if (res.Equals(resolution))
            {
                return;
            }
            if (back)
            {
                control.SetResolution(res.X, res.Y, false);
            }
            if (history)
            {                
                historyControl.add(new HistoryResolution(this, resolution, res));
            }
            resolution = res;

            foreach (var l in layers)
            {
                l.SetResolution(res);
            }
            canvas.Width = topCanvas.Width = res.X;
            canvas.Height = topCanvas.Height = res.Y;
        }

        public void SetShapePrimaryColor(Brush c)
        {
            primaryColor = c;
            if (shape != null) shape.SetPrimaryColor(c, true);
        }

        public void SetShapeSecondaryColor(Brush c)
        {
            secondaryColor = c;
            if (shape != null) shape.SetSecondaryColor(c, true);
        }


        public void SetBackgroundColor(Brush c)
        {
            if (selectLayer != null)
            {
                historyControl.add(new HistoryBackgroundColor(selectLayer, selectLayer.GetBackground(), c));
                selectLayer.SetBackground(c);
            }
        }

        public Brush GetBackgroundColor()
        {
            if (selectLayer != null) return selectLayer.GetBackground();
            return null;
        }

        public Brush GetShapePrimaryColor()
        {
            return primaryColor;
        }

        public Brush GetShapeSecondaryColor()
        {
            return secondaryColor;
        }

        public void SetShapeThickness(double t)
        {
            thickness = t;
            if (shape != null) shape.SetThickness(t, true);
        }

        public double GetShapeThickness()
        {
            return thickness;
        }

        public void ShapeDelete()
        {
            if (shape != null)
            {
                state = DrawEnum.DRAW;
                shape.Delete();
                historyControl.add(new HistoryShapeDelete(shape));
                shape = null;
            }
        }

        public void SetActiveShape(ToolEnum s)
        {
            if (state == DrawEnum.DRAWING)
            {
                shape.Delete();
                state = DrawEnum.DRAW;
            }
            if (selectLayer != null)
            {
                if (s == ToolEnum.SELECT)
                {
                    selectLayer.UnsetSelectable();
                    selectLayer.SetSelectable();
                }
                else
                {
                    selectLayer.UnsetSelectable();
                }
            }
            activeShape = s;
        }

        public void MouseDown(MouseButtonEventArgs e)
        {
            if (selectLayer == null)
            {
                return;
            }
            if (state == DrawEnum.DRAW)
            {
                StartDraw(e);
            }
            else
            {
                if (!shape.HitTest())
                {
                    StopEdit();
                    if (activeShape == ToolEnum.SELECT)
                    {
                        selectLayer.UnsetSelectable();
                        selectLayer.SetSelectable();
                    }
                    StartDraw(e);
                }
            }
        }

        void StartDraw(MouseButtonEventArgs e)
        {
            if (state == DrawEnum.DRAW && activeShape != ToolEnum.SELECT)
            {
                switch (activeShape)
                {
                    case ToolEnum.LINE:
                        shape = new Shapes.Line(this, selectLayer);
                        break;
                    case ToolEnum.POLYLINE:
                        shape = new Shapes.PolyLine(this, selectLayer);
                        break;
                    case ToolEnum.RECT:
                        shape = new Shapes.Rectangle(this, selectLayer);
                        break;
                    case ToolEnum.ELLIPSE:
                        shape = new Shapes.Ellipse(this, selectLayer);
                        break;
                    case ToolEnum.POLYGON:
                        shape = new Shapes.Polygon(this, selectLayer);
                        break;
                    case ToolEnum.TEXT:
                        shape = new Shapes.Text(this, selectLayer);
                        break;
                }
                startDraw = e.GetPosition(canvas);
                shape.DrawMouseDown(e.GetPosition(canvas), e);
            }
        }

        public void StartMoveShape(Point start, Point mys)
        {
            posunStart = start;
            posunStartMys = mys;
            state = DrawEnum.MOVING;
        }

        public void MouseMove(Point e)
        {
            switch (state)
            {
                case DrawEnum.DRAWING:
                    shape.DrawMouseMove(e);
                    break;
                case DrawEnum.EDIT:
                    shape.MoveDrag(e);
                    break;
                case DrawEnum.MOVING:
                    shape.MoveShape(posunStart.X + (e.X - posunStartMys.X), posunStart.Y + (e.Y - posunStartMys.Y));
                    break;
            }
        }

        public void MouseUp(MouseButtonEventArgs e)
        {
            switch (state)
            {
                case DrawEnum.DRAWING:
                    if(!shape.multiDraw && startDraw == e.GetPosition(canvas))
                    {
                        shape.Delete();
                        state = DrawEnum.DRAW;
                        return;
                    }
                    shape.DrawMouseUp(e.GetPosition(canvas), e);
                    break;
                case DrawEnum.EDIT:
                    shape.StopDrag();
                    break;
                case DrawEnum.MOVING:
                    Point start = posunStart;
                    Point stop = shape.GetPosition();
                    if (!start.Equals(stop))
                    {
                        historyControl.add(new HistoryShapeMove(shape, start, stop));
                    }
                    state = DrawEnum.EDIT;
                    break;
            }          
        }

        public void StartDraw()
        {
            state = DrawEnum.DRAWING;
        }

        public void StopDraw()
        {
            historyControl.add(new HistoryShape(shape));
            state = DrawEnum.EDIT;
        }

        public void StartEdit()
        {
            state = DrawEnum.EDIT;
        }

        public void StopEdit()
        {
            if (state == DrawEnum.EDIT)
            {
                shape.StopEdit();
                shape = null;
                state = DrawEnum.DRAW;
            }
        }

        public void SetPrimaryColor(Brush c)
        {
            control.SetWindowPrimaryBrush(c);
        }

        public void SetSecondaryColor(Brush c)
        {
            control.SetWindowSecondaryBrush(c);
        }

        public void SetThickness(double t)
        {
            control.SetWindowThickness(t);
        }

        public void PasteImage(BitmapSource bmi)
        {
            StopEdit();
            ImageBrush brush = new ImageBrush(bmi);
            control.SetResolution(Math.Max(bmi.Width,resolution.X), Math.Max(bmi.Height,resolution.Y));
            Shapes.Shape shape = new Shapes.Image(this, selectLayer, bmi, new System.Windows.Point(0, 0), bmi.Width, bmi.Height);
            historyControl.add(new HistoryShape(shape));
        }

        public void SetPath(string p)
        {
            path = p;
            string name = new Regex("[a-zA-Z0-9]+.[a-zA-Z0-9]+$").Matches(path)[0].ToString();
            SetName(name);
        }

        public void SetName(string name)
        {
            this.name = name;
            tabItem.Header = name;
        }

        public Canvas CreateImage()
        {
            Canvas canvas = new Canvas();
            canvas.Width = resolution.X;
            canvas.Height = resolution.Y;
            foreach (var layer in layers)
            {
                canvas.Children.Add(layer.CreateImage());
            }
            return canvas;
        }

        public void SaveAsFile(string path)
        {
            Regex r = new Regex("\\.[a-zA-Z0-9]+$");
            string suffix = r.Matches(path)[0].ToString().ToLower();
            SetPath(path);
            switch (suffix)
            {
                case ".html":
                    new FileSaver.HTML().Save(this);
                    break;
                case ".jpg":
                    new FileSaver.JPEG().Save(this); ;
                    break;
                case ".bmp":
                    new FileSaver.BMP().Save(this);
                    break;
                case ".png":
                default:
                    new FileSaver.PNG().Save(this);
                    break;
            }
        }

        public void OpenFromFile(string path)
        {
            SetPath(path);
            Regex r = new Regex("\\.[a-zA-Z0-9]+$");
            string suffix = r.Matches(path)[0].ToString().ToLower();
            switch (suffix)
            {
                case ".html":
                    new FileOpener.HTML().Open(this);
                    break;
                case ".jpg":
                    new FileOpener.JPEG().Open(this);
                    break;
                case ".bmp":
                    new FileOpener.BMP().Open(this);
                    break;
                case ".png":
                    new FileOpener.PNG().Open(this);
                    break;
            }
            historyControl.Enable();
        }

        public void PasteShape(jsonDeserialize.Shape s)
        {
            StopEdit();
        }

        public void ChangeZoom()
        {
            if (shape != null) shape.ChangeZoom();
        }
    }
}
