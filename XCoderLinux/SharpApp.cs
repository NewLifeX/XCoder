using Gtk;
//using XApi;
using XNet;

namespace XCoder
{
    class SharpApp : Window
    {
        #region 窗口初始化

        #endregion
        VBox vbox = new VBox(false, 2);

        public SharpApp() : base("新生命码神工具")
        {
            //SetDefaultSize(250, 200);
            SetPosition(WindowPosition.Center);

            DeleteEvent += delegate { Application.Quit(); };

            AddMenu();

            //AddButton();


            var xapi = new FrmMain();

            vbox.PackStart(xapi, true, true, 0);

            Add(vbox);

            //Show();
            ShowAll();
        }
        
        void AddMenu()
        {
            var mb = new MenuBar();

            var filemenu = new Menu();
            var file = new MenuItem("文件")
            {
                Submenu = filemenu
            };

            var agr = new AccelGroup();
            AddAccelGroup(agr);

            var newi = new ImageMenuItem(Stock.New, agr);
            newi.AddAccelerator("activate", agr, new AccelKey(Gdk.Key.n, Gdk.ModifierType.ControlMask, AccelFlags.Visible));
            filemenu.Append(newi);

            var open = new ImageMenuItem(Stock.Open, agr);
            open.AddAccelerator("activate", agr, new AccelKey(Gdk.Key.n, Gdk.ModifierType.ControlMask, AccelFlags.Visible));
            filemenu.Append(open);

            var sep = new SeparatorMenuItem();
            filemenu.Append(sep);

            var exit = new ImageMenuItem(Stock.Quit, agr);
            exit.AddAccelerator("activate", agr, new AccelKey(Gdk.Key.q, Gdk.ModifierType.ControlMask, AccelFlags.Visible));

            //var exit = new MenuItem("退出");
            exit.Activated += (sender, args) =>
            {
                Application.Quit();
            };
            filemenu.Append(exit);

            mb.Append(file);

            //var vbox = new VBox(false, 2);
            vbox.PackStart(mb, false, false, 0);
            //vbox.PackStart(new Label("2333"), false, false, 0);

            //Add(vbox);
        }
    }
}
