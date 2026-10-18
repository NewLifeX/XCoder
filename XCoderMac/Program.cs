using System.Text;
using Avalonia;

namespace XCoderMac;

/// <summary>Mac 独立启动入口，不执行原 XCoderAv.Program 的 Windows/联网初始化。</summary>
internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    private static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<MacApp>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
