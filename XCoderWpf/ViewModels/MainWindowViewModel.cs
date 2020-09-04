using System;
using System.Collections.Generic;
using System.Windows.Controls;
using HandyControl.Controls;
using HandyControl.Data;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using XCoderWpf.Models;
using XCoderWpf.Views;

namespace XCoderWpf.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _region;
        public  List<MainMenuModel> MainMenuList => new List<MainMenuModel>
        {
            new MainMenuModel { Id = 0 , Pid = 0 , IconFont = "\xe94d", Header = "数据库", BackColor = "#218868" },
            new MainMenuModel { Id = 1 , Pid = 0 , IconFont = "\xe635", Header = "数据建模", BackColor = "#218868", Tag = nameof(DataBasePublish) },
            new MainMenuModel { Id = 2 , Pid = 0 , IconFont = "\xe6b6", Header = "网络工具", BackColor = "#EE3B3B", Tag = nameof(RegexWindow)},
            new MainMenuModel { Id = 3 , Pid = 0 , IconFont = "\xe6e1", Header = "RPC工具", BackColor = "#218868" },

            new MainMenuModel { Id = 20, Pid = 20, IconFont = "\xe8fd", Header = "串口工具", BackColor = "#EE3B3B" },
            new MainMenuModel { Id = 21, Pid = 20, IconFont = "\xe755", Header = "地图接口", BackColor = "#218868" },
            new MainMenuModel { Id = 22, Pid = 20, IconFont = "\xe635", Header = "正则表达式", BackColor = "#218868", Tag = nameof(RegexWindow) },
            new MainMenuModel { Id = 23, Pid = 20, IconFont = "\xe6b6", Header = "图标水印", BackColor = "#EE3B3B" },
            new MainMenuModel { Id = 24, Pid = 20, IconFont = "\xe6e1", Header = "加密解密", BackColor = "#218868" },

            new MainMenuModel { Id = 50, Pid = 50, IconFont = "\xe8e4", Header = "语音助手", BackColor = "#EE3B3B" },
            new MainMenuModel { Id = 52, Pid = 50, IconFont = "\xe755", Header = "文件夹统计", BackColor = "#218868" },
            new MainMenuModel { Id = 53, Pid = 50, IconFont = "\xe635", Header = "文件编码", BackColor = "#218868" }
        };

        private String _title = "";
        public String Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }


        public MainWindowViewModel(IRegionManager regionManager) => _region = regionManager;

        public DelegateCommand<FunctionEventArgs<object>> SwitchItemCmd => new Lazy<DelegateCommand<FunctionEventArgs<object>>>(() => new DelegateCommand<FunctionEventArgs<object>>(OnSwitchItem)).Value;
        public DelegateCommand<string> SelectCmd => new Lazy<DelegateCommand<string>>(() => new DelegateCommand<string>(Select)).Value;



        private void OnSwitchItem(FunctionEventArgs<object> info)
        {
            if (!(info.Info is SideMenuItem item)) return;
            _region.RequestNavigate("ContentRegion", item.Tag != null ? item.Tag.ToString() : "Overview");
        }


        private void Select(string header) => Growl.Success(header);
    }
}
