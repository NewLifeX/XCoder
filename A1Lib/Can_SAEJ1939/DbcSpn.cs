using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALib.Can_SAEJ1939
{
    /// <summary>
    /// SPN 解读辅助方法。
    /// 本套模型把 SPN/FMI 作为信号属性（<c>BA_ "SPN" SG_ ...</c> / <c>BA_ "FMI" SG_ ...</c>）承载；
    /// <see cref="DbcSignal.Spn"/>、<see cref="DbcSignal.FmiList"/> 等强类型入口见 <see cref="DbcSignal"/>，
    /// 此处的扩展方法用于在 SPN 索引为空的场合直接从属性字典取值（例如手工构造 <see cref="DbcSignal"/>）。
    /// </summary>
    public static class DbcSpnHelper
    {
        /// <summary>SPN 属性名</summary>
        public const String AttrSpn = "SPN";

        /// <summary>FMI 属性名</summary>
        public const String AttrFmi = "FMI";

        /// <summary>报文级通信错误 DTC 的 FMI 属性名</summary>
        public const String AttrCommErrorFmi = "CommunicationErrorDTCFMI";

        /// <summary>从属性字典直接读取 SPN（未走索引时使用）</summary>
        public static long? SpnFromAttributes(this DbcSignal sig)
        {
            if (sig?.Attributes == null) return null;
            return sig.Attributes.TryGetValue(AttrSpn, out var v) ? DbcSpn.ParseNumber(v)?.LongValue : null;
        }

        /// <summary>从属性字典直接读取 FMI 列表（未走索引时使用）</summary>
        public static List<int> FmiListFromAttributes(this DbcSignal sig)
        {
            if (sig?.Attributes == null) return new List<int>();
            return sig.Attributes.TryGetValue(AttrFmi, out var v) ? DbcSpn.ParseIntList(v) : new List<int>();
        }

        /// <summary>从属性字典直接读取报文级通信错误 DTC 的 FMI</summary>
        public static List<int> CommErrorFmiFromAttributes(this DbcMessage msg)
        {
            if (msg?.Attributes == null) return new List<int>();
            return msg.Attributes.TryGetValue(AttrCommErrorFmi, out var v) ? DbcSpn.ParseIntList(v) : new List<int>();
        }
    }

    /// <summary>SPN 数值包装，内部用于承载 DBC 属性里读出的数值</summary>
    internal class DbcSpn
    {
        public long LongValue;

        public double DoubleValue => LongValue;

        /// <summary>
        /// 把属性值转为数值。
        /// DBC 中的整型属性（SPN）被解析为 double，字符串型属性（FMI）保留原文，
        /// 这里统一兜住两种情况，避免上层做类型判断。
        /// </summary>
        public static DbcSpn ParseNumber(Object value)
        {
            if (value == null) return null;

            switch (value)
            {
                case double d: return new DbcSpn { LongValue = (long)Math.Round(d) };
                case float f: return new DbcSpn { LongValue = (long)Math.Round(f) };
                case int i: return new DbcSpn { LongValue = i };
                case long l: return new DbcSpn { LongValue = l };
                case String s:
                    if (Double.TryParse(s.Trim(), System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out var pv))
                        return new DbcSpn { LongValue = (long)Math.Round(pv) };
                    return null;
                default:
                    return Double.TryParse(value.ToString(), out var dv) ? new DbcSpn { LongValue = (long)Math.Round(dv) } : null;
            }
        }

        /// <summary>
        /// 把 FMI 属性转为整数列表。
        /// 该 DBC 里 FMI 是 STRING，形如 <c>"5;6"</c>；也兼容单值 <c>"5"</c>、
        /// 逗号分隔，以及直接是数值（INT 属性）的情形。
        /// </summary>
        public static List<int> ParseIntList(Object value)
        {
            var list = new List<int>();
            if (value == null) return list;

            var text = value as String;
            if (text == null)
            {
                var n = ParseNumber(value);
                if (n != null) list.Add((int)n.LongValue);
                return list;
            }

            foreach (var part in text.Split(new[] { ';', ',', '|', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (Int32.TryParse(part.Trim(), out var v)) list.Add(v);
            }

            if (list.Count == 0)
            {
                var n = ParseNumber(text);
                if (n != null) list.Add((int)n.LongValue);
            }

            return list;
        }
    }
}
