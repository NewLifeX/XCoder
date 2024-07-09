using System.Net.NetworkInformation;
using System.Text;
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

    static TimerX _timer;
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

        // 使用跟踪
        //client.UseTrace();

        Application.ApplicationExit += (s, e) => client.Logout("ApplicationExit");

        // 可能需要多次尝试
        _timer = new TimerX(TryConnectServer, client, 0, 5_000) { Async = true };

        _Client = client;
    }

    private static async Task TryConnectServer(Object state)
    {
        if (!NetworkInterface.GetIsNetworkAvailable() || AgentInfo.GetIps().IsNullOrEmpty())
        {
            return;
        }

        var client = state as StarClient;

        try
        {
            await client.Login();
            //await CheckUpgrade(client);
        }
        catch (Exception ex)
        {
            // 登录报错后，加大定时间隔，输出简单日志
            //_timer.Period = 30_000;
            if (_timer.Period < 30_000) _timer.Period += 5_000;

            XTrace.Log?.Error(ex.Message);

            return;
        }

        _timer.TryDispose();
    }
}