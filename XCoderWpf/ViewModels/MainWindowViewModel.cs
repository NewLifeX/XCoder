using System;
using System.Windows.Controls;
using HandyControl.Controls;
using HandyControl.Data;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace XCoderWpf.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _region;
        public DelegateCommand<SelectionChangedEventArgs> SwitchItemCommand { get; set; }

        private String _title = "码神工具";
        public String Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _region = regionManager;
            SwitchItemCommand = new DelegateCommand<SelectionChangedEventArgs>(OnSwitchItem);
        }

        public DelegateCommand<FunctionEventArgs<object>> SwitchItemCmd => new Lazy<DelegateCommand<FunctionEventArgs<object>>>(() =>
             new DelegateCommand<FunctionEventArgs<object>>(SwitchItem)).Value;
        private void SwitchItem(FunctionEventArgs<object> info)
        {
            if (!(info.Info is SideMenuItem item)) return;
            _region.RequestNavigate("ContentRegion", item.Tag != null ? item.Tag.ToString() : "Overview");
            Growl.Info((info.Info as SideMenuItem)?.Header.ToString());
        }

        private void OnSwitchItem(SelectionChangedEventArgs e)
        {
            if (!(e.AddedItems[0] is ListBoxItem item)) return;
            _region.RequestNavigate("ContentRegion", item.Tag != null ? item.Tag.ToString() : "Overview");
        }

        public DelegateCommand<string> SelectCmd => new Lazy<DelegateCommand<string>>(() =>
            new DelegateCommand<string>(Select)).Value;

        private void Select(string header) => Growl.Success(header);
    }
}
