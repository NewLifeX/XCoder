using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrazyCoder.Models;
using NewLife;
using NewLife.Log;
using XCode;
using XCode.DataAccessLayer;

namespace CrazyCoder.ViewModels;

/// <summary>跨库数据同步 ViewModel</summary>
public partial class DataSyncViewModel : ObservableObject
{
    #region 属性

    /// <summary>所有可用连接</summary>
    public ObservableCollection<String> Connections { get; } = [];

    /// <summary>目标连接列表（排除源连接）</summary>
    public ObservableCollection<String> TargetConnections { get; } = [];

    /// <summary>选中的源连接</summary>
    [ObservableProperty]
    private String _sourceConn = "";

    /// <summary>选中的目标连接</summary>
    [ObservableProperty]
    private String _targetConn = "";

    /// <summary>源数据库是否已连接</summary>
    [ObservableProperty]
    private Boolean _isSourceConnected;

    /// <summary>目标数据库是否已连接</summary>
    [ObservableProperty]
    private Boolean _isTargetConnected;

    /// <summary>同步表集合</summary>
    public ObservableCollection<SyncTableModel> Tables { get; } = [];

    /// <summary>是否同步架构</summary>
    [ObservableProperty]
    private Boolean _syncSchema = true;

    /// <summary>是否忽略错误</summary>
    [ObservableProperty]
    private Boolean _ignoreError;

    /// <summary>是否正在同步中</summary>
    [ObservableProperty]
    private Boolean _isSyncing;

    /// <summary>同步进度文本</summary>
    [ObservableProperty]
    private String _progressText = "";

    /// <summary>状态文本</summary>
    [ObservableProperty]
    private String _status = "就绪";

    /// <summary>源 DAL</summary>
    private DAL _sourceDal;

    /// <summary>源数据表集合</summary>
    private IList<IDataTable> _sourceTables;

    #endregion

    #region 构造

