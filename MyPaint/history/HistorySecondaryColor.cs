﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint
{
    public class HistorySecondaryColor : IHistoryNode
    {
        public IMyShape shape;
        public Brush o, n;
        public HistorySecondaryColor(IMyShape s, Brush oldBrush, Brush newBrush)
        {
            shape = s;
            o = oldBrush;
            n = newBrush;
        }

        public void back()
        {
            shape.setSecondaryColor(o);
        }

        public void forward()
        {
            shape.setSecondaryColor(n);
        }
    }
}
