using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using System.Timers;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;
using MyPaint.History;
using System.Threading;

namespace MyPaint
{
    public enum MyEnum
    {
        PRIMARY, SECONDARY, BACKGROUND
    }

    public class MainControl
    {
        public FileControl file;
        public MainWindow w;
        ScaleTransform scale, revScale;
        Point resolution;

        ClipboardControl clipboardControl;
        List<TabItem> tabItems;
        TabItem tabAdd;
        Dictionary<TabItem, FileControl> files;
        Brush primaryBrush, secondaryBrush, layerColor;
        ToolEnum tool = ToolEnum.LINE;
        double thickness = 1;
        public MainControl(MainWindow ww)
        {
            w = ww;
            scale = new ScaleTransform(1, 1);
            revScale = new ScaleTransform(1, 1);
            files = new Dictionary<TabItem, FileControl>();
            clipboardControl = new ClipboardControl(this);
            SetTool(ToolEnum.LINE);

            TransformGroup g = new TransformGroup();

            g.Children.Add(scale);
            w.canvas_out.LayoutTransform = g;
            g = new TransformGroup();
            g.Children.Add(revScale);
            w.resolution.LayoutTransform = g;


            tabItems = new List<TabItem>();

            tabAdd = new TabItem();
            tabAdd.Header = "+";
            tabItems.Add(tabAdd);

            w.tabControl.DataContext = tabItems;
            w.tabControl.SelectedIndex = 0;
            SetFileActive(NewFile());          
        }

        private FileControl NewFile()
        {
            TabItem tab = AddTabItem();
            FileControl file = new FileControl(this, revScale, tab, w.topCanvas);
            file.SetName("Bez názvu");
            file.SetResolution(new Point(500, 400));
            file.historyControl.Enable();
            files[tab] = file;
            return file;
        }

        private void SetFileActive(FileControl file)
        {
            if (files.Keys.Contains(file.tabItem))
            {
                if (this.file != null) this.file.Deactivate();
                this.file = file;
                this.file.StopEdit();
                this.file.SetShapePrimaryColor(primaryBrush);
                this.file.SetShapeSecondaryColor(secondaryBrush);
                this.file.SetShapeThickness(thickness);
                this.file.SetActiveShape(tool);
                this.file.historyControl.redraw();
                this.file.Activate();
                SetResolution(this.file.resolution.X, this.file.resolution.Y, false);
                SetPath(this.file.path);
                w.setBackgroundBrush(this.file.GetBackgroundColor());
                w.canvas.Children.Clear();
                w.canvas.Children.Add(this.file.canvas);
                w.tabControl.SelectedItem = file.tabItem;
                w.layers.ItemsSource = this.file.layers;
            }
        }

        private TabItem AddTabItem()
        {
            int count = tabItems.Count;
            TabItem tab = new TabItem();
            tab.Header = string.Format("Tab {0}", count);
            tab.Name = string.Format("tab{0}", count);
            tab.HeaderTemplate = w.tabControl.FindResource("TabHeader") as DataTemplate;
            w.tabControl.DataContext = null;
            tabItems.Insert(count - 1, tab);
            w.tabControl.DataContext = tabItems;
            return tab;
        }

        public void TabControlChange(TabItem tab)
        {           
            if (tab != null && tab.Header != null)
            { 
                if (tab.Equals(tabAdd))
                {
                    SetFileActive(NewFile());
                }
                else
                {
                    SetFileActive(files[tab]);
                }
            }
        }

