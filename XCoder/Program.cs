using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using NewLife;
using NewLife.Log;
using NewLife.Net;
using NewLife.Threading;
using NewLife.Xml;
using Stardust;
using XCode.DataAccessLayer;

namespace XCoder
{
    static class Program
    {
        /// <summary>应用程序的主入口点。</summary>
        [STAThread]
        static void Main()
        {
#if NC30
            XTrace2.UseWinForm();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
#else
            XTrace.UseWinForm();
#endif

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

        private static void TryConnectServer(Object state)
        {
            var client = state as StarClient;
            var set = XConfig.Current;
            client.Login().Wait();
            CheckUpgrade(client, set.Channel);

            // 登录成功，销毁定时器
            //TimerX.Current.Period = 0;
            _timer.TryDispose();
            _timer = null;
        }

        private static void CheckUpgrade(StarClient client, String channel)
        {
            // 检查更新
            var ur = client.Upgrade(channel).Result;
            if (ur != null)
            {
                var rs = client.ProcessUpgrade(ur);

                // 强制更新时，马上重启
                if (rs && ur.Force)
                {
                    //StopWork("Upgrade");

                    // 重新拉起进程
                    var star = "XCoder.exe";
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