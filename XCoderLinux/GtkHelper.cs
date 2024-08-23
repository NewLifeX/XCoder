using System.Diagnostics;
using System.Runtime.InteropServices;
using NewLife;
using NewLife.Log;
using NewLife.Web;

namespace XCoder;

/// <summary>GTK助手，自动检查安装GTK运行时</summary>
public class GtkHelper
{
    #region 属性
    public String GtkRoot { get; set; }

    public String GtkPath { get; set; }

    public Version Version { get; set; }
    #endregion

    #region 主要方法
    public static Task CheckRuntime(Int32 msTimeout = 3_000)
    {
        var task = Task.Run(async () =>
        {
            var gtk = new GtkHelper { Log = XTrace.Log };
            if (!gtk.Check()) await gtk.DownloadAsync();

            gtk.Install();
        });
        // 最多等3秒
        task.Wait(msTimeout);

        return task;
    }

    /// <summary>检查是否安装有GTK运行时</summary>
    /// <returns></returns>
    public Boolean Check()
    {
        // 只处理Windows
        if (!Runtime.Windows) return true;

        // LOCALDATA
        if (GtkRoot.IsNullOrEmpty())
        {
            var data = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            GtkRoot = data.CombinePath("Gtk").GetFullPath();
        }
        //XTrace.WriteLine("查找GTK运行时：{0}", GtkRoot);

        var di = GtkRoot.AsDirectory();
        if (!di.Exists) return false;

        var dis = di.GetDirectories();
        if (dis == null || dis.Length == 0) return false;

        //var gtk = dis.OrderByDescending(e => e.Name).FirstOrDefault();
        //GtkPath = gtk.FullName;
        //Version = new Version(gtk.Name.TrimStart('v', 'V'));
        foreach (var item in dis)
        {
            var gtk = item.FullName.CombinePath("libgdk-3-0.dll");
            if (File.Exists(gtk))
            {
                GtkPath = item.FullName;

                try
                {
                    Version = new Version(item.Name.TrimStart('v', 'V'));
                }
                catch { }

                break;
            }
        }

        if (GtkPath.IsNullOrEmpty()) return false;

        XTrace.WriteLine("发现GTK运行时：[{0}] {1}", Version, GtkPath);

        return true;
    }

    /// <summary>下载</summary>
    /// <returns></returns>
    public async Task<Boolean> DownloadAsync()
    {
        var set = NewLife.Setting.Current;

        //var client = new WebClientX
        //{
        //    Log = XTrace.Log
        //};

        //var links = client.GetLinks(set.PluginServer);
        //var lnk = links.First(e => e.Name.EqualIgnoreCase("Gtk"));

        //var file = client.DownloadLink(set.PluginServer, "Gtk", GtkRoot);
        var file = await DownloadLinkAsync(set.PluginServer, "Gtk", GtkRoot);
        var link = new Link();
        link.Parse(file);

        XTrace.WriteLine("版本：{0}", link.Version);

        //client.DownloadLinkAndExtract(set.PluginServer, "Gtk", gtk, true);

        GtkPath = GtkRoot.CombinePath(link.Version + "");
        Version = link.Version;

        Extract(file, GtkPath, true);

        return true;
    }

    /// <summary>安装</summary>
    public void Install()
    {
        if (!GtkPath.IsNullOrEmpty()) SetDllDirectory(GtkPath);
    }
    #endregion

