using System;
using HandyControl.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XCoderWpf.Common;
using XCoderWpf.ViewModels;

namespace XCoderWpf.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            new MainMenu(DataContext as MainWindowViewModel, MainSideMenu).InitializeSystemMenu();
        }

        #region Change Skin
        private void ButtonConfig_OnClick(Object sender, RoutedEventArgs e) => PopupConfig.IsOpen = true;

        private void ButtonSkins_OnClick(Object sender, RoutedEventArgs e)
        {
            if (!(e.OriginalSource is Button button) || !(button.Tag is SkinType tag)) return;
            PopupConfig.IsOpen = false;
            ((App)Application.Current).UpdateSkin(tag);
        }
        #endregion
    }
}
