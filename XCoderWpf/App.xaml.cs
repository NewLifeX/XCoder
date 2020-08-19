using System;
using System.Windows;
using HandyControl.Data;
using HandyControl.Themes;
using HandyControl.Tools;
using Prism.Ioc;
using Prism.Regions;
using XCoderWpf.Views;

namespace XCoderWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell() => Container.Resolve<MainWindow>();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Container.Resolve<IRegionManager>().RegisterViewWithRegion("ContentRegion", typeof(Overview));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<DataBasePublish>();
            containerRegistry.RegisterForNavigation<RegexWindow>();
            //containerRegistry.RegisterForNavigation<UploadToCloud>();
            //containerRegistry.RegisterForNavigation<MusicPlayer>();
            //containerRegistry.RegisterForNavigation<Login>();
        }

        internal void UpdateSkin(SkinType skin)
        {
            SharedResourceDictionary.SharedDictionaries.Clear();
            Resources.MergedDictionaries.Add(ResourceHelper.GetSkin(skin));
            Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml")
            });
            Current.MainWindow?.OnApplyTemplate();
        }
    }
}
