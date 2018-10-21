using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MyPaint.FileSaver
{
    public class HTML : FileSaver
    {
        override protected void Thread_save() { 
            string filename = dc.Path;
            System.IO.StreamWriter file = new System.IO.StreamWriter(filename);
            file.WriteLine("<!DOCTYPE HTML>");
            file.WriteLine("<html>");
            file.WriteLine("<head>");
            file.WriteLine("<meta http-equiv=\"content-type\" content=\"text/html; charset = utf-8\">");
            
            file.WriteLine("</head>");
            file.WriteLine("<body>");
            file.WriteLine("<canvas width=\"" + dc.Resolution.X + "\" height=\"" + dc.Resolution.Y + "\" style=\"border: 1px solid black;\" id=\"MyPaint\"></canvas>");
            file.WriteLine("<script>");
            StreamReader sr = new StreamReader("..\\..\\js.js");
            var minifier = new Microsoft.Ajax.Utilities.Minifier();
            file.WriteLine(minifier.MinifyJavaScript(sr.ReadToEnd()));
            file.WriteLine("var ctx = document.getElementById(\"MyPaint\").getContext(\"2d\");");
            var pic = new Serializer.Picture();
            pic.resolution = new Serializer.Point(dc.Resolution.X, dc.Resolution.Y);
            pic.layers = new List<Serializer.Layer>();
            foreach (var layer in dc.layers)
            {
                pic.layers.Add(layer.CreateSerializer());
            }
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = int.MaxValue;
            var json = s.Serialize(pic);
            file.WriteLine("var json = " + json + ";");
            //file.WriteLine("function draw(t,e){if(!(json.layers.length<=t))if(-1==e)json.layers[t].visible?(ctx.beginPath(),ctx.moveTo(.5,.5),ctx.rect(.5,.5,json.resolution.x+.5,json.resolution.y+.5),ctx.fillStyle=brush(json.layers[t].color,{x:0,y:0},json.resolution.x,json.resolution.y),ctx.fill()):t++,draw(t,0);else{if(json.layers[t].shapes.length<=e)return void draw(t+1,-1);var a=json.layers[t].shapes[e];switch(ctx.beginPath(),ctx.lineWidth=a.lineWidth,a.type){case'LINE':(o={}).x=Math.min(a.A.x,a.B.x),o.y=Math.min(a.A.y,a.B.y);var x=Math.abs(a.A.x-a.B.x),s=Math.abs(a.A.y-a.B.y);ctx.strokeStyle=brush(a.stroke,o,x,s),ctx.moveTo(a.A.x+.5,a.A.y+.5),ctx.lineTo(a.B.x+.5,a.B.y+.5),ctx.closePath(),ctx.stroke(),ctx.fill(),draw(t,e+1);break;case'RECTANGLE':(o={}).x=Math.min(a.A.x,a.B.x),o.y=Math.min(a.A.y,a.B.y);x=Math.abs(a.A.x-a.B.x),s=Math.abs(a.A.y-a.B.y);ctx.strokeStyle=brush(a.stroke,o,x,s),ctx.fillStyle=brush(a.fill,o,x,s),ctx.moveTo(a.A.x+.5,a.A.y+.5),ctx.lineTo(a.B.x+.5,a.A.y+.5),ctx.lineTo(a.B.x+.5,a.B.y+.5),ctx.lineTo(a.A.x+.5,a.B.y+.5),ctx.closePath(),ctx.stroke(),ctx.fill(),draw(t,e+1);break;case'ELLIPSE':(o={}).x=Math.min(a.A.x,a.B.x),o.y=Math.min(a.A.y,a.B.y);x=Math.abs(a.A.x-a.B.x),s=Math.abs(a.A.y-a.B.y);ctx.strokeStyle=brush(a.stroke,o,x,s),ctx.fillStyle=brush(a.fill,o,x,s),ctx.ellipse((a.A.x+a.B.x)/2,(a.A.y+a.B.y)/2,Math.abs(a.A.x-a.B.x)/2,Math.abs(a.A.y-a.B.y)/2,0,0,2*Math.PI),ctx.closePath(),ctx.stroke(),ctx.fill(),draw(t,e+1);break;case'POLYLINE':var o={x:1/0,y:1/0},r={x:-1/0,y:-1/0};for(var i in a.points)o.x=Math.min(o.x,a.points[i].x),o.y=Math.min(o.y,a.points[i].y),r.x=Math.max(r.x,a.points[i].x),r.y=Math.max(r.y,a.points[i].y);x=r.x-o.x,s=r.y-o.y;ctx.strokeStyle=brush(a.stroke,o,x,s),ctx.fillStyle=brush(a.fill,o,x,s),ctx.moveTo(a.points[0].x+.5,a.points[0].y+.5);for(i=1;i<a.points.length;i++)ctx.lineTo(a.points[i].x+.5,a.points[i].y+.5);ctx.stroke(),ctx.fill('evenodd'),draw(t,e+1);break;case'QLINE':ctx.strokeStyle=brush(a.stroke,o,x,s),ctx.fillStyle=brush(a.fill,o,x,s),ctx.beginPath(),ctx.moveTo(a.A.x+.5,a.A.y+.5),ctx.quadraticCurveTo(a.B.x+.5,a.B.y+.5,a.C.x+.5,a.C.y+.5),ctx.closePath(),ctx.stroke(),ctx.fill(),draw(t,e+1);break;case'TEXT':var l=ctx.getImageData(a.A.x,a.A.y,a.w,a.h);l=imageText(a,l),ctx.putImageData(l,a.A.x,a.A.y),draw(t,e+1);break;case'POLYGON':o={x:1/0,y:1/0},r={x:-1/0,y:-1/0};for(var i in a.points)o.x=Math.min(o.x,a.points[i].x),o.y=Math.min(o.y,a.points[i].y),r.x=Math.max(r.x,a.points[i].x),r.y=Math.max(r.y,a.points[i].y);x=r.x-o.x,s=r.y-o.y;ctx.strokeStyle=brush(a.stroke,o,x,s),ctx.fillStyle=brush(a.fill,o,x,s),ctx.moveTo(a.points[0].x+.5,a.points[0].y+.5);for(i=1;i<a.points.length;i++)ctx.lineTo(a.points[i].x+.5,a.points[i].y+.5);ctx.closePath(),ctx.stroke(),ctx.fill('evenodd'),draw(t,e+1);break;case'IMAGE':var n=new Image;n.onload=function(){ctx.moveTo(.5,.5),ctx.drawImage(n,a.A.x,a.A.y),draw(t,e+1)},n.src='data:image/png;base64,'+a.b64}}}function brush(t,e,a,x){if(null==t)return'rgba(0,0,0,0)';switch(t.type){case'COLOR':return'rgba('+t.R+','+t.G+','+t.B+','+(t.A/255).toString().replace(',','.')+')';case'LG':var s=ctx.createLinearGradient(e.x+t.S.x*a,e.y+t.S.y*x,e.x+t.E.x*a,e.y+t.E.y*x);for(var o in t.stops)s.addColorStop(t.stops[o].offset,brush(t.stops[o].color));return s;case'RG':s=ctx.createRadialGradient(e.x+t.E.x*a,e.y+t.E.y*x,0,e.x+t.E.x*a,e.y+t.E.y*x,a/2);for(var o in t.stops)s.addColorStop(t.stops[o].offset,brush(t.stops[o].color));return s;case'NULL':return'rgba(0,0,0,0)'}}function imageText(t,e){var a=document.createElement('canvas');a.width=t.w,a.height=t.h;var x=a.getContext('2d');x.putImageData(e,0,0),x.beginPath(),x.moveTo(0,0),x.rect(0,0,t.w,t.h),x.fillStyle=brush(t.fill,{x:0,y:0},t.w,t.h),x.fill(),x.textBaseline='top',x.font=t.lineWidth+'px '+t.font,x.fillStyle=brush(t.stroke,{x:0,y:0},t.w,t.h);for(var s=t.b64.split('\\n'),o=0;o<s.length;o++)x.fillStyle=brush(t.stroke,{x:0,y:1.15*t.lineWidth*o},x.measureText(s[o]).width,1.15*t.lineWidth),x.fillText(s[o],0,0+o*(1.15*t.lineWidth));return x.getImageData(0,0,t.w,t.h)};");
            file.WriteLine("draw(0, -1);");

            file.WriteLine("</script>");
            file.WriteLine("</body>");
            file.WriteLine("</html>");
            file.Close();
        }
    }
}
