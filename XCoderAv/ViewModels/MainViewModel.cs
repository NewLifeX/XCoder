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
            new MenuModel() { IconFont = "📊", Title = "数据建模", BackColor = "#218868", ViewType = typeof(DataModelingWindow) },
            new MenuModel() { IconFont = "🌐", Title = "网络工具", BackColor = "#EE3B3B", ViewType = typeof(NetworkWindow) },
            new MenuModel() { IconFont = "🔗", Title = "RPC工具", BackColor = "#218868" },
            new MenuModel() { IconFont = "🔌", Title = "串口工具", BackColor = "#EE3B3B", ViewType = typeof(SerialPortWindow) },
            new MenuModel() { IconFont = "🔀", Title = "Modbus RTU", BackColor = "#218868", ViewType = typeof(ModbusRtuWindow) },
            new MenuModel() { IconFont = "🔀", Title = "Modbus TCP", BackColor = "#EE3B3B", ViewType = typeof(ModbusTcpWindow) },
            new MenuModel() { IconFont = "⚡", Title = "I/O控制", BackColor = "#218868", ViewType = typeof(IoControlWindow) },
            new MenuModel() { IconFont = "🗺️", Title = "地图接口", BackColor = "#218868", ViewType = typeof(MapWindow) },
            new MenuModel() { IconFont = "🔍", Title = "正则表达式", BackColor = "#218868", ViewType = typeof(RegexWindow) },
            new MenuModel() { IconFont = "🎨", Title = "图标水印", BackColor = "#EE3B3B", ViewType = typeof(IconToolWindow) },
            new MenuModel() { IconFont = "🔒", Title = "加密解密", BackColor = "#218868", ViewType = typeof(SecurityWindow) },
            new MenuModel() { IconFont = "🗣️", Title = "语音助手", BackColor = "#EE3B3B", ViewType = typeof(SpeechWindow) },
            new MenuModel() { IconFont = "📁", Title = "文件夹统计", BackColor = "#218868", ViewType = typeof(FolderStatWindow) },
            new MenuModel() { IconFont = "📄", Title = "文件编码", BackColor = "#218868", ViewType = typeof(FileEncodingWindow) },
            new MenuModel() { IconFont = "📍", Title = "GPS辅助", BackColor = "#218868", ViewType = typeof(GpsWindow) },
            new MenuModel() { IconFont = "📡", Title = "MQTT客户端", BackColor = "#EE3B3B", ViewType = typeof(MqttWindow) },
            new MenuModel() { IconFont = "💾", Title = "Redis管理", BackColor = "#218868", ViewType = typeof(RedisManagerWindow) },
            new MenuModel() { IconFont = "🔄", Title = "数据同步", BackColor = "#EE3B3B", ViewType = typeof(DataSyncWindow) },
            new MenuModel() { IconFont = "🌍", Title = "IP设置", BackColor = "#218868", ViewType = typeof(IpConfigWindow) },
            new MenuModel() { IconFont = "🔐", Title = "SSH工具", BackColor = "#EE3B3B", ViewType = typeof(SshWindow) },
            new MenuModel() { IconFont = "🔧", Title = "API调试", BackColor = "#218868", ViewType = typeof(ApiDebugWindow) },
            new MenuModel() { IconFont = "💬", Title = "消息调试", BackColor = "#EE3B3B", ViewType = typeof(MessageDebugWindow) },
            new MenuModel() { IconFont = "🔎", Title = "API发现", BackColor = "#218868", ViewType = typeof(ApiDiscoverWindow) },
            new MenuModel() { IconFont = "📱", Title = "手机备份", BackColor = "#EE3B3B", ViewType = typeof(BackupWindow) },
            new MenuModel() { IconFont = "🔌", Title = "USB设备", BackColor = "#218868", ViewType = typeof(UsbDeviceWindow) },
            new MenuModel() { IconFont = "🎵", Title = "声卡选择", BackColor = "#EE3B3B", ViewType = typeof(AudioDeviceWindow) },
            new MenuModel() { IconFont = "🎶", Title = "梅尔频谱", BackColor = "#218868", ViewType = typeof(MelSpectrumWindow) },
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