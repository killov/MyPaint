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
        public MyShape shape;
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
            if (activeShape == MyEnum.SELECT)
            {
                if (shape != null)
                {
                    shape.changeLayer(null);
                    control.addHistory(new HistoryShapeChangeLayer(shape, selectLayer, layers[i]));
                }
                if (selectLayer != null) selectLayer.unsetSelectable();
                layers[i].setSelectable();
            }
            selectLayer = layers[i];
            control.setBackgroundColor(selectLayer.color, false);
            if (shape != null) shape.changeLayer(selectLayer);
        }

        public void setResolution(Point res)
        {
            resolution = res;
            foreach (var l in layers)
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
                control.addHistory(new HistoryBackgroundColor(selectLayer, selectLayer.getColor(), c));
                selectLayer.setColor(c);
            }
        }

        public Brush getShapePrimaryColor()
        {
            return primaryColor;
        }

        public Brush getShapeSecondaryColor()
        {
            return secondaryColor;
        }

        public Brush getBackgroundColor()
        {
            return selectLayer.color;
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
                control.addHistory(new HistoryShapeDelete(shape));
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
                    MyShape sh = shape;
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
                        control.addHistory(new HistoryShapeMove(shape, start, stop));
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
            control.lockDraw();
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
    }
}
