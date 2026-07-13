using System.Collections.ObjectModel;
using System.Management;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;

namespace CrazyCoder.ViewModels;

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

/// <summary>USB 设备检测 ViewModel</summary>
public partial class UsbDeviceViewModel : ObservableObject
{
    #region 属性
    /// <summary>Hub 列表</summary>
    public ObservableCollection<String> HubList { get; } = [];

    /// <summary>选中的 Hub</summary>
    [ObservableProperty]
    private Int32 _selectedHubIndex;

    /// <summary>设备信息</summary>
    [ObservableProperty]
    private String _deviceInfo = "";

    /// <summary>日志输出</summary>
    [ObservableProperty]
    private String _log = "";

    private Dictionary<Int32, List<UsbDeviceInfo>> _hubs = [];
    #endregion

    #region 构造
    /// <summary>实例化 USB 设备检测 ViewModel</summary>
    public UsbDeviceViewModel()
    {
        RefreshDevices();
    }
    #endregion

    #region 方法
    /// <summary>刷新设备列表</summary>
    [RelayCommand]
    private void RefreshDevices()
    {
        HubList.Clear();
        _hubs.Clear();

        try
        {
            var roots = new Dictionary<Int32, List<UsbDeviceInfo>>();

            using (var searcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_PnPEntity WHERE PNPDeviceID LIKE 'USB%'"))
            {
                var devices = searcher.Get();
                foreach (var device in devices)
                {
                    var name = device["Name"]?.ToString();
                    var pnpId = device["PNPDeviceID"]?.ToString();
                    if (name == null || pnpId == null) continue;

                    if (pnpId.IndexOf("&MI_", StringComparison.Ordinal) >= 0) continue;

                    var location = GetLocationInformation(pnpId);
                    if (location == null) continue;

                    var phead = "Port_#";
                    var hhead = "Hub_#";
                    if (location.IndexOf(phead, StringComparison.Ordinal) < 0) continue;

                    var strs = location.Split('.');
                    var port = strs[0].TrimStart(phead).ToInt();
                    var hub = strs[1].TrimStart(hhead).ToInt();
                    var cid = GetContainerId(pnpId);
                    var (vid, pid) = GetVidPid(pnpId);

                    var dev = new UsbDeviceInfo
                    {
                        Port = port,
                        Name = name,
                        ContainerId = cid,
                        VID = vid,
                        PID = pid,
                        Hub = hub
                    };

                    if (roots.ContainsKey(hub))
                        roots[hub].Add(dev);
                    else
                        roots[hub] = [dev];
                }

                // 处理复合设备
                foreach (var device in devices)
                {
                    var name = device["Name"]?.ToString();
                    var pnpId = device["PNPDeviceID"]?.ToString();
                    if (name == null || pnpId == null) continue;

                    if (pnpId.IndexOf("&MI_", StringComparison.Ordinal) < 0) continue;

                    var cid = GetContainerId(pnpId);

                    UsbDeviceInfo? parent = null;
                    foreach (var kv in roots)
                    {
                        parent = kv.Value.FirstOrDefault(d => d.ContainerId == cid);
                        if (parent != null) break;
                    }

                    if (parent == null) continue;

                    if (parent.SubDevices.IsNullOrEmpty())
                        parent.SubDevices = name;
                    else
                        parent.SubDevices += " + " + name;
                }
            }

            // 排序
            foreach (var kv in roots)
            {
                kv.Value.Sort((x, y) => x.Port.CompareTo(y.Port));
            }

            _hubs = roots;

            foreach (var kv in roots.OrderBy(k => k.Key))
            {
                HubList.Add($"Hub_{kv.Key}");
            }

            if (HubList.Count > 0) SelectedHubIndex = 0;

            AppendLog($"刷新完成，找到 {_hubs.Count} 个 HUB");
        }
        catch (Exception ex)
        {
            AppendLog($"刷新失败: {ex.Message}");
        }
    }

    partial void OnSelectedHubIndexChanged(Int32 value)
    {
        if (value < 0 || value >= HubList.Count) return;

        var hubText = HubList[value];
        var hub = hubText.TrimStart("Hub_").ToInt();

        if (!_hubs.TryGetValue(hub, out var devs)) return;

        var sb = new StringBuilder();
        foreach (var dev in devs)
        {
            sb.AppendLine($"端口 {dev.Port,2}  VID_{dev.VID} PID_{dev.PID}  {dev.ContainerId}");
            if (!dev.SubDevices.IsNullOrEmpty())
                sb.AppendLine($"         {dev.SubDevices}");
            else
                sb.AppendLine($"         {dev.Name}");
        }

        DeviceInfo = sb.ToString();
    }

    private static String GetLocationInformation(String pnpId)
    {
        try
        {
            var path = $@"SYSTEM\CurrentControlSet\Enum\{pnpId}\Device Parameters";
            using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(path);
            if (key != null)
            {
                var val = key.GetValue("LocationInformation");
                if (val != null) return val.ToString();
            }
            return "";
        }
        catch
        {
            return "";
        }
    }

    private static String GetContainerId(String pnpId)
    {
        try
        {
            var path = $@"SYSTEM\CurrentControlSet\Enum\{pnpId}";
            using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(path);
            return key?.GetValue("ContainerId")?.ToString() ?? "";
        }
        catch
        {
            return "";
        }
    }

    private static (String vid, String pid) GetVidPid(String pnpId)
    {
        var vid = "";
        var pid = "";

        var p = pnpId.IndexOf("VID_", StringComparison.Ordinal);
        if (p >= 0) vid = pnpId.Substring(p + 4, 4);

        p = pnpId.IndexOf("PID_", StringComparison.Ordinal);
        if (p >= 0) pid = pnpId.Substring(p + 4, 4);

        return (vid, pid);
    }

    private void AppendLog(String msg)
    {
        Log += $"{msg}\r\n";
    }
    #endregion
}
