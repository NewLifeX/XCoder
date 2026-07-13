using System.IO;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;
using NewLife.IO;

namespace CrazyCoder.ViewModels;

/// <summary>文件编码转换 ViewModel</summary>
public partial class FileEncodingViewModel : ObservableObject
{
    #region 属性
    /// <summary>文件夹路径</summary>
    [ObservableProperty]
    private String _folderPath = "";

    /// <summary>文件后缀过滤</summary>
    [ObservableProperty]
    private String _suffixFilter = "*.cs;*.aspx;*.html;*.js;*.css;*.xml;*.json";

    /// <summary>目标编码</summary>
    [ObservableProperty]
    private String _targetEncoding = "UTF-8";

    /// <summary>可用编码列表</summary>
    public String[] Encodings { get; } = ["UTF-8", "UTF-8 NoBOM", "ASNI", "Unicode", "gb2312"];

    /// <summary>扫描结果</summary>
    [ObservableProperty]
    private String _scanResult = "";

    /// <summary>日志输出</summary>
    [ObservableProperty]
    private String _log = "";
    #endregion

    #region 方法
    static FileEncodingViewModel()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    /// <summary>选择文件夹</summary>
    [RelayCommand]
    private void BrowseFolder()
    {
        // View 层通过回调设置 FolderPath
    }

    /// <summary>扫描文件</summary>
    [RelayCommand]
    private void Scan()
    {
        var path = FolderPath;
        if (path.IsNullOrEmpty() || !Directory.Exists(path))
        {
            Log += "请选择有效的文件夹\r\n";
            return;
        }

        var extensions = SuffixFilter.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var sb = new StringBuilder();
        var count = 0;

        foreach (var ext in extensions)
        {
            var files = Directory.GetFiles(path, ext, SearchOption.AllDirectories);
            foreach (var file in files)
            {
                try
                {
                    using var fs = File.OpenRead(file);
                    var buf = new Byte[fs.Length > 1024 ? 1024 : (Int32)fs.Length];
                    fs.Read(buf, 0, buf.Length);

                    var enc = DetectEncoding(buf);
                    var relative = file[(path.Length + 1)..];
                    sb.AppendLine($"{(enc ?? "未知"),-12} {relative}");
                    count++;
                }
                catch
                {
                    // 跳过无法读取的文件
                }
            }
        }

        ScanResult = sb.ToString();
        Log += $"扫描完成，共 {count} 个文件\r\n";
    }

    /// <summary>批量转换编码</summary>
    [RelayCommand]
    private void Convert()
    {
        var path = FolderPath;
        if (path.IsNullOrEmpty() || !Directory.Exists(path))
        {
            Log += "请选择有效的文件夹\r\n";
            return;
        }

        var targetEnc = GetEncoding(TargetEncoding);
        if (targetEnc == null)
        {
            Log += $"不支持的编码: {TargetEncoding}\r\n";
            return;
        }

        var extensions = SuffixFilter.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var total = 0;

        foreach (var ext in extensions)
        {
            var files = Directory.GetFiles(path, ext, SearchOption.AllDirectories);
            foreach (var file in files)
            {
                try
                {
                    var content = File.ReadAllText(file);
                    // 跳过已经是目标编码的文件
                    var currentEnc = DetectEncoding(File.ReadAllBytes(file));
                    if (currentEnc != null && currentEnc.EqualIgnoreCase(TargetEncoding)) continue;

                    File.WriteAllText(file, content, targetEnc);
                    total++;
                }
                catch (Exception ex)
                {
                    Log += $"转换失败: {file} - {ex.Message}\r\n";
                }
            }
        }

        Log += $"转换完成，共处理 {total} 个文件\r\n";
    }

    private static Encoding GetEncoding(String name)
    {
        return name switch
        {
            "UTF-8" => Encoding.UTF8,
            "UTF-8 NoBOM" => new UTF8Encoding(false),
            "ASNI" => Encoding.ASCII,
            "Unicode" => Encoding.Unicode,
            "gb2312" => Encoding.GetEncoding("gb2312"),
            _ => Encoding.UTF8
        };
    }

    private static String DetectEncoding(Byte[] buf)
    {
        if (buf.Length >= 3 && buf[0] == 0xEF && buf[1] == 0xBB && buf[2] == 0xBF)
            return "UTF-8";
        if (buf.Length >= 2 && buf[0] == 0xFF && buf[1] == 0xFE)
            return "Unicode";
        if (buf.Length >= 2 && buf[0] == 0xFE && buf[1] == 0xFF)
            return "Unicode BigEndian";

        return null;
    }
    #endregion
}
