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
        // 保留开发命令原工作目录，避免截图写入真实用户配置目录。
        Services.MacUserPaths.CaptureInitialWorkingDirectory(Environment.CurrentDirectory);
        // 在初始化任何上游 XCode/NewLife 类之前完成用户级目录重定向。
        Services.MacUserPaths.Initialize();
#if DEBUG
        if (args.Contains("--smoke-connection-storage"))
        {
            try
            {
                // 使用独立临时 HOME 运行：不操作开发者实际数据库和钥匙串。
                Services.MacConnectionStore.Initialize();
                var entry = new Services.MacConnection
                {
                    Name = "LocalDemo",
                    Provider = "SQLite",
                    ConnectionString = "Data Source=:memory:",
                    PasswordMode = "none"
                };
                Services.MacConnectionStore.Save([entry]);
                if (!XCode.DataAccessLayer.DAL.ConnStrs.ContainsKey("LocalDemo"))
                    throw new InvalidOperationException("保存后 XCode DAL 未注册连接");
                var text = File.ReadAllText(Services.MacUserPaths.ConnectionsFile);
                if (!text.Contains("LocalDemo") || text.Contains("secret"))
                    throw new InvalidOperationException("JSON 未保存或泄露明文");
                if (File.GetUnixFileMode(Services.MacUserPaths.ConnectionsFile) !=
                    (UnixFileMode.UserRead | UnixFileMode.UserWrite))
                    throw new InvalidOperationException("配置文件权限应为 600");

                var changed = text.Replace("LocalDemo", "LocalDemoEdited");
                File.WriteAllText(Services.MacUserPaths.ConnectionsFile, changed);
                Services.MacConnectionStore.Reload();
                if (!XCode.DataAccessLayer.DAL.ConnStrs.ContainsKey("LocalDemoEdited") ||
                    XCode.DataAccessLayer.DAL.ConnStrs.ContainsKey("LocalDemo"))
                    throw new InvalidOperationException("手动修改文件后未重载到 XCode");

                // 不允许默认钥匙串模式把密码直接写到 JSON 连接字符串。
                var leaked = new Services.MacConnection
                {
                    Name = "Bad",
                    ConnectionString = "Server=127.0.0.1;Password=secret",
                    Provider = "MySql",
                    PasswordMode = "keychain"
                };
                var forbidden = false;
                try { Services.MacConnectionStore.Validate(leaked); }
                catch (InvalidDataException) { forbidden = true; }
                if (!forbidden) throw new InvalidOperationException("明文密码未被禁止");

                // 出错的外部编辑不能破坏运行中的已加载列表。
                File.WriteAllText(Services.MacUserPaths.ConnectionsFile, "{invalidJson");
                try { Services.MacConnectionStore.Reload(); throw new InvalidOperationException("非法 JSON 仍被接受"); }
                catch (System.Text.Json.JsonException) { }
                if (Services.MacConnectionStore.Connections.Count != 1)
                    throw new InvalidOperationException("非法 JSON 破坏了内存连接");
                Services.MacConnectionStore.Save([entry]);
                Console.WriteLine("CONNECTION STORAGE OK: save, DAL, manual reload, file permissions, invalid JSON, plaintext protection");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("CONNECTION STORAGE FAILED: " + ex);
                Environment.Exit(1);
            }
        }
#endif
        try
        {
            Services.MacConnectionStore.Initialize();
        }
        catch (Exception ex)
        {
            // 用户手工编辑坏 JSON 不应该导致整个 XCoder 无法启动。
            // 保留原文件，让用户通过连接管理器的 Finder 按钮检查并修复。
            Console.Error.WriteLine($"连接配置载入失败：{ex.Message}");
        }
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    private static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<MacApp>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
