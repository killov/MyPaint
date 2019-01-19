using System.Windows;
using MyPaint.Shapes;

namespace MyPaint
{
    public delegate void OnMouseDownDelegate(Point e, Shape s, bool enableMoving = true);
}
