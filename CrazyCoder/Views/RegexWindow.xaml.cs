using System.Windows;
using System.Windows.Controls;
using CrazyCoder.ViewModels;

namespace CrazyCoder.Views
{
    /// <summary>正则表达式工具窗口</summary>
    public partial class RegexWindow : Window
    {
        /// <summary>ViewModel</summary>
        public RegexViewModel ViewModel { get; }

        /// <summary>实例化正则表达式工具窗口</summary>
        public RegexWindow()
        {
            InitializeComponent();

            ViewModel = new RegexViewModel();
            DataContext = ViewModel;

            // 选中匹配/分组/捕获项时在源文本中高亮
            lvMatch.SelectionChanged += OnMatchSelectionChanged;
            lvGroup.SelectionChanged += OnGroupSelectionChanged;
            lvCapture.SelectionChanged += OnCaptureSelectionChanged;
        }

        /// <summary>匹配项选中变更 - 在源文本中高亮</summary>
        private void OnMatchSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.SelectedMatch is not { } match) return;
            if (match.Match == null) return;

            txtSource.Focus();
            txtSource.Select(match.Position, match.Length);
            txtSource.ScrollToLine(GetLineFromIndex(txtSource.Text, match.Position) - 1);
        }

        /// <summary>分组选中变更 - 在源文本中高亮</summary>
        private void OnGroupSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.SelectedGroup is not { } group) return;
            if (group.Group == null) return;

            txtSource.Focus();
            txtSource.Select(group.Position, group.Length);
            txtSource.ScrollToLine(GetLineFromIndex(txtSource.Text, group.Position) - 1);
        }

        /// <summary>捕获选中变更 - 在源文本中高亮</summary>
        private void OnCaptureSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.SelectedCapture is not { } capture) return;
            if (capture.Capture == null) return;

            txtSource.Focus();
            txtSource.Select(capture.Position, capture.Length);
            txtSource.ScrollToLine(GetLineFromIndex(txtSource.Text, capture.Position) - 1);
        }

        /// <summary>从索引计算行号（1-based）</summary>
        private static Int32 GetLineFromIndex(String text, Int32 index)
        {
            if (String.IsNullOrEmpty(text) || index <= 0) return 1;
            var line = 1;
            for (var i = 0; i < index && i < text.Length; i++)
            {
                if (text[i] == '\n') line++;
            }
            return line;
        }
    }
}
