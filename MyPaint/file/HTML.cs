using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MyPaint.file
{
    public class HTML
    {
        public static void open(DrawControl dc, string filename)
        {
            using (StreamReader sr = new StreamReader(@filename))
            {
                string code = sr.ReadToEnd();
                string a = new Regex("width=\"(.+?)\"").Matches(code)[0].Groups[1].ToString();
                double width = Double.Parse(new Regex("width=\"(.+?)\"").Matches(code)[0].Groups[1].ToString());
                double height = Double.Parse(new Regex("height=\"(.+?)\"").Matches(code)[0].Groups[1].ToString());
                dc.control.setResolution(width, height);
                Regex r = new Regex("var json = (.+);");
                string json = r.Matches(code)[0].Groups[1].ToString();
                jsonDeserialize.Picture pic = (jsonDeserialize.Picture)new JavaScriptSerializer().Deserialize(json, typeof(jsonDeserialize.Picture));
                dc.layers.Clear();
                foreach (var l in pic.layers)
                {
                    dc.layers.Add(new MyLayer(dc.canvas, dc, l));
                }
                dc.setActiveLayer(dc.layers.Count - 1);
            }
            dc.lockDraw();
        }

        public static void save(DrawControl dc, string filename)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(filename);
            file.WriteLine("<!DOCTYPE HTML>");
            file.WriteLine("<html>");
            file.WriteLine("<head>");
            file.WriteLine("<meta http-equiv=\"content-type\" content=\"text/html; charset = utf-8\">");
            file.WriteLine("</head>");
            file.WriteLine("<body>");
            file.WriteLine("<canvas width=\"" + dc.resolution.X + "\" height=\"" + dc.resolution.Y + "\" style=\"border: 1px solid black;\" id=\"MyPaint\"></canvas>");
            file.WriteLine("<script>");
            file.WriteLine("var ctx = document.getElementById(\"MyPaint\").getContext(\"2d\");");
            var pic = new jsonSerialize.Picture();
            pic.resolution = new jsonSerialize.Point(dc.resolution.X, dc.resolution.Y);
            pic.layers = new List<jsonSerialize.Layer>();
            foreach (var layer in dc.layers)
            {
                pic.layers.Add(layer.render());
            }
            var json = new JavaScriptSerializer().Serialize(pic);
            file.WriteLine("var json = " + json + ";");

            file.WriteLine("function draw(t,a){if(!(json.layers.length<=t))if(-1==a)ctx.moveTo(0,0),ctx.rect(0,0,json.resolution.x,json.resolution.y),ctx.fillStyle=brush(json.layers[t].color,{x:0,y:0},json.resolution.x,json.resolution.y),ctx.fill(),draw(t,0);else{if(json.layers[t].shapes.length<=a)return void draw(t+1,-1);var e=json.layers[t].shapes[a];switch(ctx.beginPath(),ctx.lineWidth=e.lineWidth,e.type){case'LINE':(o={}).x=Math.min(e.A.x,e.B.x),o.y=Math.min(e.A.y,e.B.y);var s=Math.abs(e.A.x-e.B.x),x=Math.abs(e.A.y-e.B.y);ctx.strokeStyle=brush(e.stroke,o,s,x),ctx.moveTo(e.A.x,e.A.y),ctx.lineTo(e.B.x,e.B.y),ctx.closePath(),ctx.stroke(),ctx.fill(),draw(t,a+1);break;case'RECTANGLE':(o={}).x=Math.min(e.A.x,e.B.x),o.y=Math.min(e.A.y,e.B.y);var s=Math.abs(e.A.x-e.B.x),x=Math.abs(e.A.y-e.B.y);ctx.strokeStyle=brush(e.stroke,o,s,x),ctx.fillStyle=brush(e.fill,o,s,x),ctx.moveTo(e.A.x,e.A.y),ctx.lineTo(e.B.x,e.A.y),ctx.lineTo(e.B.x,e.B.y),ctx.lineTo(e.A.x,e.B.y),ctx.closePath(),ctx.stroke(),ctx.fill(),draw(t,a+1);break;case'ELLIPSE':(o={}).x=Math.min(e.A.x,e.B.x),o.y=Math.min(e.A.y,e.B.y);var s=Math.abs(e.A.x-e.B.x),x=Math.abs(e.A.y-e.B.y);ctx.strokeStyle=brush(e.stroke,o,s,x),ctx.fillStyle=brush(e.fill,o,s,x),ctx.ellipse((e.A.x+e.B.x)/2,(e.A.y+e.B.y)/2,Math.abs(e.A.x-e.B.x)/2,Math.abs(e.A.y-e.B.y)/2,0,0,2*Math.PI),ctx.closePath(),ctx.stroke(),ctx.fill(),draw(t,a+1);break;case'POLYGON':var o={};o.x=1/0,o.y=1/0;var r={};r.x=-1/0,r.y=-1/0;for(var t in e.points)o.x=Math.min(o.x,e.points[t].x),o.y=Math.min(o.y,e.points[t].y),r.x=Math.max(r.x,e.points[t].x),r.y=Math.max(r.y,e.points[t].y);var s=r.x-o.x,x=r.y-o.y;ctx.strokeStyle=brush(e.stroke,o,s,x),ctx.fillStyle=brush(e.fill,o,s,x),ctx.moveTo(e.points[0].x,e.points[0].y);for(t=1;t<e.points.length;t++)ctx.lineTo(e.points[t].x,e.points[t].y);ctx.closePath(),ctx.stroke(),ctx.fill('evenodd'),draw(t,a+1);break;case'IMAGE':var l=new Image;l.onload=function(){ctx.moveTo(0,0),ctx.drawImage(l,e.A.x,e.A.y),draw(t,a+1)},l.src='data:image/png;base64,'+e.b64}}}function brush(t,a,e,s){if(null==t)return'rgba(0,0,0,0)';switch(t.type){case'COLOR':return'rgba('+t.R+','+t.G+','+t.B+','+(t.A/255).toString().replace(',','.')+')';case'LG':var x=ctx.createLinearGradient(a.x+t.S.x*e,a.y+t.S.y*s,a.x+t.E.x*e,a.y+t.E.y*s);for(var o in t.stops)x.addColorStop(t.stops[o].offset,brush(t.stops[o].color));return x}}draw(0,-1);");
            file.WriteLine("</script>");
            file.WriteLine("</body>");
            file.WriteLine("</html>");
            file.Close();
        }
    }
}
