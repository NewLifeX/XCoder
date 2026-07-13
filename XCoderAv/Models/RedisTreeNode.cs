using CommunityToolkit.Mvvm.ComponentModel;

namespace XCoderAv.Models;

/// <summary>Redis 树节点</summary>
public partial class RedisTreeNode : ObservableObject
{
    /// <summary>节点标题</summary>
    [ObservableProperty]
    private String _title = "";

    /// <summary>关联数据</summary>
    public Object? Tag { get; set; }

    /// <summary>是否展开</summary>
    [ObservableProperty]
    private Boolean _isExpanded;

    /// <summary>是否选中</summary>
    [ObservableProperty]
    private Boolean _isSelected;

    /// <summary>父节点</summary>
    public RedisTreeNode? Parent { get; set; }

    /// <summary>子节点</summary>
    public List<RedisTreeNode> Children { get; } = [];
}
