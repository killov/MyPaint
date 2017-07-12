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

namespace MyPaint
{
    enum MyEnum
    {
        LINE, RECT, ELLIPSE, POLYGON, PRIMARY, SECONDARY
    }

    class Control
    {
        MyEnum activeShape;
        MyEnum activeColor;
        public MyBrush color = new MyBrush(0, 0, 0);
        public MyBrush fcolor = new MyBrush(null);
        string path = "";
        bool change = false;
        public MainWindow w;
        public MyShape shape;
        public bool draw = false;
        public bool startdraw = false;
        public bool candraw = true;
        public bool drag = false;
        Point posunStart = new Point();
        Point posunStartMys = new Point();
        public List<MyShape> shapes = new List<MyShape>();
        public double StrokeThickness = 1;

        public Control(MainWindow ww)
        {
            w = ww;
            setDrawShape(MyEnum.LINE);
            setActiveColor(MyEnum.PRIMARY);
        }

        public void newC()
        {
            if (!saveDialog()) return;
            shapes = new List<MyShape>();
            clearCanvas();
            setPath("");
            change = false;
        }

        void clearCanvas()
        {
            stopDraw();
            w.canvas.Children.RemoveRange(0, w.canvas.Children.Count - 1);
            w.canvas.Background = Brushes.White;
        }

        public void setColor(MyBrush c)
        {
            switch (activeColor)
            {
                case MyEnum.PRIMARY:
                    color = c;
                    w.primaryColor.Fill = c.brush;
                    if (shape != null) shape.setPrimaryColor(c);
                    break;
                case MyEnum.SECONDARY:
                    fcolor = c;
                    w.secondaryColor.Fill = c.brush;
                    if (shape != null) shape.setSecondaryColor(c);
                    break;
            }           
        }

        public void open()
        {
            if (!saveDialog()) return;
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".png";
            dialog.Filter = "Soubory png (*.png)|*.png";
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                clearCanvas();
                string filename = dialog.FileName;
                MemoryStream memoStream = new MemoryStream();

                using (FileStream fs = File.OpenRead(@filename))
                {
                    fs.CopyTo(memoStream);
                    BitmapImage bmi = new BitmapImage();
                    bmi.BeginInit();
                    bmi.StreamSource = memoStream;
                    bmi.EndInit();
                    ImageBrush brush = new ImageBrush(bmi);
                    w.canvas.Background = brush;
                    fs.Close();
                }

                setPath(filename);
            }
        }

        public void saveAs()
        { 
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.DefaultExt = ".png";
            dialog.Filter = "Soubory png (*.png)|*.png|Soubory HTML (*.html)|*.html";
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
            stopDraw();
            Regex r = new Regex(".[a-zA-Z0-9]+$");
            string suffix = r.Matches(path)[0].ToString().ToLower();
            switch (suffix)
            {
                case ".html":
                case ".htm":
                    saveAsHTML(path);
                    break;
                default:
                case ".png":
                    saveAsPNG(path);
                    break;
            }
            
            change = false;
        }

        void saveAsPNG(string path)
        {
            try
            {
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)w.canvas.RenderSize.Width,
                (int)w.canvas.RenderSize.Height, 96, 96, PixelFormats.Default);
                rtb.Render(w.canvas);
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                using (var fs = File.OpenWrite(@path))
                {
                    encoder.Save(fs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nepovedlo se uložit soubor");
            }
        }

        void saveAsHTML(string path)
        {
            try
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(path);
                file.WriteLine("<!DOCTYPE HTML>");
                file.WriteLine("<html>");
                file.WriteLine("<head>");
                file.WriteLine("<meta http-equiv=\"content-type\" content=\"text/html; charset = utf-8\">");
                file.WriteLine("</head>");
                file.WriteLine("<body>");
                file.WriteLine("<canvas width=\"500\" height=\"400\" border=\"1\" id=\"MyPaint\"></canvas>");
                file.WriteLine("<script>");
                file.WriteLine("var ctx = document.getElementById(\"MyPaint\").getContext(\"2d\");");
                foreach (var shape in shapes)
                {
                    file.WriteLine(shape.renderShape());
                }
                file.WriteLine("</script>");
                file.WriteLine("</body>");
                file.WriteLine("</html>");
                file.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nepovedlo se uložit soubor");
            }
        }

        internal void delete()
        {
            if(shape != null)
            {
                shape.delete();
                draw = false;
                candraw = true;
            }
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
                    break;
                case MyEnum.SECONDARY:
                    w.primaryColor.Stroke = null;
                    w.secondaryColor.Stroke = Brushes.Orange;
                    break;
            }
        }

        public void setDrawShape(MyEnum s)
        {
            Style def = w.FindResource("MyButton") as Style;
            Style act = w.FindResource("MyButtonActive") as Style;
            w.button_line.Style = def;
            w.button_rectangle.Style = def;
            w.button_ellipse.Style = def;
            w.button_polygon.Style = def;
            switch (s)
            {
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
            activeShape = s;
            stopDraw();
        }

        public void mouseDown(MouseButtonEventArgs e)
        {   
            if (candraw)
            {
                startDraw(e);
            }
            else
            {
                if (!shape.hitTest())
                {
                    stopDraw();
                    startDraw(e);
                }
            }
        }

        void startDraw(MouseButtonEventArgs e)
        {
            if (!draw)
            {
                change = true;
                switch (activeShape)
                {
                    case MyEnum.LINE:
                        shape = new MyLine(this);
                        break;
                    case MyEnum.RECT:
                        shape = new MyRectangle(this);
                        break;
                    case MyEnum.ELLIPSE:
                        shape = new MyEllipse(this);
                        break;
                    case MyEnum.POLYGON:
                        shape = new MyPolygon(this);
                        break;
                }
                shape.setPrimaryColor(color);
                shape.setSecondaryColor(fcolor);
                shapes.Add(shape);
                shape.mouseDown(e);
            }
        }

        public void startMoveShape(Point start, Point mys)
        {
            posunStart = start;
            posunStartMys = mys;
            drag = true;
        }

        public void mouseMove(MouseEventArgs e)
        {
            if(draw) shape.mouseMove(e);
            if (!candraw)
            {
                shape.moveDrag(e);
                if (drag)
                {
                    shape.moveShape(posunStart.X + (e.GetPosition(w.canvas).X - posunStartMys.X),  posunStart.Y + (e.GetPosition(w.canvas).Y - posunStartMys.Y));
                }
            }
        }

        public void mouseUp(MouseButtonEventArgs e)
        {
            if (draw)
            {
                shape.mouseUp(e);    
            }
            if (!candraw)
            {
                shape.stopDrag();
                drag = false;
            }
        }

        public void stopDraw()
        {
            if (!candraw)
            {
                candraw = true;
                shape.stopDraw();
                shape = null;
                w.canvas.Children.Remove(w.res);
                w.canvas.Children.Add(w.res);
            }
            
        }
    }
}

