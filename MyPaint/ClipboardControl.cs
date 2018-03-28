using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace MyPaint
{
    class ClipboardControl
    {
        MainControl control;
        public ClipboardControl(MainControl c)
        {
            control = c;
        }

        public void Copy(BitmapSource s)
        {
            while (true)
            {
                Console.Write("sdhasudgaisudg");
            }  
        }

        public void Copy(Shapes.Shape s)
        {
            //todos
        }

        public void Paste()
        {
            if (Clipboard.ContainsImage())
            {
                PasteImage();
            }
        }

        void PasteImage()
        {
            BitmapSource s = Clipboard.GetImage();
            Clipboard.SetImage(s);
            s = Clipboard.GetImage();
            control.file.PasteImage(s);
        }
    }
}
