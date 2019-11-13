using System;
using Gtk;
using NewLife.Log;

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
