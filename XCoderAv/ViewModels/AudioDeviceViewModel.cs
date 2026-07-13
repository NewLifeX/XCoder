using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;
using XCoderAv.Models;

namespace XCoderAv.ViewModels;

/// <summary>声卡选择器 ViewModel</summary>
public partial class AudioDeviceViewModel : ObservableObject
{
    #region 属性
    /// <summary>设备列表</summary>
    public ObservableCollection<String> DeviceList { get; } = [];

    /// <summary>选中的设备</summary>
    [ObservableProperty]
    private Int32 _selectedDeviceIndex;

    /// <summary>设备详细信息</summary>
    [ObservableProperty]
    private String _deviceDetail = "";

    /// <summary>音量</summary>
    [ObservableProperty]
    private Double _volumeValue = 50;

    /// <summary>日志输出</summary>
    [ObservableProperty]
    private String _log = "";

    private Dictionary<String, AudioDeviceInfo> _devices = [];
    #endregion

    #region 构造
    /// <summary>实例化声卡选择器 ViewModel</summary>
    public AudioDeviceViewModel()
    {
        // NAudio 在 Avalonia 跨平台不可用，显示友好提示
        AppendLog("声卡选择器在跨平台环境下使用简化功能。");
        AppendLog("完整功能需要 NAudio 库（仅支持 Windows）。");
    }
    #endregion

    #region 方法
    /// <summary>刷新设备列表</summary>
    [RelayCommand]
    private void RefreshDevices()
    {
        DeviceList.Clear();
        _devices.Clear();

        try
        {
            // 跨平台兼容：使用简化设备枚举
            AppendLog("刷新设备功能在跨平台环境下受限。");
        }
        catch (Exception ex)
        {
            AppendLog($"刷新设备失败: {ex.Message}");
        }
    }

    partial void OnSelectedDeviceIndexChanged(Int32 value)
    {
        if (value < 0 || value >= DeviceList.Count) return;

        var name = DeviceList[value];
        if (!_devices.TryGetValue(name, out var dev)) return;

        var sb = new StringBuilder();
        sb.AppendLine($"设备: {dev.Name}");
        sb.AppendLine($"友好名称: {dev.FriendlyName}");
        sb.AppendLine($"播放: {(dev.HasPlayback ? "是" : "否")}");
        sb.AppendLine($"录音: {(dev.HasRecording ? "是" : "否")}");
        sb.AppendLine($"声道数: {dev.Channels}");
        sb.AppendLine($"采样率: {dev.SampleRate} Hz");

        DeviceDetail = sb.ToString();
        VolumeValue = dev.Volume * 100;
    }

    /// <summary>测试播放</summary>
    [RelayCommand]
    private void TestPlayback()
    {
        AppendLog("播放测试功能在跨平台环境受限。");
    }

    private void AppendLog(String msg)
    {
        Log += $"{msg}\r\n";
    }
    #endregion
}
