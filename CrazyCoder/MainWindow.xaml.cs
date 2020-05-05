using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CrazyCoder
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region 开发工具
        private void Regex_Click(Object sender, RoutedEventArgs e)
        {
            //frame.Navigate(new Uri("Views\\Regex.xaml", UriKind.Relative));
        }

        private void Ico_Click(Object sender, RoutedEventArgs e)
        {

        }

        private void Security_Click(Object sender, RoutedEventArgs e)
        {

        }

        private void Speech_Click(Object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 网络通信
        private void Network_Click(Object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 帮助
        private void CheckUpdate_Click(Object sender, RoutedEventArgs e)
        {

        }

        private void About_Click(Object sender, RoutedEventArgs e)
        {
            //frame.Navigate(new Uri("https://github.com/newlifex"));
            //Process.Start("https://www.newlifex.com");
        }
        #endregion
    }
}