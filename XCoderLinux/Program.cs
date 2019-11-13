using System;
using System.Threading.Tasks;
using Gtk;
using NewLife.Log;
using NewLife.Threading;

namespace XCoder
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            XTrace.UseConsole();

            GLib.ExceptionManager.UnhandledException += ExceptionManager_UnhandledException;

            // 检查环境
            var task = Task.Run(async () =>
            {
                var gtk = new GtkHelper { Log = XTrace.Log };
                if (!gtk.Check()) await gtk.DownloadAsync();

                gtk.Install();
            });
            // 最多等3秒
            task.Wait(3_000);

            Application.Init();
            var window = new SharpApp();
            window.ShowAll();
            //XTrace2.UseWinForm(window);
            Application.Run();
        }

        private static void ExceptionManager_UnhandledException(GLib.UnhandledExceptionArgs args)
        {
            if (args.ExceptionObject is Exception ex) XTrace.WriteException(ex);
        }
    }
}
