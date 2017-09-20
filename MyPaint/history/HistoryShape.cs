﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint
{
    public class HistoryShape : IHistoryNode
    {
        public IMyShape shape;
        public HistoryShape(IMyShape s)
        {
            shape = s;
        }

        public void back()
        {
            shape.delete();
        }

        public void forward()
        {
            shape.refresh();
        }
    }
}
