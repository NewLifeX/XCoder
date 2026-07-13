using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;

namespace CrazyCoder.ViewModels;

/// <summary>手机备份 ViewModel</summary>
public partial class BackupViewModel : ObservableObject
{
    #region 属性
    /// <summary>目标目录</summary>
    [ObservableProperty]
    private String _destDir = "";

    /// <summary>源目录1</summary>
    [ObservableProperty]
    private String _srcDir1 = "";

    /// <summary>源目录2</summary>
    [ObservableProperty]
    private String _srcDir2 = "";

    /// <summary>源目录3</summary>
    [ObservableProperty]
    private String _srcDir3 = "";

    /// <summary>源目录4</summary>
    [ObservableProperty]
    private String _srcDir4 = "";

    /// <summary>源目录5</summary>
    [ObservableProperty]
    private String _srcDir5 = "";

    /// <summary>允许删除文件</summary>
    [ObservableProperty]
    private Boolean _allowDelete;

    /// <summary>是否正在备份</summary>
    [ObservableProperty]
    private Boolean _isBackingUp;

    /// <summary>日志输出</summary>
    [ObservableProperty]
    private String _log = "";
    #endregion

    #region 备份
    /// <summary>浏览目录</summary>
    [RelayCommand]
    private void Browse(String propertyName)
    {
        // View 层通过回调设置
    }

    /// <summary>开始备份</summary>
    [RelayCommand]
    private async void StartBackup()
    {
        if (IsBackingUp) return;

        var dest = DestDir;
        if (dest.IsNullOrEmpty())
        {
            Log += "目标目录不能为空\r\n";
            return;
        }

        var srcs = new[] { SrcDir1, SrcDir2, SrcDir3, SrcDir4, SrcDir5 }
            .Where(s => !s.IsNullOrEmpty())
            .ToArray();

        if (srcs.Length == 0)
        {
            Log += "源目录不能为空\r\n";
            return;
        }

        IsBackingUp = true;
        AppendLog($"开始备份到 {dest}...");

        try
        {
            var total = 0;
            foreach (var src in srcs)
            {
                total += await Task.Run(() => DoBackup(dest, src, AllowDelete));
            }

            AppendLog($"备份完成，共处理 {total} 个文件");
        }
        catch (Exception ex)
        {
            AppendLog($"备份出错: {ex.Message}");
        }
        finally
        {
            IsBackingUp = false;
        }
    }

    private static Int32 DoBackup(String dest, String src, Boolean allowDelete)
    {
        if (src.IsNullOrEmpty()) return 0;

        var total = 0;
        foreach (var fi in src.AsDirectory().GetAllFiles("*.jpg;*.jpeg;*.png;*.mp4;*.m4a;*.aac;*.mp3", true))
        {
            var newName = fi.Name;
            var p = fi.Name.IndexOf('~');
            if (p > 0)
            {
                newName = $"{fi.Name[..p]}{fi.Extension}";
            }

            if (TryGetTime(fi, ref newName, out var dt))
            {
                newName = $"{dest}\\{dt:yyyy}\\{dt:yyyyMM}\\{newName}";
                newName.EnsureDirectory(true);

                if (!File.Exists(newName))
                {
                    if (allowDelete)
                        fi.MoveTo(newName);
                    else
                        fi.CopyTo(newName);

                    total++;
                }
                else
                {
                    var nfi = newName.AsFile();
                    if (allowDelete && fi.Length == nfi.Length)
                    {
                        fi.Delete();
                        total++;
                    }
                }
            }
        }

        return total;
    }

    private static Boolean TryGetTime(FileInfo fi, ref String fileName, out DateTime time)
    {
        time = DateTime.MinValue;

        var ss = fileName.Split('_', '-', '.');
        if (ss.Length >= 2 && ss[1].Length >= 6 &&
            DateTime.TryParseExact($"{ss[0]}_{ss[1][..6]}", "yyyyMMdd_HHmmss", null,
                System.Globalization.DateTimeStyles.None, out var dt) && dt.Year > 2000)
        {
            time = dt;
            return true;
        }

        if (ss.Length >= 3 && ss[2].Length >= 6 &&
            DateTime.TryParseExact($"{ss[1]}_{ss[2][..6]}", "yyyyMMdd_HHmmss", null,
                System.Globalization.DateTimeStyles.None, out dt) && dt.Year > 2000)
        {
            time = dt;
            return true;
        }

        if (fileName.Length == 3 + 14 + 4 && fileName.StartsWith("IMG"))
        {
            var str = fileName.TrimStart("IMG").Substring(null, ".");
            if (str.Length >= 14 && DateTime.TryParseExact(str[..14], "yyyyMMddHHmmss", null,
                    System.Globalization.DateTimeStyles.None, out dt) && dt.Year > 2000)
            {
                time = dt;
                return true;
            }
        }

        return false;
    }

    private void AppendLog(String msg)
    {
        Log += $"{msg}\r\n";
    }
    #endregion
}
