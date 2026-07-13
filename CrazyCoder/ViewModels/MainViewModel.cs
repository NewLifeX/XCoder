using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrazyCoder.Models;
using CrazyCoder.Views;
using NewLife.Reflection;
// DataModeling, RedisManager, DataSync 窗口已在 Menus 中注册 Type

namespace CrazyCoder.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            Menus =
            [
                new MenuModel() { IconFont = "\xe635", Title = "数据建模", BackColor = "#218868", Type = typeof(DataModelingWindow) },
                new MenuModel() { IconFont = "\xe6b6", Title = "Redis 管理器", BackColor = "#EE3B3B", Type = typeof(RedisManagerWindow) },
                new MenuModel() { IconFont = "\xe6e1", Title = "跨库同步", BackColor = "#218868", Type = typeof(DataSyncWindow) },
                new MenuModel() { IconFont = "\xe6b6", Title = "网络工具", BackColor = "#EE3B3B", Type = typeof(NetworkWindow) },
                new MenuModel() { IconFont = "\xe6e1", Title = "RPC工具", BackColor = "#218868" },
                new MenuModel() { IconFont = "\xe614", Title = "串口工具", BackColor = "#EE3B3B", Type = typeof(SerialPortWindow) },
                new MenuModel() { IconFont = "\xe614", Title = "Modbus RTU", BackColor = "#EE3B3B", Type = typeof(ModbusRtuWindow) },
                new MenuModel() { IconFont = "\xe6b6", Title = "Modbus TCP", BackColor = "#EE3B3B", Type = typeof(ModbusTcpWindow) },
                new MenuModel() { IconFont = "\xe755", Title = "I/O 控制", BackColor = "#218868", Type = typeof(IoControlWindow) },
                new MenuModel() { IconFont = "\xe755", Title = "地图接口", BackColor = "#218868" },
                new MenuModel() { IconFont = "\xe635", Title = "正则表达式", BackColor = "#218868", Type = typeof(RegexWindow) },
                new MenuModel() { IconFont = "\xe6b6", Title = "图标水印", BackColor = "#EE3B3B", Type = typeof(IconToolWindow) },
                new MenuModel() { IconFont = "\xe6e1", Title = "加密解密", BackColor = "#218868", Type = typeof(SecurityWindow) },
                new MenuModel() { IconFont = "\xe614", Title = "语音助手", BackColor = "#EE3B3B", Type = typeof(SpeechWindow) },
                new MenuModel() { IconFont = "\xe755", Title = "GPS 辅助", BackColor = "#218868", Type = typeof(GpsWindow) },
                new MenuModel() { IconFont = "\xe635", Title = "MQTT 客户端", BackColor = "#EE3B3B", Type = typeof(MqttWindow) },
                new MenuModel() { IconFont = "\xe755", Title = "文件夹统计", BackColor = "#218868", Type = typeof(FolderStatWindow) },
                new MenuModel() { IconFont = "\xe635", Title = "文件编码", BackColor = "#218868" },
            ];

            SelectedMenu = Menus[0];
        }

        /// <summary>菜单集合</summary>
        public ObservableCollection<MenuModel> Menus { get; }

        [ObservableProperty]
        private MenuModel _selectedMenu = null!;

        [RelayCommand]
        private void Select(MenuModel model)
        {
            SelectedMenu = model;

            if (model.Type != null)
            {
                var window = model.Type.CreateInstance() as Window;
                window?.Show();
            }
        }
    }
}