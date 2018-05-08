using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace MyPaint.FileOpener
{
    public abstract class FileOpener
    {
        protected FileControl dc;
        protected MainControl control;


        public static void OpenFromFile(MainControl c, FileControl f, string path)
        {
            f.SetPath(path);
            Regex r = new Regex("\\.[a-zA-Z0-9]+$");
            string suffix = r.Matches(path)[0].ToString().ToLower();
            switch (suffix)
            {
                case ".html":
                new HTML().Open(c, f);
                break;
                case ".jpg":
                new JPEG().Open(c, f);
                break;
                case ".bmp":
                new BMP().Open(c, f);
                break;
                case ".png":
                new PNG().Open(c, f);
                break;
            }
            f.HistoryControl.Enable();
        }

        public void Open(MainControl c, FileControl dc)
        {
            control = c;
            this.dc = dc;
            try
            {
                Thread_open();
                dc.Control.SetFileActive(dc);
                c.AdjustZoom(dc.Resolution.X, dc.Resolution.Y);
            }
            catch
            {
                MessageBox.Show("Nepodařilo se otevřít soubor");
                control.FileClose(dc);
            }
            
        }


        abstract protected void Thread_open();
    }
}
