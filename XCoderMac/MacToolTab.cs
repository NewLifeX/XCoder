using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Media;

namespace XCoderMac;

/// <summary>单个工具标签。工具视图与原始窗口实例随标签共存，切换不重新初始化。</summary>
public sealed class MacToolTab : INotifyPropertyChanged
{
    private bool _isActive;

    public MacTool Tool { get; }
    public string Title => Tool.Title;
    public string Icon => Tool.Icon;
    public Window SourceWindow { get; }
    public Control Content { get; }

    public MacToolTab(MacTool tool, Window sourceWindow, Control content)
    {
        Tool = tool;
        SourceWindow = sourceWindow;
        Content = content;
    }

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive == value) return;
            _isActive = value;
            Changed();
            Changed(nameof(TabBackground));
            Changed(nameof(TabForeground));
        }
    }

    public IBrush TabBackground => IsActive
        ? new SolidColorBrush(Color.Parse("#E5EFFD"))
        : new SolidColorBrush(Color.Parse("#F6F8FC"));

    public IBrush TabForeground => IsActive
        ? new SolidColorBrush(Color.Parse("#22599B"))
        : new SolidColorBrush(Color.Parse("#526781"));

    public event PropertyChangedEventHandler? PropertyChanged;
    private void Changed([CallerMemberName] string? property = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
}
