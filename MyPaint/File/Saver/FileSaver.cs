﻿using System;
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


namespace MyPaint.FileSaver
{
    public abstract class FileSaver
    {
        protected FileControl dc;

        public static void SaveAsFile(MainControl c, FileControl dc,string path)
        {
            Regex r = new Regex("\\.[a-zA-Z0-9]+$");
            string suffix = r.Matches(path)[0].ToString().ToLower();
            switch (suffix)
            {
                case ".html":
                    new HTML().Save(dc);
                    break;
                case ".jpg":
                    new JPEG().Save(dc); ;
                    break;
                case ".bmp":
                    new BMP().Save(dc);
                    break;
                case ".png":
                default:
                    new PNG().Save(dc);
                    break;
            }
        }

        public void Save(FileControl dc)
        {
            this.dc = dc;
            Thread t = new Thread(Save);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void Save()
        {
            try
            {
                Thread_save();
            }
            catch
            {

            }
        }

        abstract protected void Thread_save();
    }
}
