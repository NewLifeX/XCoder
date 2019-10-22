using Gtk;
using XApi;

namespace XCoder
{
    class SharpApp : Window
    {
        VBox vbox = new VBox(false, 2);
        private Box xapi;

        public SharpApp() : base("新生命码神工具")
        {
            SetDefaultSize(250, 200);
            SetPosition(WindowPosition.Center);

            DeleteEvent += delegate { Application.Quit(); };

            AddMenu();

            //AddButton();


            xapi = new FrmMain(Orientation.Horizontal);

            vbox.PackStart(xapi, false, false, 0);

            Add(vbox);

            //Show();
            ShowAll();
        }

        void AddButton()
        {
            var fix = new Fixed();

            var btn1 = new Button("Button")
            {
                Sensitive = false
            };
            var btn2 = new Button("Button");
            var btn3 = new Button(Stock.Close);
            var btn4 = new Button("Button");
            btn4.SetSizeRequest(80, 40);

            fix.Put(btn1, 2 * 20, 2 * 30);
            fix.Put(btn2, 2 * 100, 2 * 30);
            fix.Put(btn3, 2 * 20, 2 * 80);
            fix.Put(btn4, 2 * 100, 2 * 80);
            vbox.PackStart(fix, false, false, 0);
            //Add(fix);
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
