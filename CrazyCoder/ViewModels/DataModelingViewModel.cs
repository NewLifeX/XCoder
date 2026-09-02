using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrazyCoder.Models;
using Microsoft.Win32;
using NewLife;
using NewLife.Log;
using XCode.Code;
using XCode.DataAccessLayer;

namespace CrazyCoder.ViewModels;

/// <summary>数据建模/代码生成器 ViewModel。支持数据库连接与 XML 模型文件两种数据源，勾选表/视图生成实体类</summary>
public partial class DataModelingViewModel : ObservableObject
{
    #region 属性

    /// <summary>数据源类型</summary>
    private enum SourceKinds { Db, Xml }

    /// <summary>是否使用 XML 模型文件数据源</summary>
    [ObservableProperty]
    private Boolean _isXmlSource;

    /// <summary>是否使用数据库连接数据源</summary>
    public Boolean IsDbSource
    {
        get => !IsXmlSource;
        set => IsXmlSource = !value;
    }

    /// <summary>数据库连接面板可见性</summary>
    public Visibility DbVisibility => IsXmlSource ? Visibility.Collapsed : Visibility.Visible;

    /// <summary>模型文件面板可见性</summary>
    public Visibility XmlVisibility => IsXmlSource ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>模型文件路径</summary>
    [ObservableProperty]
    private String _modelFile = "";

    /// <summary>当前连接名</summary>
    [ObservableProperty]
    private String _connName = "";

    /// <summary>是否已连接</summary>
    [ObservableProperty]
    private Boolean _isConnected;

    /// <summary>数据库类型图标（pack URI）</summary>
    [ObservableProperty]
    private String _dbIcon = "";

    /// <summary>是否包含视图</summary>
    [ObservableProperty]
    private Boolean _includeView = true;

    /// <summary>关键字过滤</summary>
    [ObservableProperty]
    private String _keyword = "";

    /// <summary>数据表列表（按包含视图/关键字过滤后的展示列表）</summary>
    public ObservableCollection<TableItemModel> Tables { get; } = [];

    /// <summary>是否有数据表</summary>
    public Boolean HasTables => Tables.Count > 0;

    /// <summary>命名空间</summary>
    [ObservableProperty]
    private String _nameSpace = "";

    /// <summary>实体连接名</summary>
    [ObservableProperty]
    private String _entityConnName = "";

    /// <summary>输出路径</summary>
    [ObservableProperty]
    private String _outputPath = "";

    /// <summary>实体基类</summary>
    [ObservableProperty]
    private String _baseClass = "Entity";

    /// <summary>使用中文文件名</summary>
    [ObservableProperty]
    private Boolean _useCNFileName;

    /// <summary>连接按钮文本</summary>
    public String ConnectButtonText => IsConnected ? "断开" : "连接";

    /// <summary>状态文本</summary>
    [ObservableProperty]
    private String _status = "就绪";

    /// <summary>所有可用连接</summary>
    public ObservableCollection<String> Connections { get; } = [];

    /// <summary>当前已加载的数据源（null 表示未加载）</summary>
    private SourceKinds? _loadedSource;

    /// <summary>全部表/视图（数据源原始列表，保留勾选状态）</summary>
    private List<TableItemModel> _allTables = [];

    /// <summary>是否包含视图变化时刷新列表</summary>
    partial void OnIncludeViewChanged(Boolean value) => ApplyFilter();

    /// <summary>关键字变化时刷新列表</summary>
    partial void OnKeywordChanged(String value) => ApplyFilter();

    /// <summary>连接状态变化时更新按钮文本</summary>
    partial void OnIsConnectedChanged(Boolean value) => OnPropertyChanged(nameof(ConnectButtonText));

