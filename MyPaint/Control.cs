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


namespace MyPaint
{
    public enum MyEnum
    {
        LINE, RECT, ELLIPSE, POLYGON, PRIMARY, SECONDARY
    }

    public class Control
    {
        MyEnum activeShape;
        public MyEnum activeColor;
        public Brush color = Brushes.Black;
        public Brush fcolor = null;
        public double thickness;
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

        public Control(MainWindow ww)
        {
            w = ww;
            setDrawShape(MyEnum.LINE);
            setActiveColor(MyEnum.PRIMARY);
            setColor(Brushes.Black);
            setThickness(1);
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

        public void setColor(Brush c)
        {
            setCB(c);
            switch (activeColor)
            {
                case MyEnum.PRIMARY:
                    color = c;
                    w.primaryColor.Fill = c;
                    if (shape != null) shape.setPrimaryColor(c);
                    break;
                case MyEnum.SECONDARY:
                    fcolor = c;
                    w.secondaryColor.Fill = c;
                    if (shape != null) shape.setSecondaryColor(c);
                    break;
            }
        }

        void setCB(Brush color)
        {
            if (color!= null && color is SolidColorBrush)
                color.Freeze();
            w.CB.Brush = color;
        }

        public Brush getColor()
        {
            switch (activeColor)
            {
                case MyEnum.PRIMARY:
                    return color;
                case MyEnum.SECONDARY:
                    return fcolor;
                default:
                    return color;
            }
        }

        public void setThickness(double t)
        {
            thickness = t;
            if (shape != null) shape.setThickness(t);
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
                file.WriteLine("<canvas width=\"500\" height=\"450\" style=\"border: 1px solid black;\" id=\"MyPaint\"></canvas>");
                file.WriteLine("<script>");
                file.WriteLine("var ctx = document.getElementById(\"MyPaint\").getContext(\"2d\");");
                List<json.Shape> sh = new List<json.Shape>();
                foreach (var shape in shapes)
                {
                    sh.Add(shape.renderShape());  
                }
                var json = new JavaScriptSerializer().Serialize(sh);
                file.WriteLine("var json = "+json+";");

                file.WriteLine("for (var j in json) { var shape = json[j]; ctx.beginPath(); ctx.lineWidth = shape.lineWidth; switch (shape.type) { case 'LINE': ctx.strokeStyle = brush(shape.stroke); ctx.moveTo(shape.A.x, shape.A.y); ctx.lineTo(shape.B.x, shape.B.y); break; case 'RECTANGLE': ctx.strokeStyle = brush(shape.stroke); ctx.fillStyle = brush(shape.fill); ctx.moveTo(shape.A.x, shape.A.y); ctx.lineTo(shape.B.x, shape.A.y); ctx.lineTo(shape.B.x, shape.B.y); ctx.lineTo(shape.A.x, shape.B.y); break; case 'ELLIPSE': ctx.strokeStyle = brush(shape.stroke); ctx.fillStyle = brush(shape.fill); ctx.ellipse((shape.A.x + shape.B.x) / 2, (shape.A.y + shape.B.y) / 2, Math.abs(shape.A.x - shape.B.x) / 2, Math.abs(shape.A.y - shape.B.y) / 2, 0, 0, 2 * Math.PI); break; case 'POLYGON': ctx.strokeStyle = brush(shape.stroke); ctx.fillStyle = brush(shape.fill); ctx.moveTo(shape.points[0].x, shape.points[0].y); for (var i = 1; i < shape.points.length; i++) { console.log(1); ctx.lineTo(shape.points[i].x, shape.points[i].y); } break; } ctx.closePath(); ctx.stroke(); ctx.fill(); }  function brush(b) { if (b == null) return 'rgba(0,0,0,0)'; switch (b.type) { case 'COLOR': return 'rgba(' + b.R + ',' + b.G + ',' + b.B + ',' + (b.A / 255.0).toString().replace(',', '.') + ')'; } }");
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
            setCB(getColor());
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
                shape.setThickness(thickness);
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

