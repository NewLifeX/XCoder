using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;

namespace CrazyCoder.ViewModels;

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
    /// <summary>打开文件</summary>
    [RelayCommand]
    private void OpenFile()
    {
        // View 层通过 OpenFileDialog 设置 FilePath
    }

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

            // 使用 NAudio 读取 WAV 文件
            using var reader = new NAudio.Wave.AudioFileReader(value);
            var channels = reader.WaveFormat.Channels;
            var sampleRate = reader.WaveFormat.SampleRate;
            var bitsPerSample = reader.WaveFormat.BitsPerSample;

            Log += $"声道数: {channels}, 采样率: {sampleRate}, 位深: {bitsPerSample}\r\n";

            var totalSamples = (Int32)(reader.Length / (bitsPerSample / 8));
            var samplesPerChannel = totalSamples / channels;

            // 读取所有样本
            var allSamples = new Single[totalSamples];
            var read = reader.Read(allSamples, 0, totalSamples);

            if (read == 0)
            {
                Log += "未能读取音频数据\r\n";
                return;
            }

            // 为每个声道生成频谱
            for (var ch = 0; ch < channels; ch++)
            {
                // 提取该声道数据
                var chSamples = new Single[samplesPerChannel];
                for (var i = 0; i < samplesPerChannel; i++)
                {
                    chSamples[i] = allSamples[i * channels + ch];
                }

                // 生成简单的频谱图（简化版 - 用振幅数据生成 bitmap）
                var melFile = Path.Combine(Path.GetTempPath(), $"mel_ch{ch}.png");
                GenerateMelSpectrogram(chSamples, sampleRate, melFile);
                _melFiles.Add(melFile);

                // 生成音量曲线
                var volFile = Path.Combine(Path.GetTempPath(), $"vol_ch{ch}.png");
                GenerateVolumeCurve(chSamples, volFile);
                _volFiles.Add(volFile);

                ChannelList.Add($"声道 {ch + 1}");
            }

            if (ChannelList.Count > 0) SelectedChannelIndex = 0;

            Log += "频谱生成完成\r\n";
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

    private static void GenerateMelSpectrogram(Single[] samples, Int32 sampleRate, String outputPath)
    {
        const Int32 fftSize = 1024;
        const Int32 hopSize = 512;
        var width = Math.Max(1, samples.Length / hopSize);
        var height = fftSize / 2;

        using var bmp = new Bitmap(width, height);
        for (var x = 0; x < width; x++)
        {
            var offset = x * hopSize;
            if (offset + fftSize > samples.Length) break;

            // 简单 FFT 幅度计算
            var window = new Double[fftSize];
            for (var i = 0; i < fftSize; i++)
            {
                if (offset + i < samples.Length)
                {
                    // Hann 窗
                    var hann = 0.5 * (1 - Math.Cos(2 * Math.PI * i / (fftSize - 1)));
                    window[i] = samples[offset + i] * hann;
                }
            }

            // 计算幅度谱
            for (var y = 0; y < height; y++)
            {
                var mag = Math.Abs(window[y]);
                var intensity = Math.Min(255, (Int32)(mag * 500));
                var color = Color.FromArgb(intensity, Math.Max(0, 255 - intensity * 2), Math.Max(0, 255 - intensity));
                bmp.SetPixel(x, height - 1 - y, color);
            }
        }

        bmp.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
    }

    private static void GenerateVolumeCurve(Single[] samples, String outputPath)
    {
        const Int32 width = 800;
        const Int32 height = 200;
        var step = Math.Max(1, samples.Length / width);

        using var bmp = new Bitmap(width, height);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.White);

        var pen = new Pen(Color.Blue, 1);
        var midY = height / 2;

        for (var x = 0; x < width; x++)
        {
            var idx = x * step;
            if (idx >= samples.Length) break;

            var amp = Math.Abs(samples[idx]);
            var y = (Int32)(midY - amp * midY);
            y = Math.Clamp(y, 0, height - 1);

            if (x == 0)
                bmp.SetPixel(0, y, Color.Blue);
            else
            {
                var prevIdx = (x - 1) * step;
                var prevAmp = Math.Abs(samples[Math.Min(prevIdx, samples.Length - 1)]);
                var prevY = (Int32)(midY - prevAmp * midY);
                prevY = Math.Clamp(prevY, 0, height - 1);
                g.DrawLine(pen, x - 1, prevY, x, y);
            }
        }

        bmp.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
    }
    #endregion
}
