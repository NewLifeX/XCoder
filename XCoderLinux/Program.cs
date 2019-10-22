using System;
using Gtk;

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
            new SharpApp();
            Application.Run();
        }
    }
}
