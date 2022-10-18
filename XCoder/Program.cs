using System.Diagnostics;
using System.Text;
using NewLife;
using NewLife.Log;
using NewLife.Threading;
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

#if NET5_0_OR_GREATER
        XTrace.UseWinForm();
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
#else
        XTrace.UseWinForm();
#endif
        MachineInfo.RegisterAsync();

        StartClient();

        StringHelper.EnableSpeechTip = XConfig.Current.SpeechTip;

        if (XConfig.Current.IsNew) "学无先后达者为师，欢迎使用新生命码神工具！".SpeechTip();

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new FrmMDI());
    }

    static TimerX _timer;
    static StarClient _Client;
    //static ServiceManager _Manager;
    private static void StartClient()
    {
        var set = XConfig.Current;
        var server = set.Server;
        if (server.IsNullOrEmpty()) return;

        XTrace.WriteLine("初始化服务端地址：{0}", server);

        var client = new StarClient(server)
        {
            Code = set.Code,
            Secret = set.Secret,
            ProductCode = "CrazyCoder",
            Log = XTrace.Log,
        };

        // 登录后保存证书
        client.OnLogined += (s, e) =>
        {
            var inf = client.Info;
            if (inf != null && !inf.Code.IsNullOrEmpty())
            {
                set.Code = inf.Code;
                set.Secret = inf.Secret;
                set.Save();
            }
        };

        client.UseTrace();

        Application.ApplicationExit += (s, e) => client.Logout("ApplicationExit");

        // 可能需要多次尝试
        _timer = new TimerX(TryConnectServer, client, 0, 5_000) { Async = true };

        _Client = client;
    }

    private static async Task TryConnectServer(Object state)
    {
        var client = state as StarClient;
        var set = XConfig.Current;
        await client.Login();
        await CheckUpgrade(client, set.Channel);

        // 登录成功，销毁定时器
        //TimerX.Current.Period = 0;
        _timer.TryDispose();
        _timer = null;
    }

    private static String _lastVersion;
    private static async Task CheckUpgrade(StarClient client, String channel)
    {
        var ug = new Stardust.Web.Upgrade { Log = XTrace.Log };

        // 检查更新
        var ur = await client.Upgrade(channel);
        if (ur != null && ur.Version != _lastVersion)
        {
            ug.Url = ur.Source;
            await ug.Download();

            // 检查文件完整性
            if (ur.FileHash.IsNullOrEmpty() || ug.CheckFileHash(ur.FileHash))
            {
                // 执行更新，解压缩覆盖文件
                var rs = ug.Update();
                if (rs && !ur.Executor.IsNullOrEmpty()) ug.Run(ur.Executor);
                _lastVersion = ur.Version;

                // 去除多余入口文件
                ug.Trim("StarAgent");

                // 强制更新时，马上重启
                if (rs && ur.Force)
                {
                    //StopWork("Upgrade");

                    // 重新拉起进程
                    var star = "CrazyCoder.exe";
                    XTrace.WriteLine("强制升级，拉起进程 {0} -upgrade", star.GetFullPath());
                    Process.Start(star.GetFullPath(), "-upgrade");

                    //var p = Process.GetCurrentProcess();
                    //p.Close();
                    //p.Kill();
                    Application.Exit();
                }
            }
        }
    }
}