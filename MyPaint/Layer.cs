using MyPaint.History;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyPaint
{
    public class Layer : INotifyPropertyChanged
    {
        public string Name { get; set; }
        bool vis;
        Brush col;
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

        public Brush Color {
            get
            {
                return col;
            }
            set {
                col = value;
                if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Color"));
            }
        }

        public bool IsNotFirst
        {
            get
            {
                return file.layers.First() != this;
            }
        }

        public bool IsNotLast
        {
            get
            {
                return file.layers.Last() != this;
            }
        }

        Canvas cv;
        public Canvas canvas;
        public Brush color;
        Serializer.Brush background = Serializer.Brush.Create(null);
        private FileControl file;
        public List<Shapes.Shape> shapes = new List<Shapes.Shape>();

        public Layer(Canvas c, FileControl dc)
        {
            cv = c;
            canvas = new Canvas();
            cv.Children.Add(canvas);
            SetResolution(dc.resolution);
            file = dc;
        }

        public Layer(Canvas c, FileControl dc, Deserializer.Layer layer)
        {
            cv = c;
            canvas = new Canvas();
            cv.Children.Add(canvas);
            Name = layer.name;
            canvas.Name = layer.name;
            SetResolution(dc.resolution);
            file = dc;
            visible = layer.visible;
            SetBackground(layer.color == null ? null : layer.color.CreateBrush());
            foreach (var shape in layer.shapes)
            {
                shapes.Add(shape.Create(file, this));
            }
        }

        public void SetResolution(Point res)
        {
            canvas.Width = res.X;
            canvas.Height = res.Y;
        }

        public void SetBackground(Brush c)
        {
            canvas.Background = c;
            background = Serializer.Brush.Create(c);
            color = c;
        }

        public Brush GetBackground()
        {
            return color;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public Serializer.Layer CreateSerializer()
        {
            Serializer.Layer la = new Serializer.Layer();
            la.color = Serializer.Brush.Create(color);
            la.visible = visible;
            la.name = Name;
            la.shapes = new List<Serializer.Shape>();
            foreach (var shape in shapes)
            {
                la.shapes.Add(shape.CreateSerializer());
            }
            return la;
        }

        public Canvas CreateImage()
        {
            Canvas canvas = new Canvas();
            canvas.Background = background.CreateBrush();
            canvas.Width = file.resolution.X;
            canvas.Height = file.resolution.Y;
            if (visible)
            {
                foreach (var shape in shapes)
                {
                    shape.CreateImage(canvas);
                }
            }
            return canvas;
        }

        public void Up()
        {
            int i = file.layers.IndexOf(this);
            if (i > 0)
            {
                SetPosition(i - 1);
                file.historyControl.Add(new History.HistoryLayerPosition(this, i, i - 1));
            }
        }

        public void Down()
        {
            int i = file.layers.IndexOf(this);
            if (i < file.layers.Count - 1)
            {
                SetPosition(i + 1);
                file.historyControl.Add(new History.HistoryLayerPosition(this, i, i + 1));
            }
        }

        public void SetPosition(int i)
        {
            file.layers.Move(file.layers.IndexOf(this), i);
            cv.Children.Remove(canvas);
            cv.Children.Insert(i, canvas);
        }

        public void Remove(bool history = true)
        {
            if(history) file.historyControl.Add(new HistoryLayerRemove(this, file.layers.IndexOf(this)));
            file.layers.Remove(this);
            cv.Children.Remove(canvas);
            UnsetSelectable();
            if(file.selectLayer == this)
            {
                file.StopEdit();
                file.selectLayer = null;
            }
            
        }

        public void Add(int i = -1)
        {
            if(i == -1)
            {
                file.layers.Add(this);
                cv.Children.Add(canvas);
            }
            else
            {
                file.layers.Remove(this);
                file.layers.Insert(i, this);
                cv.Children.Remove(canvas);
                cv.Children.Insert(i, canvas);
            }
        }

        public void SetSelectable()
        {
            foreach(var shape in shapes)
            {
                shape.HideVirtualShape();
                shape.ShowVirtualShape((e, s, m) =>
                {
                    SetShapeSelect(e, s, m);
                });
            }
        }

        private void SetShapeSelect(Point e, Shapes.Shape shape, bool enableMoving)
        {            
            if (file.shape != null)
            {
                file.shape.StopEdit();
            }
            UnsetSelectable();
            SetSelectable();
            file.shape = shape;
            shape.SetActive();
            if(enableMoving) shape.StartMove(e);
        }

        public void UnsetSelectable()
        {
            foreach (var shape in shapes)
            {
                shape.HideVirtualShape();
            }
        }

        public void SetActive(bool act)
        {
            Color = act ? Brushes.Orange : null;
        }
    }
}
