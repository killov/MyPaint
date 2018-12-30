using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.IO;

namespace MyPaint.FileSaver
{
    public class HTML : FileSaver
    {
        override protected void Thread_save()
        {
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
            pic.Resolution = new Serializer.Point(dc.Resolution.X, dc.Resolution.Y);
            pic.Layers = new List<Serializer.Layer>();
            foreach (var layer in dc.layers)
            {
                pic.Layers.Add(layer.CreateSerializer());
            }

            var json = JsonConvert.SerializeObject(pic, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            file.WriteLine("var json = " + json + ";");

            file.WriteLine("draw(0, -1);");

            file.WriteLine("</script>");
            file.WriteLine("</body>");
            file.WriteLine("</html>");
            file.Close();
        }
    }
}