    #region 辅助
    /// <summary>分析指定页面指定名称的链接，并下载到目标目录，返回目标文件</summary>
    /// <remarks>
    /// 根据版本或时间降序排序选择
    /// </remarks>
    /// <param name="urls">指定页面</param>
    /// <param name="name">页面上指定名称的链接</param>
    /// <param name="destdir">要下载到的目标目录</param>
    /// <returns>返回已下载的文件，无效时返回空</returns>
    private async Task<String> DownloadLinkAsync(String urls, String name, String destdir)
    {
        Log.Info("下载链接 {0}，目标 {1}", urls, name);

        var names = name.Split(",", ";");

        var file = "";
        Link link = null;
        Exception lastError = null;
        foreach (var url in urls.Split(",", ";"))
        {
            try
            {
                var http = new HttpClient();
                var html = await http.GetStringAsync(url);
                var ls = Link.Parse(html, url);
                if (ls.Length == 0) return file;

                // 过滤名称后降序排序，多名称时，先确保前面的存在，即使后面名称也存在并且也时间更新都不能用
                foreach (var item in names)
                {
                    link = ls.Where(e => !e.Url.IsNullOrWhiteSpace())
                       .Where(e => e.Name.EqualIgnoreCase(item) || e.FullName.Equals(item))
                       .OrderByDescending(e => e.Version)
                       .ThenByDescending(e => e.Time)
                       .FirstOrDefault();
                    if (link != null) break;
                }
            }
            catch (Exception ex)
            {
                lastError = ex;
            }
            if (link != null) break;
        }
        if (link == null)
        {
            if (lastError != null) throw lastError;

            return file;
        }

        var linkName = link.FullName;
        var file2 = destdir.CombinePath(linkName).EnsureDirectory();

        // 已经提前检查过，这里几乎不可能有文件存在
        if (File.Exists(file2))
        {
            // 如果连接名所表示的文件存在，并且带有时间，那么就智能是它啦
            var p = linkName.LastIndexOf("_");
            if (p > 0 && (p + 8 + 1 == linkName.Length || p + 14 + 1 == linkName.Length))
            {
                Log.Info("分析得到文件 {0}，目标文件已存在，无需下载 {1}", linkName, link.Url);
                return file2;
            }
        }

        Log.Info("分析得到文件 {0}，准备下载 {1}，保存到 {2}", linkName, link.Url, file2);
        // 开始下载文件，注意要提前建立目录，否则会报错
        file2 = file2.EnsureDirectory(true);

        var sw = Stopwatch.StartNew();
        {
            var http = new HttpClient();
            var ns = await http.GetStreamAsync(link.Url);
            using var fs = File.OpenWrite(file2);
            await ns.CopyToAsync(fs);
        }
        //TaskEx.Run(() => DownloadFileAsync(link.Url, file2)).Wait();
        sw.Stop();

        if (File.Exists(file2))
        {
            Log.Info("下载完成，共{0:n0}字节，耗时{1:n0}毫秒", file2.AsFile().Length, sw.ElapsedMilliseconds);
            file = file2;
        }

        return file;
    }

    /// <summary>分析指定页面指定名称的链接，并下载到目标目录，解压Zip后返回目标文件</summary>
    /// <param name="urls">提供下载地址的多个目标页面</param>
    /// <param name="name">页面上指定名称的链接</param>
    /// <param name="destdir">要下载到的目标目录</param>
    /// <param name="overwrite">是否覆盖目标同名文件</param>
    /// <returns></returns>
    private String Extract(String file, String destdir, Boolean overwrite = false)
    {
        if (file.IsNullOrEmpty()) return null;

        // 解压缩
        try
        {
            Log.Info("解压缩到 {0}", destdir);
            //ZipFile.ExtractToDirectory(file, destdir);
            file.AsFile().Extract(destdir, overwrite);

            // 删除zip
            File.Delete(file);

            return file;
        }
        catch (Exception ex)
        {
            Log.Error(ex?.GetTrue()?.ToString());

            // 这个时候出现异常，删除zip
            if (!file.IsNullOrEmpty() && File.Exists(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
        }

        return null;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    static extern Int32 SetDllDirectory(String pathName);
    #endregion

    #region 日志
    public ILog Log { get; set; } = Logger.Null;

    public void WriteLog(String format, params Object[] args) => Log?.Info(format, args);
    #endregion
}
