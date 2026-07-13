using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XCoderAv.Models;

namespace XCoderAv.ViewModels;

/// <summary>正则表达式工具 ViewModel</summary>
public partial class RegexViewModel : ObservableObject
{
    #region 属性

    /// <summary>正则表达式</summary>
    [ObservableProperty]
    private String _pattern = "";

    /// <summary>源文本</summary>
    [ObservableProperty]
    private String _sourceText = "";

    /// <summary>替换文本</summary>
    [ObservableProperty]
    private String _replacement = "";

    /// <summary>忽略大小写</summary>
    [ObservableProperty]
    private Boolean _ignoreCase;

    /// <summary>多行模式</summary>
    [ObservableProperty]
    private Boolean _multiline;

    /// <summary>单行模式</summary>
    [ObservableProperty]
    private Boolean _singleline;

    /// <summary>忽略空白</summary>
    [ObservableProperty]
    private Boolean _ignorePatternWhitespace;

    /// <summary>匹配模式（false 为替换模式）</summary>
    [ObservableProperty]
    private Boolean _isMatchMode = true;

    /// <summary>当前选项文本</summary>
    [ObservableProperty]
    private String _optionText = "RegexOptions.None";

    /// <summary>状态文本</summary>
    [ObservableProperty]
    private String _status = "就绪";

    /// <summary>目录路径</summary>
    [ObservableProperty]
    private String _directoryPath = "";

    /// <summary>文件过滤</summary>
    [ObservableProperty]
    private String _fileFilter = "*.*";

    /// <summary>选中的匹配项</summary>
    [ObservableProperty]
    private RegexMatchItem? _selectedMatch;

    /// <summary>选中的分组</summary>
    [ObservableProperty]
    private RegexGroupItem? _selectedGroup;

    /// <summary>选中的捕获</summary>
    [ObservableProperty]
    private RegexCaptureItem? _selectedCapture;

    /// <summary>匹配结果集合</summary>
    public ObservableCollection<RegexMatchItem> Matches { get; } = [];

    /// <summary>分组结果集合</summary>
    public ObservableCollection<RegexGroupItem> Groups { get; } = [];

    /// <summary>捕获结果集合</summary>
    public ObservableCollection<RegexCaptureItem> Captures { get; } = [];

    /// <summary>执行按钮文本</summary>
    public String ExecuteButtonText => IsMatchMode ? "正则匹配" : "正则替换";

    /// <summary>替换模式</summary>
    public Boolean IsReplaceMode => !IsMatchMode;

    #endregion

    #region 构造

    /// <summary>当前正则对象（用于分组名称查询）</summary>
    private Regex? _regex;

    #endregion

    #region 选项变更

    partial void OnIgnoreCaseChanged(Boolean value) => UpdateOptionText();
    partial void OnMultilineChanged(Boolean value) => UpdateOptionText();
    partial void OnSinglelineChanged(Boolean value) => UpdateOptionText();
    partial void OnIgnorePatternWhitespaceChanged(Boolean value) => UpdateOptionText();

    partial void OnIsMatchModeChanged(Boolean value)
    {
        OnPropertyChanged(nameof(ExecuteButtonText));
        OnPropertyChanged(nameof(IsReplaceMode));
    }

    #endregion

    #region 选中项变更

    partial void OnSelectedMatchChanged(RegexMatchItem? value)
    {
        Groups.Clear();
        Captures.Clear();

        if (value?.Match == null) return;

        var reg = _regex;
        var m = value.Match;
        for (var i = 0; i < m.Groups.Count; i++)
        {
            var g = m.Groups[i];
            var name = reg?.GroupNameFromNumber(i) ?? i.ToString();
            Groups.Add(new RegexGroupItem
            {
                Index = i,
                Name = name,
                Value = g.Value,
                Position = g.Index,
                Length = g.Length,
                Group = g
            });
        }
    }

    partial void OnSelectedGroupChanged(RegexGroupItem? value)
    {
        Captures.Clear();

        if (value?.Group == null) return;

        var g = value.Group;
        for (var i = 0; i < g.Captures.Count; i++)
        {
            var c = g.Captures[i];
            Captures.Add(new RegexCaptureItem
            {
                Index = i,
                Value = c.Value,
                Position = c.Index,
                Length = c.Length,
                Capture = c
            });
        }
    }

    #endregion

    #region 辅助方法

    /// <summary>更新选项文本</summary>
    void UpdateOptionText()
    {
        var sb = new StringBuilder();
        var options = GetOptions();
        if (options == RegexOptions.None)
        {
            sb.Append("RegexOptions.None");
        }
        else
        {
            if ((options & RegexOptions.IgnoreCase) != 0) AppendOption(sb, "IgnoreCase");
            if ((options & RegexOptions.Multiline) != 0) AppendOption(sb, "Multiline");
            if ((options & RegexOptions.Singleline) != 0) AppendOption(sb, "Singleline");
            if ((options & RegexOptions.IgnorePatternWhitespace) != 0) AppendOption(sb, "IgnorePatternWhitespace");
        }
        OptionText = sb.ToString();
    }

    /// <summary>追加选项名称</summary>
    static void AppendOption(StringBuilder sb, String name)
    {
        if (sb.Length > 0) sb.Append(" | ");
        sb.Append("RegexOptions.");
        sb.Append(name);
    }

    /// <summary>获取当前 RegexOptions</summary>
    RegexOptions GetOptions()
    {
        var options = RegexOptions.None;
        if (IgnoreCase) options |= RegexOptions.IgnoreCase;
        if (Multiline) options |= RegexOptions.Multiline;
        if (Singleline) options |= RegexOptions.Singleline;
        if (IgnorePatternWhitespace) options |= RegexOptions.IgnorePatternWhitespace;
        return options;
    }

