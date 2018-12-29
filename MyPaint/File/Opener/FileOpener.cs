using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace MyPaint.FileOpener
{
    public abstract class FileOpener
    {
        protected FileControl dc;
        protected MainControl control;
        bool fail = false;

        public static async Task OpenFromFile(MainControl c, FileControl f, string path)
        {
            f.SetPath(path);
            Regex r = new Regex("\\.[a-zA-Z0-9]+$");
            string suffix = r.Matches(path)[0].ToString().ToLower();
            switch (suffix)
            {
                case ".html":
                    await new HTML().Open(c, f);
                    break;
                case ".jpg":
                    await new JPEG().Open(c, f);
                    break;
                case ".bmp":
                    await new BMP().Open(c, f);
                    break;
                case ".png":
                    await new PNG().Open(c, f);
                    break;
            }
            f.HistoryControl.Enable();
        }

        public async Task Open(MainControl c, FileControl dc)
        {
            control = c;
            this.dc = dc;

            await Task.Run(() =>
            {
                try
                {
                    Thread_open();
                    fail = false;
                }
                catch
                {
                    fail = true;
                }
            });
            if (fail)
            {
                MessageBox.Show("Nepodařilo se otevřít soubor");
                control.FileClose(dc);
                return;
            }
            dc.Control.SetFileActive(dc);
            c.AdjustZoom(dc.Resolution.X, dc.Resolution.Y);
        }


        abstract protected void Thread_open();
    }
}
