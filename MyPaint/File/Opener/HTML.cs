using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MyPaint.FileOpener
{
    public class HTML : FileOpener
    {
        override protected void thread_open()
        {
            using (StreamReader sr = new StreamReader(dc.path))
            {
                string code = sr.ReadToEnd();
                string a = new Regex("width=\"(.+?)\"").Matches(code)[0].Groups[1].ToString();
                double width = Double.Parse(new Regex("width=\"(.+?)\"").Matches(code)[0].Groups[1].ToString());
                double height = Double.Parse(new Regex("height=\"(.+?)\"").Matches(code)[0].Groups[1].ToString());
                dc.setResolution(new System.Windows.Point(width, height), false, true);
                Regex r = new Regex("var json = (.+);");
                string json = r.Matches(code)[0].Groups[1].ToString();
                JavaScriptSerializer dd = new JavaScriptSerializer();
                dd.MaxJsonLength = int.MaxValue;
                jsonDeserialize.Picture pic = (jsonDeserialize.Picture)dd.Deserialize(json, typeof(jsonDeserialize.Picture));
                dc.deleteLayers();
                foreach (var l in pic.layers)
                {
                    dc.layers.Add(new MyLayer(dc.canvas, dc, l));
                }
                dc.setActiveLayer(dc.layers.Count - 1);
            }
            dc.lockDraw();
        }

    }
}
