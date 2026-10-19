using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using XCoderAv.ViewModels;
using XCoderMac.Services;

namespace XCoderMac.Views;

/// <summary>
/// 通用 Mac 工具宿主：不复制或改动 XCoderAv 的 XAML / ViewModel / 事件绑定。
/// 将原窗口内容迁到 Mac 工作区，保留原窗口实例以维持事件处理程序和日志回调的生命周期。
/// </summary>
public partial class MacLegacyToolWindow : Window
{
    private readonly Window _originalWindow = null!; // 供 XAML 设计器使用的空构造函数不持有上游窗口。

    public Type OriginalToolType { get; private set; } = typeof(Window);
    public object? HostedViewModel => _originalWindow?.DataContext;

    public MacLegacyToolWindow()
    {
        InitializeComponent();
    }

    public MacLegacyToolWindow(MacTool tool) : this()
    {
        if (!typeof(Window).IsAssignableFrom(tool.WindowType))
            throw new ArgumentException("工具必须为 Avalonia Window", nameof(tool));

        OriginalToolType = tool.WindowType;

        // 先初始化源工具控件及事件，再转移到 Mac 窗口宿主；原 Window 实例继续由此字段持有。
        _originalWindow = (Window?)Activator.CreateInstance(tool.WindowType)
            ?? throw new InvalidOperationException($"无法实例化工具：{tool.Title}");

        ToolTitle.Text = tool.Title;
        ToolDescription.Text = tool.Description;
        Title = $"{tool.Title} · XCoder";

        var content = _originalWindow.Content;
        if (content == null)
            throw new InvalidOperationException($"工具“{tool.Title}”没有可展示的界面内容。");

        // ContentControl.Content setter 会移除旧的逻辑父级。
        _originalWindow.Content = null;
        HostedContent.Content = content;
        HostedContent.DataContext = _originalWindow.DataContext;

        // 加密解密旧版命令会对非法 HEX/Base64/密钥输入直接抛异常。
        // 在用户能够点击之前隔离所有命令，避免未处理异常终止 Mac 主程序。
        if (tool.Title == "加密解密" && HostedContent.DataContext is SecurityViewModel securityVm &&
            content is Control securityRoot)
        {
            var guarded = MacSecurityCommandGuard.Attach(securityRoot, securityVm);
            System.Diagnostics.Debug.WriteLine($"Security command guarded at construction: {guarded}");
            // 绑定值在实际加入主窗口时可能再次更新，附着后补检查一次。
            securityRoot.AttachedToVisualTree += (_, _) =>
            {
                var count = MacSecurityCommandGuard.Attach(securityRoot, securityVm);
                System.Diagnostics.Debug.WriteLine($"Security command guarded on attach: {count}");
            };
        }

        var sourceWidth = _originalWindow.Width;
        var sourceHeight = _originalWindow.Height;
        Width = Math.Clamp(sourceWidth + 55, 840, 1460);
        Height = Math.Clamp(sourceHeight + 120, 660, 1020);
        MinWidth = Math.Min(Width, 760);
        MinHeight = Math.Min(Height, 550);

        if (content is Control root)
        {
            root.Margin = new Thickness(5);
        }

        Closed += (_, _) =>
        {
            // 业务窗口不再显示，原数据上下文和关联 UI 事件会随 Mac 宿主一起释放。
            HostedContent.Content = null;
        };
    }
}
