using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HandyControl.Controls;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using XCode.Code;
using XCode.DataAccessLayer;
using XCoderWpf.Models;

namespace XCoderWpf.ViewModels
{
    public class DataBasePublishViewModel : BindableBase
    {
        private DataSourceType _dataSourceType;
        /// <summary>
        /// 选中的连接
        /// </summary>
        private ConnectionStringModel _selectConnectionStringModel;
        /// <summary>
        /// 连接列表
        /// </summary>
        private List<ConnectionStringModel> _connectionStringList;
        /// <summary>
        /// 表列表
        /// </summary>
        private IList<TableInfoModel> _tableList { get; set; }

        public DataBasePublishViewModel()
        {
            _connectionStringList = new List<ConnectionStringModel>(DAL.ConnStrs.Select(x => new ConnectionStringModel
            {
                Title = x.Key,
                Server = x.Value,
                IconSource = new Uri($"/Resources/Images/SqlServer/{DAL.Create(x.Key).DbType.ToString().ToLower()}.png", UriKind.Relative),
                SelectConncectionCmd = new Lazy<DelegateCommand<ConnectionStringModel>>(() => new DelegateCommand<ConnectionStringModel>((x) => _selectConnectionStringModel = x)).Value,
            }));
            _connectionStringCollection = new ObservableCollection<ConnectionStringModel>(_connectionStringList);
            CurrentDirectoryFullPath = Environment.CurrentDirectory + @"\";
            _tableCollection = new ObservableCollection<TableInfoModel>();
        }


        #region Property
        public int XmlTableListCount { get => _xmlTableListCount; set => SetProperty(ref _xmlTableListCount, value); }
        private int _xmlTableListCount;

        public int DbTableListCount { get => _dbTableListCount; set => SetProperty(ref _dbTableListCount, value); }
        private int _dbTableListCount;

        public string FileName { get => _fileName; set => SetProperty(ref _fileName, value); }
        private string _fileName;

        /// <summary>
        /// 连接筛选
        /// </summary>
        public string SearchConnectionStringFilter
        {
            get => _searchConnectionStringFilter;
            set
            {
                SetProperty(ref _searchConnectionStringFilter, value);
                _connectionStringCollection.Clear();
                _connectionStringCollection.AddRange(_searchConnectionStringFilter?.Trim().Length > 0 ? _connectionStringList.Where(x => x.Title.ToLower().Contains(_searchConnectionStringFilter.ToLower())) : _connectionStringList);
            }
        }
        private string _searchConnectionStringFilter;

        /// <summary>
        /// 展示的连接列表
        /// </summary>
        public ObservableCollection<ConnectionStringModel> ConnectionStringCollection { get => _connectionStringCollection; set => SetProperty(ref _connectionStringCollection, value); }
        private ObservableCollection<ConnectionStringModel> _connectionStringCollection;

        /// <summary>
        /// 当前工作目录
        /// </summary>
        public string CurrentDirectoryFullPath { get; set; } /*{ get => _currentDirectory; set => SetProperty(ref _currentDirectory, value); }*/

        public string CurrentDirectoryShortPath => $@"{(CurrentDirectoryFullPath.Length > 5 ? $@"{CurrentDirectoryFullPath.Substring(0, CurrentDirectoryFullPath.Length / 2)}" : CurrentDirectoryFullPath)}".Trim('\\') + @"...\";

        public string OutputPath { get => _outputPath; set => SetProperty(ref _outputPath, value); }
        private string _outputPath = "OutputPathxxx";

        public string NameSpace { get => _nameSpace; set => SetProperty(ref _nameSpace, value); }
        private string _nameSpace = "NameSpacexxx";

        public string EntityConnName { get => _entityConnName; set => SetProperty(ref _entityConnName, value); }
        private string _entityConnName = "EntityConnNamexxx";

        public string BaseClass { get => _baseClass; set => SetProperty(ref _baseClass, value); }
        private string _baseClass = "BaseClassxxx";

        public bool IsAllSelected { get => _isAllSelected; set { SetProperty(ref _isAllSelected, value); foreach (var item in _tableCollection) item.IsChecked = value; } }
        private bool _isAllSelected;

        public bool IsContainsView { get => _isContainsView; set { SetProperty(ref _isContainsView, value); _ = SetTablesListByFilter(); } }
        private bool _isContainsView = true;

