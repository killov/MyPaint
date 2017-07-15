﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;

namespace MyPaint
{
    public interface MyShape
    {
        void setPrimaryColor(Brush b);

        void setSecondaryColor(Brush b);

        void setThickness(double b);

        void mouseDown(MouseButtonEventArgs e);

        void mouseMove(MouseEventArgs e);

        void mouseUp(MouseButtonEventArgs e);

        void moveDrag(MouseEventArgs e);

        void stopDrag();

        void stopDraw();

        void moveShape(double x, double y);

        json.Shape renderShape();

        void setHit(bool hit);

        bool hitTest();

        void delete();
    }
}
