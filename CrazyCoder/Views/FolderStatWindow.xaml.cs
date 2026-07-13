using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using NewLife.Caching;

namespace CrazyCoder.Views;

/// <summary>文件夹大小统计工具</summary>
public partial class FolderStatWindow : Window
{
    private const String DummyNode = "\0";

    private readonly ICache _cache = new MemoryCache();

    #region 初始化
    /// <summary>实例化文件夹大小统计窗口</summary>
    public FolderStatWindow()
    {
        InitializeComponent();

        // 加载固定硬盘驱动器作为根节点
        foreach (var item in DriveInfo.GetDrives())
        {
            if (item.DriveType == DriveType.Fixed)
            {
                var header = $"{item.Name,-10} ({FormatSize(item.TotalSize)})";
                var tn = new TreeViewItem
                {
                    Header = header,
                    Tag = item.RootDirectory
                };
                // 预加虚拟子节点，展开时触发 MakeTree
                tn.Items.Add(new TreeViewItem { Header = DummyNode });
                treeView.Items.Add(tn);
            }
        }
    }
    #endregion

    #region 构造目录树
    private void MakeTree(TreeViewItem parent)
    {
        if (parent.Tag is not DirectoryInfo di) return;

        WriteLog($"展开目录 {di.FullName}");

        parent.Items.Clear();

        var list = new List<FileSystemInfo>();
        list.AddRange(di.GetDirectories());
        list.AddRange(di.GetFiles());

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

            var header = String.Format("{0,-" + len + "} {1,10}", item.Name, FormatSize(size));
            var tn = new TreeViewItem
            {
                Header = header,
                Tag = item,
                Background = GetBrush(size)
            };

            if (item is DirectoryInfo)
            {
                // 预加虚拟子节点，展开时继续加载
                tn.Items.Add(new TreeViewItem { Header = DummyNode });

                // 后台线程异步统计文件夹大小
                var captured = tn;
                Task.Run(() => TongJi(captured));
            }

            parent.Items.Add(tn);
        }
    }

    /// <summary>格式化文件大小</summary>
    private static String FormatSize(Int64 size)
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
    #endregion

    #region 统计文件夹大小
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

    #region 统计目录并设置大小
    private void TongJi(TreeViewItem node)
    {
        if (node.Tag is not DirectoryInfo di) return;

        try
        {
            var size = FolderSize(di);
            var header = node.Header as String;
            if (header != null && header.Length >= 10)
            {
                var str = header[..^10] + $"{FormatSize(size),10}";
                Dispatcher.Invoke(() =>
                {
                    node.Header = str;
                    node.Background = GetBrush(size);
                });
            }
        }
        catch (Exception ex)
        {
            WriteLog(ex.ToString());
        }
    }

    /// <summary>根据大小获取背景色</summary>
    private static Brush GetBrush(Int64 size)
    {
        if (size > 1024L * 1024 * 1024) return Brushes.Yellow;
        if (size > 100L * 1024 * 1024) return Brushes.LawnGreen;
        if (size > 1024L * 1024) return Brushes.LightBlue;
        if (size > 1024L) return Brushes.MistyRose;
        return Brushes.White;
    }
    #endregion

    #region 展开折叠目录树
    private void OnItemExpanded(Object sender, RoutedEventArgs e)
    {
        if (sender is TreeViewItem item && item.Tag is DirectoryInfo)
        {
            // 只有虚拟子节点时触发懒加载
            if (item.Items.Count == 1 && item.Items[0] is TreeViewItem dummy && DummyNode.Equals(dummy.Header))
            {
                MakeTree(item);
            }
        }
    }

    private void OnItemCollapsed(Object sender, RoutedEventArgs e)
    {
        if (sender is TreeViewItem item && item.Tag is DirectoryInfo)
        {
            // 折叠后清空，再次展开时重新计算
            item.Items.Clear();
            item.Items.Add(new TreeViewItem { Header = DummyNode });
        }
    }
    #endregion

    #region 右键菜单
    private static String GetSelectedPath(TreeView tree)
    {
        if (tree.SelectedItem is TreeViewItem node && node.Tag is FileSystemInfo fi)
            return fi.FullName;

        return null;
    }

    private void OnItemPreviewMouseRightButtonDown(Object sender, MouseButtonEventArgs e)
    {
        if (sender is TreeViewItem item)
        {
            item.Focus();
            item.IsSelected = true;
        }
    }

    private void OnOpenDirectory(Object sender, RoutedEventArgs e)
    {
        var path = GetSelectedPath(treeView);
        if (String.IsNullOrEmpty(path)) return;

        try
        {
            if (File.Exists(path))
                Process.Start("explorer.exe", "/select,\"" + path + "\"");
            else if (Directory.Exists(path))
                Process.Start("explorer.exe", "\"" + path + "\"");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "打开出错");
        }
    }

    private void OnDelete(Object sender, RoutedEventArgs e)
    {
        var path = GetSelectedPath(treeView);
        if (String.IsNullOrEmpty(path)) return;

        if (MessageBox.Show($"准备删除{path}\n删除将不可恢复，是否删除？", "确认删除", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            return;

        try
        {
            if (File.Exists(path))
                File.Delete(path);
            else if (Directory.Exists(path))
                DeleteRecursive(new DirectoryInfo(path));

            if (treeView.SelectedItem is TreeViewItem node)
            {
                RemoveNode(node);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "删除出错");
        }
    }

    private void RemoveNode(TreeViewItem node)
    {
        // 从父级容器移除（父级可能是 TreeView 或 TreeViewItem）
        if (node.Parent is ItemsControl parentControl)
        {
            parentControl.Items.Remove(node);
        }
    }

    /// <summary>递归删除目录</summary>
    private void DeleteRecursive(DirectoryInfo di)
    {
        foreach (var item in di.GetFiles())
        {
            WriteLog($"删除 {item.FullName}");
            try { item.Delete(); }
            catch (Exception ex) { WriteLog(ex.Message); }
        }
        foreach (var item in di.GetDirectories())
        {
            DeleteRecursive(item);
        }
        try { di.Delete(true); }
        catch (Exception ex) { WriteLog(ex.Message); }
    }
    #endregion

    #region 日志
    /// <summary>输出日志到 RichTextBox</summary>
    private void WriteLog(String message)
    {
        Dispatcher.Invoke(() =>
        {
            var paragraph = new Paragraph(new Run(message))
            {
                Foreground = Brushes.White,
                Margin = new Thickness(0)
            };
            txtLog.Document.Blocks.Add(paragraph);
            txtLog.ScrollToEnd();
        });
    }
    #endregion
}
