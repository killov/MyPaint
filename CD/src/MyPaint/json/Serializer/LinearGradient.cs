using System.Collections.Generic;
using System.Windows.Media;

namespace MyPaint.Serializer
{
    public class LinearGradient : Brush
    {
        public string type = "LG";
        public Point S, E;
        public List<GradientStop> stops;

        public LinearGradient()
        {
        }

        public LinearGradient(LinearGradientBrush brush)
        {
            S = new Serializer.Point(brush.StartPoint.X, brush.StartPoint.Y);
            E = new Serializer.Point(brush.EndPoint.X, brush.EndPoint.Y);
            stops = new List<Serializer.GradientStop>();
            foreach (var stop in brush.GradientStops)
            {
                GradientStop s = new GradientStop();
                s.Color = new Serializer.Color(stop.Color.R, stop.Color.G, stop.Color.B, stop.Color.A);
                s.Offset = stop.Offset;
                stops.Add(s);
            }
        }

        public override System.Windows.Media.Brush CreateBrush()
        {
            LinearGradientBrush lg = new LinearGradientBrush();
            lg.StartPoint = new System.Windows.Point(S.X, S.Y);
            lg.EndPoint = new System.Windows.Point(E.X, E.Y);
            foreach (var stop in stops)
            {
                lg.GradientStops.Add(new System.Windows.Media.GradientStop(stop.Color.createColor(), stop.Offset));
            }
            return lg;
        }
    }
}
