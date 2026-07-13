using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NAudio.CoreAudioApi;
using NewLife;

namespace CrazyCoder.ViewModels;

/// <summary>音频设备信息</summary>
public class AudioDeviceInfo
{
    /// <summary>设备 ID</summary>
    public String Id { get; set; } = "";

    /// <summary>设备名称</summary>
    public String Name { get; set; } = "";

    /// <summary>设备友好名称</summary>
    public String FriendlyName { get; set; } = "";

    /// <summary>声道数</summary>
    public Int32 Channels { get; set; }

    /// <summary>采样率</summary>
    public Int32 SampleRate { get; set; }

    /// <summary>是否有播放功能</summary>
    public Boolean HasPlayback { get; set; }

    /// <summary>是否有录音功能</summary>
    public Boolean HasRecording { get; set; }

    /// <summary>音量</summary>
    public Single Volume { get; set; } = 0.5f;
}

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

    private MMDeviceEnumerator _enumerator;
    private Dictionary<String, AudioDeviceInfo> _devices = [];
    #endregion

    #region 构造
    /// <summary>实例化声卡选择器 ViewModel</summary>
    public AudioDeviceViewModel()
    {
        try
        {
            _enumerator = new MMDeviceEnumerator();
            RefreshDevices();
        }
        catch (Exception ex)
        {
            AppendLog($"初始化音频设备失败: {ex.Message}");
        }
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
            // 枚举播放设备
            var playbackDevices = _enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            foreach (var dev in playbackDevices)
            {
                var id = dev.ID;
                if (!_devices.ContainsKey(id))
                {
                    _devices[id] = new AudioDeviceInfo
                    {
                        Id = id,
                        Name = dev.FriendlyName,
                        FriendlyName = dev.DeviceFriendlyName,
                        HasPlayback = true,
                        Channels = dev.AudioClient.MixFormat.Channels,
                        SampleRate = dev.AudioClient.MixFormat.SampleRate,
                    };
                }
                else
                {
                    _devices[id].HasPlayback = true;
                }
            }

            // 枚举录音设备
            var recordingDevices = _enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            foreach (var dev in recordingDevices)
            {
                var id = dev.ID;
                if (!_devices.ContainsKey(id))
                {
                    _devices[id] = new AudioDeviceInfo
                    {
                        Id = id,
                        Name = dev.FriendlyName,
                        FriendlyName = dev.DeviceFriendlyName,
                        HasRecording = true,
                        Channels = dev.AudioClient.MixFormat.Channels,
                        SampleRate = dev.AudioClient.MixFormat.SampleRate,
                    };
                }
                else
                {
                    _devices[id].HasRecording = true;
                }
            }

            foreach (var kv in _devices)
            {
                DeviceList.Add(kv.Value.Name);
            }

            if (DeviceList.Count > 0) SelectedDeviceIndex = 0;

            AppendLog($"刷新完成，找到 {_devices.Count} 个音频设备");
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
        var dev = _devices.Values.FirstOrDefault(d => d.Name == name);
        if (dev == null) return;

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
        // NAudio 播放测试需要额外实现，这里简化处理
        AppendLog("播放测试功能需要在 View 层实现");
    }

    private void AppendLog(String msg)
    {
        Log += $"{msg}\r\n";
    }
    #endregion
}
