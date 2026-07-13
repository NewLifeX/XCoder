#nullable enable

using System;
using System.Text.RegularExpressions;

namespace CrazyCoder.Models
{
    /// <summary>正则捕获结果项</summary>
    public class RegexCaptureItem
    {
        /// <summary>序号</summary>
        public Int32 Index { get; set; }

        /// <summary>值</summary>
        public String Value { get; set; } = "";

        /// <summary>位置</summary>
        public Int32 Position { get; set; }

        /// <summary>长度</summary>
        public Int32 Length { get; set; }

        /// <summary>位置描述</summary>
        public String Location => $"({Position},{Length})";

        /// <summary>原始 Capture 对象</summary>
        public Capture? Capture { get; set; }
    }
}