        public void TabControlDelete(TabItem tab)
        {
            if (tab != null)
            {
                TabItem selectedTab = w.tabControl.SelectedItem as TabItem;
                FileControl file = files[tab];
                file.Deactivate();
                if (file.historyControl.change())
                {
                    if (!SaveDialog()) return;
                }
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

        public void AddLayer()
        {
            if(file != null) file.AddLayer();
        }

        public void LayerChanged(int index)
        {
            if (file != null) file.SetActiveLayer(index);
        }

        public void Back()
        {
            if (file != null) file.StopEdit();
            if (file != null) file.historyControl.back();
        }

        public void Forward()
        {
            if (file != null) file.historyControl.forward();
        }

        public void SetHistory(bool b, bool f)
        {
            w.setHistory(b, f);
        }

        public void SetZoom(double zoom)
        {
            scale.ScaleX = zoom;
            scale.ScaleY = zoom;
            revScale.ScaleX = 1 / zoom;
            revScale.ScaleY = 1 / zoom;
            if (file != null) file.ChangeZoom();
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

        public void NewC()
        {
            SetFileActive(NewFile());
        }

        public Brush GetPrimaryBrush()
        {
            return primaryBrush;
        }

        public void SetPrimaryBrush(Brush c)
        {
            file.SetShapePrimaryColor(c);
            primaryBrush = c;
        }

        public Brush GetSecondaryBrush()
        {
            return secondaryBrush;
        }

        public void SetSecondaryBrush(Brush c)
        {
            file.SetShapeSecondaryColor(c);
            secondaryBrush = c;
        }

        public void SetBackgroundBrush(Brush c)
        {
            file.SetBackgroundColor(c);
            layerColor = c;
        }

        public void SetThickness(double t)
        {
            file.SetShapeThickness(t);
            thickness = t;
        }

        public void SetWindowPrimaryBrush(Brush c)
        {
            w.setPrimaryBrush(c);
            primaryBrush = c;
        }

        public void SetWindowSecondaryBrush(Brush c)
        {
            w.setSecondaryBrush(c);
            secondaryBrush = c;
        }

        public void SetWindowBackgroundBrush(Brush c)
        {
            w.setBackgroundBrush(c);
            layerColor = c;
        }

        public void SetWindowThickness(double t)
        {
            w.setThickness(t);
            thickness = t;
        }

        public void Open()
        {      
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".html";
            dialog.Filter = "Všechny podporované |*.html;*.png;*.jpg;*.bmp|Soubory HTML (*.html)|*.html|Soubory PNG (*.png)|*.png|Soubory JPEG (*.jpg)|*.jpg|Soubory BMP (*.bmp)|*.bmp";
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                TabItem tab = AddTabItem();
                FileControl filee = new FileControl(this, revScale, tab, w.topCanvas);
                files[tab] = filee;
                filee.SetName("Bez názvu");
                string filename = dialog.FileName;
                filee.OpenFromFile(filename);
              
                SetFileActive(files[tab]);
            }
        }

        public void SetPath(string path)
        {
            w.labelPath.Content = path;
        }

        public bool SaveAs()
        { 
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.DefaultExt = ".html";
            dialog.Filter = "Soubory HTML (*.html)|*.html|Soubory PNG (*.png)|*.png|Soubory JPEG (*.jpg)|*.jpg|Soubory BMP (*.bmp)|*.bmp";
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                string filename = dialog.FileName;
                SaveAs(filename);
                file.SetPath(filename);
                SetPath(filename);
            }
            return result.HasValue && result.Value;
        }

        void SaveAs(string path)
        {
            file.StopEdit();
            Regex r = new Regex("\\.[a-zA-Z0-9]+$");
            string suffix = r.Matches(path)[0].ToString().ToLower();
            file.SaveAsFile(path);
            file.historyControl.setNotChange();
            
        }

        public bool Save()
        {
            if (file.path == "")
            {
                return SaveAs();
            }
            else
            {
                SaveAs(file.path);
                return true;
            }
        }

        public bool SaveDialog()
        {
            MessageBoxResult result = MessageBox.Show("Chcete uložit změny?", "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                return Save();
            }
            else if(result == MessageBoxResult.Cancel)
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
            w.button_select.Style = def;
            w.button_line.Style = def;
            w.button_polyline.Style = def;
            w.button_rectangle.Style = def;
            w.button_ellipse.Style = def;
            w.button_polygon.Style = def;
            w.button_text.Style = def;
            switch (s)
            {
                case ToolEnum.SELECT:
                    w.button_select.Style = act;
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
                case ToolEnum.TEXT:
                    w.button_text.Style = act;
                    break;
            }
            if (file != null) file.StopEdit();
            if (file != null) file.SetActiveShape(s);
            tool = s;
        }

        public void MouseDown(MouseButtonEventArgs e)
        {
            if (file != null) file.MouseDown(e); 
        }

        public void MouseMove(MouseEventArgs e)
        {
            if (file != null) file.MouseMove(e.GetPosition(w.canvas));
        }

        public void MouseUp(MouseButtonEventArgs e)
        {
            if (file != null) file.MouseUp(e);
        }

        public void Closed(System.ComponentModel.CancelEventArgs e)
        {
            foreach(var f in files)
            {
                FileControl file = f.Value;
                SetFileActive(f.Value);
                if (file.historyControl.change())
                {
                    if (!SaveDialog())
                    {
                        e.Cancel = true;
                        break;
                    }
                }
            }
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if(file != null) file.ShapeDelete();
            }
            if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.C))
            {
                
            }
            if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.V))
            {
                clipboardControl.Paste();
            }
        }
    }
}

