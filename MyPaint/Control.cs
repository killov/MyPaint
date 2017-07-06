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

namespace WpfApplication1
{
    enum DrawShape
    {
        LINE, RECT, ELLIPSE, POLYGON
    }

    class Control
    {
        DrawShape dShape;
        public Brush color = Brushes.Black;
        public Brush fcolor = null;
        string path = "";
        bool change = false;
        public MainWindow w;
        MyShape shape;
        public bool draw = false;
        public bool startdraw = false;
        public bool candraw = true;
        public bool drag = false;
        Point posunStart = new Point();
        Point posunStartMys = new Point();
        public double StrokeThickness = 1;

        public Control(MainWindow ww)
        {
            w = ww;
            setDrawShape(DrawShape.LINE);   
        }

        public void newC()
        {
            if (!saveDialog()) return;
            
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
            color = c;
            w.colors.Stroke = c;
            stopDraw();
        }

        public void setfColor(Brush c)
        {
            fcolor = c;
            w.colors.Fill = c;
            stopDraw();
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
            dialog.Filter = "Soubory png (*.png)|*.png";
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
            //((BitmapImage)((ImageBrush)w.canvas.Background).ImageSource).EndInit();
            stopDraw();
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

            change = false;
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

        public void setDrawShape(DrawShape s)
        {
            w.cara.Background = null;
            w.obdelnik.Background = null;
            w.elipsa.Background = null;
            w.polygon.Background = null;
            switch (s)
            {
                case DrawShape.LINE:
                    w.cara.Background = System.Windows.Media.Brushes.GreenYellow;
                    break;
                case DrawShape.RECT:
                    w.obdelnik.Background = System.Windows.Media.Brushes.GreenYellow;
                    break;
                case DrawShape.ELLIPSE:
                    w.elipsa.Background = System.Windows.Media.Brushes.GreenYellow;
                    break;
                case DrawShape.POLYGON:
                    w.polygon.Background = System.Windows.Media.Brushes.GreenYellow;
                    break;
            }
            dShape = s;
            stopDraw();
        }

        public void mouseDown(MouseButtonEventArgs e)
        {
            if (candraw)
            {
                if (!draw)
                {
                    change = true;
                    switch (dShape)
                    {
                        case DrawShape.LINE:
                            shape = new MyLine(this);
                            break;
                        case DrawShape.RECT:
                            shape = new MyRectangle(this);
                            break;
                        case DrawShape.ELLIPSE:
                            shape = new MyEllipse(this);
                            break;
                        case DrawShape.POLYGON:
                            shape = new MyPolygon(this);
                            break;
                    }
                    shape.mouseDown(e);
                }
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
                w.canvas.Children.Remove(w.res);
                w.canvas.Children.Add(w.res);
            }
            
        }
    }
}

