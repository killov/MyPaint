using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MyPaint.FileSaver
{
    public abstract class FileSaver
    {
        protected FileControl dc;

        public static async Task<bool> SaveAsFile(MainControl c, FileControl dc, string path)
        {
            Regex r = new Regex("\\.[a-zA-Z0-9]+$");
            string suffix = r.Matches(path)[0].ToString().ToLower();
            switch (suffix)
            {
                case ".html":
                    return await new HTML().Save(dc);
                case ".jpg":
                    return await new JPEG().Save(dc);
                case ".bmp":
                    return await new BMP().Save(dc);
                case ".png":
                default:
                    return await new PNG().Save(dc);
            }
        }

        public async Task<bool> Save(FileControl dc)
        {
            this.dc = dc;

            return await StartSTATask(() =>
            {
                bool fail = false;
                try
                {
                    SaveImage();
                }
                catch
                {
                    MessageBox.Show("Nepodařilo se uložit soubor");
                    fail = true;
                }

                return !fail;
            });
        }

        abstract protected void SaveImage();

        public static Task<TResult> StartSTATask<TResult>(Func<TResult> action)
        {
            var tcs = new TaskCompletionSource<TResult>();
            var thread = new Thread(() =>
            {
                try
                {
                    TResult result = action();
                    tcs.SetResult(result);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            return tcs.Task;
        }
    }
}
