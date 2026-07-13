using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;
using XCoderAv.Models;

namespace XCoderAv.ViewModels;

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

            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Ram || drive.Name.StartsWith("A:")) continue;
            }

            // 简化版 USB 检测（跨平台兼容）
            AppendLog("USB 设备检测功能在 Windows 上使用 WMI 提供完整信息。");
            AppendLog($"找到 {roots.Count} 个 HUB");
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

    private void AppendLog(String msg)
    {
        Log += $"{msg}\r\n";
    }
    #endregion
}
