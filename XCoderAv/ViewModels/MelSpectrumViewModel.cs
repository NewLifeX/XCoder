using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;

namespace XCoderAv.ViewModels;

/// <summary>音频梅尔频谱 ViewModel</summary>
public partial class MelSpectrumViewModel : ObservableObject
{
    #region 属性
    /// <summary>声道列表</summary>
    public ObservableCollection<String> ChannelList { get; } = [];

    /// <summary>选中的声道索引</summary>
    [ObservableProperty]
    private Int32 _selectedChannelIndex;

    /// <summary>文件路径</summary>
    [ObservableProperty]
    private String _filePath = "";

    /// <summary>状态日志</summary>
    [ObservableProperty]
    private String _log = "";

    /// <summary>梅尔频谱图路径</summary>
    [ObservableProperty]
    private String _melImagePath = "";

    /// <summary>音量曲线图路径</summary>
    [ObservableProperty]
    private String _volumeImagePath = "";

    private List<String> _melFiles = [];
    private List<String> _volFiles = [];
    #endregion

    #region 方法
    partial void OnFilePathChanged(String value)
    {
        if (value.IsNullOrEmpty() || !File.Exists(value)) return;

        ChannelList.Clear();
        _melFiles.Clear();
        _volFiles.Clear();

        if (!value.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
        {
            Log += "仅支持 WAV 文件\r\n";
            return;
        }

        try
        {
            Log += $"正在处理: {value}\r\n";

            // NAudio 在 Avalonia 跨平台不可用，显示提示
            Log += "音频频谱生成需要 NAudio 库（仅支持 Windows）。\r\n";
            Log += "在跨平台环境下请使用其他工具处理。\r\n";
        }
        catch (Exception ex)
        {
            Log += $"处理失败: {ex.Message}\r\n";
        }
    }

    partial void OnSelectedChannelIndexChanged(Int32 value)
    {
        if (value < 0 || value >= _melFiles.Count) return;

        MelImagePath = _melFiles[value];
        VolumeImagePath = _volFiles[value];
    }
    #endregion
}
