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
using System.IO;

namespace MyPaint
{
    public class FileControl
    {
        public string Path { get; private set; }
        public string Name { get; private set; }
        public TabItem TabItem { get; private set; }
        public MainControl Control { get; private set; }
        public HistoryControl HistoryControl { get; private set; }
        public Canvas Canvas { get; private set; }
        public Canvas TopCanvas { get; private set; }
        public Shapes.Shape Shape { get; private set; }
        Point posunStart = new Point();
        Point posunStartMys = new Point();
        public Point Resolution { get; private set; }
        Point startDraw;
        ToolEnum activeTool;
        public ObservableCollection<Layer> layers = new ObservableCollection<Layer>();
        public Layer SelectLayer { get; private set; }
        public ScaleTransform RevScale { get; private set; }
        private DrawEnum state = DrawEnum.DRAW;
        public double Zoom {
            set {
                zoom = value;
                ChangeZoom();
            }
            get {
                return zoom;
            }
        }
        private double zoom = 1;

        private int layerCounter = 1;

        public FileControl(MainControl c, ScaleTransform revScale, TabItem ti, Canvas tC)
        {
            Control = c;
            Canvas = new Canvas();
            TopCanvas = tC;
            HistoryControl = new HistoryControl(c);
            ResetLayers();
            HistoryControl.Clear();
            RevScale = revScale;
            TabItem = ti;
        }

        public void AddLayer()
        {
            Layer layer = new Layer(Canvas, this) { Name = "layer_" + layerCounter, visible = true };
            layers.Add(layer);
            layerCounter++;
            HistoryControl.Add(new HistoryLayerAdd(layer));
            SetActiveLayer(layers.Count - 1); 
        }

        public void ResetLayers()
        {
            layerCounter = 1;
            DeleteLayers();
            AddLayer();
        }

        public void DeleteLayers()
        {
            layers.Clear();
        }

        public void SetActiveLayer(int i)
        {
            if (i == -1) return;
            if (layers[i].Equals(SelectLayer)) return;
            Layer old = SelectLayer;
            SelectLayer = layers[i];
            if (activeTool == ToolEnum.SELECT)
            {
                if (old != null) old.UnsetSelectable();
                SelectLayer.SetSelectable();
            }     
            Control.w.layers.SelectedIndex = i;
            Control.w.SetBackgroundBrush(SelectLayer.Background);
            if (Shape != null)
            {
                Shape.ChangeLayer(SelectLayer, true);
                Shape.StopEdit();
                Shape.SetActive();
            }         
        }

        public void Activate()
        {
            if (SelectLayer != null)
            {
                if (activeTool == ToolEnum.SELECT)
                {
                    SelectLayer.SetSelectable();
                }
                else
                {
                    SelectLayer.UnsetSelectable();
                }
            }
        }

        public void Deactivate()
        {
            if (SelectLayer != null)
            {
                if (activeTool == ToolEnum.SELECT)
                { 
                    SelectLayer.UnsetSelectable();
                }
            }
            if(state == DrawEnum.DRAWING)
            {
                Shape.Delete();
                Shape = null;
                state = DrawEnum.DRAW;
            }
            StopEdit();
        }

        public void SetResolution(Point res, bool back = false, bool history = false)
        {
            if (back)
            {
                Control.SetResolution(res.X, res.Y, false);
            }
            if (history)
            {                
                HistoryControl.Add(new HistoryResolution(this, Resolution, res));
                Resolution = res;
            }
            foreach (var l in layers)
            {
                l.SetResolution(res);
            }
            Canvas.Width = TopCanvas.Width = res.X;
            Canvas.Height = TopCanvas.Height = res.Y;
        }

        public void SetShapePrimaryColor(Brush c)
        {
            if (Shape != null) Shape.SetPrimaryColor(c, true);
        }

        public void SetShapeSecondaryColor(Brush c)
        {
            if (Shape != null) Shape.SetSecondaryColor(c, true);
        }

        public void SetShapeThickness(double t)
        {
            if (Shape != null) Shape.SetThickness(t, true);
        }

        public void SetTextFont(FontFamily f)
        {
            if (Shape != null && Shape is Shapes.Text)
            {
                ((Shapes.Text)Shape).SetFont(f, true);
            }
        }

