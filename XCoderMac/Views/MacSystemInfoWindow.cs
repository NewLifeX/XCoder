using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Interactivity;

namespace XCoderMac.Views;

/// <summary>通过 macOS system_profiler 获取真实硬件枚举信息，不使用 Windows WMI/NAudio。</summary>
public abstract class MacSystemInfoWindow : Window, INotifyPropertyChanged
{
    private readonly string _dataType;
    private readonly ListBox _devices;
    private readonly TextBox _details;
    private readonly TextBlock _status;
    private readonly Button _refresh;

    public new event PropertyChangedEventHandler? PropertyChanged;
    public string StatusText
    {
        get => _status.Text ?? "";
        set
        {
            if (_status.Text == value) return;
            _status.Text = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusText)));
        }
    }

    private readonly ObservableCollection<MacHardwareItem> _items = new();

    protected MacSystemInfoWindow(string title, string subtitle, string dataType)
    {
        _dataType = dataType;
        Title = $"{title} · XCoder";
        Width = 1110;
        Height = 760;
        MinWidth = 830;
        MinHeight = 580;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Background = Brush("#F7F9FC");

        _devices = new ListBox
        {
            ItemsSource = _items,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0)
        };
        _details = new TextBox
        {
            IsReadOnly = true,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            FontFamily = new FontFamily("Menlo,Consolas,monospace"),
            FontSize = 12,
            Padding = new Thickness(16),
            Background = Brush("#F9FBFE"),
            BorderBrush = Brush("#E1E8F3"),
            CornerRadius = new CornerRadius(10)
        };
        _status = new TextBlock { Text = "点击刷新获取本机设备", FontSize = 12, Foreground = Brush("#7185A0") };
        _refresh = new Button
        {
            Content = "刷新设备",
            Background = Brush("#286BC7"),
            Foreground = Brushes.White,
            CornerRadius = new CornerRadius(9),
            Padding = new Thickness(22, 11)
        };
        _refresh.Click += async (_, _) => await RefreshAsync();
        _devices.SelectionChanged += (_, _) =>
        {
            if (_devices.SelectedItem is MacHardwareItem item) _details.Text = item.Detail;
        };

        var header = new Grid { ColumnDefinitions = ColumnDefinitions.Parse("*,Auto") };
        header.Children.Add(new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            Spacing = 5,
            Children =
            {
                new TextBlock { Text = title, FontSize = 25, FontWeight = FontWeight.SemiBold, Foreground = Brush("#263C5A") },
                new TextBlock { Text = subtitle, FontSize = 12, Foreground = Brush("#93A0B0") }
            }
        });
        Grid.SetColumn(_refresh, 1);
        _refresh.VerticalAlignment = VerticalAlignment.Center;
        header.Children.Add(_refresh);

        var content = new Grid { ColumnDefinitions = ColumnDefinitions.Parse("340,14,*") };
        var left = Panel("设备列表");
        Grid.SetColumn(left, 0);
        Grid.SetRow(_devices, 1);
        _devices.Margin = new Thickness(10, 0, 10, 10);
        left.Children.Add(_devices);
        content.Children.Add(left);

        var right = Panel("设备详情（只读）");
        Grid.SetColumn(right, 2);
        Grid.SetRow(_details, 1);
        _details.Margin = new Thickness(12, 0, 12, 12);
        right.Children.Add(_details);
        content.Children.Add(right);

        var root = new Grid
        {
            Margin = new Thickness(28, 0, 28, 14),
            RowDefinitions = RowDefinitions.Parse("92,*,40")
        };
        root.Children.Add(header);
        Grid.SetRow(content, 1);
        root.Children.Add(content);
        Grid.SetRow(_status, 2);
        _status.VerticalAlignment = VerticalAlignment.Center;
        root.Children.Add(_status);
        Content = root;

        Opened += async (_, _) => await RefreshAsync();
    }

    private static Grid Panel(string title)
    {
        var grid = new Grid
        {
            RowDefinitions = RowDefinitions.Parse("Auto,*"),
            Background = Brush("#FFFFFF"),
            Margin = new Thickness(0)
        };
        var heading = new TextBlock
        {
            Text = title,
            FontWeight = FontWeight.SemiBold,
            Foreground = Brush("#304968"),
            FontSize = 16,
            Margin = new Thickness(16)
        };
        grid.Children.Add(heading);
        return grid;
    }

    private static IBrush Brush(string color) => new SolidColorBrush(Color.Parse(color));

    private async Task RefreshAsync()
    {
        _refresh.IsEnabled = false;
        _status.Text = "正在读取 macOS 系统设备信息…";
        try
        {
            var psi = new ProcessStartInfo("/usr/sbin/system_profiler")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            psi.ArgumentList.Add(_dataType);
            psi.ArgumentList.Add("-json");

            var raw = await Task.Run(async () =>
            {
                using var process = Process.Start(psi) ?? throw new InvalidOperationException("无法启动系统设备枚举程序");
                var stdout = process.StandardOutput.ReadToEndAsync();
                var stderr = process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();
                var output = await stdout;
                var error = await stderr;
                if (process.ExitCode != 0) throw new InvalidOperationException(error);
                return output;
            });

            using var doc = JsonDocument.Parse(raw);
            var results = new ObservableCollection<MacHardwareItem>();
            if (doc.RootElement.TryGetProperty(_dataType, out var items))
                Visit(items, results);

            _items.Clear();
            foreach (var item in results) _items.Add(item);
            _details.Text = _items.Count > 0 ? _items[0].Detail : "未检测到相关设备，或 macOS 未提供此类设备的详细信息。";
            if (_items.Count > 0) _devices.SelectedIndex = 0;
            StatusText = $"读取完成：{_items.Count} 项设备记录";
        }
        catch (Exception ex)
        {
            StatusText = $"读取失败：{ex.Message}";
        }
        finally
        {
            _refresh.IsEnabled = true;
        }
    }

    private static void Visit(JsonElement element, ObservableCollection<MacHardwareItem> output)
    {
        if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray()) Visit(item, output);
            return;
        }
        if (element.ValueKind != JsonValueKind.Object) return;

        if (element.TryGetProperty("_name", out var name))
        {
            var title = name.ToString();
            var detail = string.Join(Environment.NewLine,
                element.EnumerateObject()
                       .Where(kv => kv.Name != "_items")
                       .Select(kv => $"{kv.Name}: {kv.Value}"));
            output.Add(new MacHardwareItem(title, detail));
        }

        if (element.TryGetProperty("_items", out var children))
            Visit(children, output);
    }

    private sealed record MacHardwareItem(string Title, string Detail)
    {
        public override string ToString() => Title;
    }
}

public sealed class MacUsbWindow : MacSystemInfoWindow
{
    public MacUsbWindow() : base("USB 设备", "macOS 系统硬件枚举 · 只读检测", "SPUSBDataType") { }
}

public sealed class MacAudioWindow : MacSystemInfoWindow
{
    public MacAudioWindow() : base("声卡与音频设备", "CoreAudio 系统信息 · 输入、输出与采样率", "SPAudioDataType") { }
}
