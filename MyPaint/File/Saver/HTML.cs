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
            string filename = dc.path;
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
            var pic = new Serializer.Picture();
            pic.resolution = new Serializer.Point(dc.resolution.X, dc.resolution.Y);
            pic.layers = new List<Serializer.Layer>();
            foreach (var layer in dc.layers)
            {
                pic.layers.Add(layer.CreateSerializer());
            }
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = int.MaxValue;
            var json = s.Serialize(pic);
            file.WriteLine("var json = " + json + ";");

            file.WriteLine("draw(0, -1); function draw(i, j) { if (json.layers.length <= i) return; if (j == -1) { ctx.moveTo(0, 0); ctx.rect(0, 0, json.resolution.x, json.resolution.y); ctx.fillStyle = brush(json.layers[i].color, { x: 0, y: 0 }, json.resolution.x, json.resolution.y); ctx.fill(); if (!json.layers[i].visible) { i++; } draw(i, 0); } else { if (json.layers[i].shapes.length <= j) { draw(i + 1, -1); return; } var shape = json.layers[i].shapes[j]; ctx.beginPath(); ctx.lineWidth = shape.lineWidth; switch (shape.type) { case 'LINE': var A = {}; A.x = Math.min(shape.A.x, shape.B.x); A.y = Math.min(shape.A.y, shape.B.y); var w = Math.abs(shape.A.x - shape.B.x); var h = Math.abs(shape.A.y - shape.B.y); ctx.strokeStyle = brush(shape.stroke, A, w, h); ctx.moveTo(shape.A.x, shape.A.y); ctx.lineTo(shape.B.x, shape.B.y); ctx.closePath(); ctx.stroke(); ctx.fill(); draw(i, j + 1); break; case 'RECTANGLE': var A = {}; A.x = Math.min(shape.A.x, shape.B.x); A.y = Math.min(shape.A.y, shape.B.y); var w = Math.abs(shape.A.x - shape.B.x); var h = Math.abs(shape.A.y - shape.B.y); ctx.strokeStyle = brush(shape.stroke, A, w, h); ctx.fillStyle = brush(shape.fill, A, w, h); ctx.moveTo(shape.A.x, shape.A.y); ctx.lineTo(shape.B.x, shape.A.y); ctx.lineTo(shape.B.x, shape.B.y); ctx.lineTo(shape.A.x, shape.B.y); ctx.closePath(); ctx.stroke(); ctx.fill(); draw(i, j + 1); break; case 'ELLIPSE': var A = {}; A.x = Math.min(shape.A.x, shape.B.x); A.y = Math.min(shape.A.y, shape.B.y); var w = Math.abs(shape.A.x - shape.B.x); var h = Math.abs(shape.A.y - shape.B.y); ctx.strokeStyle = brush(shape.stroke, A, w, h); ctx.fillStyle = brush(shape.fill, A, w, h); ctx.ellipse((shape.A.x + shape.B.x) / 2, (shape.A.y + shape.B.y) / 2, Math.abs(shape.A.x - shape.B.x) / 2, Math.abs(shape.A.y - shape.B.y) / 2, 0, 0, 2 * Math.PI); ctx.closePath(); ctx.stroke(); ctx.fill(); draw(i, j + 1); break; case 'POLYGON': var A = {}; A.x = Infinity; A.y = Infinity; var B = {}; B.x = -Infinity; B.y = -Infinity; for (var i in shape.points) { A.x = Math.min(A.x, shape.points[i].x); A.y = Math.min(A.y, shape.points[i].y); B.x = Math.max(B.x, shape.points[i].x); B.y = Math.max(B.y, shape.points[i].y); } var w = B.x - A.x; var h = B.y - A.y; ctx.strokeStyle = brush(shape.stroke, A, w, h); ctx.fillStyle = brush(shape.fill, A, w, h); ctx.moveTo(shape.points[0].x, shape.points[0].y); for (var i = 1; i < shape.points.length; i++) { ctx.lineTo(shape.points[i].x, shape.points[i].y); } ctx.closePath(); ctx.stroke(); ctx.fill('evenodd'); draw(i, j + 1); break; case 'IMAGE': var image = new Image(); image.onload = function () { ctx.moveTo(0, 0); ctx.drawImage(image, shape.A.x, shape.A.y); draw(i, j + 1); }; image.src = 'data:image/png;base64,' + shape.b64; } } }  function brush(b, A, w, h) { if (b == null) return 'rgba(0,0,0,0)'; switch (b.type) { case 'COLOR': return 'rgba(' + b.R + ',' + b.G + ',' + b.B + ',' + (b.A / 255.0).toString().replace(',', '.') + ')'; case 'LG': var grd = ctx.createLinearGradient(A.x + b.S.x * w, A.y + b.S.y * h, A.x + b.E.x * w, A.y + b.E.y * h); for (var i in b.stops) { grd.addColorStop(b.stops[i].offset, brush(b.stops[i].color)); } return grd; case 'RG': var grd = ctx.createRadialGradient(A.x + b.E.x * w, A.y + b.E.y * h, 0, A.x + b.E.x * w, A.y + b.E.y * h, w / 2);  for (var i in b.stops) { grd.addColorStop(b.stops[i].offset, brush(b.stops[i].color)); } return grd; } }");
            file.WriteLine("</script>");
            file.WriteLine("</body>");
            file.WriteLine("</html>");
            file.Close();
        }
    }
}
