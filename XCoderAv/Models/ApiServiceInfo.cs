namespace XCoderAv.Models;

/// <summary>API 服务信息</summary>
public class ApiServiceInfo
{
    /// <summary>名称</summary>
    public String Name { get; set; } = "";

    /// <summary>IP</summary>
    public String RemoteIP { get; set; } = "";

    /// <summary>端口</summary>
    public Int32 Port { get; set; }

    /// <summary>ID</summary>
    public Int32 Id { get; set; }

    /// <summary>版本</summary>
    public String Version { get; set; } = "";

    /// <summary>编码</summary>
    public String Code { get; set; } = "";

    /// <summary>地址</summary>
    public String Address { get; set; } = "";
}
