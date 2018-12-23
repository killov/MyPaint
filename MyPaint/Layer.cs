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
        bool vis;
        public bool visible
        {
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

        Canvas cv;
        public Canvas canvas;
        Brush background;
        Serializer.Brush sBackground = Serializer.Brush.Create(null);
        private DrawControl file;
        private FileControl f;
        private List<Shapes.Shape> Shapes { get; set; }

        public Layer(Canvas c, FileControl fil, DrawControl dc)
        {
            cv = c;
            canvas = new Canvas();
            Shapes = new List<Shapes.Shape>();
            cv.Children.Add(canvas);
            SetResolution(fil.Resolution);
            file = dc;
            f = fil;
        }

        public Layer(Canvas c, FileControl fil, DrawControl dc, Deserializer.Layer layer)
        {
            cv = c;
            canvas = new Canvas();
            Shapes = new List<Shapes.Shape>();
            cv.Children.Add(canvas);
            Name = layer.name;
            canvas.Name = layer.name;
            SetResolution(fil.Resolution);
            file = dc;
            visible = layer.visible;
            Background = layer.color == null ? null : layer.color.CreateBrush();
            foreach (var shape in layer.shapes)
            {
                shape.Create(file, this);
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
            la.color = sBackground;
            la.visible = visible;
            la.name = Name;
            la.shapes = new List<Serializer.Shape>();
            foreach (var shape in Shapes)
            {
                la.shapes.Add(shape.CreateSerializer());
            }
            return la;
        }

        public Canvas CreateImage()
        {
            Canvas canvas = new Canvas();
            canvas.Background = sBackground.CreateBrush();
            canvas.Width = f.Resolution.X;
            canvas.Height = f.Resolution.Y;
            if (visible)
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
                file.HistoryControl.Add(new History.HistoryLayerPosition(this, i, i - 1));
            }
        }

        public void Down()
        {
            int i = f.layers.IndexOf(this);
            if (i < f.layers.Count - 1)
            {
                SetPosition(i + 1);
                file.HistoryControl.Add(new History.HistoryLayerPosition(this, i, i + 1));
            }
        }

        public void SetPosition(int i)
        {
            f.layers.Move(f.layers.IndexOf(this), i);
            cv.Children.Remove(canvas);
            cv.Children.Insert(i, canvas);
        }

        public void Remove(bool history = true)
        {
            if (history) file.HistoryControl.Add(new HistoryLayerRemove(this, f.layers.IndexOf(this)));
            f.layers.Remove(this);
            cv.Children.Remove(canvas);
            if (file.SelectLayer == this)
            {
                file.StopEdit();
                file.UnselectLayer();
                UnsetSelectable();
            }
        }

        public void Add(int i = -1)
        {
            if (i == -1)
            {
                f.layers.Add(this);
                cv.Children.Add(canvas);
            }
            else
            {
                f.layers.Remove(this);
                f.layers.Insert(i, this);
                cv.Children.Remove(canvas);
                cv.Children.Insert(i, canvas);
            }
        }

        public void SetSelectable()
        {
            foreach (var shape in Shapes)
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
            if (file.Shape != null)
            {
                file.Shape.StopEdit();
            }
            SetSelectable();
            file.SetShapeActive(shape);
            if (enableMoving) shape.StartMove(e);
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
                file.HistoryControl.Add(new HistoryLayerName(this, Name, name));
            }
            Name = name;
        }
    }
}
