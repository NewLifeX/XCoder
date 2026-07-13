#nullable enable

using System;
using System.Text.RegularExpressions;

namespace CrazyCoder.Models
{
    /// <summary>正则匹配结果项</summary>
    public class RegexMatchItem
    {
        /// <summary>序号</summary>
        public Int32 Index { get; set; }

        /// <summary>匹配值</summary>
        public String Value { get; set; } = "";

        /// <summary>行号</summary>
        public Int32 Line { get; set; }

        /// <summary>位置</summary>
        public Int32 Position { get; set; }

        /// <summary>长度</summary>
        public Int32 Length { get; set; }

        /// <summary>位置描述</summary>
        public String Location => $"({Line},{Position},{Length})";

        /// <summary>原始 Match 对象</summary>
        public Match? Match { get; set; }
    }
}
