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
            Application.Init();
            var window = new SharpApp();
            Window.InteractiveDebugging = true;//打开调试工具
            XTrace2.UseWinForm(window);
            Application.Run();
        }
    }
}
