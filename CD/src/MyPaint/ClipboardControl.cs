using System.Windows;
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
            if (control.file.DrawControl.Shape != null && control.file.DrawControl.ShapeDrawed())
            {
                control.file.DrawControl.ShapeDelete();
            }
        }

        public void Copy()
        {
            if (control.file.DrawControl.Shape != null && control.file.DrawControl.ShapeDrawed())
            {
                Shapes.Shape s = control.file.DrawControl.Shape;
                if (s is Shapes.Image)
                {
                    Shapes.Image im = (Shapes.Image)s;
                    Clipboard.SetImage(im.CreateBitmap());
                    clipboard = null;
                }
                else if (s is Shapes.Area)
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
                if (clipboard != null)
                {
                    PasteShape(clipboard);
                }
            }
        }

        void PasteImage()
        {
            try
            {
                BitmapSource s = Clipboard.GetImage();
                Clipboard.SetImage(s);
                s = Clipboard.GetImage();
                control.file.DrawControl.PasteImage(s);
            }
            catch
            {

            }
        }

        void PasteShape(Serializer.Shape s)
        {


            control.file.DrawControl.PasteShape(s);
        }
    }
}
