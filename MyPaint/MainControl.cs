using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyPaint
{
    public class MainControl
    {
        public FileControl file;
        public MainWindow w;
        ScaleTransform scale;
        Point resolution;

        ClipboardControl clipboardControl;
        List<TabItem> tabItems;
        TabItem tabAdd;
        Dictionary<TabItem, FileControl> files;
        Brush primaryBrush, secondaryBrush, layerColor;
        FontFamily font;

        public Canvas TopCanvas { get; private set; }
        public ScaleTransform RevScale { get; private set; }

        double fontSize;
        ToolEnum tool = ToolEnum.LINE;
        double thickness = 1;
        public MainControl(MainWindow ww)
        {
            w = ww;
            TopCanvas = w.topCanvas;
            scale = new ScaleTransform(1, 1);
            RevScale = new ScaleTransform(1, 1);
            files = new Dictionary<TabItem, FileControl>();
            clipboardControl = new ClipboardControl(this);
            SetTool(ToolEnum.LINE);

            TransformGroup g = new TransformGroup();

            g.Children.Add(scale);
            w.canvas_out.LayoutTransform = g;
            g = new TransformGroup();
            g.Children.Add(RevScale);
            w.resolution.LayoutTransform = g;


            tabItems = new List<TabItem>();

            tabAdd = new TabItem();
            tabAdd.Header = "+";
            tabItems.Add(tabAdd);

            w.tabControl.DataContext = tabItems;
            w.tabControl.SelectedIndex = 0;
            NewC();
        }

        private FileControl NewFile()
        {
            TabItem tab = CreateTabItem();
            AddTabItem(tab);
            FileControl file = new FileControl();
            file.Init(this);
            file.TabItem = tab;
            file.SetName("Bez názvu");
            file.SetResolution(new Point(500, 400), false, true);
            file.HistoryControl.Enable();
            files[tab] = file;

            return file;
        }

        public void SetFileActive(FileControl file)
        {
            if (files.Keys.Contains(file.TabItem))
            {
                if (this.file != null) this.file.Deactivate();
                this.file = file;
                this.file.DrawControl.SetTool(tool);
                this.file.HistoryControl.Redraw();
                SetWindowZoom(file.Zoom);
                SetResolution(this.file.Resolution.X, this.file.Resolution.Y, false);
                SetPath(this.file.Path);
                w.SetBackgroundBrush(this.file.DrawControl.GetBackgroundColor());
                w.canvas.Children.Clear();
                w.topCanvas.Children.Clear();
                w.canvas.Children.Add(this.file.Canvas);
                w.tabControl.SelectedItem = file.TabItem;
                w.layers.ItemsSource = this.file.layers;
                w.layers.SelectedItem = file.DrawControl.SelectLayer;
                this.file.Activate();
            }
        }

        private void AddTabItem(TabItem tabItem)
        {
            int count = tabItems.Count;
            tabItems.Insert(count - 1, tabItem);
            w.tabControl.DataContext = tabItems;
        }

        private TabItem CreateTabItem()
        {
            TabItem tab = new TabItem();
            tab.Name = string.Format("tab{0}", tabItems.Count);
            tab.HeaderTemplate = w.tabControl.FindResource("TabHeader") as DataTemplate;
            w.tabControl.DataContext = null;

            return tab;
        }

        public void TabControlChange(TabItem tab)
        {
            if (tab != null && tab.Header != null)
            {
                if (tab.Equals(tabAdd))
                {
                    NewC();
                }
                else
                {
                    SetFileActive(files[tab]);
                }
            }
        }

        public async void TabControlDelete(TabItem tab)
        {
            if (tab != null)
            {
                TabItem selectedTab = w.tabControl.SelectedItem as TabItem;
                FileControl file = files[tab];
                if (file.HistoryControl.Change())
                {
                    if (!await SaveDialog()) return;
                }
                file.Deactivate();
                files.Remove(tab);
                w.tabControl.DataContext = null;
                tabItems.Remove(tab);
                w.tabControl.DataContext = tabItems;
                if (selectedTab == null || selectedTab.Equals(tab))
                {
                    selectedTab = tabItems[0];
                }
                w.tabControl.SelectedItem = selectedTab;
            }
        }

        public void FileClose(FileControl file)
        {
            TabControlDelete(file.TabItem);
        }

        public void AddLayer()
        {
            if (file != null) file.AddLayer();
        }

        public void LayerChanged(int index)
        {
            if (file != null) file.DrawControl.SetActiveLayer(index);
        }

        public void Back()
        {
            if (file != null)
            {
                file.DrawControl.StopEdit();
                file.HistoryControl.Back();
                file.Activate();
            }
        }

        public void Forward()
        {
            if (file != null)
            {
                file.HistoryControl.Forward();
                file.Activate();
            }
        }

        public void SetHistory(bool b, bool f)
        {
            w.SetHistory(b, f);
        }

        public void SetWindowZoom(double zoom)
        {
            w.zoom.Value = zoom * 100;
        }

        public void SetZoom(double zoom)
        {
            scale.ScaleX = zoom;
            scale.ScaleY = zoom;
            RevScale.ScaleX = 1 / zoom;
            RevScale.ScaleY = 1 / zoom;
            if (file != null) file.Zoom = zoom;
        }

        public void SetResolution(double ws, double hs, bool back = true)
        {
            resolution = new Point(ws, hs);
            w.canvas.Width = ws;
            w.canvas.Height = hs;
            w.canvas_out.Width = ws;
            w.canvas_out.Height = hs;
            w.topCanvas.Width = ws;
            w.topCanvas.Height = hs;
            Canvas.SetLeft(w.resolution, ws);
            Canvas.SetTop(w.resolution, hs);
            w.labelResolution.Content = String.Format("{0}x{1}", (int)ws, (int)hs);
            if (back && file != null) file.SetResolution(resolution);
        }

        public void SetResolutionEnd()
        {
            if (file != null) file.SetResolution(resolution, false, true);
        }

        public void NewC()
        {
            FileControl f = NewFile();
            f.ResetLayers();
            f.HistoryControl.Clear();
            SetFileActive(f);
        }

        public Brush GetPrimaryBrush()
        {
            return primaryBrush;
        }

        public void SetPrimaryBrush(Brush c)
        {
            file.DrawControl.SetShapeBrush(BrushEnum.PRIMARY, c);
            primaryBrush = c;
        }

        public Brush GetSecondaryBrush()
        {
            return secondaryBrush;
        }

        public void SetSecondaryBrush(Brush c)
        {
            file.DrawControl.SetShapeBrush(BrushEnum.SECONDARY, c);
            secondaryBrush = c;
        }

        public void SetBackgroundBrush(Brush c)
        {
            file.DrawControl.SetBackgroundColor(c);
            layerColor = c;
        }

        public double GetThickness()
        {
            return thickness;
        }

        public void SetThickness(double t)
        {
            file.DrawControl.SetShapeThickness(t);
            thickness = t;
        }

        public FontFamily GetTextFont()
        {
            return font;
        }

        public void SetTextFont(FontFamily f)
        {
            file.DrawControl.SetTextFont(f);
            font = f;
        }

        public double GetTextFontSize()
        {
            return fontSize;
        }

        public void SetTextFontSize(double s)
        {
            file.DrawControl.SetTextFontSize(s);
            fontSize = s;
        }

        public void SetWindowPrimaryBrush(Brush c)
        {
            w.SetPrimaryBrush(c);
            primaryBrush = c;
        }

        public void SetWindowSecondaryBrush(Brush c)
        {
            w.SetSecondaryBrush(c);
            secondaryBrush = c;
        }

        public void SetWindowBackgroundBrush(Brush c)
        {
            w.SetBackgroundBrush(c);
            layerColor = c;
        }

        public void SetWindowThickness(double t)
        {
            w.SetThickness(t);
            thickness = t;
        }

        public void SetWindowTextFont(FontFamily f)
        {
            w.SetFont(f);
            font = f;
        }

        public void SetWindowTextSize(double s)
        {
            w.SetFontSize(s);
            fontSize = s;
        }

        public void ShowWindowFontPanel(bool t)
        {
            w.ShowFontPanel(t);
        }

        public bool Open()
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".html";
            dialog.Filter = "Všechny podporované |*.html;*.png;*.jpg;*.bmp|Soubory HTML (*.html)|*.html|Soubory PNG (*.png)|*.png|Soubory JPEG (*.jpg)|*.jpg|Soubory BMP (*.bmp)|*.bmp";
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                string filename = dialog.FileName;
                FileControl file = FileOpener.FileOpener.OpenFromFile(filename);
                if (file != null)
                {
                    //w.SetLoading(true);
                    TabItem tab = CreateTabItem();
                    file.Init(this);
                    files[tab] = file;
                    tab.Header = file.Name;
                    file.TabItem = tab;


                    file.InitDraw();
                    AddTabItem(tab);
                    SetFileActive(file);
                }
                //sw.SetLoading(false);
            }
            return true;
        }

        public void SetPath(string path)
        {
            w.labelPath.Content = path;
        }

        public async Task<bool> SaveAs()
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.DefaultExt = ".html";
            dialog.Filter = "Soubory HTML (*.html)|*.html|Soubory PNG (*.png)|*.png|Soubory JPEG (*.jpg)|*.jpg|Soubory BMP (*.bmp)|*.bmp";
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                string filename = dialog.FileName;
                if (await SaveAs(filename))
                {
                    file.SetPath(filename);
                    SetPath(filename);
                    return true;
                }
            }
            return false;
        }

        async Task<bool> SaveAs(string path)
        {
            file.DrawControl.StopEdit();
            w.SetLoading(true);
            Regex r = new Regex("\\.[a-zA-Z0-9]+$");
            string suffix = r.Matches(path)[0].ToString().ToLower();
            file.SetPath(path);
            if (await FileSaver.FileSaver.SaveAsFile(this, file, path))
            {
                file.HistoryControl.SetNotChange();
                w.SetLoading(false);
                return true;
            }
            w.SetLoading(false);
            return false;
        }

        public async Task<bool> Save()
        {
            if (file.Path == null)
            {
                return await SaveAs();
            }
            else
            {
                return await SaveAs(file.Path);
            }
        }

        public async Task<bool> SaveDialog()
        {
            MessageBoxResult result = MessageBox.Show("Chcete uložit změny?", "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                return await Save();
            }
            else if (result == MessageBoxResult.Cancel)
            {
                return false;
            }
            return true;
        }

        public void Exit()
        {
            w.Close();
        }

        public void SetTool(ToolEnum s)
        {
            Style def = w.FindResource("MyButton") as Style;
            Style act = w.FindResource("MyButtonActive") as Style;
            w.button_pencil.Style = def;
            w.button_select_area.Style = def;
            w.button_select.Style = def;
            w.button_line.Style = def;
            w.button_polyline.Style = def;
            w.button_rectangle.Style = def;
            w.button_ellipse.Style = def;
            w.button_polygon.Style = def;
            w.button_q_curve.Style = def;
            w.button_text.Style = def;
            switch (s)
            {
                case ToolEnum.SELECTAREA:
                    w.button_select_area.Style = act;
                    break;
                case ToolEnum.SELECT:
                    w.button_select.Style = act;
                    break;
                case ToolEnum.PENCIL:
                    w.button_pencil.Style = act;
                    break;
                case ToolEnum.LINE:
                    w.button_line.Style = act;
                    break;
                case ToolEnum.POLYLINE:
                    w.button_polyline.Style = act;
                    break;
                case ToolEnum.RECT:
                    w.button_rectangle.Style = act;
                    break;
                case ToolEnum.ELLIPSE:
                    w.button_ellipse.Style = act;
                    break;
                case ToolEnum.POLYGON:
                    w.button_polygon.Style = act;
                    break;
                case ToolEnum.QUADRATICCURVE:
                    w.button_q_curve.Style = act;
                    break;
                case ToolEnum.TEXT:
                    w.button_text.Style = act;
                    break;
            }
            if (file != null) file.DrawControl.StopEdit();
            if (file != null) file.DrawControl.SetTool(s);
            tool = s;
        }

        public void MouseDown(MouseButtonEventArgs e)
        {
            if (file != null) file.DrawControl.MouseDown(e.GetPosition(w.topCanvas), e);
        }

        public void MouseMove(MouseEventArgs e = null)
        {
            Point pos = Mouse.GetPosition(w.topCanvas);
            if (file != null) file.DrawControl.MouseMove(pos);
        }

        public void MouseUp(MouseButtonEventArgs e)
        {
            if (file != null) file.DrawControl.MouseUp(e.GetPosition(w.topCanvas), e);
        }

        public async void Closed(System.ComponentModel.CancelEventArgs e)
        {
            foreach (var f in files)
            {
                FileControl file = f.Value;
                SetFileActive(f.Value);
                if (file.HistoryControl.Change())
                {
                    if (!await SaveDialog())
                    {
                        e.Cancel = true;
                        break;
                    }
                }
            }
        }

        public async void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                Delete();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.C:
                        Copy();
                        break;
                    case Key.X:
                        Cut();
                        break;
                    case Key.V:
                        Paste();
                        break;
                    case Key.S:
                        await Save();
                        break;
                    case Key.O:
                        Open();
                        break;
                    case Key.N:
                        NewC();
                        break;
                }
            }
        }

        public bool ContextMenuShape()
        {
            return file != null && file.DrawControl.ContextMenuShape();
        }

        public void Cut()
        {
            clipboardControl.Cut();
        }

        public void Paste()
        {
            clipboardControl.Paste();
        }

        public void Copy()
        {
            clipboardControl.Copy();
        }

        public void Delete()
        {
            if (file != null) file.DrawControl.ShapeDelete();
        }

        public void SetShapeTop()
        {
            if (file != null) file.DrawControl.SetShapePosition(-1);
        }

        public void SetShapeBottom()
        {
            if (file != null) file.DrawControl.SetShapePosition(0);
        }

        public void AdjustZoom(double x, double y)
        {
            double a = w.canvas_outer.ActualWidth / x;
            double b = w.canvas_outer.ActualHeight / y;
            double zoom;
            if (a < b)
            {
                zoom = a;
            }
            else
            {
                zoom = b;
            }
            if (zoom < 1)
            {
                zoom = Math.Floor(zoom * 10) / 10;
                if (zoom * 100 < w.zoom.Value)
                {
                    SetWindowZoom(zoom);
                }
            }
        }

    }
}

