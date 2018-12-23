﻿using MyPaint.History;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        public DrawControl DrawControl { get; private set; }
        Point posunStart = new Point();
        Point posunStartMys = new Point();
        public Point Resolution { get; private set; }
        Point startDraw;
        ToolEnum activeTool;
        public ObservableCollection<Layer> layers = new ObservableCollection<Layer>();
        public ScaleTransform RevScale { get; private set; }
        private DrawEnum state = DrawEnum.DRAW;
        public double Zoom
        {
            set
            {
                zoom = value;
                DrawControl.ChangeZoom();
            }
            get
            {
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

            RevScale = revScale;
            TabItem = ti;
            DrawControl = new DrawControl(c, revScale, tC, this, HistoryControl);
            ResetLayers();
            HistoryControl.Clear();
        }

        public void AddLayer()
        {
            Layer layer = new Layer(Canvas, this, DrawControl) { Name = "layer_" + layerCounter, visible = true };
            layers.Add(layer);
            layerCounter++;
            HistoryControl.Add(new HistoryLayerAdd(layer));
            DrawControl.SetActiveLayer(layers.Count - 1);
        }

        public void ResetLayers()
        {
            layerCounter = 1;
            DeleteLayers();
            AddLayer();
            DrawControl.SelectLayer.Background = Brushes.White;
        }

        public void DeleteLayers()
        {
            layers.Clear();
        }

        public void Activate()
        {
            DrawControl.Activate();
        }

        public void Deactivate()
        {
            DrawControl.Deactivate();
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

        public void SetResolutionByHistoryControl(Point res)
        {
            Control.SetResolution(res.X, res.Y, false);
            Resolution = res;
            foreach (var l in layers)
            {
                l.SetResolution(res);
            }
            Canvas.Width = TopCanvas.Width = res.X;
            Canvas.Height = TopCanvas.Height = res.Y;
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

        public void StartMoveShape(Point start, Point mys)
        {
            posunStart = start;
            posunStartMys = mys;
            state = DrawEnum.MOVING;
        }

        public void StartDraw()
        {
            state = DrawEnum.DRAWING;
        }

        public void StartEdit()
        {
            state = DrawEnum.EDIT;
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

        public void SetPath(string p)
        {
            Path = p;
            SetName(System.IO.Path.GetFileNameWithoutExtension(Path));
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

        public BitmapSource CreateBitmap(double x, double y, double width, double height)
        {
            ContentControl cc = new ContentControl();
            Rect rect = new Rect(-x, -y, width, height);
            cc.Content = CreateImage();
            cc.Arrange(rect);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Default);
            rtb.Render(cc);

            BitmapEncoder encoder = new BmpBitmapEncoder();
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
