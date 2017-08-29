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

namespace MyPaint
{
    public enum MyEnum
    {
        SELECT, LINE, RECT, ELLIPSE, POLYGON, PRIMARY, SECONDARY, BACKGROUND
    }

    public class MainControl
    {
        
        HistoryControl historyControl;
        DrawControl drawControl;
        public MyEnum activeColor;
        string path = "";
        bool change = false;
        public MainWindow w;
        ScaleTransform scale, revScale;
        public Point resolution;
        public bool resolutionDrag = false;

        public MainControl(MainWindow ww)
        {
            w = ww;
            scale = new ScaleTransform(1, 1);
            revScale = new ScaleTransform(1, 1);
            historyControl = new HistoryControl(this);
            drawControl = new DrawControl(this, w.canvas, w.res, revScale);
            setTool(MyEnum.LINE);
            setActiveColor(MyEnum.PRIMARY);
            setColor(Brushes.Black);
            setThickness(1);
            setResolution(500, 400);
            historyControl.clear();
            w.layers.ItemsSource = drawControl.layers;
            TransformGroup g = new TransformGroup();
            
            g.Children.Add(scale);
            w.canvas_out.LayoutTransform = g;
            g = new TransformGroup();
            g.Children.Add(revScale);
            w.resolution.LayoutTransform = g;
        }

        public void addLayer()
        {
            drawControl.addLayer();
        }

        public void layerChanged()
        {
            drawControl.setActiveLayer(w.layers.SelectedIndex);
        }

        public void back()
        {
            drawControl.stopDraw();
            historyControl.back();
        }

        public void forward()
        {
            historyControl.forward();
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

        public void setResolution(double ws, double hs)
        {
            resolution = new Point(ws, hs);
            w.canvas.Width = ws;
            w.canvas.Height = hs;
            w.canvas_out.Width = ws;
            w.canvas_out.Height = hs;
            w.res.Width = ws;
            w.res.Height = hs;
            Canvas.SetLeft(w.resolution, ws);
            Canvas.SetTop(w.resolution, hs);
            w.labelResolution.Content = String.Format("{0}x{1}", (int)ws, (int)hs);
            drawControl.setResolution(resolution);   
        }

        public void newC()
        {
            if (!saveDialog()) return;            
            setPath("");
            change = false;
            setResolution(500, 400);     
            drawControl.clear();
            historyControl.clear();
        }

        

        public void setColor(Brush c)
        {
            setCB(c);
            switch (activeColor)
            {
                case MyEnum.PRIMARY:
                    drawControl.setPrimaryColor(c);
                    w.primaryColor.Fill = c;  
                    break;
                case MyEnum.SECONDARY:
                    drawControl.setSecondaryColor(c);
                    w.secondaryColor.Fill = c;                    
                    break;
                case MyEnum.BACKGROUND:
                    drawControl.setBackgroundColor(c);
                    w.backgroundColor.Fill = c;
                    break;
            }
        }

        public void setPrimaryColor(Brush c)
        {
            if (activeColor == MyEnum.PRIMARY) setCB(c);
            drawControl.setPrimaryColor(c);
            w.primaryColor.Fill = c;
        }

        public void setSecondaryColor(Brush c)
        {
            if (activeColor == MyEnum.SECONDARY) setCB(c);
            drawControl.setSecondaryColor(c);
            w.secondaryColor.Fill = c;
        }

        public void setBackgroundColor(Brush c)
        {
            if (activeColor == MyEnum.BACKGROUND) setCB(c);
            w.backgroundColor.Fill = c;
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
                    return drawControl.getPrimaryColor();
                case MyEnum.SECONDARY:
                    return drawControl.getSecondaryColor();
                case MyEnum.BACKGROUND:
                    return drawControl.getBackgroundColor();
                default:
                    return drawControl.getPrimaryColor();
            }
        }

        public void setThickness(double t)
        {
            drawControl.setThickness(t);
            w.thickness.Value = t;
        }

        public void stopDraw()
        {
            drawControl.stopDraw();
        }

        public void open()
        {
            if (!saveDialog()) return;
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".html";
            dialog.Filter = "Všechny podporované |*.html;*.png;*.jpg;*.bmp|Soubory HTML (*.html)|*.html|Soubory PNG (*.png)|*.png|Soubory JPEG (*.jpg)|*.jpg|Soubory BMP (*.bmp)|*.bmp";
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                drawControl.clearCanvas();
                historyControl.clear();
                string filename = dialog.FileName;

                Regex r = new Regex("\\.[a-zA-Z0-9]+$");
                string suffix = r.Matches(filename)[0].ToString().ToLower();

                switch (suffix)
                {
                    case ".html":
                        file.HTML.open(drawControl, filename);
                        break;
                    case ".jpg":
                        file.JPEG.open(drawControl, filename);
                        break;
                    case ".bmp":
                        file.BMP.open(drawControl, filename);
                        break;
                    case ".png":
                    default:
                        file.PNG.open(drawControl, filename);
                        break;
                }
                setPath(filename);
            }
        }

        public void lockDraw()
        {
            w.canvas.Children.Remove(w.res);
            w.canvas.Children.Add(w.res);
        }

        public void saveAs()
        { 
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.DefaultExt = ".html";
            dialog.Filter = "Soubory HTML (*.html)|*.html|Soubory PNG (*.png)|*.png|Soubory JPEG (*.jpg)|*.jpg|Soubory BMP (*.bmp)|*.bmp";
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                string filename = dialog.FileName;
                saveAs(filename);
                setPath(filename);
            }
        }

        public void setPath(string p)
        {
            path = p;
            w.labelPath.Content = p;
        }

        void saveAs(string path)
        {
            drawControl.stopDraw();
            Regex r = new Regex("\\.[a-zA-Z0-9]+$");
            string suffix = r.Matches(path)[0].ToString().ToLower();
            try
            {
                switch (suffix)
                {
                    case ".html":
                    case ".htm":
                        file.HTML.save(drawControl, path);
                        break;
                    case ".jpg":
                        file.JPEG.save(drawControl, path);
                        break;
                    case ".bmp":
                        file.BMP.save(drawControl, path);
                        break;
                    default:
                    case ".png":
                        file.PNG.save(drawControl, path);
                        break;
                }
            }catch(Exception ex)
            {
                MessageBox.Show("Nepovedlo se uložit soubor");
            }
            change = false;
        }

        public void delete()
        {
            drawControl.delete();
        }

        public void save()
        {
            if (path == "")
            {
                saveAs();
            }
            else
            {
                saveAs(path);
            }
        }

        public bool saveDialog()
        {
            if (change)
            {
                MessageBoxResult result = MessageBox.Show("Chcete uložit změny?", "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    save();
                }
                else if(result == MessageBoxResult.Cancel)
                {
                    return false;
                }
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
            drawControl.stopDraw();
            drawControl.setActiveShape(s); 
        }

        public void mouseDown(MouseButtonEventArgs e)
        {
            drawControl.mouseDown(e); 
        }

        public void addHistory(IHistoryNode s)
        {
            historyControl.add(s);
        }

        public void mouseMove(MouseEventArgs e)
        {
            drawControl.mouseMove(e);
        }

        public void mouseUp(MouseButtonEventArgs e)
        {
            drawControl.mouseUp(e);
        }

        public void setChange(bool b)
        {
            change = b;
        }
    }
}

