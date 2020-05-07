using System.Collections.ObjectModel;
using System.Windows;
using CrazyCoder.Models;
using CrazyCoder.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NewLife.Reflection;

namespace CrazyCoder.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            Menus = new ObservableCollection<MenuModel>();
            menus.Add(new MenuModel() { IconFont = "\xe635", Title = "数据建模", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe6b6", Title = "网络工具", BackColor = "#EE3B3B" });
            menus.Add(new MenuModel() { IconFont = "\xe6e1", Title = "RPC工具", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe614", Title = "串口工具", BackColor = "#EE3B3B" });
            menus.Add(new MenuModel() { IconFont = "\xe755", Title = "地图接口", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe635", Title = "正则表达式", BackColor = "#218868", Type = typeof(RegexWindow) });
            menus.Add(new MenuModel() { IconFont = "\xe6b6", Title = "图标水印", BackColor = "#EE3B3B" });
            menus.Add(new MenuModel() { IconFont = "\xe6e1", Title = "加密解密", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe614", Title = "语音助手", BackColor = "#EE3B3B" });
            menus.Add(new MenuModel() { IconFont = "\xe755", Title = "文件夹统计", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe635", Title = "文件编码", BackColor = "#218868" });

            SelectedMenu = Menus[0];

            SelectedCommand = new RelayCommand<MenuModel>(t => Select(t));
        }

        private ObservableCollection<MenuModel> menus;
        /// <summary>菜单集合</summary>
        public ObservableCollection<MenuModel> Menus
        {
            get => menus;
            set { menus = value; RaisePropertyChanged(); }
        }

        private MenuModel selectedMenu;
        /// <summary>选中菜单</summary>
        public MenuModel SelectedMenu
        {
            get => selectedMenu;
            set { selectedMenu = value; RaisePropertyChanged(); }
        }

        public RelayCommand<MenuModel> SelectedCommand { get; set; }

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