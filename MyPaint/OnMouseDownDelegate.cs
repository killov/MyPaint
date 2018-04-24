using MyPaint.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyPaint
{
    public delegate void OnMouseDownDelegate(Point e, Shape s, bool enableMoving = true);
}
