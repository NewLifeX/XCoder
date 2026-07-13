using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XCoderAv.Models;
using XCoderAv.Views;

namespace XCoderAv.ViewModels;

/// <summary>主视图模型</summary>
public partial class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        Menus =
        [
            new MenuModel() { IconFont = "📊", Title = "数据建模", BackColor = "#218868" },
            new MenuModel() { IconFont = "🌐", Title = "网络工具", BackColor = "#EE3B3B", ViewType = typeof(NetworkWindow) },
            new MenuModel() { IconFont = "🔗", Title = "RPC工具", BackColor = "#218868" },
            new MenuModel() { IconFont = "🔌", Title = "串口工具", BackColor = "#EE3B3B" },
            new MenuModel() { IconFont = "🗺️", Title = "地图接口", BackColor = "#218868" },
            new MenuModel() { IconFont = "🔍", Title = "正则表达式", BackColor = "#218868", ViewType = typeof(RegexWindow) },
            new MenuModel() { IconFont = "🎨", Title = "图标水印", BackColor = "#EE3B3B", ViewType = typeof(IconToolWindow) },
            new MenuModel() { IconFont = "🔒", Title = "加密解密", BackColor = "#218868", ViewType = typeof(SecurityWindow) },
            new MenuModel() { IconFont = "🗣️", Title = "语音助手", BackColor = "#EE3B3B", ViewType = typeof(SpeechWindow) },
            new MenuModel() { IconFont = "📁", Title = "文件夹统计", BackColor = "#218868", ViewType = typeof(FolderStatWindow) },
            new MenuModel() { IconFont = "📄", Title = "文件编码", BackColor = "#218868" },
            new MenuModel() { IconFont = "📍", Title = "GPS辅助", BackColor = "#218868", ViewType = typeof(GpsWindow) },
            new MenuModel() { IconFont = "📡", Title = "MQTT客户端", BackColor = "#EE3B3B", ViewType = typeof(MqttWindow) },
        ];

        SelectedMenu = Menus[0];
    }

    /// <summary>菜单集合</summary>
    public ObservableCollection<MenuModel> Menus { get; }

    [ObservableProperty]
    private MenuModel _selectedMenu = null!;

    partial void OnSelectedMenuChanged(MenuModel value)
    {
        // 点击菜单项时打开对应的工具窗口
        if (value.ViewType == null) return;

        // 检查窗口是否已打开
        var existing = GetOpenedWindow(value.ViewType);
        if (existing != null)
        {
            existing.Activate();
            return;
        }

        // 创建新窗口
        if (Activator.CreateInstance(value.ViewType) is Window window)
        {
            window.Show();
        }
    }

    /// <summary>获取已打开的窗口</summary>
    private static Window? GetOpenedWindow(Type viewType)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            foreach (var w in desktop.Windows)
            {
                if (w.GetType() == viewType)
                    return w;
            }
        }
        return null;
    }
}