    /// <summary>实例化数据同步 ViewModel</summary>
    public DataSyncViewModel()
    {
        LoadConnections();
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

    /// <summary>连接源数据库</summary>
    [RelayCommand]
    private async Task ConnectSource()
    {
        if (SourceConn.IsNullOrEmpty()) return;

        try
        {
            Status = "正在连接源数据库...";
            _sourceDal = DAL.Create(SourceConn);

            await Task.Run(() =>
            {
                _sourceTables = _sourceDal.Tables;
            });

            if (_sourceTables == null) return;

            // 填充表列表
            Tables.Clear();
            foreach (var tb in _sourceTables.OrderBy(e => e.TableName))
            {
                Tables.Add(new SyncTableModel
                {
                    Name = tb.TableName,
                    DisplayName = tb.DisplayName,
                    EnableSync = true,
                });
            }

            // 加载行数
            await Task.Run(() => LoadRowCounts());

            // 更新目标列表
            TargetConnections.Clear();
            foreach (var item in Connections.Where(e => e != SourceConn))
            {
                TargetConnections.Add(item);
            }

            IsSourceConnected = true;
            Status = $"已连接源 {SourceConn}，共 {Tables.Count} 个表";
        }
        catch (Exception ex)
        {
            Status = $"连接失败：{ex.Message}";
            XTrace.WriteException(ex);
        }
    }

    /// <summary>断开源数据库</summary>
    [RelayCommand]
    private void DisconnectSource()
    {
        _sourceDal = null;
        _sourceTables = null;
        Tables.Clear();
        TargetConnections.Clear();
        IsSourceConnected = false;
        IsTargetConnected = false;
        Status = "已断开源数据库";
    }

    /// <summary>连接目标数据库</summary>
    [RelayCommand]
    private async Task ConnectTarget()
    {
        if (TargetConn.IsNullOrEmpty()) return;

        try
        {
            Status = "正在连接目标数据库...";
            var dal = DAL.Create(TargetConn);

            await Task.Run(() =>
            {
                var tables = dal.Tables;
                if (tables == null) return;

                foreach (var item in Tables)
                {
                    var tb = tables.FirstOrDefault(e => e.TableName.EqualIgnoreCase(item.Name));
                    if (tb != null)
                    {
                        var sb = new SelectBuilder { Table = item.Name };
                        item.TargetCount = dal.SelectCount(sb);
                    }
                }
            });

            IsTargetConnected = true;
            Status = $"已连接目标 {TargetConn}";
        }
        catch (Exception ex)
        {
            Status = $"连接目标失败：{ex.Message}";
            XTrace.WriteException(ex);
        }
    }

    /// <summary>断开目标数据库</summary>
    [RelayCommand]
    private void DisconnectTarget()
    {
        IsTargetConnected = false;
        Status = "已断开目标数据库";
    }

    /// <summary>加载源表行数</summary>
    private void LoadRowCounts()
    {
        if (_sourceDal == null || _sourceTables == null) return;

        foreach (var item in Tables)
        {
            var sb = new SelectBuilder { Table = item.Name };
            item.SourceCount = _sourceDal.SelectCount(sb);
        }
    }

    /// <summary>开始同步数据</summary>
    [RelayCommand]
    private async Task SyncData()
    {
        if (_sourceDal == null || TargetConn.IsNullOrEmpty()) return;

        var syncTables = Tables.Where(e => e.EnableSync).ToList();
        if (syncTables.Count == 0)
        {
            Status = "没有选中任何需要同步的表";
            return;
        }

        IsSyncing = true;
        Status = "正在同步...";

        try
        {
            await Task.Run(() =>
            {
                var dal = DAL.Create(TargetConn);
                var targetTables = dal.Tables ?? new List<IDataTable>();

                _sourceDal.Db.ShowSQL = false;
                _sourceDal.Session.ShowSQL = false;
                dal.Db.ShowSQL = false;
                dal.Session.ShowSQL = false;

                var total = syncTables.Count;
                var index = 0;

                foreach (var item in syncTables)
                {
                    index++;
                    ProgressText = $"[{index}/{total}] {item.Name}";

                    try
                    {
                        if (!SyncSchema && !targetTables.Any(e => e.TableName.EqualIgnoreCase(item.Name)))
                        {
                            item.Description = "目标表不存在，跳过！";
                        }
                        else
                        {
                            var tb = _sourceTables?.FirstOrDefault(e => e.TableName == item.Name);
                            if (tb != null)
                            {
                                var rs = _sourceDal.Sync(tb, TargetConn, SyncSchema);
                                item.SyncCount = rs;
                                item.Description = "成功！";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        item.Description = ex.Message;
                        XTrace.WriteException(ex);

                        if (!IgnoreError) throw;
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // 刷新 UI
                        var _ = Tables;
                    });
                }

                _sourceDal.Session.ShowSQL = true;
                _sourceDal.Db.ShowSQL = true;
                dal.Session.ShowSQL = true;
                dal.Db.ShowSQL = true;

                ProgressText = "";
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Status = $"同步完成！共处理 {total} 个表";
                });
            });
        }
        catch (Exception ex)
        {
            Status = $"同步失败：{ex.Message}";
            XTrace.WriteException(ex);
        }
        finally
        {
            IsSyncing = false;
        }
    }

    /// <summary>全选</summary>
    [RelayCommand]
    private void SelectAll()
    {
        foreach (var item in Tables)
        {
            item.EnableSync = true;
        }
    }

    /// <summary>反选</summary>
    [RelayCommand]
    private void InvertSelection()
    {
        foreach (var item in Tables)
        {
            item.EnableSync = !item.EnableSync;
        }
    }

    /// <summary>选择源多出（源有目标无）的表</summary>
    [RelayCommand]
    private async Task SelectDifferent()
    {
        if (TargetConn.IsNullOrEmpty()) return;

        await Task.Run(() =>
        {
            var dal = DAL.Create(TargetConn);
            var tables = dal.Tables;
            if (tables == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var item in Tables)
                {
                    item.EnableSync = !tables.Any(e => e.TableName.EqualIgnoreCase(item.Name));
                }
            });
        });
    }

    #endregion
}
