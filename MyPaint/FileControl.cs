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
        MyEnum activeShape;
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
            resetLayers();
            historyControl.clear();
            this.revScale = revScale;
            tabItem = ti;
            lockDraw();
        }

        public void addLayer()
        {
            Layer layer = new Layer(canvas, this) { Name = "layer_" + layerCounter, visible = true };
            layers.Add(layer);
            layerCounter++;
            setActiveLayer(layers.Count - 1, false);
            lockDraw();
            historyControl.add(new HistoryLayerAdd(layer));
        }

        public void resetLayers()
        {
            layerCounter = 1;
            deleteLayers();
            addLayer();
        }

        public void deleteLayers()
        {
            foreach (var l in layers)
            {
                l.Delete();
            }
            layers.Clear();
        }

        public void setActiveLayer(int i, bool history = true)
        {
            if (i == -1) return;
            foreach (Layer l in layers)
            {
                l.SetActive(false);
            }
            if (activeShape == MyEnum.SELECT)
            {
                if (shape != null)
                {
                    shape.changeLayer(null);
                    if (history) historyControl.add(new HistoryShapeChangeLayer(shape, selectLayer, layers[i]));
                }
                if (selectLayer != null) selectLayer.UnsetSelectable();
                layers[i].SetSelectable();
            }
            selectLayer = layers[i];
            selectLayer.SetActive(true);
            control.w.setBackgroundBrush(selectLayer.color);
            if (shape != null) shape.changeLayer(selectLayer);
        }

        public void Activate()
        {
            if (selectLayer != null)
            {
                if (activeShape == MyEnum.SELECT)
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
                if (activeShape == MyEnum.SELECT)
                { 
                    selectLayer.UnsetSelectable();
                }
            }
            stopEdit();
        }

        public void setResolution(Point res, bool back = false, bool history = false)
        {
            if (res.Equals(resolution))
            {
                return;
            }
            if (back)
            {
                control.setResolution(res.X, res.Y, false);
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

        public void clear()
        {
            clearCanvas();
        }

        public void clearCanvas()
        {
            stopEdit();
            resetLayers();
        }

        public void setShapePrimaryColor(Brush c)
        {
            primaryColor = c;
            if (shape != null) shape.setPrimaryColor(c, true);
        }

        public void setShapeSecondaryColor(Brush c)
        {
            secondaryColor = c;
            if (shape != null) shape.setSecondaryColor(c, true);
        }


        public void setBackgroundColor(Brush c)
        {
            if (selectLayer != null)
            {
                historyControl.add(new HistoryBackgroundColor(selectLayer, selectLayer.GetBackground(), c));
                selectLayer.SetBackground(c);
            }
        }

        public Brush getBackgroundColor()
        {
            if (selectLayer != null) return selectLayer.GetBackground();
            return null;
        }

        public Brush getShapePrimaryColor()
        {
            return primaryColor;
        }

        public Brush getShapeSecondaryColor()
        {
            return secondaryColor;
        }

        public void SetShapeThickness(double t)
        {
            thickness = t;
            if (shape != null) shape.setThickness(t, true);
        }

        public double getShapeThickness()
        {
            return thickness;
        }

        public void shapeDelete()
        {
            if (shape != null)
            {
                state = DrawEnum.DRAW;
                shape.delete();
                historyControl.add(new HistoryShapeDelete(shape));
                shape = null;
            }
        }

        public void setActiveShape(MyEnum s)
        {
            if (selectLayer != null)
            {
                if (s == MyEnum.SELECT)
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

        public void mouseDown(MouseButtonEventArgs e)
        {
            if (selectLayer == null)
            {
                return;
            }
            if (state == DrawEnum.DRAW)
            {
                startDraw(e);
            }
            else
            {
                if (!shape.hitTest())
                {
                    stopEdit();
                    if (activeShape == MyEnum.SELECT)
                    {
                        selectLayer.UnsetSelectable();
                        selectLayer.SetSelectable();
                    }
                    startDraw(e);
                }
            }
        }

        void startDraw(MouseButtonEventArgs e)
        {
            if (state == DrawEnum.DRAW && activeShape != MyEnum.SELECT)
            {
                switch (activeShape)
                {
                    case MyEnum.LINE:
                        shape = new Shapes.Line(this, selectLayer);
                        break;
                    case MyEnum.RECT:
                        shape = new Shapes.Rectangle(this, selectLayer);
                        break;
                    case MyEnum.ELLIPSE:
                        shape = new Shapes.Ellipse(this, selectLayer);
                        break;
                    case MyEnum.POLYGON:
                        shape = new Shapes.Polygon(this, selectLayer);
                        break;
                }
                historyControl.add(new HistoryShape(shape));
                shape.drawMouseDown(e.GetPosition(canvas), e);
            }
        }

        public void startMoveShape(Point start, Point mys)
        {
            posunStart = start;
            posunStartMys = mys;
            state = DrawEnum.MOVING;
        }

        public void mouseMove(Point e)
        {
            switch (state)
            {
                case DrawEnum.DRAWING:
                    shape.drawMouseMove(e);
                    break;
                case DrawEnum.EDIT:
                    shape.moveDrag(e);
                    break;
                case DrawEnum.MOVING:
                    shape.moveShape(posunStart.X + (e.X - posunStartMys.X), posunStart.Y + (e.Y - posunStartMys.Y));
                    break;
            }
        }

        public void mouseUp(MouseButtonEventArgs e)
        {

            switch (state)
            {
                case DrawEnum.DRAWING:
                    shape.drawMouseUp(e.GetPosition(canvas), e);
                    break;
                case DrawEnum.EDIT:
                    shape.stopDrag();
                    break;
                case DrawEnum.MOVING:
                    Point start = posunStart;
                    Point stop = shape.getPosition();
                    if (!start.Equals(stop))
                    {
                        historyControl.add(new HistoryShapeMove(shape, start, stop));
                    }
                    state = DrawEnum.EDIT;
                    break;
            }          
        }

        public void startDraw()
        {
            state = DrawEnum.DRAWING;
        }

        public void stopDraw()
        {
            state = DrawEnum.EDIT;
        }

        public void startEdit()
        {
            state = DrawEnum.EDIT;
        }

        public void stopEdit()
        {
            if (state == DrawEnum.EDIT)
            {
                shape.stopEdit();
                shape = null;
                lockDraw();
                state = DrawEnum.DRAW;
            }
        }

        public void lockDraw()
        {
            
        }

        public void setPrimaryColor(Brush c)
        {
            control.setWindowPrimaryBrush(c);
        }

        public void setSecondaryColor(Brush c)
        {
            control.setWindowSecondaryBrush(c);
        }

        public void setThickness(double t)
        {
            control.setWindowThickness(t);
        }

        public void pasteImage(BitmapSource bmi)
        {
            stopEdit();
            ImageBrush brush = new ImageBrush(bmi);
            control.setResolution(bmi.Width, bmi.Height);
            Shapes.Shape shape = new Shapes.Image(this, selectLayer, bmi, new System.Windows.Point(0, 0), bmi.Width, bmi.Height);
            historyControl.add(new HistoryShape(shape));
        }

        public void setPath(string p)
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

        public Canvas create()
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
            setPath(path);
            switch (suffix)
            {
                case ".html":
                    new FileSaver.HTML().save(this);
                    break;
                case ".jpg":
                    new FileSaver.JPEG().save(this); ;
                    break;
                case ".bmp":
                    new FileSaver.BMP().save(this);
                    break;
                case ".png":
                default:
                    new FileSaver.PNG().save(this);
                    break;
            }
        }

        public void OpenFromFile(string path)
        {
            setPath(path);
            Regex r = new Regex("\\.[a-zA-Z0-9]+$");
            string suffix = r.Matches(path)[0].ToString().ToLower();
            switch (suffix)
            {
                case ".html":
                    new FileOpener.HTML().open(this);
                    break;
                case ".jpg":
                    new FileOpener.JPEG().open(this);
                    break;
                case ".bmp":
                    new FileOpener.BMP().open(this);
                    break;
                case ".png":
                    new FileOpener.PNG().open(this);
                    break;
            }
            historyControl.Enable();
        }

        public void PasteShape(jsonDeserialize.Shape s)
        {
            stopEdit();

        }
    }
}
