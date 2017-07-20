﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint.jsonDeserialize
{
    public class Brush
    {
        public string type;
        public Byte R, G, B, A;
        public Point S, E;
        public List<LinearGradientStop> stops;

        public System.Windows.Media.Color createColor()
        {
            return Color.FromArgb(A, R, G, B);
        }

        public System.Windows.Media.Brush createBrush()
        {
            switch (type)
            {
                case "COLOR":
                    return new SolidColorBrush(Color.FromArgb(A, R, G, B));
                case "LG":
                    LinearGradientBrush lg = new LinearGradientBrush();
                    lg.StartPoint = new System.Windows.Point(S.x,S.y);
                    lg.EndPoint = new System.Windows.Point(E.x, E.y);
                    foreach(var stop in stops)
                    {
                        lg.GradientStops.Add(new GradientStop(stop.color.createColor(), stop.offset));
                    }
                    return lg;
                default:
                    return null;
            }
        }
    }
}