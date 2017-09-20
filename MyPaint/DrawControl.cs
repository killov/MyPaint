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

namespace MyPaint
{
    public class DrawControl
    {
        public MainControl control;
        public Brush nullBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 255));
        public Canvas canvas;
        public Canvas topCanvas;
        public Brush primaryColor, secondaryColor;
        public double thickness;
        public IMyShape shape;
        public bool draw = false;
        public bool startdraw = false;
        public bool candraw = true;
        public bool drag = false;
        Point posunStart = new Point();
        Point posunStartMys = new Point();
        public Point resolution;
        MyEnum activeShape;
        public ObservableCollection<MyLayer> layers = new ObservableCollection<MyLayer>();
        public MyLayer selectLayer;
        public ScaleTransform revScale;
        private int layerCounter = 1;

        public DrawControl(MainControl c, Canvas ca, Canvas tc, ScaleTransform revScale)
        {
            control = c;
            canvas = ca;
            topCanvas = tc;
            resetLayers();
            this.revScale = revScale;
        }

        public void addLayer()
        {
            MyLayer layer = new MyLayer(canvas, this) { Name = "layer_" + layerCounter, visible = true };
            layers.Add(layer);
            layerCounter++;
            setActiveLayer(layers.Count - 1);
            lockDraw();
            control.addHistory(new HistoryLayerAdd(layer));
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

        public void setActiveLayer(int i)
        {
            if (i == -1) return;
            control.w.layers.SelectedIndex = i;
            if(activeShape == MyEnum.SELECT)
            {
                if (shape != null) shape.changeLayer(null);
                if(selectLayer != null) selectLayer.unsetSelectable();
                layers[i].setSelectable();
            }
            selectLayer = layers[i];
            control.setBackgroundColor(selectLayer.color);
            if (shape != null) shape.changeLayer(selectLayer);
        }

        public void setResolution(Point res)
        {
            resolution = res;
            foreach(var l in layers)
            {
                l.setResolution(res);
            }
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

        public void setPrimaryColor(Brush c)
        {
            primaryColor = c;
            if (shape != null) shape.setPrimaryColor(c);
        }

        public void setSecondaryColor(Brush c)
        {
            secondaryColor = c;
            if (shape != null) shape.setSecondaryColor(c);
        }


        public void setBackgroundColor(Brush c)
        {
            if (selectLayer != null) selectLayer.setColor(c);
        }

        public Brush getPrimaryColor()
        {
            return primaryColor;
        }

        public Brush getSecondaryColor()
        {
            return secondaryColor;
        }

        public Brush getBackgroundColor()
        {
            return selectLayer.color;
        }

        public void setThickness(double t)
        {
            thickness = t;
            if (shape != null) shape.setThickness(t);
        }

        public void delete()
        {
            if (shape != null)
            {
                shape.delete();
                draw = false;
                candraw = true;
                control.addHistory(new HistoryShapeDelete(shape));
                shape = null;               
            }
        }

        public void setActiveShape(MyEnum s)
        {
            if(selectLayer != null)
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
            if(selectLayer == null)
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
                    IMyShape sh = shape;
                    stopDraw();
                    if (activeShape == MyEnum.SELECT)
                    {
                        selectLayer.setShapeSelectable(sh);
                    }
                    startDraw(e);
                }
            }
        }

        void startDraw(MouseButtonEventArgs e)
        {
            if (!draw && activeShape != MyEnum.SELECT)
            {
                control.setChange(true);
                switch (activeShape)
                {
                    case MyEnum.LINE:
                        shape = new MyLine(this, selectLayer);
                        break;
                    case MyEnum.RECT:
                        shape = new MyRectangle(this, selectLayer);
                        break;
                    case MyEnum.ELLIPSE:
                        shape = new MyEllipse(this, selectLayer);
                        break;
                    case MyEnum.POLYGON:
                        shape = new MyPolygon(this, selectLayer);
                        break;
                }
                control.addHistory(new HistoryShape(shape));
                shape.setPrimaryColor(primaryColor);
                shape.setSecondaryColor(secondaryColor);
                shape.setThickness(thickness);
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
            if (draw) shape.mouseMove(e);
            if (!candraw)
            {
                shape.moveDrag(e);
                if (drag)
                {
                    shape.moveShape(posunStart.X + (e.GetPosition(canvas).X - posunStartMys.X), posunStart.Y + (e.GetPosition(canvas).Y - posunStartMys.Y));
                }
            }
        }

        public void mouseUp(MouseButtonEventArgs e)
        {
            if (draw)
            {
                shape.mouseUp(e);
            }
            else if (!candraw)
            {
                shape.stopDrag();
                drag = false;
                control.addHistory(new HistoryShapeMove(shape, posunStart, shape.getPosition()));
            }
        }

        public void stopDraw()
        {
            if (!candraw)
            {
                candraw = true;
                shape.stopDraw();
                shape = null;
                lockDraw();
            }

        }

        public void lockDraw()
        {
            control.lockDraw();
        }
    }
}
