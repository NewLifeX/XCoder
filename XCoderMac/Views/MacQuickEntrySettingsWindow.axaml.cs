using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using XCoderMac.Services;

namespace XCoderMac.Views;

public sealed class MacQuickEntryOption : INotifyPropertyChanged
{
    private bool _isSelected;

    public string Title { get; }
    public string Category { get; }
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;
            _isSelected = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public MacQuickEntryOption(string title, string category, bool selected)
    {
        Title = title;
        Category = category;
        _isSelected = selected;
    }
}

/// <summary>管理所有工具的用户自定义快捷入口，不改动真实业务数据。</summary>
public partial class MacQuickEntrySettingsWindow : Window
{
    private bool _updating;
    public ObservableCollection<MacQuickEntryOption> Options { get; } = new();
    public ObservableCollection<MacQuickEntryOption> SelectedEntries { get; } = new();
    public string[] ResultTitles => SelectedEntries.Select(option => option.Title).ToArray();

    public MacQuickEntrySettingsWindow() : this(MacQuickEntryStore.Defaults) { }

    public MacQuickEntrySettingsWindow(IEnumerable<string> existing)
    {
        InitializeComponent();
        DataContext = this;
        foreach (var tool in MacToolCatalog.Tools)
        {
            var entry = new MacQuickEntryOption(tool.Title, tool.Category, selected: false);
            entry.PropertyChanged += Option_PropertyChanged;
            Options.Add(entry);
        }

        _updating = true;
        foreach (var title in existing.Distinct(StringComparer.Ordinal))
        {
            var option = Options.FirstOrDefault(x => x.Title == title);
            if (option == null) continue;
            option.IsSelected = true;
            SelectedEntries.Add(option);
        }
        _updating = false;
        UpdateCount();
    }

    private void Option_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_updating || e.PropertyName != nameof(MacQuickEntryOption.IsSelected) ||
            sender is not MacQuickEntryOption item)
            return;

        if (item.IsSelected && !SelectedEntries.Contains(item))
        {
            SelectedEntries.Add(item);
            SelectedEntriesList.SelectedItem = item;
        }
        else if (!item.IsSelected)
            SelectedEntries.Remove(item);
        UpdateCount();
    }

    private void UpdateCount() => SelectedCountText.Text = $"{SelectedEntries.Count} 个";

    private void MoveUp_Click(object? sender, RoutedEventArgs e) => MoveSelection(-1);
    private void MoveDown_Click(object? sender, RoutedEventArgs e) => MoveSelection(1);

    private void MoveSelection(int delta)
    {
        if (SelectedEntriesList.SelectedItem is not MacQuickEntryOption selected) return;
        var from = SelectedEntries.IndexOf(selected);
        var to = from + delta;
        if (to < 0 || to >= SelectedEntries.Count) return;
        SelectedEntries.Move(from, to);
        SelectedEntriesList.SelectedItem = selected;
    }

    private void ResetDefaults_Click(object? sender, RoutedEventArgs e) =>
        ApplySelection(MacQuickEntryStore.Defaults);

    private void ApplySelection(IEnumerable<string> titles)
    {
        _updating = true;
        try
        {
            SelectedEntries.Clear();
            var selectedTitles = titles.ToHashSet(StringComparer.Ordinal);
            foreach (var option in Options) option.IsSelected = selectedTitles.Contains(option.Title);
            foreach (var title in titles)
            {
                var option = Options.FirstOrDefault(x => x.Title == title);
                if (option != null && !SelectedEntries.Contains(option)) SelectedEntries.Add(option);
            }
            SelectedEntriesList.SelectedItem = null;
        }
        finally
        {
            _updating = false;
            UpdateCount();
        }
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e) => Close(false);
    private void Save_Click(object? sender, RoutedEventArgs e) => Close(true);

#if DEBUG
    public void SmokeTestEditor()
    {
        var before = ResultTitles;
        var candidate = Options.First(option => !option.IsSelected);
        candidate.IsSelected = true;
        if (!ResultTitles.Contains(candidate.Title) || SelectedCountText.Text != $"{SelectedEntries.Count} 个")
            throw new InvalidOperationException("勾选入口未添加到快捷入口列表");

        SelectedEntriesList.SelectedItem = candidate;
        MoveSelection(-1);
        if (SelectedEntries.Count >= 2 && ResultTitles[^1] == candidate.Title)
            throw new InvalidOperationException("上移快捷入口未生效");

        candidate.IsSelected = false;
        if (ResultTitles.Contains(candidate.Title))
            throw new InvalidOperationException("取消勾选未移除快捷入口");

        ApplySelection(before);
        if (!before.SequenceEqual(ResultTitles))
            throw new InvalidOperationException("恢复原快捷入口失败");

        ApplySelection([]);
        if (ResultTitles.Length != 0)
            throw new InvalidOperationException("空快捷入口未被正确保存");

        ApplySelection(MacQuickEntryStore.Defaults);
        if (!ResultTitles.SequenceEqual(MacQuickEntryStore.Defaults))
            throw new InvalidOperationException("恢复默认入口顺序错误");

        Console.WriteLine("QUICK ENTRY EDITOR OK: add, remove, reorder, empty list, restore defaults");
    }
#endif
}
