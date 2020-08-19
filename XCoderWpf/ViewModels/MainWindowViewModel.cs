using System;
using System.Windows.Controls;
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

        private void OnSwitchItem(SelectionChangedEventArgs e)
        {
            if (!(e.AddedItems[0] is ListBoxItem item)) return;
            _region.RequestNavigate("ContentRegion", item.Tag != null ? item.Tag.ToString() : "Overview");
        }
    }
}
