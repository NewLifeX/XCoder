using System.Data.Common;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using XCode.DataAccessLayer;

namespace XCoderMac.Services;

/// <summary>可编辑的文件格式：密码默认仅存在 Keychain；明确选择明文时才写在 JSON。</summary>
public sealed class MacConnection
{
    public string Name { get; set; } = "";
    public string Provider { get; set; } = "SQLite";
    public string ConnectionString { get; set; } = "";
    /// <summary>keychain | plaintext | none</summary>
    public string PasswordMode { get; set; } = "keychain";
    public string? Password { get; set; }

    public MacConnection Copy() => (MacConnection)MemberwiseClone();
}

public sealed class MacConnectionsDocument
{
    public int Version { get; set; } = 1;
    public List<MacConnection> Connections { get; set; } = [];
}

/// <summary>原子写入 + 备份；手改文件可重新加载；以 DAL.AddConnStr 接入已有 VM。</summary>
public static class MacConnectionStore
{
    private static readonly JsonSerializerOptions Json = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };
    private static readonly HashSet<string> Registered = new(StringComparer.OrdinalIgnoreCase);
    public static List<MacConnection> Connections { get; private set; } = [];

    public static void Initialize()
    {
        if (!File.Exists(MacUserPaths.ConnectionsFile))
            SaveDocument(new MacConnectionsDocument());
        Reload();
    }

    public static void Reload()
    {
        var document = JsonSerializer.Deserialize<MacConnectionsDocument>(
            File.ReadAllText(MacUserPaths.ConnectionsFile), Json)
            ?? throw new InvalidDataException("连接配置 JSON 为空");
        if (document.Version != 1) throw new InvalidDataException("不支持的连接配置版本，请先备份或升级");
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in document.Connections)
        {
            Validate(entry);
            if (!names.Add(entry.Name)) throw new InvalidDataException($"重复的连接名称：{entry.Name}");
        }

        // 先完整校验并解析，错误 JSON 不改变运行中的连接集合。
        var prepared = document.Connections.Select(c => (c, ConnectionString: Compose(c))).ToArray();
        foreach (var old in Registered.Where(name => !names.Contains(name)).ToArray())
        {
            DAL.ConnStrs.TryRemove(old, out _);
            Registered.Remove(old);
        }

        foreach (var (entry, connStr) in prepared)
        {
            DAL.AddConnStr(entry.Name, connStr, null, entry.Provider);
            // 本版本 DAL.ConnStr 只有 get；重置已打开的同名 DAL 的连接及表缓存。
            if (Registered.Contains(entry.Name))
                DAL.Create(entry.Name).Reset();
            Registered.Add(entry.Name);
        }
        Connections = document.Connections.Select(c => c.Copy()).ToList();
    }

    public static void Save(IReadOnlyList<MacConnection> connections)
    {
        var doc = new MacConnectionsDocument { Connections = connections.Select(c => c.Copy()).ToList() };
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in doc.Connections)
        {
            Validate(entry);
            if (!names.Add(entry.Name)) throw new InvalidDataException($"连接名称不能重复：{entry.Name}");
        }
        SaveDocument(doc);
        Reload();
    }

    private static void SaveDocument(MacConnectionsDocument doc)
    {
        Directory.CreateDirectory(MacUserPaths.ConfigDirectory);
        var target = MacUserPaths.ConnectionsFile;
        var temp = target + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(doc, Json) + "\n");
            File.WriteAllBytes(temp, bytes);
            File.SetUnixFileMode(temp, UnixFileMode.UserRead | UnixFileMode.UserWrite);
            if (File.Exists(target))
            {
                Directory.CreateDirectory(MacUserPaths.BackupDirectory);
                var backup = Path.Combine(MacUserPaths.BackupDirectory,
                    $"connections-{DateTime.Now:yyyyMMdd-HHmmss-fff}.json");
                File.Copy(target, backup);
                File.SetUnixFileMode(backup, UnixFileMode.UserRead | UnixFileMode.UserWrite);
            }
            File.Move(temp, target, true);
        }
        finally { if (File.Exists(temp)) File.Delete(temp); }
    }

    public static string Compose(MacConnection entry)
    {
        var cs = new DbConnectionStringBuilder { ConnectionString = entry.ConnectionString };
        var password = entry.PasswordMode.ToLowerInvariant() switch
        {
            "keychain" => MacKeychainSecrets.Read(entry.Name),
            "plaintext" => entry.Password,
            "none" => null,
            _ => throw new InvalidDataException("不支持的密码保存模式"),
        };
        if (!string.IsNullOrEmpty(password)) cs["Password"] = password;
        return cs.ConnectionString;
    }

    public static void Validate(MacConnection entry)
    {
        if (string.IsNullOrWhiteSpace(entry.Name) || entry.Name.Length > 100 ||
            entry.Name.Any(c => char.IsControl(c)))
            throw new InvalidDataException("连接名称不能为空、过长或包含控制字符");
        if (string.IsNullOrWhiteSpace(entry.Provider))
            throw new InvalidDataException("数据库类型不能为空");
        if (entry.PasswordMode is not ("keychain" or "plaintext" or "none"))
            throw new InvalidDataException("密码模式只能是 keychain、plaintext 或 none");
        if (string.IsNullOrWhiteSpace(entry.ConnectionString))
            throw new InvalidDataException($"连接 {entry.Name} 缺少连接字符串");
        try
        {
            var builder = new DbConnectionStringBuilder { ConnectionString = entry.ConnectionString };
            if (entry.PasswordMode != "plaintext" &&
                builder.Keys.Cast<string>().Any(k => k.Equals("Password", StringComparison.OrdinalIgnoreCase)
                    || k.Equals("Pwd", StringComparison.OrdinalIgnoreCase)
                    || k.Equals("Pass", StringComparison.OrdinalIgnoreCase)))
                throw new InvalidDataException($"连接 {entry.Name} 的字符串中包含密码，请移至钥匙串模式的独立密码输入框");
        }
        catch (ArgumentException ex) { throw new InvalidDataException($"连接 {entry.Name} 格式有误：{ex.Message}"); }
        if (entry.PasswordMode != "plaintext" && !string.IsNullOrEmpty(entry.Password))
            throw new InvalidDataException($"连接 {entry.Name} 的密码不能以明文方式放入此模式");
    }
}
