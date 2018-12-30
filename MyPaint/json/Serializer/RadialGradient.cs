using System.Collections.Generic;
using System.Windows.Media;

namespace MyPaint.Serializer
{
    public class RadialGradient : Brush
    {
        public string type = "RG";
        public Point S, E, RA;
        public List<GradientStop> Stops;

        public RadialGradient()
        {
        }

        public RadialGradient(RadialGradientBrush brush)
        {
            S = new Serializer.Point(brush.GradientOrigin);
            E = new Serializer.Point(brush.Center);
            RA = new Serializer.Point(brush.RadiusX, brush.RadiusY);
            Stops = new List<Serializer.GradientStop>();
            foreach (var stop in brush.GradientStops)
            {
                GradientStop s = new GradientStop();
                s.Color = new Serializer.Color(stop.Color.R, stop.Color.G, stop.Color.B, stop.Color.A);
                s.Offset = stop.Offset;
                Stops.Add(s);
            }
        }

        public override System.Windows.Media.Brush CreateBrush()
        {
            RadialGradientBrush rg = new RadialGradientBrush();
            rg.GradientOrigin = new System.Windows.Point(S.X, S.Y);
            rg.Center = new System.Windows.Point(E.X, E.Y);
            rg.RadiusX = RA.X;
            rg.RadiusY = RA.Y;
            foreach (var stop in Stops)
            {
                rg.GradientStops.Add(new System.Windows.Media.GradientStop(stop.Color.createColor(), stop.Offset));
            }
            return rg;
        }
    }
}
