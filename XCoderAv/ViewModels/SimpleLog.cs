using NewLife.Log;

namespace XCoderAv.ViewModels;

/// <summary>文本日志输出</summary>
internal class SimpleLog : Logger
{
    /// <summary>写入回调</summary>
    public Action<String>? WriteAction { get; set; }

    /// <summary>写日志</summary>
    protected override void OnWrite(LogLevel level, String format, params Object?[] args)
    {
        WriteAction?.Invoke(Format(level, format, args));
    }

    private static String Format(LogLevel level, String format, params Object[] args)
    {
        if (args.Length > 0)
            return $"[{level}] {String.Format(format, args)}";
        return $"[{level}] {format}";
    }
}
