using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using XCode.DataAccessLayer;

namespace XCoderWpf.ViewModels
{
    public class DataBasePublishViewModel : BindableBase
    {
        private IList<string> _datalist;
        public IList<string> DataList
        {
            get { return _datalist; }
            set { SetProperty(ref _datalist, value); }
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

        public DataBasePublishViewModel()
        {
            var list = DAL.ConnStrs.Keys.ToList();
            DataList = new ObservableCollection<string>
            {
                "mssql111",
                "mssql112",
                "mssql113",
                "mssql114",
                "mssql115",
                "mssql1117",
            };

            Menus = new ObservableCollection<MenuModel>();
            menus.Add(new MenuModel() { IconFont = "\xe635", Title = "数据建模", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe6b6", Title = "网络工具", BackColor = "#EE3B3B" });
            menus.Add(new MenuModel() { IconFont = "\xe6e1", Title = "RPC工具", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe614", Title = "串口工具", BackColor = "#EE3B3B" });
            menus.Add(new MenuModel() { IconFont = "\xe755", Title = "地图接口", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe635", Title = "正则表达式", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe6b6", Title = "图标水印", BackColor = "#EE3B3B" });
            menus.Add(new MenuModel() { IconFont = "\xe6e1", Title = "加密解密", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe614", Title = "语音助手", BackColor = "#EE3B3B" });
            menus.Add(new MenuModel() { IconFont = "\xe755", Title = "文件夹统计", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe635", Title = "文件编码", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe635", Title = "文件编码", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe635", Title = "文件编码", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe635", Title = "文件编码", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe635", Title = "文件编码", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe635", Title = "文件编码", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe635", Title = "文件编码", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe635", Title = "文件编码", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe635", Title = "文件编码", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe635", Title = "文件编码", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe635", Title = "文件编码", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe635", Title = "文件编码", BackColor = "#218868" });
            menus.Add(new MenuModel() { IconFont = "\xe635", Title = "asdfasfasf", BackColor = "#218868" });

            SelectedMenu = Menus[0];
        }
    }

    public class MenuModel
    {
        public String IconFont { get; set; }

        public String Title { get; set; }

        public String BackColor { get; set; }

        public Type Type { get; set; }
    }
}
