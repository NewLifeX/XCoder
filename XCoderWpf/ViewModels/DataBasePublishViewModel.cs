using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

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
        public DataBasePublishViewModel()
        {
            DataList = new ObservableCollection<string>
            {
                "mssql111",
                "mssql112",
                "mssql113",
                "mssql114",
                "mssql115",
                "mssql1117",
            };
        }
    }
}
