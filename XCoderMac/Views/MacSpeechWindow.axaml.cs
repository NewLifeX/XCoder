using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using XCoderMac.Dialogs;

namespace XCoderMac.Views;

/// <summary>调用系统自带 /usr/bin/say，取代 XCoderAv 中只支持 Windows 的语音占位。</summary>
public partial class MacSpeechWindow : Window, INotifyPropertyChanged
{
    private Process? _speakingProcess;
    private bool _voicesLoaded;
    private string? _selectedVoice;
    private string _statusText = "就绪";
    private string _exportFilePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "XCoderMacSpeech.aiff");

    public new event PropertyChangedEventHandler? PropertyChanged;
    public ObservableCollection<string> Voices { get; } = new();

    public string? SelectedVoice
    {
        get => _selectedVoice;
        set { _selectedVoice = value; Changed(); }
    }

    public string StatusText
    {
        get => _statusText;
        set { _statusText = value; Changed(); }
    }

    public string ExportFilePath
    {
        get => _exportFilePath;
        set { _exportFilePath = value; Changed(); }
    }

    public MacSpeechWindow()
    {
        InitializeComponent();
        DataContext = this;
        Opened += async (_, _) => await EnsureTabVoicesAsync();
        Closed += (_, _) => StopReading();
    }

    private void Changed([CallerMemberName] string? property = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

    private static ProcessStartInfo NewSay(string text, int rate, string? voice, string? outputFile = null)
    {
        var info = new ProcessStartInfo("/usr/bin/say")
        {
            UseShellExecute = false,
            RedirectStandardError = true
        };
        info.ArgumentList.Add("-r");
        info.ArgumentList.Add(rate.ToString());
        if (!string.IsNullOrWhiteSpace(voice))
        {
            info.ArgumentList.Add("-v");
            info.ArgumentList.Add(voice);
        }
        if (outputFile is not null)
        {
            info.ArgumentList.Add("-o");
            info.ArgumentList.Add(outputFile);
        }
        info.ArgumentList.Add(text);
        return info;
    }

    public async Task EnsureTabVoicesAsync()
    {
        if (_voicesLoaded) return;
        _voicesLoaded = true;
        await LoadVoicesAsync();
    }

    public void StopForTab() => StopReading();

    private async Task LoadVoicesAsync()
    {
        try
        {
            var psi = new ProcessStartInfo("/usr/bin/say")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            psi.ArgumentList.Add("-v");
            psi.ArgumentList.Add("?");
            using var process = Process.Start(psi) ?? throw new InvalidOperationException("无法调用 macOS say");
            var textTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            var output = await textTask;
            var error = await errorTask;
            if (process.ExitCode != 0) throw new InvalidOperationException(error);

            Voices.Clear();
            foreach (var line in output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = Regex.Split(line.Trim(), @"\s{2,}");
                if (parts.Length > 0 && !string.IsNullOrWhiteSpace(parts[0]))
                    Voices.Add(parts[0].Trim());
            }

            SelectedVoice = Voices.Count > 0 ? Voices[0] : null;
            StatusText = $"已加载 {Voices.Count} 个系统语音";
        }
        catch (Exception ex)
        {
            StatusText = $"加载语音列表失败：{ex.Message}";
        }
    }

    private int Rate()
    {
        if (!int.TryParse(RateBox.Text, out var value) || value is < 80 or > 400)
            throw new ArgumentException("语速应为 80～400 词/分钟");
        return value;
    }

    private async void Speak_Click(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TextToSpeak.Text))
        {
            StatusText = "请输入要朗读的内容";
            return;
        }
        StopReading();
        try
        {
            var psi = NewSay(TextToSpeak.Text, Rate(), SelectedVoice);
            var process = Process.Start(psi) ?? throw new InvalidOperationException("语音进程启动失败");
            _speakingProcess = process;
            StatusText = "正在朗读…";
            SpeakButton.IsEnabled = false;

            var errorTask = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            var error = await errorTask;
            StatusText = process.ExitCode == 0 ? "朗读完成" : $"朗读中断：{error}";
        }
        catch (Exception ex)
        {
            StatusText = $"朗读失败：{ex.Message}";
        }
        finally
        {
            _speakingProcess?.Dispose();
            _speakingProcess = null;
            SpeakButton.IsEnabled = true;
        }
    }

    private void Stop_Click(object? sender, RoutedEventArgs e)
    {
        StopReading();
        StatusText = "已停止朗读";
    }

    private void StopReading()
    {
        try
        {
            if (_speakingProcess is { HasExited: false } process) process.Kill();
        }
        catch (InvalidOperationException) { }
    }

    private async void Export_Click(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TextToSpeak.Text))
        {
            StatusText = "请输入要导出的朗读内容";
            return;
        }

        try
        {
            var file = Path.GetFullPath(ExportFilePath.Trim());
            if (!file.EndsWith(".aiff", StringComparison.OrdinalIgnoreCase))
            {
                StatusText = "请输入 .aiff 格式的文件名";
                return;
            }
            if (!Directory.Exists(Path.GetDirectoryName(file)))
            {
                StatusText = "输出目录不存在";
                return;
            }

            if (File.Exists(file) && !await ConfirmDialog.ShowAsync((TopLevel.GetTopLevel(sender as Control) as Window) ?? this,
                    "覆盖已有音频文件？", $"目标文件已存在：{file}", "确认覆盖"))
                return;

            StatusText = "正在生成音频文件…";
            var psi = NewSay(TextToSpeak.Text, Rate(), SelectedVoice, file);
            using var process = Process.Start(psi) ?? throw new InvalidOperationException("无法启动语音合成");
            var errorTask = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            var error = await errorTask;
            StatusText = process.ExitCode == 0 ? $"已导出：{file}" : $"导出失败：{error}";
        }
        catch (Exception ex)
        {
            StatusText = $"导出失败：{ex.Message}";
        }
    }
}
