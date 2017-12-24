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

        public void copy(BitmapSource s)
        {
            //todo
        }

        public void copy(Shapes.MyShape s)
        {
            //todos
        }

        public void paste()
        {
            if (Clipboard.ContainsImage())
            {
                pasteImage();
            }
        }

        void pasteImage()
        {
            BitmapSource s = Clipboard.GetImage();
            Clipboard.SetImage(s);
            s = Clipboard.GetImage();
            control.drawControl.pasteImage(s);
        }
    }
}
