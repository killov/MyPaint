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
    public class MyLayer : INotifyPropertyChanged
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
                return drawControl.layers.First() != this;
            }
        }

        public bool IsNotLast
        {
            get
            {
                return drawControl.layers.Last() != this;
            }
        }

        Canvas cv;
        public Canvas canvas;
        public Brush color;
        private DrawControl drawControl;
        public List<Shapes.MyShape> shapes = new List<Shapes.MyShape>();

        public MyLayer(Canvas c, DrawControl dc)
        {
            cv = c;
            canvas = new Canvas();
            canvas.Name = Name;
            cv.Children.Add(canvas);
            setResolution(dc.resolution);
            drawControl = dc;
        }


        public MyLayer(Canvas c, DrawControl dc, jsonDeserialize.Layer layer)
        {
            cv = c;
            canvas = new Canvas();
            cv.Children.Add(canvas);
            Name = layer.name;
            canvas.Name = layer.name;
            setResolution(dc.resolution);
            drawControl = dc;
            visible = layer.visible;
            setColor(layer.color == null ? null : layer.color.createBrush());
            foreach (var shape in layer.shapes)
            {
                switch (shape.type)
                {
                    case "LINE":
                        shapes.Add(new Shapes.MyLine(dc, this, shape));
                        break;
                    case "RECTANGLE":
                        shapes.Add(new Shapes.MyRectangle(dc, this, shape));
                        break;
                    case "ELLIPSE":
                        shapes.Add(new Shapes.MyEllipse(dc, this, shape));
                        break;
                    case "POLYGON":
                        shapes.Add(new Shapes.MyPolygon(dc, this, shape));
                        break;
                    case "IMAGE":
                        shapes.Add(new Shapes.MyImage(dc, this, shape));
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

        public Brush getColor()
        {
            return color;
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
            la.name = Name;
            la.shapes = new List<jsonSerialize.Shape>();
            foreach (var shape in shapes)
            {
                la.shapes.Add(shape.renderShape());
            }
            return la;
        }

        public void up()
        {
            int i = drawControl.layers.IndexOf(this);
            if (i > 0)
            {
                setPosition(i - 1);
                drawControl.historyControl.add(new History.HistoryLayerPosition(this, i, i - 1));
            }
        }

        public void down()
        {
            int i = drawControl.layers.IndexOf(this);
            if (i < drawControl.layers.Count - 1)
            {
                setPosition(i + 1);
                drawControl.historyControl.add(new History.HistoryLayerPosition(this, i, i + 1));
            }
        }

        public void setPosition(int i)
        {
            drawControl.layers.Move(drawControl.layers.IndexOf(this), i);
            cv.Children.Remove(canvas);
            cv.Children.Insert(i, canvas);
        }

        public void remove(bool history = true)
        {
            if(history) drawControl.historyControl.add(new HistoryLayerRemove(this, drawControl.layers.IndexOf(this)));
            drawControl.layers.Remove(this);
            cv.Children.Remove(canvas);
            unsetSelectable();
            if(drawControl.selectLayer == this)
            {
                drawControl.stopDraw();
                drawControl.selectLayer = null;
            }
            
        }

        public void add(int i = -1)
        {
            if(i == -1)
            {
                drawControl.layers.Add(this);
                cv.Children.Add(canvas);
            }
            else
            {
                drawControl.layers.Remove(this);
                drawControl.layers.Insert(i, this);
                cv.Children.Remove(canvas);
                cv.Children.Insert(i, canvas);
            }
        }

        public void setSelectable()
        {
            foreach(var shape in shapes)
            {
                shape.hideVirtualShape();
                shape.showVirtualShape((e, s) =>
                {
                    setShapeSelect(e, s);
                });
            }
        }

        public void setShapeSelect(Point e, Shapes.MyShape shape)
        {            
            if (drawControl.shape != null)
            {
                drawControl.shape.stopEdit();
            }
            unsetSelectable();
            setSelectable();
            drawControl.shape = shape;
            shape.setActive();
            shape.startMove(e);
        }

        public void setShapeSelectable(Shapes.MyShape shape)
        {
            shape.showVirtualShape((ee, s) =>
            {
                setShapeSelect(ee, s);
            });
        }

        public void unsetSelectable()
        {
            foreach (var shape in shapes)
            {
                shape.hideVirtualShape();
            }
        }

        public void setActive(bool act)
        {
            Color = act ? Brushes.Orange : null;
        }
    }
}
