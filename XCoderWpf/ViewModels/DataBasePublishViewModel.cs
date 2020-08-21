using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using XCode.DataAccessLayer;
using XCoderWpf.Models;

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

        private ObservableCollection<ConnectionStringModel> _connectionStringList;
        /// <summary>菜单集合</summary>
        public ObservableCollection<ConnectionStringModel> ConnectionStringList { get => _connectionStringList; set { SetProperty(ref _connectionStringList, value); } }

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
            ConnectionStringList = new ObservableCollection<ConnectionStringModel>(list.Select(x => new ConnectionStringModel
            {
                Title = x,
                IconSource = new Uri("/Resources/Images/SqlServer/mysql.png",UriKind.Relative),
                Server = "192.168.102.1"
            }));
           
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
