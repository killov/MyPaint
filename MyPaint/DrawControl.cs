using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MyPaint.History;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;

namespace MyPaint
{
    public class DrawControl
    {
        public string path = "";
        public string name;
        public TabItem tabItem;
        public MainControl control;
        public HistoryControl historyControl;
        public Brush nullBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 255));
        public Canvas canvas;
        public Canvas topCanvas;
        public Brush primaryColor, secondaryColor;
        public double thickness;
        public Shapes.MyShape shape;
        public bool draw = false;
        public bool startdraw = false;
        public bool candraw = true;
        public bool drag = false;
        bool shapeMoved = false;
        Point posunStart = new Point();
        Point posunStartMys = new Point();
        public Point resolution;
        MyEnum activeShape;
        public ObservableCollection<MyLayer> layers = new ObservableCollection<MyLayer>();
        public MyLayer selectLayer;
        public ScaleTransform revScale;
        
        private int layerCounter = 1;

        public DrawControl(MainControl c, ScaleTransform revScale, TabItem ti)
        {
            control = c;
            canvas = new Canvas();
            topCanvas = new Canvas();
            topCanvas.Background = nullBrush;
            topCanvas.Width = canvas.Width;
            topCanvas.Height = canvas.Height;
            historyControl = new HistoryControl(c);
            resetLayers();
            historyControl.clear();
            this.revScale = revScale;
            tabItem = ti;
            lockDraw();
        }

        public void addLayer()
        {
            MyLayer layer = new MyLayer(canvas, this) { Name = "layer_" + layerCounter, visible = true };
            layers.Add(layer);
            layerCounter++;
            setActiveLayer(layers.Count - 1, false);
            lockDraw();
            historyControl.add(new HistoryLayerAdd(layer));
        }

        public void resetLayers()
        {
            layerCounter = 1;
            deleteLayers();
            addLayer();
        }

        public void deleteLayers()
        {
            foreach (var l in layers)
            {
                l.delete();
            }
            layers.Clear();
        }

        public void setActiveLayer(int i, bool history = true)
        {
            if (i == -1) return;
            foreach(MyLayer l in layers)
            {
                l.setActive(false);
            }
            if (activeShape == MyEnum.SELECT)
            {
                if (shape != null)
                {
                    shape.changeLayer(null);
                    if(history) historyControl.add(new HistoryShapeChangeLayer(shape, selectLayer, layers[i]));
                }
                if (selectLayer != null) selectLayer.unsetSelectable();
                layers[i].setSelectable();
            }
            selectLayer = layers[i];
            selectLayer.setActive(true);
            control.setBackgroundColor(selectLayer.color, false);
            if (shape != null) shape.changeLayer(selectLayer);
        }

        public void setResolution(Point res, bool back = false)
        {
            if (res.Equals(resolution))
            {
                return;
            }
            if (back)
            {
                control.setResolution(res.X, res.Y, false);
            }
            historyControl.add(new HistoryResolution(this, resolution, res));
            resolution = res;
            foreach (var l in layers)
            {
                l.setResolution(res);
            }
            canvas.Width = topCanvas.Width = res.X;
            canvas.Height = topCanvas.Height = res.Y;
        }

        public void clear()
        {
            clearCanvas();
        }

        public void clearCanvas()
        {
            stopDraw();
            resetLayers();
        }

        public void setShapePrimaryColor(Brush c)
        {
            primaryColor = c;
            if (shape != null) shape.setPrimaryColor(c, true);
        }

        public void setShapeSecondaryColor(Brush c)
        {
            secondaryColor = c;
            if (shape != null) shape.setSecondaryColor(c, true);
        }


        public void setBackgroundColor(Brush c)
        {
            if (selectLayer != null)
            {
                historyControl.add(new HistoryBackgroundColor(selectLayer, selectLayer.getColor(), c));
                selectLayer.setColor(c);
            }
        }

        public Brush getBackgroundColor()
        {
            if (selectLayer != null) return selectLayer.getColor();
            return null;
        }

        public Brush getShapePrimaryColor()
        {
            return primaryColor;
        }

        public Brush getShapeSecondaryColor()
        {
            return secondaryColor;
        }

        public void setShapeThickness(double t)
        {
            thickness = t;
            if (shape != null) shape.setThickness(t, true);
        }

        public double getShapeThickness()
        {
            return thickness;
        }

        public void shapeDelete()
        {
            if (shape != null)
            {
                shape.delete();
                draw = false;
                candraw = true;
                historyControl.add(new HistoryShapeDelete(shape));
                shape = null;
            }
        }

        public void setActiveShape(MyEnum s)
        {
            if (selectLayer != null)
            {
                if (s == MyEnum.SELECT)
                {
                    selectLayer.unsetSelectable();
                    selectLayer.setSelectable();
                }
                else
                {
                    selectLayer.unsetSelectable();
                }
            }
            activeShape = s;
        }

        public void mouseDown(MouseButtonEventArgs e)
        {
            if (selectLayer == null)
            {
                return;
            }
            if (candraw)
            {
                startDraw(e);
            }
            else
            {
                if (!shape.hitTest())
                {
                    Shapes.MyShape sh = shape;
                    stopDraw();
                    if (activeShape == MyEnum.SELECT)
                    {
                        selectLayer.unsetSelectable();
                        selectLayer.setSelectable();
                        
                    }
                    startDraw(e);
                }
            }
        }

        void startDraw(MouseButtonEventArgs e)
        {
            if (!draw && activeShape != MyEnum.SELECT)
            {
                switch (activeShape)
                {
                    case MyEnum.LINE:
                        shape = new Shapes.MyLine(this, selectLayer);
                        break;
                    case MyEnum.RECT:
                        shape = new Shapes.MyRectangle(this, selectLayer);
                        break;
                    case MyEnum.ELLIPSE:
                        shape = new Shapes.MyEllipse(this, selectLayer);
                        break;
                    case MyEnum.POLYGON:
                        shape = new Shapes.MyPolygon(this, selectLayer);
                        break;
                }
                historyControl.add(new HistoryShape(shape));
                shape.drawMouseDown(e.GetPosition(canvas), e);
            }
        }

        public void startMoveShape(Point start, Point mys)
        {
            posunStart = start;
            posunStartMys = mys;
            drag = true;
            shapeMoved = false;
        }

        public void mouseMove(Point e)
        {
            if (draw) shape.drawMouseMove(e);
            if (!candraw)
            {
                shape.moveDrag(e);
                if (drag)
                {
                    shape.moveShape(posunStart.X + (e.X - posunStartMys.X), posunStart.Y + (e.Y - posunStartMys.Y));
                }
            }
        }

        public void mouseUp(MouseButtonEventArgs e)
        {
            if (draw)
            {
                shape.drawMouseUp(e.GetPosition(canvas), e);
            }
            else if (!candraw)
            {
                if (!drag)
                {
                    shape.stopDrag();            
                }else
                {
                    drag = false;
                    Point start = posunStart;
                    Point stop = shape.getPosition();
                    if (!start.Equals(stop))
                    {
                        historyControl.add(new HistoryShapeMove(shape, start, stop));
                    }
                }
            }
        }

        public void stopDraw()
        {
            if (!candraw)
            {
                candraw = true;
                shape.stopEdit();
                shape = null;
                lockDraw();
            }
        }

        public void lockDraw()
        {
            canvas.Children.Remove(topCanvas);
            canvas.Children.Add(topCanvas);
        }

        public void setPrimaryColor(Brush c)
        {
            control.setPrimaryColor(c, false);
        }

        public void setSecondaryColor(Brush c)
        {
            control.setSecondaryColor(c, false);
        }

        public void setThickness(double t)
        {
            control.setThickness(t, false);
        }

        public void pasteImage(BitmapSource bmi)
        {
            stopDraw();
            ImageBrush brush = new ImageBrush(bmi);
            control.setResolution(bmi.Width, bmi.Height);
            Shapes.MyShape shape = new Shapes.MyImage(this, selectLayer, bmi, new System.Windows.Point(0, 0), bmi.Width, bmi.Height);
            historyControl.add(new HistoryShape(shape));
        }

        public void setPath(string p)
        {
            path = p;
            string name = new Regex("[a-zA-Z0-9]+.[a-zA-Z0-9]+$").Matches(path)[0].ToString();
            setName(name);
        }

        public void setName(string name)
        {
            this.name = name;
            tabItem.Header = name;
        }

        public Canvas create()
        {
            Canvas canvas = new Canvas();
            canvas.Width = resolution.X;
            canvas.Height = resolution.Y;
            canvas.Background = Brushes.Blue;
            foreach(var layer in layers)
            {
                canvas.Children.Add(layer.createImage());
            }
            return canvas;
        }
    }
}