        public void SetTextFontSize(double s)
        {
            if (Shape != null && Shape is Shapes.Text)
            {
                ((Shapes.Text)Shape).SetFontSize(s, true);
            }
        }

        public void SetShapePosition(int pos)
        {
            if (Shape != null && !(Shape is Shapes.Area)) Shape.SetPosition(pos, true);
        }

        public void SetBackgroundColor(Brush c)
        {
            if (SelectLayer != null)
            {
                HistoryControl.Add(new HistoryBackgroundColor(SelectLayer, SelectLayer.Background, c));
                SelectLayer.Background = c;
            }
        }

        public Brush GetBackgroundColor()
        {
            if (SelectLayer != null) return SelectLayer.Background;
            return null;
        }

        public Brush GetShapePrimaryColor()
        {
            return Control.GetPrimaryBrush();
        }

        public Brush GetShapeSecondaryColor()
        {
            return Control.GetSecondaryBrush();
        }

        public double GetShapeThickness()
        {
            return Control.GetThickness();
        }

        public FontFamily GetTextFont()
        {
            return Control.GetTextFont();
        }

        public double GetTextFontSize()
        {
            return Control.GetTextFontSize();
        }

        public void ShowWindowFontPanel(bool t)
        {
            Control.ShowWindowFontPanel(t);
        }

        public void ShapeDelete()
        {
            if (Shape != null)
            {
                state = DrawEnum.DRAW;
                int pos = Shape.Delete();
                HistoryControl.Add(new HistoryShapeDelete(Shape, pos));
                Shape = null;
            }
        }

        public void SetTool(ToolEnum s)
        {
            if (state == DrawEnum.DRAWING)
            {
                Shape.Delete();
                state = DrawEnum.DRAW;
            }
            if (SelectLayer != null)
            {
                if (s == ToolEnum.SELECT)
                {
                    SelectLayer.UnsetSelectable();
                    SelectLayer.SetSelectable();
                }
                else
                {
                    SelectLayer.UnsetSelectable();
                }
            }
            activeTool = s;
        }

        public void MouseDown(MouseButtonEventArgs e)
        {
            if (SelectLayer == null)
            {
                return;
            }
            if (state == DrawEnum.DRAW)
            {
                StartDraw(e);
            }
            else
            {
                if (!Shape.HitTest())
                {
                    StopEdit();
                    StartDraw(e);
                }
            }
        }

        void StartDraw(MouseButtonEventArgs e)
        {
            if (state == DrawEnum.DRAW && activeTool != ToolEnum.SELECT)
            {
                switch (activeTool)
                {
                    case ToolEnum.LINE:
                        Shape = new Shapes.Line(this, SelectLayer);
                        break;
                    case ToolEnum.POLYLINE:
                        Shape = new Shapes.PolyLine(this, SelectLayer);
                        break;
                    case ToolEnum.RECT:
                        Shape = new Shapes.Rectangle(this, SelectLayer);
                        break;
                    case ToolEnum.ELLIPSE:
                        Shape = new Shapes.Ellipse(this, SelectLayer);
                        break;
                    case ToolEnum.POLYGON:
                        Shape = new Shapes.Polygon(this, SelectLayer);
                        break;
                    case ToolEnum.QUADRATICCURVE:
                        Shape = new Shapes.QuadraticCurve(this, SelectLayer);
                        break;
                    case ToolEnum.TEXT:
                        Shape = new Shapes.Text(this, SelectLayer);
                        break;
                    case ToolEnum.SELECTAREA:
                        Shape = new Shapes.Area(this, SelectLayer);
                        break;
                }
                startDraw = e.GetPosition(Canvas);
                Shape.DrawMouseDown(e.GetPosition(Canvas), e);
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
                    Shape.DrawMouseMove(e);
                    break;
                case DrawEnum.EDIT:
                    Shape.MoveDrag(e);
                    break;
                case DrawEnum.MOVING:
                    Shape.MoveShape(posunStart.X + (e.X - posunStartMys.X), posunStart.Y + (e.Y - posunStartMys.Y));
                    break;
            }
        }

