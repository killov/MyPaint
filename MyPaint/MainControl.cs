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
        LINE, RECT, ELLIPSE, POLYGON, PRIMARY, SECONDARY, BACKGROUND
    }

    public class MainControl
    {
        
        HistoryControl historyControl;
        DrawControl drawControl;
        MyEnum activeShape;
        public MyEnum activeColor;
        public Brush bcolor = null;
        string path = "";
        bool change = false;
        public MainWindow w;

        Point resolution;
        public bool resolutionDrag = false;

        public MainControl(MainWindow ww)
        {
            w = ww;
            historyControl = new HistoryControl(this);
            drawControl = new DrawControl(this, w.canvas, w.res);
            setDrawShape(MyEnum.LINE);
            setActiveColor(MyEnum.PRIMARY);
            setColor(Brushes.Black);
            setThickness(1);
            setResolution(500, 400);
            historyControl.clear();
            w.layers.ItemsSource = drawControl.layers;

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
            
        }

        public void newC()
        {
            if (!saveDialog()) return;            
            setPath("");
            change = false;
            setResolution(500, 400);
            historyControl.clear();
            drawControl.clear();       
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
                    bcolor = c;
                    w.backgroundColor.Fill = c;
                    w.canvas.Background = c;
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
                    return drawControl.getPrimaryColor();
                case MyEnum.SECONDARY:
                    return drawControl.getSecondaryColor(); ;
                case MyEnum.BACKGROUND:
                    return bcolor;
                default:
                    return drawControl.getPrimaryColor();
            }
        }

        public void setThickness(double t)
        {
            drawControl.setThickness(t);
        }

        public void stopDraw()
        {
            drawControl.stopDraw();
        }

        public void open()
        {
            if (!saveDialog()) return;
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".png";
            dialog.Filter = "Soubory png (*.png)|*.png|Soubory HTML (*.html)|*.html";
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                drawControl.clearCanvas();
                string filename = dialog.FileName;

                Regex r = new Regex("\\.[a-zA-Z0-9]+$");
                string suffix = r.Matches(filename)[0].ToString().ToLower();

                switch (suffix)
                {
                    case ".html":
                    case ".htm":
                        using (StreamReader sr = new StreamReader(@filename))
                        {
                            string code = sr.ReadToEnd();
                            string a = new Regex("width=\"(.+?)\"").Matches(code)[0].Groups[1].ToString();
                            double width = Double.Parse(new Regex("width=\"(.+?)\"").Matches(code)[0].Groups[1].ToString());
                            double height = Double.Parse(new Regex("height=\"(.+?)\"").Matches(code)[0].Groups[1].ToString());
                            setResolution(width, height);
                            r = new Regex("var json = (.+);");
                            string json = r.Matches(code)[0].Groups[1].ToString();
                            jsonDeserialize.Shape[] sh = (jsonDeserialize.Shape[]) new JavaScriptSerializer().Deserialize(json, typeof(jsonDeserialize.Shape[]));
                            foreach(var shape in sh)
                            {
                                switch (shape.type)
                                {
                                    case "LINE":
                                        drawControl.shapes.Add(new MyLine(drawControl, w.canvas, shape));
                                        break;
                                    case "RECTANGLE":
                                        drawControl.shapes.Add(new MyRectangle(drawControl, w.canvas, shape));
                                        break;
                                    case "ELLIPSE":
                                        drawControl.shapes.Add(new MyEllipse(drawControl, w.canvas, shape));
                                        break;
                                    case "POLYGON":
                                        drawControl.shapes.Add(new MyPolygon(drawControl, w.canvas, shape));
                                        break;
                                    case "IMAGE":
                                        drawControl.shapes.Add(new MyImage(drawControl, w.canvas, shape));
                                        break;
                                }
                            }
                        }
                        lockDraw();
                        break;
                    case ".png":
                    default:
                        MemoryStream memoStream = new MemoryStream();
                        using (FileStream fs = File.OpenRead(@filename))
                        {
                            fs.CopyTo(memoStream);
                            BitmapImage bmi = new BitmapImage();
                            bmi.BeginInit();
                            bmi.StreamSource = memoStream;
                            bmi.EndInit();
                            ImageBrush brush = new ImageBrush(bmi);
                            setResolution(bmi.Width, bmi.Height);
                            drawControl.shapes.Add(new MyImage(drawControl, w.canvas, brush, bmi.Width, bmi.Height));
                            w.canvas.Children.Remove(w.res);
                            w.canvas.Children.Add(w.res);
                            fs.Close();
                        }
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
            drawControl.stopDraw();
            Regex r = new Regex("\\.[a-zA-Z0-9]+$");
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
                file.WriteLine("<canvas width=\""+resolution.X+ "\" height=\"" + resolution.Y + "\" style=\"border: 1px solid black;\" id=\"MyPaint\"></canvas>");
                file.WriteLine("<script>");
                file.WriteLine("var ctx = document.getElementById(\"MyPaint\").getContext(\"2d\");");
                List<jsonSerialize.Shape> sh = new List<jsonSerialize.Shape>();
                foreach (var shape in drawControl.shapes)
                {
                    sh.Add(shape.renderShape());  
                }
                var json = new JavaScriptSerializer().Serialize(sh);
                file.WriteLine("var json = "+json+";");

                file.WriteLine("function draw(t){if(!(json.length<=t)){var a=json[t];switch(ctx.beginPath(),ctx.lineWidth=a.lineWidth,a.type){case'LINE':(s={}).x=Math.min(a.A.x,a.B.x),s.y=Math.min(a.A.y,a.B.y);var x=Math.abs(a.A.x-a.B.x),e=Math.abs(a.A.y-a.B.y);ctx.strokeStyle=brush(a.stroke,s,x,e),ctx.moveTo(a.A.x,a.A.y),ctx.lineTo(a.B.x,a.B.y),ctx.closePath(),ctx.stroke(),ctx.fill(),draw(t+1);break;case'RECTANGLE':(s={}).x=Math.min(a.A.x,a.B.x),s.y=Math.min(a.A.y,a.B.y);var x=Math.abs(a.A.x-a.B.x),e=Math.abs(a.A.y-a.B.y);ctx.strokeStyle=brush(a.stroke,s,x,e),ctx.fillStyle=brush(a.fill,s,x,e),ctx.moveTo(a.A.x,a.A.y),ctx.lineTo(a.B.x,a.A.y),ctx.lineTo(a.B.x,a.B.y),ctx.lineTo(a.A.x,a.B.y),ctx.closePath(),ctx.stroke(),ctx.fill(),draw(t+1);break;case'ELLIPSE':(s={}).x=Math.min(a.A.x,a.B.x),s.y=Math.min(a.A.y,a.B.y);var x=Math.abs(a.A.x-a.B.x),e=Math.abs(a.A.y-a.B.y);ctx.strokeStyle=brush(a.stroke,s,x,e),ctx.fillStyle=brush(a.fill,s,x,e),ctx.ellipse((a.A.x+a.B.x)/2,(a.A.y+a.B.y)/2,Math.abs(a.A.x-a.B.x)/2,Math.abs(a.A.y-a.B.y)/2,0,0,2*Math.PI),ctx.closePath(),ctx.stroke(),ctx.fill(),draw(t+1);break;case'POLYGON':var s={};s.x=1/0,s.y=1/0;var r={};r.x=-1/0,r.y=-1/0;for(var o in a.points)s.x=Math.min(s.x,a.points[o].x),s.y=Math.min(s.y,a.points[o].y),r.x=Math.max(r.x,a.points[o].x),r.y=Math.max(r.y,a.points[o].y);var x=r.x-s.x,e=r.y-s.y;ctx.strokeStyle=brush(a.stroke,s,x,e),ctx.fillStyle=brush(a.fill,s,x,e),ctx.moveTo(a.points[0].x,a.points[0].y);for(o=1;o<a.points.length;o++)ctx.lineTo(a.points[o].x,a.points[o].y);ctx.closePath(),ctx.stroke(),ctx.fill('evenodd'),draw(t+1);break;case'IMAGE':var c=new Image;c.onload=function(){ctx.moveTo(0,0),ctx.drawImage(c,a.A.x,a.A.y),draw(t+1)},c.src='data:image/png;base64,'+a.b64}}}function brush(t,a,x,e){if(null==t)return'rgba(0,0,0,0)';switch(t.type){case'COLOR':return'rgba('+t.R+','+t.G+','+t.B+','+(t.A/255).toString().replace(',','.')+')';case'LG':var s=ctx.createLinearGradient(a.x+t.S.x*x,a.y+t.S.y*e,a.x+t.E.x*x,a.y+t.E.y*e);for(var r in t.stops)s.addColorStop(t.stops[r].offset,brush(t.stops[r].color));return s}}draw(0);");
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
            drawControl.secActiveShape(s);
            drawControl.stopDraw();
        }

        public void mouseDown(MouseButtonEventArgs e)
        {
            drawControl.mouseDown(e); 
        }

        public void addHistory(MyShape s)
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