    /// <summary>数据源切换时切换面板，并自动恢复已加载过的数据源</summary>
    partial void OnIsXmlSourceChanged(Boolean value)
    {
        OnPropertyChanged(nameof(DbVisibility));
        OnPropertyChanged(nameof(XmlVisibility));

        if (value)
        {
            if (_loadedSource == SourceKinds.Xml) return;

            _loadedSource = null;
            ClearTables();
            if (!ModelFile.IsNullOrEmpty())
                _ = LoadXml();
            else
                Status = "请选择 XML 模型文件";
        }
        else
        {
            if (_loadedSource == SourceKinds.Db) return;

            _loadedSource = null;
            ClearTables();
            if (IsConnected)
                _ = LoadDb();
            else
                Status = "请选择数据库连接并连接";
        }
    }

    #endregion

    #region 静态

    /// <summary>内置数据库类型图标文件名（小写）</summary>
    private static readonly String[] _icons = ["access", "mysql", "oracle", "postgresql", "sqlite", "sqlserver"];

    /// <summary>根据数据库类型返回图标 pack URI，未知类型回退 none</summary>
    private static String GetDbIcon(DatabaseType dbType)
    {
        var name = dbType.ToString().ToLower();
        if (!_icons.Contains(name)) name = "none";

        return $"pack://application:,,,/Images/SqlServer/{name}.png";
    }

    #endregion

    #region 构造

    /// <summary>实例化数据建模 ViewModel</summary>
    public DataModelingViewModel()
    {
        LoadConnections();

        if (Connections.Count > 0)
            ConnName = Connections[0];

        Tables.CollectionChanged += (s, e) => OnPropertyChanged(nameof(HasTables));
    }

    private void LoadConnections()
    {
        Connections.Clear();
        foreach (var item in DAL.ConnStrs.Keys.OrderBy(e => e))
        {
            Connections.Add(item);
        }
    }

    #endregion

    #region 命令

    /// <summary>连接/断开数据库</summary>
    [RelayCommand]
    private async Task Connect()
    {
        if (IsConnected)
        {
            _loadedSource = null;
            ClearTables();
            IsConnected = false;
            DbIcon = "";
            Status = "已断开";
            return;
        }

        await LoadDb();
    }

    /// <summary>刷新数据表</summary>
    [RelayCommand]
    private async Task RefreshTables()
    {
        if (ConnName.IsNullOrEmpty()) return;

        await LoadDb();
    }

    /// <summary>选择 XML 模型文件并导入表结构</summary>
    [RelayCommand]
    private void SelectModelFile()
    {
        var dlg = new OpenFileDialog
        {
            Title = "选择 XCode 模型文件",
            Filter = "模型文件|*.xml|所有文件|*.*",
        };
        if (dlg.ShowDialog() != true) return;

        ModelFile = dlg.FileName;
        _ = LoadXml();
    }

    /// <summary>全选</summary>
    [RelayCommand]
    private void CheckAll()
    {
        foreach (var item in Tables)
        {
            item.IsChecked = true;
        }
    }

    /// <summary>反选</summary>
    [RelayCommand]
    private void InverseCheck()
    {
        foreach (var item in Tables)
        {
            item.IsChecked = !item.IsChecked;
        }
    }

    /// <summary>生成勾选表的代码</summary>
    [RelayCommand]
    private async Task GenerateChecked()
    {
        var list = _allTables.Where(e => e.IsChecked).Select(e => e.Table).ToList();
        if (list.Count == 0)
        {
            Status = "请先勾选要生成的表";
            return;
        }

        await Generate(list);
    }

    /// <summary>生成全部表的代码</summary>
    [RelayCommand]
    private async Task GenerateAll()
    {
        var list = Tables.Select(e => e.Table).ToList();
        if (list.Count == 0) return;

        await Generate(list);
    }

    /// <summary>打开输出目录</summary>
    [RelayCommand]
    private void OpenOutputDir()
    {
        var dir = OutputPath.GetFullPath();
        if (!Directory.Exists(dir))
            dir = AppDomain.CurrentDomain.BaseDirectory;

        Process.Start("explorer.exe", "\"" + dir + "\"");
    }

    #endregion

    #region 辅助

