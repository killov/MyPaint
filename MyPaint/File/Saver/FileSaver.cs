﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        public void Save(FileControl dc)
        {
            this.dc = dc;
            Thread t = new Thread(Thread_save);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        abstract protected void Thread_save();
    }
}