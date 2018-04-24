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
