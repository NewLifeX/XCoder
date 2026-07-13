using System.Text;
using Avalonia;
using NewLife;
using NewLife.Log;
using Stardust;

namespace XCoderAv;

class Program
{
    public static void Main(String[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        MachineInfo.RegisterAsync();

        StartClient();

        var set = XConfig.Current;
        StringHelper.EnableSpeechTip = set.SpeechTip;

        if (set.IsNew)
        {
            try { "学无先后达者为师，欢迎使用新生命码神工具！".SpeechTip(); } catch { }
        }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    static StarClient? _Client;
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

        NewLife.Model.Host.RegisterExit(() => client.Logout("ApplicationExit"));

        _Client = client;
    }
}
