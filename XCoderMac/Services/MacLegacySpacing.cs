using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;

namespace XCoderMac.Services;

/// <summary>
/// 全项目旧 XCoderAv 窗口的安全留白适配；只填补原本零间距的分组/纵向行，
/// 不改业务绑定，不粗暴修改每个 Grid 的列宽，且保留专页已有的精调布局。
/// </summary>
internal static class MacLegacySpacing
{
    private static readonly string[] Tailored =
    [
        "MQTT 客户端", "API 发现", "文件编码", "消息调试", "加密解密"
    ];

    public static (int Groups, int Rows, int Stacks) Apply(Control content, string toolTitle)
    {
        if (Tailored.Contains(toolTitle)) return (0, 0, 0);

        var groups = 0;
        var rows = 0;
        var stacks = 0;
        foreach (var group in content.GetLogicalDescendants().OfType<Border>()
                     .Where(border => border.Classes.Contains("GroupBorder")))
        {
            // 旧工具通常只留 4~6pt，卡片紧贴。对全部未专项调整的旧工具
            // 统一为 10pt，并避免修改曾经明确加大过间距的分组。
            if (group.Margin.Bottom <= 6 && group.Margin.Left == 0 &&
                group.Margin.Right == 0 && group.Margin.Top == 0)
            {
                group.Margin = new Thickness(0, 0, 0, 10);
                groups++;
            }

            if (group.Child is StackPanel stack && stack.Orientation == Orientation.Vertical &&
                stack.Spacing <= 0 && stack.Children.Count > 1)
            {
                stack.Spacing = 10;
                stacks++;
            }

            // 仅调分组内容的直接纵向布局；内层分列、占位列、绝对高度、复杂表格
            // 不做统一列距修改，防止打乱 DataModeling/Modbus 等复杂专页。
            if (group.Child is Grid grid && grid.RowDefinitions.Count is >= 2 and <= 8 &&
                grid.RowSpacing <= 0 &&
                grid.RowDefinitions.All(row => row.Height.IsAuto || row.Height.IsStar))
            {
                grid.RowSpacing = 7;
                rows++;
            }
        }
        return (groups, rows, stacks);
    }
}
