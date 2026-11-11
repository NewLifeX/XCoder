using System.Text.Json;

namespace XCoderMac.Services;

/// <summary>
/// “快捷入口”是当前 macOS 用户的个人偏好，不写入签名 .app 或仓库。
/// </summary>
public static class MacQuickEntryStore
{
    public static readonly string[] Defaults =
    [
        "数据建模", "API 调试", "正则表达式", "文件编码",
        "网络工具", "MQTT 客户端", "SSH 工具", "Redis 管理"
    ];

    public static string FilePath => Path.Combine(MacUserPaths.ConfigDirectory, "quick-entries.json");

    public static string[] Load(string? path = null)
    {
        path ??= FilePath;
        if (!File.Exists(path)) return [.. Defaults];
        try
        {
            using var json = JsonDocument.Parse(File.ReadAllText(path));
            if (json.RootElement.ValueKind != JsonValueKind.Object ||
                !json.RootElement.TryGetProperty("tools", out var names) ||
                names.ValueKind != JsonValueKind.Array)
                throw new InvalidDataException("缺少 tools 列表");
            var entries = new List<string>();
            foreach (var entry in names.EnumerateArray())
            {
                if (entry.ValueKind != JsonValueKind.String)
                    throw new InvalidDataException("快捷入口必须是工具名称");
                var title = entry.GetString()!;
                if (XCoderMac.MacToolCatalog.Tools.Any(tool => tool.Title == title) &&
                    !entries.Contains(title, StringComparer.Ordinal))
                    entries.Add(title);
            }
            return [.. entries];
        }
        catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException or InvalidDataException)
        {
            // 文件损坏不覆盖；用户仍可用默认入口，编辑保存时再写入。
            System.Diagnostics.Debug.WriteLine($"读取快捷入口设置失败：{ex}");
            return [.. Defaults];
        }
    }

    public static void Save(IEnumerable<string> titles, string? path = null)
    {
        path ??= FilePath;
        var known = XCoderMac.MacToolCatalog.Tools.Select(tool => tool.Title).ToHashSet(StringComparer.Ordinal);
        var saved = titles.Where(title => known.Contains(title))
            .Distinct(StringComparer.Ordinal).ToArray();
        var parent = Path.GetDirectoryName(path)
            ?? throw new ArgumentException("快捷入口文件必须具有父目录", nameof(path));
        Directory.CreateDirectory(parent);
        var temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllText(temporary, JsonSerializer.Serialize(
                new { version = 1, tools = saved },
                new JsonSerializerOptions { WriteIndented = true }));
            if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
                File.SetUnixFileMode(temporary, UnixFileMode.UserRead | UnixFileMode.UserWrite);
            File.Move(temporary, path, true);
        }
        finally
        {
            if (File.Exists(temporary)) File.Delete(temporary);
        }
    }
}
