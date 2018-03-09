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
        SELECT, LINE, RECT, ELLIPSE, POLYGON, PRIMARY, SECONDARY, BACKGROUND
    }

    public class MainControl
    {
        public DrawControl drawControl = null;
        public MyEnum activeColor;
        string path = "";
        bool change = false;
        public MainWindow w;
        ScaleTransform scale, revScale;
        public Point resolution;
        public bool resolutionDrag = false;
        Brush cblock = null;
        ClipboardControl clipboardControl;
        private List<TabItem> tabItems;
        private TabItem tabAdd;
        private Dictionary<TabItem, DrawControl> files;
        public ToolControl toolControl;
        Brush primaryColor, secondaryColor, layerColor;

        public MainControl(MainWindow ww)
        {
            w = ww;
            scale = new ScaleTransform(1, 1);
            revScale = new ScaleTransform(1, 1);
            files = new Dictionary<TabItem, DrawControl>();
            toolControl = new ToolControl();
            clipboardControl = new ClipboardControl(this);
            setTool(MyEnum.LINE);
            setActiveColor(MyEnum.PRIMARY);
            setColor(Brushes.Black, false);
            setThickness(1, false);
            setResolution(500, 400);
            //w.Dispatcher.BeginInvoke()
            TransformGroup g = new TransformGroup();

            g.Children.Add(scale);
            w.canvas_out.LayoutTransform = g;
            g = new TransformGroup();
            g.Children.Add(revScale);
            w.resolution.LayoutTransform = g;


            tabItems = new List<TabItem>();

            // add a tabItem with + in header 
            tabAdd = new TabItem();
            tabAdd.Header = "+";

            tabItems.Add(tabAdd);


            // bind tab control
            w.tabControl.DataContext = tabItems;

            w.tabControl.SelectedIndex = 0;
            setFileActive(newFile());
            
        }

        private TabItem newFile()
        {
            TabItem tab = AddTabItem();
            DrawControl file = new DrawControl(this, revScale, tab);
            file.setName("Bez názvu");
            file.setResolution(new Point(500, 400), true);
            file.historyControl.Enable();
            files[tab] = file;
            return tab;
        }

        private void setFileActive(TabItem tab)
        {
            if (files.Keys.Contains(tab))
            {
                DrawControl file = files[tab];
                drawControl = file;
                drawControl.stopDraw();
                drawControl.setShapePrimaryColor(toolControl.primaryColor);
                drawControl.setShapeSecondaryColor(toolControl.secondaryColor);
                drawControl.setShapeThickness(toolControl.thickness);
                drawControl.setActiveShape(toolControl.tool);
                drawControl.historyControl.redraw();
                setResolution(drawControl.resolution.X, drawControl.resolution.Y, false);
                setPath(drawControl.path);
                setBackgroundColor(drawControl.getBackgroundColor(), false);
                w.canvas.Children.Clear();
                w.canvas.Children.Add(drawControl.canvas);
                w.tabControl.SelectedItem = tab;
                w.layers.ItemsSource = drawControl.layers;
            }
        }

        private TabItem AddTabItem()
        {
            int count = tabItems.Count;

            // create new tab item
            TabItem tab = new TabItem();
            tab.Header = string.Format("Tab {0}", count);
            tab.Name = string.Format("tab{0}", count);
            tab.HeaderTemplate = w.tabControl.FindResource("TabHeader") as DataTemplate;
            w.tabControl.DataContext = null;
            tabItems.Insert(count - 1, tab);
            w.tabControl.DataContext = tabItems;
            return tab;
        }

        public void tabControlChange(TabItem tab)
        {           
            if (tab != null && tab.Header != null)
            {
                
                if (tab.Equals(tabAdd))
                {
                    setFileActive(newFile());
                }
                else
                {
                    setFileActive(tab);
                }
            }
        }

        public void tabControlDelete(TabItem tab)
        {
            if (tab != null)
            {
                // get selected tab
                TabItem selectedTab = w.tabControl.SelectedItem as TabItem;
                DrawControl file = files[tab];
                stopDraw();
                if (file.historyControl.change())
                {
                    if (!saveDialog()) return;
                }
                files.Remove(tab);
                // clear tab control binding
                w.tabControl.DataContext = null;

                tabItems.Remove(tab);

                // bind tab control
                w.tabControl.DataContext = tabItems;

                // select previously selected tab. if that is removed then select first tab
                if (selectedTab == null || selectedTab.Equals(tab))
                {
                    selectedTab = tabItems[0];
                }
                w.tabControl.SelectedItem = selectedTab;
            }
        }

        public void addLayer()
        {
            if(drawControl != null) drawControl.addLayer();
        }

        public void layerChanged()
        {
            if (drawControl != null) drawControl.setActiveLayer(w.layers.SelectedIndex);
        }

        public void back()
        {
            if (drawControl != null) drawControl.stopDraw();
            if (drawControl != null) drawControl.historyControl.back();
        }

        public void forward()
        {
            if (drawControl != null) drawControl.historyControl.forward();
        }

        public void setHistory(bool b, bool f)
        {
            w.setHistory(b, f);
        }

        public void setZoom(double zoom)
        {
            scale.ScaleX = zoom;
            scale.ScaleY = zoom;
            revScale.ScaleX = 1 / zoom;
            revScale.ScaleY = 1 / zoom;
        }

        public void setResolution(double ws, double hs, bool back = true)
        {
            resolution = new Point(ws, hs);
            w.canvas.Width = ws;
            w.canvas.Height = hs;
            w.canvas_out.Width = ws;
            w.canvas_out.Height = hs;
            Canvas.SetLeft(w.resolution, ws);
            Canvas.SetTop(w.resolution, hs);
            w.labelResolution.Content = String.Format("{0}x{1}", (int)ws, (int)hs);
            if (back && drawControl != null) drawControl.setResolution(resolution);
        }

        public void newC()
        {
            setFileActive(newFile());
            
            //if (!saveDialog()) return;

            //change = false;
            //setResolution(500, 400);
            //drawControl.clear();
            //drawControl.historyControl.clear();
        }

        public void setColorCB(Brush c)
        {
            if(drawControl == null) return;
            if (cblock == null || !cblock.Equals(c)) {
                cblock = c;
                switch (activeColor)
                {
                    case MyEnum.PRIMARY:
                        setPrimaryColor(c, drawControl != null);
                        break;
                    case MyEnum.SECONDARY:
                        setSecondaryColor(c, drawControl != null);
                        break;
                    case MyEnum.BACKGROUND:
                        setBackgroundColor(c, drawControl != null);
                        break;
                }
            }
        }

        public void setColor(Brush c, bool back = true)
        {
            cblock = c;
            switch (activeColor)
            {
                case MyEnum.PRIMARY:
                    setPrimaryColor(c, back);
                    break;
                case MyEnum.SECONDARY:
                    setSecondaryColor(c, back);
                    break;
                case MyEnum.BACKGROUND:
                    setBackgroundColor(c, back);
                    break;
            }
        }

        public void setPrimaryColor(Brush c, bool back = true)
        {
            if (activeColor == MyEnum.PRIMARY) setCB(c);
            if (back) drawControl.setShapePrimaryColor(c);
            w.primaryColor.Fill = c;
            primaryColor = c;
            toolControl.primaryColor = c;
        }

        public void setSecondaryColor(Brush c, bool back = true)
        {
            if (activeColor == MyEnum.SECONDARY) setCB(c);
            if (back) drawControl.setShapeSecondaryColor(c);
            w.secondaryColor.Fill = c;
            secondaryColor = c;
            toolControl.secondaryColor = c;
        }

        public void setBackgroundColor(Brush c, bool back = true)
        {
            if (activeColor == MyEnum.BACKGROUND) setCB(c);
            if (back) drawControl.setBackgroundColor(c);
            w.backgroundColor.Fill = c;
            layerColor = c;
        }

        void setCB(Brush color)
        {
            if (color != null && color is SolidColorBrush)
                color.Freeze();
            w.CB.Brush = color;
        }

        public Brush getColor()
        {
            switch (activeColor)
            {
                case MyEnum.PRIMARY:
                    return primaryColor;
                case MyEnum.SECONDARY:
                    return secondaryColor;
                case MyEnum.BACKGROUND:
                    return layerColor;
            }
            return null;
        }

        public void setThickness(double t, bool back = true)
        {
            if (back) drawControl.setShapeThickness(t);
            w.thickness.Value = t;
            toolControl.thickness = t;
        }

        public void stopDraw()
        {
            if (drawControl != null) drawControl.stopDraw();
        }

        public void open()
        {
            
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".html";
            dialog.Filter = "Všechny podporované |*.html;*.png;*.jpg;*.bmp|Soubory HTML (*.html)|*.html|Soubory PNG (*.png)|*.png|Soubory JPEG (*.jpg)|*.jpg|Soubory BMP (*.bmp)|*.bmp";
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                TabItem tab = AddTabItem();
                DrawControl filee = new DrawControl(this, revScale, tab);
                files[tab] = filee;
                filee.setName("Bez názvu");
                string filename = dialog.FileName;

                
                filee.setPath(filename);
                filee.OpenFromFile(filename);
                
                setFileActive(tab);
            }
        }

        public void setPath(string path)
        {
            w.labelPath.Content = path;
        }

        public bool saveAs()
        { 
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.DefaultExt = ".html";
            dialog.Filter = "Soubory HTML (*.html)|*.html|Soubory PNG (*.png)|*.png|Soubory JPEG (*.jpg)|*.jpg|Soubory BMP (*.bmp)|*.bmp";
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                string filename = dialog.FileName;
                saveAs(filename);
                drawControl.setPath(filename);
                setPath(filename);
            }
            return result.HasValue && result.Value;
        }

        void saveAs(string path)
        {
            drawControl.stopDraw();
            Regex r = new Regex("\\.[a-zA-Z0-9]+$");
            string suffix = r.Matches(path)[0].ToString().ToLower();
            drawControl.SaveAsFile(path);
            drawControl.historyControl.setNotChange();
            
        }

        public void delete()
        {
            if (drawControl != null) drawControl.shapeDelete();
        }

        public bool save()
        {
            if (drawControl.path == "")
            {
                return saveAs();
            }
            else
            {
                saveAs(drawControl.path);
                return true;
            }
        }

        public bool saveDialog()
        {
            MessageBoxResult result = MessageBox.Show("Chcete uložit změny?", "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                return save();
            }
            else if(result == MessageBoxResult.Cancel)
            {
                return false;
            }
            return true;
        }

        public void exit()
        {
             w.Close();  
        }

        public void setActiveColor(MyEnum s)
        {
            activeColor = s;
            switch (s)
            {
                case MyEnum.PRIMARY:
                    w.primaryColor.Stroke = Brushes.Orange;
                    w.secondaryColor.Stroke = null;
                    w.backgroundColor.Stroke = null;
                    break;
                case MyEnum.SECONDARY:
                    w.primaryColor.Stroke = null;
                    w.secondaryColor.Stroke = Brushes.Orange;
                    w.backgroundColor.Stroke = null;
                    break;
                case MyEnum.BACKGROUND:
                    w.primaryColor.Stroke = null;
                    w.secondaryColor.Stroke = null;
                    w.backgroundColor.Stroke = Brushes.Orange;
                    break;
            }
            setCB(getColor());
        }

        public void setTool(MyEnum s)
        {
            Style def = w.FindResource("MyButton") as Style;
            Style act = w.FindResource("MyButtonActive") as Style;
            w.button_select.Style = def;
            w.button_line.Style = def;
            w.button_rectangle.Style = def;
            w.button_ellipse.Style = def;
            w.button_polygon.Style = def;
            switch (s)
            {
                case MyEnum.SELECT:
                    w.button_select.Style = act;
                    break;
                case MyEnum.LINE:
                    w.button_line.Style = act;
                    break;
                case MyEnum.RECT:
                    w.button_rectangle.Style = act;
                    break;
                case MyEnum.ELLIPSE:
                    w.button_ellipse.Style = act;
                    break;
                case MyEnum.POLYGON:
                    w.button_polygon.Style = act;
                    break;
            }
            if (drawControl != null) drawControl.stopDraw();
            if (drawControl != null) drawControl.setActiveShape(s);
            toolControl.tool = s;
        }

        public void mouseDown(MouseButtonEventArgs e)
        {
            if (drawControl != null) drawControl.mouseDown(e); 
        }

        public void mouseMove(MouseEventArgs e)
        {
            if (drawControl != null) drawControl.mouseMove(e.GetPosition(w.canvas));
        }

        public void mouseUp(MouseButtonEventArgs e)
        {
            if (drawControl != null) drawControl.mouseUp(e);
        }

        public void setChange(bool b)
        {
            change = b;
        }

        public void closed(System.ComponentModel.CancelEventArgs e)
        {
            foreach(var f in files)
            {
                DrawControl file = f.Value;
                setFileActive(f.Key);
                if (file.historyControl.change())
                {
                    if (!saveDialog())
                    {
                        e.Cancel = true;
                        break;
                    }
                }
            }
        }

        public void keyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                delete();
            }
            if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.C))
            {
                
            }
            if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.V))
            {
                clipboardControl.paste();
            }
        }
    }
}

