﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.Serializer
{
    public class Rectangle : Shape
    {
        public string type = "RECTANGLE";

        public Brush stroke, fill;
        public double lineWidth;
        public Point A, B;
    }
}