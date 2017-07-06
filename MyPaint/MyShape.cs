using System;
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
    interface MyShape
    {
        void mouseDown(MouseButtonEventArgs e);

        void mouseMove(MouseEventArgs e);

        void mouseUp(MouseButtonEventArgs e);

        void moveDrag(MouseEventArgs e);

        void stopDrag();

        void stopDraw();

        void moveShape(double x, double y);

        string renderShape();
    }
}
