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
        DrawControl drawControl;
        public ClipboardControl(DrawControl dc)
        {
            drawControl = dc;
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
            
                drawControl.pasteImage(s);
            


        }
    }
}
