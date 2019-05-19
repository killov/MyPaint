using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Text.RegularExpressions;

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
                dc.Resolution = new System.Windows.Point(width, height);
                Regex r = new Regex("var json = (.+);");
                string json = r.Matches(code)[0].Groups[1].ToString();

                Serializer.Picture pic = JsonConvert.DeserializeObject<Serializer.Picture>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    }
                });

                foreach (var l in pic.Layers)
                {
                    dc.layers.Add(new Layer(dc, l));
                }
            }
        }

    }
}
