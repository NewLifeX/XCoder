namespace XCoderAv.Models;

/// <summary>USB 设备信息</summary>
public class UsbDeviceInfo
{
    /// <summary>端口号</summary>
    public Int32 Port { get; set; }

    /// <summary>设备名称</summary>
    public String Name { get; set; } = "";

    /// <summary>容器 ID</summary>
    public String ContainerId { get; set; } = "";

    /// <summary>VID</summary>
    public String VID { get; set; } = "";

    /// <summary>PID</summary>
    public String PID { get; set; } = "";

    /// <summary>子设备</summary>
    public String SubDevices { get; set; } = "";

    /// <summary>Hub 编号</summary>
    public Int32 Hub { get; set; }
}
