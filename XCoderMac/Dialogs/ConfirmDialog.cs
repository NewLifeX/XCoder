using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace XCoderMac.Dialogs;

/// <summary>与原工具逻辑分离的 Mac 风格确认窗口。</summary>
internal static class ConfirmDialog
{
    public static Task<bool> ShowAsync(Window owner, string title, string description, string action)
    {
        var dialog = new Window
        {
            Title = title,
            Width = 490,
            Height = 260,
            CanResize = false,
            Background = Brushes.White,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var cancel = new Button
        {
            Content = "取消",
            Padding = new Thickness(20, 9),
            MinWidth = 84,
            Background = new SolidColorBrush(Color.Parse("#EEF2F8")),
            Foreground = new SolidColorBrush(Color.Parse("#45607D")),
            CornerRadius = new CornerRadius(9)
        };
        var accept = new Button
        {
            Content = action,
            Padding = new Thickness(20, 9),
            MinWidth = 110,
            Background = new SolidColorBrush(Color.Parse("#2768C5")),
            Foreground = Brushes.White,
            CornerRadius = new CornerRadius(9)
        };

        cancel.Click += (_, _) => dialog.Close(false);
        accept.Click += (_, _) => dialog.Close(true);

        var actions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 10,
            Children = { cancel, accept }
        };

        dialog.Content = new Border
        {
            Padding = new Thickness(27),
            Child = new StackPanel
            {
                Spacing = 15,
                Children =
                {
                    new TextBlock
                    {
                        Text = title,
                        FontSize = 20,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = new SolidColorBrush(Color.Parse("#253B58"))
                    },
                    new TextBlock
                    {
                        Text = description,
                        FontSize = 13,
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = new SolidColorBrush(Color.Parse("#61738B"))
                    },
                    actions
                }
            }
        };
        return dialog.ShowDialog<bool>(owner);
    }
}
