using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyPaint
{
    public class MyLayer : INotifyPropertyChanged
    {
        public string Name { get; set; }
        bool vis;
        public bool visible {
            get
            {
                return vis;
            }
            set
            {
                canvas.Visibility = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                vis = value;
            }
        }

        Canvas cv;
        public Canvas canvas;
        public Brush color;
        public List<MyShape> shapes = new List<MyShape>();

        public MyLayer(Canvas c, DrawControl dc)
        {
            cv = c;
            canvas = new Canvas();
            cv.Children.Add(canvas);
            setResolution(dc.resolution);
        }

        public MyLayer(Canvas c, DrawControl dc, jsonDeserialize.Layer layer)
        {
            cv = c;
            canvas = new Canvas();
            cv.Children.Add(canvas);
            setResolution(dc.resolution);
            visible = layer.visible;
            setColor(layer.color == null ? null : layer.color.createBrush());
            foreach (var shape in layer.shapes)
            {
                switch (shape.type)
                {
                    case "LINE":
                        shapes.Add(new MyLine(dc, this, shape));
                        break;
                    case "RECTANGLE":
                        shapes.Add(new MyRectangle(dc, this, shape));
                        break;
                    case "ELLIPSE":
                        shapes.Add(new MyEllipse(dc, this, shape));
                        break;
                    case "POLYGON":
                        shapes.Add(new MyPolygon(dc, this, shape));
                        break;
                    case "IMAGE":
                        shapes.Add(new MyImage(dc, this, shape));
                        break;
                }
            }
        }

        public void setResolution(Point res)
        {
            canvas.Width = res.X;
            canvas.Height = res.Y;
        }

        public void setColor(Brush c)
        {
            canvas.Background = c;
            color = c;
        }

        public void delete()
        {
            cv.Children.Remove(canvas);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public jsonSerialize.Layer render()
        {
            jsonSerialize.Layer la = new jsonSerialize.Layer();
            la.color = Utils.BrushToCanvas(color);
            la.visible = visible;
            la.shapes = new List<jsonSerialize.Shape>();
            foreach (var shape in shapes)
            {
                la.shapes.Add(shape.renderShape());
            }
            return la;
        }
    }
}
