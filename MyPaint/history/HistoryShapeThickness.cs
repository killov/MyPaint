﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint
{
    public class HistoryShapeThickness : IHistoryNodeSkipped
    {
        public MyShape shape;
        public double o, n;
        public HistoryShapeThickness(MyShape s, double oldT, double newT)
        {
            shape = s;
            o = oldT;
            n = newT;
        }

        public void back()
        {
            shape.setThickness(o);
        }

        public void forward()
        {
            shape.setThickness(n);
        }

        public void skip(IHistoryNodeSkipped node)
        {
            HistoryShapeThickness n = (HistoryShapeThickness)node;
            this.n = n.n;
        }

        public bool optimal(IHistoryNodeSkipped node)
        {
            return node is HistoryShapeThickness;
        }
    }
}
