﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.History
{
    public class HistoryLayerPosition : IHistoryNode
    {
        Layer layer;
        int oldP, newP;
        public HistoryLayerPosition(Layer l, int o, int n)
        {
            layer = l;
            oldP = o;
            newP = n;
        }

        public void back()
        {
            layer.SetPosition(oldP);
        }

        public void forward()
        {
            layer.SetPosition(newP);
        }
    }
}
