namespace XCoderAv.Models;

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
