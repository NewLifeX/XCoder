using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HandyControl.Controls;
using XCoderWpf.Models;
using XCoderWpf.ViewModels;
using XCoderWpf.Views;

namespace XCoderWpf.Common
{
    public class MainMenu
    {
        private readonly SolidColorBrush _solidColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F4463"));
        private readonly FontFamily _yhFontFamily = new FontFamily("微软雅黑");
        private readonly FontFamily _fontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/#iconfont");
        private readonly Thickness _margin = new Thickness(10, 0, 0, 0);
        private readonly SideMenu _sizeMenu;
        private readonly MainWindowViewModel _mainViewViewModel;

        public MainMenu(MainWindowViewModel vm, SideMenu sizeMenu) { _mainViewViewModel = vm; _sizeMenu = sizeMenu; }

        public void InitializeSystemMenu()
        {
            var menus = _mainViewViewModel.MainMenuList;
            var lvl1Data = menus.Where(x => x.Pid == x.Id).ToList();

            _sizeMenu.Items.Clear();
            var lvl1MenuList = lvl1Data.Select(x => GetSideMenuItem(x, _yhFontFamily)).ToArray();
            _sizeMenu.Items.AddRange(lvl1MenuList);

            for (var index = 0; index < lvl1Data.Count; index++)
                lvl1MenuList[index].Items.AddRange(menus.Where(x => x.Pid == lvl1Data[index].Id && x.Pid != x.Id).Select(x => GetSideMenuItem(x)));
        }

        private SideMenuItem GetSideMenuItem(MainMenuModel item, FontFamily fontFamily = null)
        {
            var side = new SideMenuItem
            {
                Header = item.Header,
                Foreground = _solidColorBrush,
                Icon = new TextBlock { Text = item.IconFont, FontFamily = _fontFamily, Margin = _margin, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(item.BackColor)) },
                Tag = item.Tag,
                //Command = _mainViewViewModel.SelectCmd,
                //CommandParameter = item.Tag,
            };
            if (fontFamily == null) return side;
            side.FontFamily = _yhFontFamily;
            side.FontSize = 20;
            return side;
        }
    }
}