        /// <summary>
        /// 表筛选
        /// </summary>
        public string SearchTableFilter { get => _searchTableFilter; set { SetProperty(ref _searchTableFilter, value); _ = SetTablesListByFilter(); } }
        private string _searchTableFilter;

        /// <summary>
        /// 表/视图列表
        /// </summary>
        public ObservableCollection<TableInfoModel> TableCollection { get => _tableCollection; set => SetProperty(ref _tableCollection, value); }
        private ObservableCollection<TableInfoModel> _tableCollection;
        #endregion





        #region Command
        public DelegateCommand ImportXmlCmd => new Lazy<DelegateCommand>(new DelegateCommand(async () =>
        {
            var openFileDialog = new OpenFileDialog();
            if (!openFileDialog.ShowDialog().Value || String.IsNullOrEmpty(openFileDialog.FileName)) return;
            try
            {
                FileName = Path.GetFileName(openFileDialog.FileName);
                var list = DAL.Import(File.ReadAllText(openFileDialog.FileName));
                //if (!cbIncludeView.Checked) list = list.Where(t => !t.IsView).ToList();
                //if (Config.NeedFix) list = Engine.FixTable(list);

                //Engine = null;
                //Engine.Tables = list;
                _tableList = list.Select(x => new TableInfoModel { IsChecked = _isAllSelected, Name = $"{x.TableName} ({(string.IsNullOrWhiteSpace(x.Description) ? x.Name : x.Description)})", IsView = x.IsView, Data = x }).ToList();
                
                //SetTables(list);
                //MessageBox.Show("导入架构成功！共" + (list == null ? 0 : list.Count) + "张表！", "导入架构", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            _dataSourceType = DataSourceType.Xml;
            await SetTablesListByFilter();
        })).Value;

        public DelegateCommand ImportConfigCmd => new Lazy<DelegateCommand>(new DelegateCommand(async () =>
        {
            IList<IDataTable> list = null;
            _tableCollection.Clear();
            try
            {
                await Task.Run(() => list = DAL.Create(_selectConnectionStringModel.Title).Tables);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            if (list == null) return;
            _tableList = list.Select(x => new TableInfoModel { IsChecked = _isAllSelected, Name = $"{x.TableName} ({(string.IsNullOrWhiteSpace(x.Description) ? x.Name : x.Description)})", IsView = x.IsView, Data = x }).ToList();
            
            _dataSourceType = DataSourceType.Db;
            await SetTablesListByFilter();
        })).Value;

        public DelegateCommand<ConnectionStringModel> SelectConncectionCmd => new Lazy<DelegateCommand<ConnectionStringModel>>(new DelegateCommand<ConnectionStringModel>((x) =>
        {
            MessageBox.Show("aaaaaaaaaa");

        })).Value;

        public DelegateCommand OpenOutputFolderCmd => new Lazy<DelegateCommand>(new DelegateCommand(() =>
        {
            var dir = _outputPath.GetFullPath();
            if (!Directory.Exists(dir)) dir = AppDomain.CurrentDomain.BaseDirectory;
            System.Diagnostics.Process.Start("explorer.exe", "\"" + dir + "\"");
        })).Value;

        public DelegateCommand BuildTablesCmd => new Lazy<DelegateCommand>(new DelegateCommand(() =>
        {
            var list = _tableCollection.Where(x => x.IsChecked).Select(x => x.Data).ToList();
            // _outputPath, _nameSpace, _entityConnName, _baseClass
            var rs = EntityBuilder.BuildTables(_tableCollection.Where(x => x.IsChecked).Select(x => x.Data).ToList(), new BuilderOption
            {
                Output = _outputPath,
                Namespace = _nameSpace,
                ConnName = _entityConnName,
                BaseClass = _baseClass
            });
        })).Value;
        #endregion





        #region Method
        private async Task<IList<TableInfoModel>> SetTablesListByFilter()
        {

            var list = _tableList.AsEnumerable();
            if (!_isContainsView) list = list.Where(t => !t.IsView);
            if (_searchTableFilter?.Trim().Length > 0) list = list.Where(x => x.Name.ToLower().Contains(_searchTableFilter.ToLower()));
            foreach (var item in list) item.IsChecked = _isAllSelected;

            var listCount = list.Count();
            XmlTableListCount = _dataSourceType == DataSourceType.Xml ? listCount : 0;
            DbTableListCount = _dataSourceType == DataSourceType.Db ? listCount : 0;

            _tableCollection.Clear();
            _tableCollection.AddRange(list);
            return await Task.FromResult(list.ToList()); ;
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

}
