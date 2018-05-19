using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace MyPaint
{
    class ClipboardControl
    {
        MainControl control;
        Serializer.Shape clipboard;

        public ClipboardControl(MainControl c)
        {
            control = c;
        }

        public void Cut()
        {
            Copy();
            if (control.file.Shape != null && control.file.ShapeDrawed())
            {
                control.file.ShapeDelete();
            }
        }

        public void Copy()
        {
            if (control.file.Shape != null && control.file.ShapeDrawed())
            {
                Shapes.Shape s = control.file.Shape;
                if (s is Shapes.Image)
                {
                    Shapes.Image im = (Shapes.Image)s;
                    Clipboard.SetImage(im.CreateBitmap());
                    clipboard = null;
                }else if (s is Shapes.Area)
                {
                    Shapes.Area area = (Shapes.Area)s;
                    Clipboard.SetImage(area.CreateBitmap());
                    clipboard = null;
                }
                else
                {
                    Clipboard.Clear();
                    clipboard = s.CreateSerializer();
                }

            }
        }

        public void Paste()
        {
            if (Clipboard.ContainsImage())
            {
                PasteImage();
            }
            else
            {
                if(clipboard != null)
                {
                    PasteShape(clipboard);
                }
            }
        }

        void PasteImage()
        {
            try { 
                BitmapSource s = Clipboard.GetImage();
                Clipboard.SetImage(s);
                s = Clipboard.GetImage();
                control.file.PasteImage(s);
            }
            catch
            {

            }
        }

        void PasteShape(Serializer.Shape s)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            ser.MaxJsonLength = int.MaxValue;
            var json = ser.Serialize(s);
            JavaScriptSerializer dd = new JavaScriptSerializer();
            dd.MaxJsonLength = int.MaxValue;
            Deserializer.Shape shape = (Deserializer.Shape)dd.Deserialize(json, typeof(Deserializer.Shape));
            control.file.PasteShape(shape);
        }
    }
}
