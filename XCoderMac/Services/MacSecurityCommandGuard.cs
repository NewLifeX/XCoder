using System.Security.Cryptography;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using XCoderAv.ViewModels;

namespace XCoderMac.Services;

/// <summary>
/// Mac 安全边界：不改动上游 SecurityViewModel 的任何算法。
/// 原窗口的命令会直接抛出输入格式异常，Avalonia Button 不捕获该异常，
/// 导致 UI 线程未处理异常进而 SIGABRT。对该工具的命令按钮提供本地异常隔离。
/// </summary>
public static class MacSecurityCommandGuard
{
    public static int Attach(Control root, SecurityViewModel vm)
    {
        var protectedCount = 0;
        foreach (var button in root.GetLogicalDescendants().OfType<Button>())
        {
            if (button.Command is SafeSecurityCommand)
            {
                protectedCount++;
                continue;
            }
            if (button.Command is not ICommand original) continue;
            var label = button.Content?.ToString() ?? "操作";
            button.Command = new SafeSecurityCommand(original, vm, label);
            protectedCount++;
        }
        return protectedCount;
    }

    public sealed class SafeSecurityCommand : ICommand
    {
        private readonly ICommand _inner;
        private readonly SecurityViewModel _vm;
        private readonly string _action;

        public SafeSecurityCommand(ICommand inner, SecurityViewModel vm, string action)
        {
            _inner = inner;
            _vm = vm;
            _action = action;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => _inner.CanExecuteChanged += value;
            remove => _inner.CanExecuteChanged -= value;
        }

        public bool CanExecute(object? parameter)
        {
            try { return _inner.CanExecute(parameter); }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                ShowError(ex);
                return false;
            }
        }

        public void Execute(object? parameter)
        {
            try
            {
                if (_inner.CanExecute(parameter)) _inner.Execute(parameter);
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                ShowError(ex);
            }
        }

        private void ShowError(Exception ex)
        {
            // 不输出用户的原文、密钥或口令到日志。
            var hint = ex switch
            {
                FormatException => "输入格式不正确，请检查 HEX、Base64 等编码内容。",
                CryptographicException => "密文或密钥无效，请检查密码和算法。",
                ArgumentException => "输入参数无效，请检查原文、密钥与格式。",
                _ => $"操作未完成：{ex.Message}"
            };
            _vm.ResultText = $"【{_action}失败】{hint}";
            System.Diagnostics.Debug.WriteLine($"SecurityCommandGuard: {_action} {ex.GetType().Name}");
        }
    }
}
