﻿using System.Text;
using NewLife;
using NewLife.Log;
using NewLife.Model;
using Stardust;

namespace XCoder;

static class Program
{
    /// <summary>应用程序的主入口点。</summary>
    [STAThread]
    static void Main()
    {
        // 支持GB2312编码
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        XTrace.UseWinForm();
        MachineInfo.RegisterAsync();

        StartClient();

        var set = XConfig.Current;
        StringHelper.EnableSpeechTip = set.SpeechTip;

        if (set.IsNew) "学无先后达者为师，欢迎使用新生命码神工具！".SpeechTip();

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
#if NET5_0_OR_GREATER
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
#endif
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

        _factory = new StarFactory(server, "CrazyCoder", null)
        {
            Log = XTrace.Log,
        };

        var client = new StarClient(server)
        {
            Code = set.Code,
            Secret = set.Secret,
            ProductCode = "CrazyCoder",

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

        //Application.ApplicationExit += (s, e) => client.Logout("ApplicationExit");
        Host.RegisterExit(() => client.Logout("ApplicationExit"));

        _Client = client;
    }
}