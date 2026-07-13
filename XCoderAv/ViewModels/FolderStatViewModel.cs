using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using NewLife.Caching;

namespace XCoderAv.ViewModels;

/// <summary>文件夹大小统计 ViewModel</summary>
public partial class FolderStatViewModel : ObservableObject
{
    #region 属性
    private readonly ICache _cache = new MemoryCache();

    /// <summary>目录根节点</summary>
    public ObservableCollection<FolderItem> Roots { get; } = [];

    /// <summary>日志文本</summary>
    [ObservableProperty]
    private String _logText = "";

    /// <summary>日志回调</summary>
    public Action<String>? OnLog { get; set; }
    #endregion

    #region 构造
    /// <summary>实例化文件夹大小统计 ViewModel</summary>
    public FolderStatViewModel()
    {
        LoadDrives();
    }

    private void LoadDrives()
    {
        foreach (var item in DriveInfo.GetDrives())
        {
            if (item.DriveType == DriveType.Fixed)
            {
                var root = new FolderItem
                {
                    Name = item.Name,
                    FullPath = item.RootDirectory.FullName,
                    Size = item.TotalSize,
                    SizeText = FormatSize(item.TotalSize),
                    IsDirectory = true,
                    IsExpanded = false
                };
                // 添加占位子节点，展开时触发加载
                root.Children.Add(new FolderItem { IsPlaceholder = true });
                Roots.Add(root);
            }
        }
    }
    #endregion

    #region 目录树操作
    /// <summary>展开目录</summary>
    public async Task ExpandFolder(FolderItem item)
    {
        if (!item.IsDirectory) return;
        if (item.Children.Count == 1 && item.Children[0].IsPlaceholder)
        {
            await LoadChildren(item);
        }
    }

    /// <summary>折叠目录</summary>
    public void CollapseFolder(FolderItem item)
    {
        if (!item.IsDirectory) return;
        item.Children.Clear();
        item.Children.Add(new FolderItem { IsPlaceholder = true });
    }

    private async Task LoadChildren(FolderItem parent)
    {
        parent.Children.Clear();

        var di = new DirectoryInfo(parent.FullPath);
        if (!di.Exists) return;

        WriteLog($"展开目录 {parent.FullPath}");

        var list = new List<FileSystemInfo>();
        try
        {
            list.AddRange(di.GetDirectories());
        }
        catch { }
        try
        {
            list.AddRange(di.GetFiles());
        }
        catch { }

        var max = 0;
        foreach (var item in list)
        {
            max = Math.Max(max, StrLen(item.Name));
        }
        max++;

        foreach (var item in list)
        {
            var len = max;
            len -= (StrLen(item.Name) - item.Name.Length);
            Int64 size;

            if (item is FileInfo fi)
                size = fi.Length;
            else
                size = -1;

            var child = new FolderItem
            {
                Name = item.Name,
                FullPath = item.FullName,
                Size = size,
                SizeText = FormatSize(size),
                IsDirectory = item is DirectoryInfo,
                DisplayName = String.Format("{0,-" + len + "} {1,10}", item.Name, FormatSize(size)),
                BackgroundColor = GetSizeColor(size)
            };

            if (item is DirectoryInfo)
            {
                // 占位子节点，展开时继续加载
                child.Children.Add(new FolderItem { IsPlaceholder = true });

                // 异步统计文件夹大小
                var captured = child;
                _ = Task.Run(() => CalculateSize(captured));
            }

            parent.Children.Add(child);
        }
    }

    private void CalculateSize(FolderItem node)
    {
        if (!node.IsDirectory) return;

        var di = new DirectoryInfo(node.FullPath);
        try
        {
            var size = FolderSize(di);
            node.Size = size;
            node.SizeText = FormatSize(size);
            node.BackgroundColor = GetSizeColor(size);
            node.DisplayName = $"{node.Name,-20} {FormatSize(size),10}";
        }
        catch (Exception ex)
        {
            WriteLog(ex.ToString());
        }
    }

    private Int64 FolderSize(DirectoryInfo di)
    {
        if (_cache.TryGetValue<Int64>(di.FullName, out var v)) return v;

        Int64 size = 0;
        try
        {
            foreach (var item in di.GetFiles())
            {
                size += item.Length;
            }
            foreach (var item in di.GetDirectories())
            {
                size += FolderSize(item);
            }
        }
        catch { }

        // 缓存30秒，避免重复遍历
        _cache.Set(di.FullName, size, 30);

        return size;
    }
    #endregion

    #region 辅助方法
    /// <summary>格式化文件大小</summary>
    public static String FormatSize(Int64 size)
    {
        if (size < 1024) return size + " Byte";
        var ds = (Double)size / 1024;
        if (ds < 1024) return ds.ToString("N2") + " K";
        ds /= 1024;
        if (ds < 1024) return ds.ToString("N2") + " M";
        ds /= 1024;
        if (ds < 1024) return ds.ToString("N2") + " G";
        ds /= 1024;
        if (ds < 1024) return ds.ToString("N2") + " T";
        return "∞";
    }

    /// <summary>计算字符串显示宽度（中文字符算2个宽度）</summary>
    private static Int32 StrLen(String str) => (str.Length + Encoding.UTF8.GetByteCount(str)) / 2;

    /// <summary>根据大小获取背景色</summary>
    private static String GetSizeColor(Int64 size)
    {
        if (size > 1024L * 1024 * 1024) return "#FFFF00";
        if (size > 100L * 1024 * 1024) return "#7CFC00";
        if (size > 1024L * 1024) return "#ADD8E6";
        if (size > 1024L) return "#FFE4E1";
        return "#FFFFFF";
    }

    /// <summary>写入日志</summary>
    public void WriteLog(String msg)
    {
        OnLog?.Invoke(msg);
    }
    #endregion
}

/// <summary>文件夹项</summary>
public partial class FolderItem : ObservableObject
{
    /// <summary>显示名称</summary>
    [ObservableProperty]
    private String _displayName = "";

    /// <summary>名称</summary>
    public String Name { get; set; } = "";

    /// <summary>完整路径</summary>
    public String FullPath { get; set; } = "";

    /// <summary>大小</summary>
    [ObservableProperty]
    private Int64 _size;

    /// <summary>大小文本</summary>
    [ObservableProperty]
    private String _sizeText = "";

    /// <summary>是否为目录</summary>
    public Boolean IsDirectory { get; set; }

    /// <summary>是否已展开</summary>
    [ObservableProperty]
    private Boolean _isExpanded;

    /// <summary>是否占位节点</summary>
    public Boolean IsPlaceholder { get; set; }

    /// <summary>背景颜色</summary>
    [ObservableProperty]
    private String _backgroundColor = "#FFFFFF";

    /// <summary>子节点</summary>
    public ObservableCollection<FolderItem> Children { get; } = [];
}