        public void MouseUp(MouseButtonEventArgs e)
        {
            switch (state)
            {
                case DrawEnum.DRAWING:
                    if(!Shape.MultiDraw && startDraw == e.GetPosition(Canvas))
                    {
                        state = DrawEnum.DRAW;
                        Shape.Delete();
                        Shape = null;
                        return;
                    }
                    Shape.DrawMouseUp(e.GetPosition(Canvas), e);
                    break;
                case DrawEnum.EDIT:
                    Shape.StopDrag();
                    break;
                case DrawEnum.MOVING:
                    Point start = posunStart;
                    Point stop = Shape.GetPosition();
                    if (!start.Equals(stop) && !(Shape is Shapes.Area))
                    {
                        HistoryControl.Add(new HistoryShapeMove(Shape, start, stop));
                    }
                    state = DrawEnum.EDIT;
                    break;
            }          
        }

        public void StartDraw()
        {
            state = DrawEnum.DRAWING;
        }

        public void StopDraw(bool addHistory = true)
        {
            if(addHistory) HistoryControl.Add(new HistoryShape(Shape));
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
                ShowWindowFontPanel(false);
                Shape.StopEdit();
                Shape = null;
                state = DrawEnum.DRAW;
                if (activeTool == ToolEnum.SELECT)
                {
                    SelectLayer.SetSelectable();
                }
            }
        }

        public void SetShapeActive(Shapes.Shape shape)
        {
            Shape = shape;
            shape.SetActive();
        }

        public void SetPrimaryColor(Brush c)
        {
            Control.SetWindowPrimaryBrush(c);
        }

        public void SetSecondaryColor(Brush c)
        {
            Control.SetWindowSecondaryBrush(c);
        }

        public void SetThickness(double t)
        {
            Control.SetWindowThickness(t);
        }

        public void SetFont(FontFamily f)
        {
            Control.SetWindowTextFont(f);
        }

        public void SetFontSize(double s)
        {
            Control.SetWindowTextSize(s);
        }

        public void PasteImage(BitmapSource bmi)
        {
            StopEdit();
            if (SelectLayer != null)
            {
                ImageBrush brush = new ImageBrush(bmi);
                
                SetResolution(new Point(Math.Max(bmi.Width, Resolution.X), Math.Max(bmi.Height, Resolution.Y)), true, true);
                Shape = new Shapes.Image(this, SelectLayer, bmi, new System.Windows.Point(0, 0), bmi.Width, bmi.Height);
                Shape.SetActive();
                HistoryControl.Add(new HistoryShape(Shape));
                Control.AdjustZoom(bmi.Width, bmi.Height);
            }
        }

        public void PasteShape(Deserializer.Shape s)
        {
            StopEdit();
            if (SelectLayer != null)
            {
                Shape = s.Create(this, SelectLayer);
                Shape.SetActive();
                HistoryControl.Add(new HistoryShape(Shape));
            }
        }

        public void SetPath(string p)
        {
            Path = p;
            string name = new Regex("[a-zA-Z0-9]+.[a-zA-Z0-9]+$").Matches(Path)[0].ToString();
            SetName(name);
        }

        public void SetName(string name)
        {
            this.Name = name;
            TabItem.Header = name;
        }

        public Canvas CreateImage()
        {
            Canvas canvas = new Canvas();
            canvas.Width = Resolution.X;
            canvas.Height = Resolution.Y;
            foreach (var layer in layers)
            {
                canvas.Children.Add(layer.CreateImage());
            }
            return canvas;
        }

        private void ChangeZoom()
        {
            if (Shape != null && ShapeDrawed()) Shape.ChangeZoom();
        }

        public bool ShapeDrawed()
        {
            return state == DrawEnum.EDIT || state == DrawEnum.MOVING;
        }

        public bool ContextMenuShape()
        {
            return ShapeDrawed() && Shape.HitTest();
        }

        public void UnselectLayer()
        {
            SelectLayer = null;
        }

        public BitmapSource CreateBitmap(double x, double y, double width, double height)
        {
            ContentControl cc = new ContentControl();
            Rect rect = new Rect(-x, -y, width, height);
            cc.Content = CreateImage();
            cc.Arrange(rect);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Default);
            rtb.Render(cc);

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            byte[] f = null;
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                f = stream.ToArray();
            }
            MemoryStream ms = new MemoryStream(f, 0, f.Length);
            BitmapImage bmi = new BitmapImage();
            bmi.BeginInit();
            bmi.StreamSource = ms;
            bmi.EndInit();
            return bmi;
        }
    }
}
