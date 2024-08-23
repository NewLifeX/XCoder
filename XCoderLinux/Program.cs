using Gtk;
using NewLife;
using NewLife.Log;
using NewLife.Model;
using Stardust;

namespace XCoder;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        XTrace.UseConsole();
        MachineInfo.RegisterAsync();

        StartClient();

        GLib.ExceptionManager.UnhandledException += ExceptionManager_UnhandledException;

        // 检查环境
        GtkHelper.CheckRuntime();

        Application.Init();
        var window = new SharpApp();
        window.ShowAll();
        // Window.InteractiveDebugging = true;
        //XTrace2.UseWinForm(window);
        Application.Run();
    }

    private static void ExceptionManager_UnhandledException(GLib.UnhandledExceptionArgs args)
    {
        if (args.ExceptionObject is Exception ex) XTrace.WriteException(ex);
    }

    static StarFactory _factory;
    static StarClient _Client;
    private static void StartClient()
    {
        var set = XConfig.Current;
        var server = set.Server;
        if (server.IsNullOrEmpty()) return;

        XTrace.WriteLine("初始化服务端地址：{0}", server);

        _factory = new StarFactory(server, "XCoderLinux", null)
        {
            Log = XTrace.Log,
        };

        var client = new StarClient(server)
        {
            Code = set.Code,
            Secret = set.Secret,
            ProductCode = "XCoderLinux",

            Tracer = _factory.Tracer,
            Log = XTrace.Log,
        };

        // 登录后保存证书
        client.OnLogined += (s, e) =>
        {
            if (client.Logined && !client.Code.IsNullOrEmpty())
            {
                set.Code = client.Code;
                set.Secret = client.Secret;
                set.Save();
            }
        };

        client.Open();

        Host.RegisterExit(() => client.Logout("ApplicationExit"));

        _Client = client;
    }
}
