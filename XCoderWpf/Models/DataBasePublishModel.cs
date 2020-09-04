using System;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using XCode.DataAccessLayer;

namespace XCoderWpf.Models
{
    public class DataBasePublishModel
    {

    }
    public enum DataSourceType
    {
        Xml,
        Db
    }

    public class ConnectionStringModel
    {
        public Uri IconSource { get; set; }
        public string Title { get; set; }
        public string Server { get; set; }
        public string ShortName => Server?.Length > 30 ? Server.Substring(0, 30) : Server ?? "";
        public DelegateCommand<ConnectionStringModel> SelectConncectionCmd { get; set; }
    }

    public class TableInfoModel : BindableBase
    {
        public bool IsChecked { get => _isChecked; set => SetProperty(ref _isChecked, value); }
        private bool _isChecked;

        public string Name { get; set; }
        public bool IsView { get; set; }
        public IDataTable Data { get; set; }
    }
}
