using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint.jsonSerialize
{
    public class LinearGradient : Brush
    {
        public string type = "LG";
        public Point S, E;
        public List<GradientStop> stops;

        public LinearGradient(LinearGradientBrush brush)
        {
            S = new jsonSerialize.Point(brush.StartPoint.X, brush.StartPoint.Y);
            E = new jsonSerialize.Point(brush.EndPoint.X, brush.EndPoint.Y);
            stops = new List<jsonSerialize.GradientStop>();
            foreach (var stop in brush.GradientStops)
            {
                GradientStop s = new GradientStop();
                s.color = new jsonSerialize.Color(stop.Color.R, stop.Color.G, stop.Color.B, stop.Color.A);
                s.offset = stop.Offset;
                stops.Add(s);
            }
        }

        public override System.Windows.Media.Brush createBrush()
        {
            LinearGradientBrush lg = new LinearGradientBrush();
            lg.StartPoint = new System.Windows.Point(S.x, S.y);
            lg.EndPoint = new System.Windows.Point(E.x, E.y);
            foreach (var stop in stops)
            {
                lg.GradientStops.Add(new System.Windows.Media.GradientStop(stop.color.createColor(), stop.offset));
            }
            return lg;
        }
    }
}