    /// <summary>从数据库连接加载表结构（反向工程）</summary>
    private async Task LoadDb()
    {
        var conn = ConnName;
        if (conn.IsNullOrEmpty()) return;

        try
        {
            Status = "正在连接...";

            // 异步加载表
            var list = await Task.Run(() => DAL.Create(conn).Tables);
            _allTables = WrapTables(list);

            IsConnected = true;
            _loadedSource = SourceKinds.Db;
            DbIcon = GetDbIcon(DAL.Create(conn).DbType);

            if (NameSpace.IsNullOrEmpty()) NameSpace = conn;
            if (EntityConnName.IsNullOrEmpty()) EntityConnName = conn;
            if (OutputPath.IsNullOrEmpty()) OutputPath = conn;

            ApplyFilter();
            Status = $"已连接 {conn}，共 {_allTables.Count} 个表/视图";
        }
        catch (Exception ex)
        {
            Status = $"连接失败：{ex.Message}";
            XTrace.WriteException(ex);
        }
    }

    /// <summary>从 XML 模型文件导入表结构</summary>
    private async Task LoadXml()
    {
        var file = ModelFile;
        if (file.IsNullOrEmpty() || !File.Exists(file)) return;

        try
        {
            Status = "正在解析模型文件...";

            // 导入模型 XML，读取表结构
            var list = await Task.Run(() => DAL.Import(File.ReadAllText(file)));
            _allTables = WrapTables(list);

            _loadedSource = SourceKinds.Xml;
            DbIcon = "";

            if (NameSpace.IsNullOrEmpty()) NameSpace = Path.GetFileNameWithoutExtension(file);
            if (OutputPath.IsNullOrEmpty()) OutputPath = Path.GetDirectoryName(file) ?? "";

            ApplyFilter();
            Status = $"已导入 {Path.GetFileName(file)}，共 {_allTables.Count} 个表/视图";
        }
        catch (Exception ex)
        {
            Status = $"导入失败：{ex.Message}";
            XTrace.WriteException(ex);
        }
    }

    /// <summary>生成实体代码</summary>
    private async Task Generate(List<IDataTable> tables)
    {
        await Task.Run(() =>
        {
            try
            {
                var option = new EntityBuilderOption
                {
                    BaseClass = BaseClass,
                    ConnName = EntityConnName,
                    Namespace = NameSpace,
                    Output = OutputPath,
                    ChineseFileName = UseCNFileName,
                };

                var sw = Stopwatch.StartNew();
                EntityBuilder.BuildTables(tables, option);
                sw.Stop();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Status = $"生成 {tables.Count} 个类完成！耗时：{sw.Elapsed}";
                });
            }
            catch (Exception ex)
            {
                XTrace.WriteException(ex);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Status = $"生成失败：{ex.Message}";
                });
            }
        });
    }

    /// <summary>过滤系统表、排序并包装为可勾选项</summary>
    private static List<TableItemModel> WrapTables(IList<IDataTable> tables)
    {
        // 过滤系统表
        var list = tables.Where(t => t.Name != "dtproperties" && t.Name != "sysconstraints" && t.Name != "syssegments");

        // 排序
        return [.. list.OrderBy(t => t.Name).Select(t => new TableItemModel(t))];
    }

    /// <summary>按包含视图与关键字过滤，重建展示列表（保留勾选状态）</summary>
    private void ApplyFilter()
    {
        var list = _allTables.AsEnumerable();
        if (!IncludeView) list = list.Where(e => !e.IsView);

        var kw = Keyword;
        if (!kw.IsNullOrWhiteSpace())
        {
            kw = kw.Trim();
            list = list.Where(e => e.Name.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                   e.Description.Contains(kw, StringComparison.OrdinalIgnoreCase));
        }

        Tables.Clear();
        foreach (var item in list)
        {
            Tables.Add(item);
        }
    }

    /// <summary>清空全部表/视图</summary>
    private void ClearTables()
    {
        _allTables = [];
        Tables.Clear();
    }

    #endregion
}
