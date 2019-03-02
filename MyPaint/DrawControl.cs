using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MyPaint.History;

namespace MyPaint
{
    public class DrawControl
    {
        public MainControl Control { get; private set; }
        public HistoryControl HistoryControl { get; private set; }

        public Canvas TopCanvas { get; private set; }
        public Shapes.Shape Shape { get; private set; }
        Point posunStart = new Point();
        Point posunStartMys = new Point();

        Point startDraw;
        ToolEnum activeTool;
        public Layer SelectLayer { get; private set; }
        public ScaleTransform RevScale { get; private set; }
        private DrawEnum state = DrawEnum.DRAW;
        FileControl file;
        public double Zoom
        {
            set
            {
                zoom = value;
                ChangeZoom();
            }
            get
            {
                return zoom;
            }
        }
        private double zoom = 1;

        public DrawControl()
        {

        }

        public void Init(MainControl c, ScaleTransform revScale, Canvas tC, FileControl f, HistoryControl historyControl)
        {
            Control = c;
            TopCanvas = tC;
            HistoryControl = historyControl;
            RevScale = revScale;
            file = f;
        }

        public void SetActiveLayer(int i)
        {
            if (i == -1) return;
            if (file.layers[i].Equals(SelectLayer)) return;
            Layer old = SelectLayer;
            SelectLayer = file.layers[i];
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
            if (state == DrawEnum.DRAWING)
            {
                Shape.Delete();
                Shape = null;
                state = DrawEnum.DRAW;
            }
            StopEdit();
        }

        public void SetShapeBrush(BrushEnum brushEnum, Brush brush)
        {
            if (Shape != null)
            {
                if (Shape.SetBrush(brushEnum, brush))
                {
                    HistoryControl.Add(new HistoryShapeBrush(Shape, brushEnum, Shape.GetBrush(brushEnum), brush));
                }
            }
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

        public void MouseDown(Point e, MouseButtonEventArgs ee)
        {
            if (SelectLayer == null)
            {
                return;
            }
            if (state == DrawEnum.DRAW)
            {
                StartDraw(e, ee);
            }
            else
            {
                if (!Shape.HitTest())
                {
                    StopEdit();
                    StartDraw(e, ee);
                }
            }
        }

        void StartDraw(Point e, MouseButtonEventArgs ee)
        {
            if (state == DrawEnum.DRAW && activeTool != ToolEnum.SELECT)
            {
                switch (activeTool)
                {
                    case ToolEnum.PENCIL:
                        Shape = new Shapes.Pencil(this, SelectLayer);
                        break;
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
                startDraw = e;
                Shape.DrawMouseDown(e, ee);
            }
        }

        public void StartMoveShape(Point start, Point mys)
        {
            posunStart = start;
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
                    Shape.MoveShape(posunStart + (e - posunStartMys));
                    break;
            }
        }

        public void MouseUp(Point e, MouseButtonEventArgs ee)
        {
            switch (state)
            {
                case DrawEnum.DRAWING:
                    if (!Shape.MultiDraw && startDraw == e)
                    {
                        state = DrawEnum.DRAW;
                        Shape.Delete();
                        Shape = null;
                        return;
                    }
                    Shape.DrawMouseUp(e, ee);
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
            if (addHistory) HistoryControl.Add(new HistoryShape(Shape));
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

                file.SetResolution(new Point(Math.Max(bmi.Width, file.Resolution.X), Math.Max(bmi.Height, file.Resolution.Y)), true, true);
                Shape = new Shapes.Image(this, SelectLayer, bmi, new System.Windows.Point(0, 0), bmi.Width, bmi.Height);
                Shape.InitDraw();
                Shape.SetActive();
                HistoryControl.Add(new HistoryShape(Shape));
                Control.AdjustZoom(bmi.Width, bmi.Height);
            }
        }

        public void PasteShape(Serializer.Shape s)
        {
            StopEdit();
            if (SelectLayer != null)
            {
                Shape = s.Create(this, SelectLayer);
                Shape.InitDraw();
                Shape.SetActive();
                HistoryControl.Add(new HistoryShape(Shape));
            }
        }

        public void ChangeZoom()
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

        public Canvas CreateImage()
        {
            Canvas canvas = new Canvas();
            canvas.Width = file.Resolution.X;
            canvas.Height = file.Resolution.Y;
            foreach (var layer in file.layers)
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
