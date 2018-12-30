using MyPaint.History;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyPaint
{
    public class Layer : INotifyPropertyChanged
    {
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public string name;
        bool _visible;
        Serializer.Layer _layer;
        public List<Shapes.Shape> _shapes;
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                canvas.Visibility = value ? Visibility.Visible : Visibility.Hidden;
                _visible = value;
                NotifyPropertyChanged("Visible");
            }
        }

        public bool IsNotFirst
        {
            get
            {
                return f.layers.First() != this;
            }
        }

        public bool IsNotLast
        {
            get
            {
                return f.layers.Last() != this;
            }
        }

        public Brush Background
        {
            get
            {
                return background;
            }
            set
            {
                canvas.Background = value;
                sBackground = Serializer.Brush.Create(value);
                background = value;
            }
        }

        Canvas canvas;
        Brush background = null;
        Serializer.Brush sBackground = Serializer.Brush.Create(null);
        private FileControl f;
        private List<Shapes.Shape> Shapes { get; set; }

        public Layer(FileControl file)
        {
            canvas = new Canvas();
            Shapes = new List<Shapes.Shape>();
            file.Canvas.Children.Add(canvas);
            SetResolution(file.Resolution);
            f = file;
        }

        public Layer(FileControl file, Serializer.Layer layer)
        {
            f = file;
            _layer = layer;
            Shapes = new List<Shapes.Shape>();
            _shapes = new List<Shapes.Shape>();
            foreach (var shape in layer.Shapes)
            {
                _shapes.Add(shape.Create(file.DrawControl, this));
            }
        }

        public void InitDraw()
        {
            canvas = new Canvas();
            if (_layer != null)
            {
                f.Canvas.Children.Add(canvas);
                Name = _layer.Name;
                canvas.Name = _layer.Name;
                Visible = _layer.Visible;
                Background = _layer.Color == null ? null : _layer.Color.CreateBrush();
                SetResolution(f.Resolution);
            }
            foreach (var shape in _shapes)
            {
                shape.InitDraw();
            }
        }

        public void SetResolution(Point res)
        {
            canvas.Width = res.X;
            canvas.Height = res.Y;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public Serializer.Layer CreateSerializer()
        {
            Serializer.Layer la = new Serializer.Layer();
            la.Color = sBackground;
            la.Visible = Visible;
            la.Name = Name;
            la.Shapes = new List<Serializer.Shape>();
            foreach (var shape in Shapes)
            {
                la.Shapes.Add(shape.CreateSerializer());
            }
            return la;
        }

        public Canvas CreateImage()
        {
            Canvas canvas = new Canvas();
            canvas.Background = sBackground.CreateBrush();
            canvas.Width = f.Resolution.X;
            canvas.Height = f.Resolution.Y;
            if (Visible)
            {
                foreach (var shape in Shapes)
                {
                    shape.CreateImage(canvas);
                }
            }
            return canvas;
        }

        public void Up()
        {
            int i = f.layers.IndexOf(this);
            if (i > 0)
            {
                SetPosition(i - 1);
                f.HistoryControl.Add(new History.HistoryLayerPosition(this, i, i - 1));
            }
        }

        public void Down()
        {
            int i = f.layers.IndexOf(this);
            if (i < f.layers.Count - 1)
            {
                SetPosition(i + 1);
                f.HistoryControl.Add(new History.HistoryLayerPosition(this, i, i + 1));
            }
        }

        public void SetPosition(int i)
        {
            f.layers.Move(f.layers.IndexOf(this), i);
            f.Canvas.Children.Remove(canvas);
            f.Canvas.Children.Insert(i, canvas);
        }

        public void Remove(bool history = true)
        {
            if (history) f.HistoryControl.Add(new HistoryLayerRemove(this, f.layers.IndexOf(this)));
            f.layers.Remove(this);
            f.Canvas.Children.Remove(canvas);
            if (f.DrawControl.SelectLayer == this)
            {
                f.DrawControl.StopEdit();
                f.DrawControl.UnselectLayer();
                UnsetSelectable();
            }
        }

        public void Add(int i = -1)
        {
            if (i == -1)
            {
                f.layers.Add(this);
                f.Canvas.Children.Add(canvas);
            }
            else
            {
                f.layers.Remove(this);
                f.layers.Insert(i, this);
                f.Canvas.Children.Remove(canvas);
                f.Canvas.Children.Insert(i, canvas);
            }
        }

        public void SetSelectable()
        {
            foreach (var shape in Shapes)
            {
                shape.SetSelectable();
            }
        }

        public void UnsetSelectable()
        {
            foreach (var shape in Shapes)
            {
                shape.HideVirtualShape();
            }
        }

        public void AddShape(Shapes.Shape shape)
        {
            Shapes.Add(shape);
            canvas.Children.Add(shape.Element);
        }

        public void InsertShape(int index, Shapes.Shape shape)
        {
            Shapes.Insert(index, shape);
            canvas.Children.Insert(index, shape.Element);
        }

        public int RemoveShape(Shapes.Shape shape)
        {
            int pos = Shapes.IndexOf(shape);
            Shapes.Remove(shape);
            canvas.Children.Remove(shape.Element);
            return pos;
        }

        public void ShapeElementChanged(Shapes.Shape shape)
        {
            int i = Shapes.IndexOf(shape);
            if (i != -1)
            {
                canvas.Children.RemoveAt(i);
                canvas.Children.Insert(i, shape.Element);
            }
        }

        public void SetName(string name, bool addHistory = false)
        {
            if (addHistory)
            {
                f.HistoryControl.Add(new HistoryLayerName(this, Name, name));
            }
            Name = name;
        }
    }
}
