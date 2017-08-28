﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyPaint
{
    public class Utils
    {
        static public jsonSerialize.Brush BrushToCanvas(Brush b)
        {
            if(b is SolidColorBrush)
            {
                Color c = ((SolidColorBrush)b).Color;
                return new jsonSerialize.Color(c.R, c.G, c.B, c.A);
            }
            if (b is LinearGradientBrush)
            {
                LinearGradientBrush g = (LinearGradientBrush)b;
                jsonSerialize.LinearGradient ret = new jsonSerialize.LinearGradient();
                ret.S = new jsonSerialize.Point(g.StartPoint.X, g.StartPoint.Y);
                ret.E = new jsonSerialize.Point(g.EndPoint.X, g.EndPoint.Y);
                ret.stops = new List<jsonSerialize.GradientStop>();
                foreach (var stop in g.GradientStops)
                {
                    jsonSerialize.GradientStop s = new jsonSerialize.GradientStop();
                    s.color = new jsonSerialize.Color(stop.Color.R, stop.Color.G, stop.Color.B, stop.Color.A);
                    s.offset = stop.Offset;
                    ret.stops.Add(s);
                }
                return ret;
            }
            if (b is RadialGradientBrush)
            {
                RadialGradientBrush g = (RadialGradientBrush)b;
                jsonSerialize.RadialGradient ret = new jsonSerialize.RadialGradient();

                

                ret.S = new jsonSerialize.Point(g.GradientOrigin.X, g.GradientOrigin.Y);
                ret.E = new jsonSerialize.Point(g.Center.X, g.Center.Y);
                ret.RA = new jsonSerialize.Point(g.RadiusX, g.RadiusY);
                ret.stops = new List<jsonSerialize.GradientStop>();
                foreach (var stop in g.GradientStops)
                {
                    jsonSerialize.GradientStop s = new jsonSerialize.GradientStop();
                    s.color = new jsonSerialize.Color(stop.Color.R, stop.Color.G, stop.Color.B, stop.Color.A);
                    s.offset = stop.Offset;
                    ret.stops.Add(s);
                }
                return ret;
            }
            return null;
        }

        public static byte[] ImageSourceToBytes(BitmapEncoder encoder, ImageSource imageSource)
        {
            byte[] bytes = null;
            var bitmapSource = imageSource as BitmapSource;

            if (bitmapSource != null)
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    bytes = stream.ToArray();
                }
            }

            return bytes;
        }

    }

   


}
