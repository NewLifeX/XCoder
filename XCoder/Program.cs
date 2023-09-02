using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using NewLife;
using NewLife.Log;
using NewLife.Threading;
using Stardust;
using Stardust.Models;
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

        XTrace.UseWinForm();
        MachineInfo.RegisterAsync();

        StartClient();

        StringHelper.EnableSpeechTip = XConfig.Current.SpeechTip;

        if (XConfig.Current.IsNew) "学无先后达者为师，欢迎使用新生命码神工具！".SpeechTip();

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
#if NET5_0_OR_GREATER
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
#endif
        Application.Run(new FrmMDI());
    }

    static TimerX _timer;
    static StarClient _Client;
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

        // 使用跟踪
        client.UseTrace();

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
        _timer = new TimerX(CheckUpgrade, null, 5_000, 600_000) { Async = true };

        client.RegisterCommand("node/upgrade", s => _timer.SetNext(-1));
    }

    private static String _lastVersion;
    private static async Task CheckUpgrade(Object data)
    {
        var client = _Client;
        using var span = client.Tracer?.NewSpan("CheckUpgrade", new { _lastVersion });

        // 运行过程中可能改变配置文件的通道
        var set = XConfig.Current;
        var ug = new Stardust.Web.Upgrade { Log = XTrace.Log };

        // 去除多余入口文件
        ug.Trim("CrazyCoder");

        // 检查更新
        var ur = await client.Upgrade(set.Channel, _lastVersion);
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

                        // 去除多余入口文件
                        ug.Trim("CrazyCoder");

                        // 强制更新时，马上重启
                        if (rs && ur.Force)
                        {
                            // 重新拉起进程
                            rs = ug.Run("CrazyCoder", "-run -upgrade");

                            if (rs)
                            {
                                var pid = Process.GetCurrentProcess().Id;
                                client.WriteInfoEvent("Upgrade", "强制更新完成，新进程已拉起，准备退出当前进程！PID=" + pid);

                                ug.KillSelf();
                            }
                            else
                            {
                                client.WriteInfoEvent("Upgrade", "强制更新完成，但拉起新进程失败");
                            }
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