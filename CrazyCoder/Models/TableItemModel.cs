using CommunityToolkit.Mvvm.ComponentModel;
using XCode.DataAccessLayer;

namespace CrazyCoder.Models;

/// <summary>可勾选数据表/视图项</summary>
public partial class TableItemModel : ObservableObject
{
    /// <summary>数据表结构</summary>
    public IDataTable Table { get; }

    /// <summary>是否勾选</summary>
    [ObservableProperty]
    private Boolean _isChecked;

    /// <summary>表名</summary>
    public String Name => Table.Name;

    /// <summary>描述</summary>
    public String Description => Table.Description ?? "";

    /// <summary>是否视图</summary>
    public Boolean IsView => Table.IsView;

    /// <summary>类型文本（表/视图）</summary>
    public String Kind => IsView ? "视图" : "表";

    /// <summary>实例化可勾选数据表项</summary>
    /// <param name="table">数据表结构</param>
    public TableItemModel(IDataTable table) => Table = table;
}
