using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        public IList<string> DataList { get { return _datalist; } set { SetProperty(ref _datalist, value); } }

        private string _searchFilter;
        public String SearchFilter
        {
            get => _searchFilter;
            set
            {

                SetProperty(ref _searchFilter, value);
                _connectionStringCollection.Clear();
                _connectionStringCollection.AddRange(_searchFilter?.Trim().Length > 0 ? _connectionStringList.Where(x => x.Title.ToLower().Contains(_searchFilter.ToLower())) : _connectionStringList);
            }
        }

        /// <summary>菜单集合</summary>
        private List<ConnectionStringModel> _connectionStringList;
        private ObservableCollection<ConnectionStringModel> _connectionStringCollection;
        public ObservableCollection<ConnectionStringModel> ConnectionStringCollection { get => _connectionStringCollection; set { SetProperty(ref _connectionStringCollection, value); } }

        private MenuModel selectedMenu;
        /// <summary>选中菜单</summary>
        public MenuModel SelectedMenu
        {
            get => selectedMenu;
            set { selectedMenu = value; RaisePropertyChanged(); }
        }


        public DataBasePublishViewModel()
        {
            //var list = DAL.ConnStrs.Keys.ToList();
            _connectionStringList = new List<ConnectionStringModel>(DAL.ConnStrs.Select(x => new ConnectionStringModel
            {
                Title = x.Key,
                IconSource = new Uri($"/Resources/Images/SqlServer/{DAL.Create(x.Key).DbType.ToString().ToLower()}.png", UriKind.Relative),
                Server = x.Value
            }));
            _connectionStringCollection = new ObservableCollection<ConnectionStringModel>(_connectionStringList);
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
