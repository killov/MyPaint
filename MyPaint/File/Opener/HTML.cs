using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace MyPaint.FileOpener
{
    public class HTML : FileOpener
    {
        override protected void Thread_open()
        {
            using (StreamReader sr = new StreamReader(dc.Path))
            {
                string code = sr.ReadToEnd();
                string a = new Regex("width=\"(.+?)\"").Matches(code)[0].Groups[1].ToString();
                double width = Double.Parse(new Regex("width=\"(.+?)\"").Matches(code)[0].Groups[1].ToString());
                double height = Double.Parse(new Regex("height=\"(.+?)\"").Matches(code)[0].Groups[1].ToString());
                dc.SetResolution(new System.Windows.Point(width, height), false, true);
                Regex r = new Regex("var json = (.+);");
                string json = r.Matches(code)[0].Groups[1].ToString();
                JavaScriptSerializer dd = new JavaScriptSerializer();
                dd.MaxJsonLength = int.MaxValue;
                Deserializer.Picture pic = (Deserializer.Picture)dd.Deserialize(json, typeof(Deserializer.Picture));
                dc.DeleteLayers();
                foreach (var l in pic.layers)
                {
                    dc.layers.Add(new Layer(dc, l));
                }
                dc.DrawControl.SetActiveLayer(dc.layers.Count - 1);
            }
        }

    }
}
