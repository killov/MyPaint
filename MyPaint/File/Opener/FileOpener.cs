using System.Text.RegularExpressions;
using System.Windows;

namespace MyPaint.FileOpener
{
    public abstract class FileOpener
    {
        protected FileControl dc;
        bool fail = false;

        public static FileControl OpenFromFile(string path)
        {
            Regex r = new Regex("\\.[a-zA-Z0-9]+$");
            string suffix = r.Matches(path)[0].ToString().ToLower();
            switch (suffix)
            {
                case ".html":
                    return new HTML().Open(path);
                case ".jpg":
                    return new JPEG().Open(path);
                case ".bmp":
                    return new BMP().Open(path);
                case ".png":
                    return new PNG().Open(path);
                default:
                    return null;
            }
        }

        public FileControl Open(string path)
        {

            dc = new FileControl();
            dc.SetPath(path);

            try
            {
                Thread_open();
                fail = false;
            }
            catch
            {
                fail = true;
            }

            if (fail)
            {
                MessageBox.Show("Nepodařilo se otevřít soubor");

                return null;
            }
            return dc;
        }

        abstract protected void Thread_open();
    }
}