    /// <summary>计算行号</summary>
    static Int32 GetLineFromIndex(String text, Int32 index)
    {
        if (String.IsNullOrEmpty(text) || index <= 0) return 1;
        var line = 1;
        for (var i = 0; i < index && i < text.Length; i++)
        {
            if (text[i] == '\n') line++;
        }
        return line;
    }

    #endregion

    #region 命令

    /// <summary>执行匹配或替换</summary>
    [RelayCommand]
    async Task ExecuteAsync()
    {
        if (String.IsNullOrEmpty(Pattern)) return;
        var text = SourceText;
        if (String.IsNullOrEmpty(text)) return;

        if (!IsMatchMode)
        {
            await ReplaceTextAsync(text);
            return;
        }

        Status = "正在匹配...";
        Matches.Clear();
        Groups.Clear();
        Captures.Clear();

        var pattern = Pattern;
        var options = GetOptions();
        var timeout = TimeSpan.FromSeconds(5);

        await Task.Run(() =>
        {
            try
            {
                var sw = Stopwatch.StartNew();
                var reg = new Regex(pattern, options, timeout);
                _regex = reg;

                var ms = reg.Matches(text);
                var idx = 1;
                foreach (Match match in ms)
                {
                    var line = GetLineFromIndex(text, match.Index);
                    var item = new RegexMatchItem
                    {
                        Index = idx++,
                        Value = match.Value,
                        Line = line,
                        Position = match.Index,
                        Length = match.Length,
                        Match = match
                    };

                    Dispatcher.UIThread.Post(() =>
                    {
                        Matches.Add(item);
                    });
                }

                sw.Stop();
                Dispatcher.UIThread.Post(() =>
                {
                    Status = $"成功匹配 {Matches.Count} 项！耗时 {sw.ElapsedMilliseconds}ms";
                });
            }
            catch (RegexMatchTimeoutException)
            {
                Dispatcher.UIThread.Post(() => Status = "正则匹配超时！");
            }
            catch (Exception ex)
            {
                Dispatcher.UIThread.Post(() => Status = $"错误：{ex.Message}");
            }
        });
    }

    /// <summary>执行替换</summary>
    async Task ReplaceTextAsync(String text)
    {
        var replacement = Replacement;
        if (String.IsNullOrEmpty(replacement))
        {
            Status = "请输入替换文本";
            return;
        }

        Status = "正在替换...";
        var pattern = Pattern;
        var options = GetOptions();
        var timeout = TimeSpan.FromSeconds(5);

        await Task.Run(() =>
        {
            try
            {
                var sw = Stopwatch.StartNew();
                var reg = new Regex(pattern, options, timeout);
                _regex = reg;

                var count = 0;
                var ms = reg.Matches(text);
                if (ms != null) count = ms.Count;

                if (count > 0)
                {
                    var result = reg.Replace(text, replacement);
                    Dispatcher.UIThread.Post(() =>
                    {
                        SourceText = result;
                    });
                }

                sw.Stop();
                Dispatcher.UIThread.Post(() =>
                {
                    Status = $"成功替换 {count} 项！耗时 {sw.ElapsedMilliseconds}ms";
                });
            }
            catch (RegexMatchTimeoutException)
            {
                Dispatcher.UIThread.Post(() => Status = "正则替换超时！");
            }
            catch (Exception ex)
            {
                Dispatcher.UIThread.Post(() => Status = $"错误：{ex.Message}");
            }
        });
    }

    /// <summary>批量替换</summary>
    [RelayCommand]
    async Task BatchReplaceAsync()
    {
        if (String.IsNullOrEmpty(Pattern))
        {
            Status = "请输入正则表达式";
            return;
        }
        var replacement = Replacement;
        if (String.IsNullOrEmpty(replacement))
        {
            Status = "请输入替换文本";
            return;
        }

        var path = DirectoryPath;
        if (String.IsNullOrEmpty(path) || !Directory.Exists(path))
        {
            Status = "请先选择有效目录";
            return;
        }

        var filter = FileFilter;
        if (String.IsNullOrEmpty(filter)) filter = "*.*";

        Status = "正在批量替换...";
        var pattern = Pattern;
        var options = GetOptions();

        await Task.Run(() =>
        {
            try
            {
                var sw = Stopwatch.StartNew();
                var reg = new Regex(pattern, options, TimeSpan.FromSeconds(5));

                var files = Directory.GetFiles(path, filter, SearchOption.AllDirectories);
                if (files == null || files.Length == 0)
                {
                    Dispatcher.UIThread.Post(() => Status = "没有符合条件的文件");
                    return;
                }

                var totalCount = 0;
                var changedCount = 0;
                String? firstContent = null;

                foreach (var item in files.Take(1000))
                {
                    try
                    {
                        var content = File.ReadAllText(item);
                        var ms = reg.Matches(content);
                        if (ms == null || ms.Count == 0) continue;

                        totalCount += ms.Count;
                        var content2 = reg.Replace(content, replacement);
                        if (content != content2)
                        {
                            File.WriteAllText(item, content2);
                            changedCount++;
                            if (firstContent == null) firstContent = content2;
                        }
                    }
                    catch
                    {
                        // 跳过无法读取的文件
                    }
                }

                sw.Stop();

                if (firstContent != null)
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        SourceText = firstContent;
                    });
                }

                Dispatcher.UIThread.Post(() =>
                {
                    Status = $"批量替换完成：{changedCount} 个文件，{totalCount} 项替换，耗时 {sw.ElapsedMilliseconds}ms";
                });
            }
            catch (Exception ex)
            {
                Dispatcher.UIThread.Post(() => Status = $"错误：{ex.Message}");
            }
        });
    }

    #endregion
}
