﻿using System;
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
        public Brush nullBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        public Canvas canvas;
        public Canvas topCanvas;
        public Brush primaryColor, secondaryColor;
        public double thickness;
        public MyShape shape;
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

        public DrawControl(MainControl c, Canvas ca, Canvas tc)
        {
            control = c;
            canvas = ca;
            topCanvas = tc;
            resetLayers();    
        }

        public void addLayer()
        {
            layers.Add(new MyLayer(canvas, this) { Name = "ahoj", visible = true });
            setActiveLayer(layers.Count - 1);
            lockDraw();
        }

        public void resetLayers()
        {
            foreach(var l in layers)
            {
                l.delete();
            }
            layers.Clear();
            addLayer();
        }

        public void setActiveLayer(int i)
        {
            if (i == -1) return;
            control.w.layers.SelectedIndex = i;
            selectLayer = layers[i];
            control.setBackgroundColor(selectLayer.color);
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
            }
        }

        public void secActiveShape(MyEnum s)
        {
            activeShape = s;
        }

        public void mouseDown(MouseButtonEventArgs e)
        {
            if (candraw)
            {
                startDraw(e);
            }
            else
            {
                if (!shape.hitTest())
                {
                    stopDraw();
                    startDraw(e);
                }
            }
        }

        void startDraw(MouseButtonEventArgs e)
        {
            if (!draw)
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
                control.addHistory(shape);
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
