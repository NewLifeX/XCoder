using System.Text.RegularExpressions;

namespace XCoderAv.Models;

/// <summary>正则分组结果项</summary>
public class RegexGroupItem
{
    /// <summary>序号</summary>
    public Int32 Index { get; set; }

    /// <summary>分组名称</summary>
    public String Name { get; set; } = "";

    /// <summary>值</summary>
    public String Value { get; set; } = "";

    /// <summary>位置</summary>
    public Int32 Position { get; set; }

    /// <summary>长度</summary>
    public Int32 Length { get; set; }

    /// <summary>位置描述</summary>
    public String Location => $"({Position},{Length})";

    /// <summary>原始 Group 对象</summary>
    public Group? Group { get; set; }
}
