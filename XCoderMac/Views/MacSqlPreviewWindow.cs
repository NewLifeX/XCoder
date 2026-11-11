using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using System.Text;

namespace XCoderMac.Views;

/// <summary>SQL 只读预览，可复制或选择本地路径保存，不会执行。</summary>
public sealed class MacSqlPreviewWindow : Window
{
    private readonly string _sql;
    private readonly TextBlock _status = new() { FontSize = 12, Foreground = new SolidColorBrush(Color.Parse("#617F9C")) };
    public MacSqlPreviewWindow(string title, string sql)
    {
        _sql = sql;
        Title = title;
        Width = 980;
        Height = 730;
        MinWidth = 620;
        MinHeight = 450;
        Background = new SolidColorBrush(Color.Parse("#F7F9FC"));
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        var grid = new Grid { Margin = new Thickness(24), RowDefinitions = new RowDefinitions("Auto,12,*,12,Auto") };
        grid.Children.Add(new TextBlock { Text = title, FontSize = 22, FontWeight = FontWeight.SemiBold,
            Foreground = new SolidColorBrush(Color.Parse("#25476B")) });
        var text = new TextBox
        {
            Name = "SqlPreviewText", Text = sql, IsReadOnly = true,
            AcceptsReturn = true, TextWrapping = TextWrapping.NoWrap,
            FontFamily = new FontFamily("Menlo,Consolas,monospace"),
            FontSize = 13, Padding = new Thickness(16)
        };
        text.Classes.Add("dark-console");
        Grid.SetRow(text, 2); grid.Children.Add(text);
        var footer = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto,10,Auto") };
        footer.Children.Add(_status);
        var save = new Button { Content = "保存 SQL 文件", Padding = new Thickness(15, 9) };
        save.Click += async (_, _) => await SaveSqlAsync();
        Grid.SetColumn(save, 1); footer.Children.Add(save);
        var close = new Button { Content = "关闭", Padding = new Thickness(15, 9) };
        close.Click += (_, _) => Close();
        Grid.SetColumn(close, 3); footer.Children.Add(close);
        Grid.SetRow(footer, 4); grid.Children.Add(footer);
        Content = grid;
        _status.Text = "只读预览 · 不会执行 SQL";
    }

    private async Task SaveSqlAsync()
    {
        try
        {
            var target = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "保存建表 SQL",
                SuggestedFileName = "schema.sql",
                DefaultExtension = "sql",
                FileTypeChoices = [new FilePickerFileType("SQL 文件") { Patterns = ["*.sql"] }]
            });
            if (target == null) return;
            await using var stream = await target.OpenWriteAsync();
            await using var writer = new StreamWriter(stream, new UTF8Encoding(false));
            await writer.WriteAsync(_sql);
            _status.Text = "SQL 已保存";
        }
        catch (Exception ex) { _status.Text = $"保存失败：{ex.Message}"; }
    }
}
