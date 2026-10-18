using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace XCoderMac;

/// <summary>Mac 专用应用程序宿主：使用独立主界面并复用现有 XCoderAv 功能。</summary>
public sealed class MacApp : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MacShellWindow();
#if DEBUG
            // 测试主页真实布局：响应式卡片列数、左侧选中项宽度，以及首页截图。
            if (System.Array.Exists(System.Environment.GetCommandLineArgs(), a => a == "--smoke-home-layout"))
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(async () =>
                {
                    try
                    {
                        var shell = (MacShellWindow)desktop.MainWindow!;
                        var output = System.IO.Path.Combine(System.Environment.CurrentDirectory,
                            "XCoderMac", "Docs", "软件界面");
                        var errors = await shell.SmokeTestHomeLayoutAsync(output);
                        desktop.Shutdown(errors == 0 ? 0 : 1);
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.Error.WriteLine("HOME LAYOUT CRASH: " + ex);
                        desktop.Shutdown(1);
                    }
                });
            }
            // 26 工具单主窗口标签页回归：测试打开、复用、切换、关闭和横向导航。
            else if (System.Array.Exists(System.Environment.GetCommandLineArgs(), a => a == "--smoke-tabs"))
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    try
                    {
                        var shell = (MacShellWindow)desktop.MainWindow!;
                        var errors = shell.SmokeTestTabs();
                        desktop.Shutdown(errors == 0 ? 0 : 1);
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.Error.WriteLine("SMOKE TABS FAILED: " + ex);
                        desktop.Shutdown(1);
                    }
                });
            }
            // 验证上游工具内容装载至 Mac 宿主后的实际窗口显示及关闭。
            else if (System.Array.Exists(System.Environment.GetCommandLineArgs(), a => a == "--smoke-host-render"))
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    var failures = 0;
                    foreach (var tool in MacToolCatalog.Tools)
                    {
                        if (tool.WindowType.Namespace != "XCoderAv.Views") continue;
                        try
                        {
                            using var scope = new System.IO.StringWriter();
                            var window = new Views.MacLegacyToolWindow(tool);
                            window.Show();
                            window.UpdateLayout();
                            window.Close();
                            System.Console.WriteLine($"HOST RENDER OK: {tool.Title}");
                        }
                        catch (System.Exception ex)
                        {
                            failures++;
                            System.Console.Error.WriteLine($"HOST RENDER FAIL: {tool.Title}: {ex}");
                        }
                    }
                    System.Console.WriteLine($"SMOKE HOST RENDER ERRORS={failures}");
                    desktop.Shutdown(failures == 0 ? 0 : 1);
                });
            }
            // 全工具窗口冒烟测试：只初始化原功能与新宿主，不发起设备连接和外部请求。
            else if (System.Array.Exists(System.Environment.GetCommandLineArgs(), a => a == "--smoke-all"))
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    var failures = 0;
                    foreach (var tool in MacToolCatalog.Tools)
                    {
                        try
                        {
                            Avalonia.Controls.Window? window = tool.WindowType.Namespace == "XCoderAv.Views"
                                ? new Views.MacLegacyToolWindow(tool)
                                : System.Activator.CreateInstance(tool.WindowType) as Avalonia.Controls.Window;
                            if (window == null) throw new System.InvalidOperationException("未实例化窗口");
                            System.Console.WriteLine($"OK: {tool.Title}");
                        }
                        catch (System.Exception ex)
                        {
                            failures++;
                            System.Console.Error.WriteLine($"FAILED: {tool.Title} -> {ex}");
                        }
                    }
                    System.Console.WriteLine($"SMOKE ALL: {MacToolCatalog.Tools.Length - failures}/{MacToolCatalog.Tools.Length} OK");
                    desktop.Shutdown(failures == 0 ? 0 : 1);
                });
            }
            // 供本地持续集成/开发检查：实例化新窗口，验证 XAML 与 VM 初始化。
            else if (System.Array.Exists(System.Environment.GetCommandLineArgs(), a => a == "--smoke-windows"))
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    try
                    {
                        _ = new Views.MacRegexWindow();
                        _ = new Views.MacDataModelingWindow();
                        _ = new Views.MacApiDebugWindow();
                        _ = new Views.MacNetworkWindow();
                        _ = new Views.MacRedisWindow();
                        System.Console.WriteLine("SMOKE WINDOWS OK: Regex, DataModeling, ApiDebug, Network, Redis");
                        desktop.Shutdown(0);
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.Error.WriteLine("SMOKE WINDOWS FAILED: " + ex);
                        desktop.Shutdown(1);
                    }
                });
            }
#endif
        }

        base.OnFrameworkInitializationCompleted();
    }
}
