using System.Diagnostics;
using System.Text;
using NewLife;
using NewLife.Log;
using NewLife.Model;
using NewLife.Threading;
using Stardust;
using Stardust.Services;

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

        var star = new StarFactory();

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
            client.WriteInfoEvent("Upgrade", $"准备从[{_lastVersion}]更新到[{ur.Version}]，开始下载 {ur.Source}");
            try
            {
                ug.Url = client.BuildUrl(ur.Source);
                await ug.Download();

                // 检查文件完整性
                var checkHash = ug.CheckFileHash(ur.FileHash);
                if (!ur.FileHash.IsNullOrEmpty() && !checkHash)
                {
                    client.WriteInfoEvent("Upgrade", "下载完成，哈希校验失败");
                }
                else
                {
                    client.WriteInfoEvent("Upgrade", "下载完成，准备解压文件");
                    if (!ug.Extract())
                    {
                        client.WriteInfoEvent("Upgrade", "解压失败");
                    }
                    else
                    {
                        if (!ur.Preinstall.IsNullOrEmpty())
                        {
                            client.WriteInfoEvent("Upgrade", "执行预安装脚本");

                            ug.Run(ur.Preinstall);
                        }

                        client.WriteInfoEvent("Upgrade", "解压完成，准备覆盖文件");

                        // 执行更新，解压缩覆盖文件
                        var rs = ug.Update();
                        if (rs && !ur.Executor.IsNullOrEmpty()) ug.Run(ur.Executor);
                        _lastVersion = ur.Version;

                        // 强制更新时，马上重启
                        if (rs && ur.Force)
                        {
                            // 重新拉起进程
                            var star = "CrazyCoder.exe";
                            XTrace.WriteLine("强制升级，拉起进程 {0} -upgrade", star.GetFullPath());
                            var p = Process.Start(star.GetFullPath(), "-upgrade");

                            if (p.WaitForExit(5_000) && p.ExitCode != 0)
                            {
                                client.WriteInfoEvent("Upgrade", "强制更新完成，但拉起新进程失败");
                            }
                            else
                            {
                                client.WriteInfoEvent("Upgrade", "强制更新完成，新进程已拉起，准备退出当前进程");

                                ug.KillSelf();
                            }

                            Application.Exit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XTrace.WriteException(ex);
                client.WriteErrorEvent("Upgrade", ex.ToString());
            }
        }
    }
}