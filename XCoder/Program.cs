using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;
using NewLife;
using NewLife.Log;
using NewLife.Threading;
using Stardust;
using Stardust.Models;

namespace XCoder;

static class Program
{
    /// <summary>应用程序的主入口点。</summary>
    [STAThread]
    static void Main()
    {
        XTrace.UseWinForm();
        MachineInfo.RegisterAsync();

        StartClient();

        var set = XConfig.Current;
        StringHelper.EnableSpeechTip = set.SpeechTip;

        if (set.IsNew) "学无先后达者为师，欢迎使用新生命码神工具！".SpeechTip();

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new FrmMDI());
    }

    static StarFactory _factory;
    static StarClient _Client;
    private static void StartClient()
    {
        var set = XConfig.Current;
        var server = set.Server;
        if (server.IsNullOrEmpty()) return;

        XTrace.WriteLine("初始化服务端地址：{0}", server);

        _factory = new StarFactory(server, "XCoder", null)
        {
            Log = XTrace.Log,
        };

        var client = new StarClient(server)
        {
            Code = set.Code,
            Secret = set.Secret,
            ProductCode = "XCoder",

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

        Application.ApplicationExit += (s, e) => client.Logout("ApplicationExit");

        _Client = client;
    }
}