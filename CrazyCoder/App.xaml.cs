using System;
using System.Diagnostics;
using System.Windows;
using NewLife;
using NewLife.Log;
using NewLife.Threading;
using Stardust;

namespace CrazyCoder
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
#if NET45
            XTrace.UseWinForm();
#endif

            StartClient();

            StringHelper.EnableSpeechTip = Setting.Current.SpeechTip;

            if (Setting.Current.IsNew) "学无先后达者为师，欢迎使用新生命码神工具！".SpeechTip();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _Client.Logout("ExitCode=" + e.ApplicationExitCode);

            base.OnExit(e);
        }

        static TimerX _timer;
        static StarClient _Client;
        private static void StartClient()
        {
            var set = Setting.Current;
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

            // 可能需要多次尝试
            _timer = new TimerX(TryConnectServer, client, 0, 5_000) { Async = true };

            _Client = client;
        }

        private static void TryConnectServer(Object state)
        {
            var client = state as StarClient;
            var set = Setting.Current;
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
                //var rs = client.ProcessUpgrade(ur);

                //// 强制更新时，马上重启
                //if (rs && ur.Force)
                //{
                //    var p = Process.GetCurrentProcess();
                //    p.Close();
                //    p.Kill();
                //}
            }
        }
    }
}