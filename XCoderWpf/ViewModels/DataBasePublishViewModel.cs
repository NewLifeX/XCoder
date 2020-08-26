using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandyControl.Controls;
using Microsoft.Win32;
using NewLife;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using XCode.DataAccessLayer;
using XCoderWpf.Models;

namespace XCoderWpf.ViewModels
{
    public class DataBasePublishViewModel : BindableBase
    {
        public IList<IDataTable> Tables { get; set; }
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

        private ConnectionStringModel _selectConnectionStringModel;
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
                Server = x.Value,
                IconSource = new Uri($"/Resources/Images/SqlServer/{DAL.Create(x.Key).DbType.ToString().ToLower()}.png", UriKind.Relative),
                SelectCmd = new Lazy<DelegateCommand<ConnectionStringModel>>(() => new DelegateCommand<ConnectionStringModel>((x) => _selectConnectionStringModel = x)).Value,

            }));
            _connectionStringCollection = new ObservableCollection<ConnectionStringModel>(_connectionStringList);
        }

        public DelegateCommand SelectCmd => new DelegateCommand(() =>
          {
              MessageBox.Show("123");
          });

        #region private method
        private void ImportModel()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog().Value || String.IsNullOrEmpty(openFileDialog.FileName)) return;
            try
            {
                var list = DAL.Import(File.ReadAllText(openFileDialog.FileName));
                //if (!cbIncludeView.Checked) list = list.Where(t => !t.IsView).ToList();
                //if (Config.NeedFix) list = Engine.FixTable(list);

                //Engine = null;
                //Engine.Tables = list;
                Tables = list;

                //SetTables(list);


                //MessageBox.Show("导入架构成功！共" + (list == null ? 0 : list.Count) + "张表！", "导入架构", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        //private  void SetTables(Object source)
        //{
        //    if (source == null)
        //    {
        //        cbTableList.DataSource = source;
        //        cbTableList.Items.Clear();
        //        return;
        //    }
        //    var list = source as List<IDataTable>;
        //    if (list != null && list.Count > 0 && list[0].DbType == DatabaseType.SqlServer) // 增加对SqlServer 2000的特殊处理  ahuang
        //    {

        //        list.RemoveAll(dt => dt.Name == "dtproperties" || dt.Name == "sysconstraints" || dt.Name == "syssegments" || dt.Description.Contains("[0E232FF0-B466-"));
        //    }

        //    // 设置前最好清空，否则多次设置数据源会用第一次绑定控件，然后实际数据是最后一次
        //    //cbTableList.DataSource = source;
        //    cbTableList.Items.Clear();
        //    if (source != null)
        //    {
        //        // 表名排序
        //        var tables = source as List<IDataTable>;
        //        if (tables == null)
        //            cbTableList.DataSource = source;
        //        else
        //        {
        //            tables.Sort((t1, t2) => t1.Name.CompareTo(t2.Name));
        //            cbTableList.DataSource = tables;
        //        }
        //        ////cbTableList.DisplayMember = "Name";
        //        //cbTableList.ValueMember = "Name";
        //    }
        //    cbTableList.Update();
        //}
        #endregion
    }

    public class MenuModel
    {
        public String IconFont { get; set; }

        public String Title { get; set; }

        public String BackColor { get; set; }

        public Type Type { get; set; }
    }
}
