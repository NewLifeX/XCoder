using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Gdk;
using Gtk;
using NewLife.Reflection;
//using XApi;
//using XNet;
using XCom;
using Window = Gtk.Window;

namespace XCoder
{
    class SharpApp : Window
    {
        #region 窗口初始化
        Task<Type[]> _load;

        private readonly VBox _windowBox = new VBox(false, 1);
        private readonly Menu _menuTool = new Menu();
        private String _title = "新生命码神工具";



        public SharpApp() : base("新生命码神工具")
        {
            _load = Task<Type[]>.Factory.StartNew(() => typeof(IXForm).GetAllSubclasses().ToArray());

            DeleteEvent += delegate
            {
                SharpApp_Closing();
                Application.Quit();
            };

            Shown += SharpApp_Shown;
            ShowAll();// 触发Shown事件、递归显示所有控件，容器ShowAll后才能显示容器内所有控件
        }

        private void SharpApp_Shown(System.Object sender, System.EventArgs e)
        {
            var set = XConfig.Current;
            if (set.Width > 0 || set.Height > 0)
            {
                DefaultWidth = set.Width;
                DefaultHeight = set.Height;
                //SetDefaultSize(set.Width, set.Height);
            }

            SetPosition(WindowPosition.Center);

            AddMenuButton();

            //AddMenu();

            var frm = new FrmMain();

            _windowBox.PackStart(frm, true, true, 0);

            Add(_windowBox);

            var asm = AssemblyX.Create(Assembly.GetExecutingAssembly());
            if (set.Title.IsNullOrEmpty()) set.Title = asm.Title;
            _title = Title = String.Format("{2} v{0} {1:HH:mm:ss}", asm.CompileVersion, asm.Compile, set.Title);

            _load.ContinueWith(t => LoadForms(t.Result));

            ShowAll();
        }

        void LoadForms(Type[] ts)
        {
            var name = XConfig.Current.LastTool + "";
            foreach (var item in ts)
            {
                if (item.FullName.EqualIgnoreCase(name))
                {
                    Application.Invoke((sender, args) =>
                    {
                        CreateForm(item.CreateInstance() as Box);
                        Title = _title + $"[{item.GetDisplayName() ?? item.FullName}]";
                    });

                    break;
                }
            }

            Application.Invoke((sender, args) =>
            {
                foreach (var item in ts)
                {
                    var menuItem = new MenuItem { Label = item.GetDisplayName() ?? item.FullName };

                    menuItem.Activated += (s, e) =>
                    {
                        var set = XConfig.Current;

                        if (set.LastTool == item.FullName) return;
                        var frm = item.CreateInstance() as Box;
                        CreateForm(frm);
                        Title = _title + $"[{menuItem.Label}]";
                    };

                    _menuTool.Append(menuItem);
                }
                _menuTool.ShowAll();
            });
        }

        private void SharpApp_Closing()
        {
            var set = XConfig.Current;
            //var area = Screen.PrimaryScreen.WorkingArea;
            //if (Left >= 0 && Top >= 0 && Width < area.Width - 60 && Height < area.Height - 60)
            {
                set.Width = AllocatedWidth;
                set.Height = AllocatedHeight;
                //set.Top = Top;
                //set.Left = Left;
                set.Save();
            }
        }
        #endregion

        #region 应用窗口
        void CreateForm(Box frm)
        {
            var name = frm.GetType().FullName;
            var cfg = XConfig.Current;
            if (name != cfg.LastTool)
            {
                cfg.LastTool = name;
                cfg.Save();
            }

            //frm.MdiParent = this;
            //frm.WindowState = FormWindowState.Maximized;
            if (_windowBox.Children.Length > 1)
            {
                _windowBox.Remove(_windowBox.Children[1]);
            }
            _windowBox.Add(frm);
            _windowBox.ShowAll();
            //frm.ShowAll();
        }
        #endregion

        #region 菜单
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

            //var _windowBox = new VBox(false, 2);
            _windowBox.PackStart(mb, false, false, 0);
            //_windowBox.PackStart(new Label("2333"), false, false, 0);

            //Add(_windowBox);
        }

        void AddMenuButton()
        {
            var mb = new MenuBar();

            mb.Append(GetMenuTool());
            mb.Append(GetMenuHelp());

            _windowBox.PackStart(mb, false, false, 0);
        }

        /// <summary>
        /// 添加帮助菜单
        /// </summary>
        /// <returns></returns>
        MenuItem GetMenuHelp()
        {
            var menu = new Menu();
            var menuItem = new MenuItem("帮助")
            {
                Submenu = menu
            };

            var inspector = new MenuItem("切换开发工具");
            inspector.Activated += (s, e) =>  InteractiveDebugging = true;
            menu.Append(inspector);

            return menuItem;
        }

        /// <summary>
        /// 添加工具菜单
        /// </summary>
        /// <returns></returns>
        MenuItem GetMenuTool()
        {
            var menu = _menuTool;
            var menuItem = new MenuItem("工具")
            {
                Submenu = menu
            };
            
            return menuItem;
        }
        #endregion
    }
}
