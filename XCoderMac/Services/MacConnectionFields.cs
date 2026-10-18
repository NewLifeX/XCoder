using System.Data.Common;

namespace XCoderMac.Services;

/// <summary>
/// 将可读的 IP/端口/库名/用户名字段与 NewLife.XCode 的连接字符串互转。
/// 不认识的驱动参数仍保留在原有连接字符串中，可通过高级模式编辑。
/// </summary>
public sealed class MacConnectionFields
{
    public string Host { get; set; } = "";
    public string Port { get; set; } = "";
    public string Database { get; set; } = "";
    public string Username { get; set; } = "";
    public string FilePath { get; set; } = "";

    public static bool IsFileProvider(string provider) =>
        provider.Equals("SQLite", StringComparison.OrdinalIgnoreCase) ||
        provider.Equals("DuckDB", StringComparison.OrdinalIgnoreCase);

    public static bool IsSimpleProvider(string provider) =>
        IsFileProvider(provider) ||
        provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase) ||
        provider.Equals("MySql", StringComparison.OrdinalIgnoreCase) ||
        provider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase);

    public static string DefaultPort(string provider) =>
        provider.ToLowerInvariant() switch
        {
            "sqlserver" => "1433",
            "mysql" => "3306",
            "postgresql" => "5432",
            "oracle" => "1521",
            "clickhouse" => "8123",
            _ => "",
        };

    public static MacConnectionFields Parse(string provider, string connectionString)
    {
        var result = new MacConnectionFields();
        if (string.IsNullOrWhiteSpace(connectionString)) return result;
        var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };

        string Get(params string[] keys)
        {
            foreach (var key in keys)
                if (builder.TryGetValue(key, out var value)) return value?.ToString() ?? "";
            return "";
        }

        if (IsFileProvider(provider))
        {
            result.FilePath = Get("Data Source", "DataSource", "Filename", "File");
            return result;
        }

        result.Database = provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)
            ? Get("Initial Catalog", "Database") : Get("Database", "Initial Catalog");
        result.Username = Get("User Id", "User ID", "Username", "Uid", "User");
        var server = Get("Server", "Host", "Data Source", "DataSource", "Address");
        result.Port = Get("Port");
        if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            // SQL Server 使用 server,port；实例名 server\\instance 原样保留。
            var comma = server.LastIndexOf(',');
            if (comma > 0 && int.TryParse(server[(comma + 1)..], out var value) && value is >= 1 and <= 65535)
            {
                result.Port = value.ToString();
                server = server[..comma];
            }
        }

        result.Host = server;
        return result;
    }

    public string Build(string provider, string baseConnectionString = "", bool allowPartial = false)
    {
        if (!IsSimpleProvider(provider))
            throw new InvalidOperationException("当前数据库类型请使用高级连接字符串模式");

        var builder = new DbConnectionStringBuilder();
        if (!string.IsNullOrWhiteSpace(baseConnectionString))
            builder.ConnectionString = baseConnectionString;

        if (IsFileProvider(provider))
        {
            if (string.IsNullOrWhiteSpace(FilePath) && !allowPartial)
                throw new InvalidOperationException("请填写数据库文件路径");
            Remove(builder, "Data Source", "DataSource", "Filename", "File");
            if (!string.IsNullOrWhiteSpace(FilePath)) builder["Data Source"] = FilePath.Trim();
            return builder.ConnectionString;
        }

        if (string.IsNullOrWhiteSpace(Host) && !allowPartial)
            throw new InvalidOperationException("请填写服务器 IP 或主机名");
        if (string.IsNullOrWhiteSpace(Database) && !allowPartial)
            throw new InvalidOperationException("请填写数据库名称");

        var port = Port.Trim();
        if (!string.IsNullOrEmpty(port) && (!int.TryParse(port, out var n) || n is < 1 or > 65535))
            throw new InvalidOperationException("端口必须为 1～65535 的数字");

        Remove(builder, "Server", "Host", "Data Source", "DataSource", "Address", "Port");
        Remove(builder, "Database", "Initial Catalog");
        Remove(builder, "User Id", "User ID", "Username", "Uid", "User");

        if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrWhiteSpace(Host))
                builder["Data Source"] = Host.Trim() + (string.IsNullOrEmpty(port) ? "" : "," + port);
            if (!string.IsNullOrWhiteSpace(Database)) builder["Initial Catalog"] = Database.Trim();
            if (!string.IsNullOrWhiteSpace(Username)) builder["User ID"] = Username.Trim();
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(Host))
                builder[provider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase) ? "Host" : "Server"] = Host.Trim();
            if (!string.IsNullOrEmpty(port)) builder["Port"] = port;
            if (!string.IsNullOrWhiteSpace(Database)) builder["Database"] = Database.Trim();
            if (!string.IsNullOrWhiteSpace(Username))
                builder[provider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase) ? "Username" : "User ID"] = Username.Trim();
        }

        return builder.ConnectionString;
    }

    private static void Remove(DbConnectionStringBuilder builder, params string[] keys)
    {
        foreach (var key in keys) builder.Remove(key);
    }
}
