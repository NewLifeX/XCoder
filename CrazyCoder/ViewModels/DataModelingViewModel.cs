using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;
using NewLife.Log;
using NewLife.Reflection;
using XCode;
using XCode.Code;
using XCode.DataAccessLayer;

namespace CrazyCoder.ViewModels;

/// <summary>数据建模/代码生成器 ViewModel</summary>
public partial class DataModelingViewModel : ObservableObject
{
    #region 属性

    /// <summary>当前连接名</summary>
    [ObservableProperty]
    private String _connName = "";

    /// <summary>是否已连接</summary>
    [ObservableProperty]
    private Boolean _isConnected;

    /// <summary>是否包含视图</summary>
    [ObservableProperty]
    private Boolean _includeView = true;

    /// <summary>数据表列表</summary>
    public ObservableCollection<IDataTable> Tables { get; } = [];

    /// <summary>选中的数据表</summary>
    [ObservableProperty]
    private IDataTable _selectedTable;

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

    /// <summary>生成泛型实体类</summary>
    [ObservableProperty]
    private Boolean _renderGenEntity;

    /// <summary>连接按钮文本</summary>
    public String ConnectButtonText => IsConnected ? "断开" : "连接";

    partial void OnIsConnectedChanged(Boolean value)
    {
        OnPropertyChanged(nameof(ConnectButtonText));
    }

    /// <summary>状态文本</summary>
    [ObservableProperty]
    private String _status = "就绪";

    /// <summary>所有可用连接</summary>
    public ObservableCollection<String> Connections { get; } = [];

    #endregion

    #region 构造

    /// <summary>实例化数据建模 ViewModel</summary>
    public DataModelingViewModel()
    {
        LoadConnections();

        if (Connections.Count > 0)
            ConnName = Connections[0];
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
            Tables.Clear();
            IsConnected = false;
            Status = "已断开";
            return;
        }

        if (ConnName.IsNullOrEmpty()) return;

        try
        {
            Status = "正在连接...";

            // 确保连接字符串可用
            var dal = DAL.Create(ConnName);
            _ = dal.ConnStr;

            // 异步加载表
            await Task.Run(() => LoadTables());

            IsConnected = true;
            Status = $"已连接 {ConnName}，共 {Tables.Count} 个表";

            if (NameSpace.IsNullOrEmpty()) NameSpace = ConnName;
            if (EntityConnName.IsNullOrEmpty()) EntityConnName = ConnName;
            if (OutputPath.IsNullOrEmpty()) OutputPath = ConnName;
        }
        catch (Exception ex)
        {
            Status = $"连接失败：{ex.Message}";
            XTrace.WriteException(ex);
        }
    }

    /// <summary>刷新数据表</summary>
    [RelayCommand]
    private async Task RefreshTables()
    {
        if (ConnName.IsNullOrEmpty()) return;

        Status = "正在刷新表...";
        await Task.Run(() => LoadTables());
        Status = $"已刷新，共 {Tables.Count} 个表";
    }

    private void LoadTables()
    {
        var dal = DAL.Create(ConnName);
        var list = dal.Tables;

        if (!IncludeView)
            list = list.Where(t => !t.IsView).ToList();

        // 过滤系统表
        list = list.Where(t => t.Name != "dtproperties" && t.Name != "sysconstraints" && t.Name != "syssegments").ToList();

        // 排序
        list = [.. list.OrderBy(t => t.Name)];

        Application.Current.Dispatcher.Invoke(() =>
        {
            Tables.Clear();
            foreach (var table in list)
            {
                Tables.Add(table);
            }
        });
    }

    /// <summary>生成选中表的代码</summary>
    [RelayCommand]
    private async Task GenerateTable()
    {
        if (SelectedTable == null) return;

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
                };

                var sw = Stopwatch.StartNew();
                var rs = EntityBuilder.BuildTables([SelectedTable], option);
                sw.Stop();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Status = $"生成 {SelectedTable.Name} 完成！耗时：{sw.Elapsed}";
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

    /// <summary>生成所有表的代码</summary>
    [RelayCommand]
    private async Task GenerateAll()
    {
        if (Tables.Count == 0) return;

        var tables = Tables.ToList();
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
                };

                var sw = Stopwatch.StartNew();
                var rs = EntityBuilder.BuildTables(tables, option);
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
}
