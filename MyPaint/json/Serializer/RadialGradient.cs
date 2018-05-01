using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint.Serializer
{
    public class RadialGradient : Brush
    {
        public string type = "RG";
        public Point S, E, RA;
        public List<GradientStop> stops;

        public RadialGradient(RadialGradientBrush brush)
        {
            S = new Serializer.Point(brush.GradientOrigin);
            E = new Serializer.Point(brush.Center);
            RA = new Serializer.Point(brush.RadiusX, brush.RadiusY);
            stops = new List<Serializer.GradientStop>();
            foreach (var stop in brush.GradientStops)
            {
                GradientStop s = new GradientStop();
                s.color = new Serializer.Color(stop.Color.R, stop.Color.G, stop.Color.B, stop.Color.A);
                s.offset = stop.Offset;
                stops.Add(s);
            }
        }

        public override System.Windows.Media.Brush CreateBrush()
        {
            RadialGradientBrush rg = new RadialGradientBrush();
            rg.GradientOrigin = new System.Windows.Point(S.x, S.y);
            rg.Center = new System.Windows.Point(E.x, E.y);
            rg.RadiusX = RA.x;
            rg.RadiusY = RA.y;
            foreach (var stop in stops)
            {
                rg.GradientStops.Add(new System.Windows.Media.GradientStop(stop.color.createColor(), stop.offset));
            }
            return rg;
        }
    }
}
