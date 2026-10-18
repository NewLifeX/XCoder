using NewLife.Log;

namespace XCoderMac.Services;

/// <summary>公开产品名称及所有持久化目录只使用 Newlife.XCoder，不依赖安装位置。</summary>
public static class MacUserPaths
{
    public const string ProductName = "Newlife.XCoder";
    // 在入口处保留调试命令的工作目录，运行期再将工作目录移到用户支持目录。
    public static string InitialWorkingDirectory { get; private set; } = Environment.CurrentDirectory;

    public static void CaptureInitialWorkingDirectory(string path) =>
        InitialWorkingDirectory = path;
    // macOS 上 SpecialFolder.Personal 常指向 ~/Documents，不能作为用户配置根目录。
    public static string UserHome => Environment.GetEnvironmentVariable("HOME") is { Length: > 0 } home
        ? Path.GetFullPath(home)
        : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    public static string SupportDirectory => Path.Combine(
        UserHome, "Library", "Application Support", ProductName);
    public static string ConfigDirectory => Path.Combine(SupportDirectory, "Config");
    public static string BackupDirectory => Path.Combine(SupportDirectory, "Backups");
    public static string DataDirectory => Path.Combine(SupportDirectory, "Data");
    public static string ConnectionsFile => Path.Combine(ConfigDirectory, "connections.json");
    public static string LogDirectory => Path.Combine(
        UserHome, "Library", "Logs", ProductName);

    /// <summary>必须早于任何 XCode/NewLife 数据访问层和窗口构造调用。</summary>
    public static void Initialize()
    {
        Directory.CreateDirectory(ConfigDirectory);
        Directory.CreateDirectory(BackupDirectory);
        Directory.CreateDirectory(DataDirectory);
        Directory.CreateDirectory(LogDirectory);
        if (OperatingSystem.IsMacOS())
        {
            // 用户配置和备份仅允许当前 macOS 登录用户访问。
            foreach (var path in new[] { SupportDirectory, ConfigDirectory, BackupDirectory, DataDirectory, LogDirectory })
                File.SetUnixFileMode(path, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        }

        // NewLife PathHelper.GetBasePath() 读取用户目录，避免生成到已签名的 .app 内。
        System.IO.PathHelper.BasePath = SupportDirectory;
        System.IO.PathHelper.BaseDirectory = SupportDirectory;
        Environment.CurrentDirectory = SupportDirectory;
        NewLife.Setting.Current.LogPath = LogDirectory;
        XTrace.LogPath = LogDirectory;
    }
}
