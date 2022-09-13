using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using NewLife.Caching;
using NewLife.Log;

namespace XCoder.FolderInfo;

[DisplayName("文件夹大小统计")]
public partial class FrmMain : Form, IXForm
{
    /// <summary>业务日志输出</summary>
    private ILog BizLog;

    #region 初始化
    public FrmMain()
    {
        InitializeComponent();

        // 动态调节宽度高度，兼容高DPI
        this.FixDpi();

        //Icon = IcoHelper.GetIcon("文件");
    }

    private void Form1_Load(Object sender, EventArgs e)
    {
        //var log = TextFileLog.Create(null, "Folder_{0:yyyy_MM_dd}.log");
        BizLog = txtLog.Combine(XTrace.Log);
        txtLog.UseWinFormControl();

        foreach (var item in DriveInfo.GetDrives())
        {
            if (item.DriveType == DriveType.Fixed)
            {
                var str = $"{item.Name,-10} ({FormatSize(item.TotalSize)})";
                var tn = treeView1.Nodes.Add(str);
                tn.Tag = item.RootDirectory;
                tn.Nodes.Add("no");
            }
        }
    }
    #endregion

    #region 构造目录树
    private void MakeTree(FileSystemInfo fsi, TreeNode Node)
    {
        BizLog.Info("展开目录 {0}", fsi.FullName);

        Node.Nodes.Clear();

        ////修正大小
        //if (Node.Text.EndsWith("-1 Byte"))
        //{
        //    Node.Text = Node.Text.Substring(0, Node.Text.Length - 8) + FormatSize(FolderSize(Node.Tag.ToString()));
        //}

        var di = fsi as DirectoryInfo;

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
            Int64 size = 0;

            if (item is FileInfo)
                size = (item as FileInfo).Length;
            else//默认不统计大小，加快显示速度
                size = -1;

            var str = String.Format("{0,-" + len.ToString() + "} {1,10}", item.Name, FormatSize(size));
            var tn = Node.Nodes.Add(str);
            tn.Tag = item;
            tn.BackColor = GetColor(size);
            tn.ContextMenuStrip = contextMenuStrip1;
            if (item is DirectoryInfo)
            {
                tn.Nodes.Add("no");

                // 使用后台线程统计大小信息
                //ThreadPool.QueueUserWorkItem(s => TongJi(tn));
                Task.Run(() => TongJi(tn));
            }
        }
    }

    private String FormatSize(Int64 size)
    {
        if (size < 1024) return size.ToString() + " Byte";
        var ds = (Double)size / 1024;
        if (ds < 1024) return ds.ToString("N2") + " K";
        ds /= 1024;
        if (ds < 1024) return ds.ToString("N2") + " M";
        ds /= 1024;
        if (ds < 1024) return ds.ToString("N2") + " G";
        ds /= 1024;
        if (ds < 1024) return ds.ToString("N2") + " T";
        throw new Exception("Too Large");
    }

    private Int32 StrLen(String str) => (str.Length + Encoding.UTF8.GetByteCount(str)) / 2;
    #endregion

    #region 统计文件夹大小
    private readonly ICache cache = new MemoryCache();

    private Int64 FolderSize(DirectoryInfo di)
    {
        Int64 size = 0;
        if (cache.TryGetValue<Int64>(di.FullName, out var v)) return v;

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

        cache.Set(di.FullName, size, 30);

        return size;
    }
    #endregion

    #region 统计目录并设置大小
    private void TongJi(Object obj)
    {
        if (obj is not TreeNode node || node.Tag is not DirectoryInfo di) return;

        try
        {
            var size = FolderSize(di);
            var str = node.Text[..^10] + $"{FormatSize(size),10}";
            //SetNodeText(node, str, GetColor(size));
            Invoke(() =>
            {
                node.Text = str;
                node.BackColor = GetColor(size);
            });
        }
        catch (Exception ex)
        {
            BizLog.Info(ex.ToString());
        }
    }

    private Color GetColor(Int64 size)
    {
        var color = Color.White;
        if (size > 1024) color = Color.MistyRose;
        if (size > 1024 * 1024) color = Color.LightBlue;
        if (size > 100 * 1024 * 1024) color = Color.LawnGreen;
        if (size > 1024 * 1024 * 1024) color = Color.Yellow;
        return color;
    }
    #endregion

    #region 展开折叠目录树
    private void treeView1_BeforeExpand(Object sender, TreeViewCancelEventArgs e)
    {
        var node = e.Node;
        if (node.Nodes != null && node.Nodes.Count > 0 /*&& node.Nodes[0].Text == "no"*/)
        {
            try
            {
                if (node.Tag is FileSystemInfo fi)
                    MakeTree(fi, node);
                //else if (node.Tag is String str)
                //    MakeTree(str + "", node);
            }
            catch { }
        }
    }

    private void treeView1_AfterCollapse(Object sender, TreeViewEventArgs e)
    {
        // 折叠后清空，使得再次展开时重新计算
        var nodes = e.Node?.Nodes;
        if (nodes != null)
        {
            nodes.Clear();
            nodes.Add("no");
        }
    }
    #endregion

    #region 右键菜单
    private String GetSelectedPath()
    {
        var node = treeView1.SelectedNode;
        if (node == null) return null;

        if (node.Tag is FileSystemInfo fi) return fi.FullName;

        return null;
    }

    private void 打开目录ToolStripMenuItem_Click(Object sender, EventArgs e)
    {
        var path = GetSelectedPath();
        if (String.IsNullOrEmpty(path)) return;

        if (path.Contains(" ")) path = "\"" + path + "\"";

        try
        {
            var exp = Environment.SystemDirectory.CombinePath("../explorer.exe").GetFullPath();
            if (File.Exists(path))
                Process.Start(exp, "/select," + path);
            else
                Process.Start(exp, path);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "打开出错");
        }
    }

    private void 删除ToolStripMenuItem_Click(Object sender, EventArgs e)
    {
        var path = GetSelectedPath();
        if (String.IsNullOrEmpty(path)) return;

        if (MessageBox.Show($"准备删除{path}{Environment.NewLine}删除将不可恢复，是否删除？", "确认删除", MessageBoxButtons.YesNo) == DialogResult.No) return;


        //if (path.Contains(" ")) path = "\"" + path + "\"";

        try
        {
            if (File.Exists(path))
                File.Delete(path);
            else
                //Directory.Delete(path, true);
                DeleteRecursive(new DirectoryInfo(path));

            treeView1.SelectedNode.Remove();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "删除出错");
        }
    }

    /// <summary>递归删除</summary>
    /// <param name="di"></param>
    private void DeleteRecursive(DirectoryInfo di)
    {
        // 删除本目录文件
        foreach (var item in di.GetFiles())
        {
            BizLog.Info("删除 {0}", item.FullName);
            try
            {
                item.Delete();
            }
            catch (Exception ex)
            {
                BizLog.Error(ex.Message);
            }
        }
        // 递归子目录
        foreach (var item in di.GetDirectories())
        {
            DeleteRecursive(item);
        }
        try
        {
            // 删除本目录
            di.Delete(true);
        }
        catch (Exception ex)
        {
            BizLog.Error(ex.Message);
        }
    }
    #endregion
}