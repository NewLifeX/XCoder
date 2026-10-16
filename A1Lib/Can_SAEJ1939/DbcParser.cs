using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ALib.Can_SAEJ1939
{
    /// <summary>
    /// DBC 文本解析器。
    /// 逐行识别关键字（与 <see cref="DbcDatabase.NewSymbols"/> 中列出的一致），
    /// 遇到无法识别的语句仅记录到 <see cref="DbcDatabase.Warnings"/> 并跳过，不影响其余内容解析。
    /// 特点：
    /// 1) 注释语句（CM_）可能跨多行（引号未闭合），已做续行拼接；
    /// 2) 信号定义（SG_）按 DBC 规范解析起始位/长度/字节序/符号/缩放/范围/单位/接收节点；
    /// 3) VAL_TABLE_ 与 VAL_ 均会落到「报文的信号」上，便于直接查值含义。
    /// </summary>
    public class DbcParser
    {
        readonly DbcDatabase _db;

        /// <summary>当前解析的报文（SG_ 归属于最近一条 BO_）</summary>
        DbcMessage _currentMessage;

        /// <summary>当前解析的取值表（BA_/CM_ 可能引用）</summary>
        readonly Dictionary<String, DbcValueTable> _valueTableMap = new Dictionary<String, DbcValueTable>(StringComparer.Ordinal);

        public DbcParser(DbcDatabase db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        #region 入口

        public void Parse(String text)
        {
            if (String.IsNullOrEmpty(text)) return;

            var lines = SplitLogicalLines(text);
            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (line.Length == 0) continue;
                if (line.StartsWith("//")) continue;   // 容许 C++ 风格注释

                try
                {
                    ParseLine(line);
                }
                catch (Exception ex)
                {
                    _db.Warnings.Add($"[{line.Substring(0, Math.Min(60, line.Length))}] 解析失败：{ex.Message}");
                }
            }

            // 解析结束，构建 SPN / PGN 索引
            _db.BuildIndex();
        }

        /// <summary>
        /// 按行切分，并把「引号未闭合」的多行语句合并为一条逻辑行。
        /// 例如：CM_ BO_ 123 "第一行\n第二行";
        /// </summary>
        static List<String> SplitLogicalLines(String text)
        {
            var result = new List<String>();
            var sb = new StringBuilder();
            var inQuote = false;
            var escaped = false;

            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];

                if (c == '\\' && inQuote && !escaped) escaped = true;
                else escaped = false;

                if (c == '"' && !escaped) inQuote = !inQuote;

                if ((c == '\r' || c == '\n') && !inQuote)
                {
                    if (sb.Length > 0) { result.Add(sb.ToString()); sb.Clear(); }
                    // 跳过 \r\n 组合
                    if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
                    continue;
                }

                if (c == '\r' || c == '\n')
                {
                    // 引号内的换行保留为空格，避免破坏注释文本
                    sb.Append(' ');
                    if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
                    continue;
                }

                sb.Append(c);
            }

            if (sb.Length > 0) result.Add(sb.ToString());
            return result;
        }

        #endregion

        #region 语句分发

        void ParseLine(String line)
        {
            // BO_ 需要在 SG_ 之前判断（SG_ 前缀不同，实际互不冲突）
            if (line.StartsWith("VERSION")) { ParseVersion(line); return; }
            if (line.StartsWith("NS_")) { ParseNewSymbols(line); return; }
            if (line.StartsWith("BS_")) { ParseBitTiming(line); return; }

            // 注意：BU_BO_REL_ / BU_SG_REL_ / BU_EV_REL_ 以 BU_ 开头，必须先于 BU_ 判断
            if (line.StartsWith("BU_BO_REL_") || line.StartsWith("BU_SG_REL_") || line.StartsWith("BU_EV_REL_")) return;
            if (line.StartsWith("BU_")) { ParseNodes(line); return; }
            if (line.StartsWith("VAL_TABLE_")) { ParseValueTable(line); return; }
            // BO_TX_BU_ 以 BO_ 开头，必须先于 BO_ 判断
            if (line.StartsWith("BO_TX_BU_")) { ParseExtraTransmitters(line); return; }
            if (line.StartsWith("BO_")) { ParseMessage(line); return; }
            if (line.StartsWith("SG_")) { ParseSignal(line); return; }

            // SG_MUL_VAL_ 以 SG_ 开头，必须先于 SG_ 判断
            if (line.StartsWith("SG_MUL_VAL_")) { ParseSignalMultiplex(line); return; }
            if (line.StartsWith("EV_")) { ParseEnvVar(line); return; }
            if (line.StartsWith("CM_")) { ParseComment(line); return; }
            if (line.StartsWith("BA_DEF_DEF_REL_")) { return; }
            if (line.StartsWith("BA_DEF_DEF_")) { ParseAttributeDefault(line); return; }
            if (line.StartsWith("BA_DEF_")) { ParseAttributeDefinition(line); return; }
            if (line.StartsWith("BA_")) { ParseAttributeValue(line); return; }
            if (line.StartsWith("VAL_")) { ParseSignalValue(line); return; }
            if (line.StartsWith("SIG_GROUP_")) { _db.SignalGroups.Add(line.TrimEnd(';').Trim()); return; }
            if (line.StartsWith("SIG_VALTYPE_")) { ParseSignalValueType(line); return; }
            if (line.StartsWith("EV_")) { ParseEnvVar(line); return; }

            // 忽略不影响解析的语句：CAT_DEF_/CAT_/FILTER/EV_DATA_/ENVVAR_DATA_/SGTYPE_*
            // 等定义型语句、NS_ 中声明但无需建模的关键字，以及 Vector 特有的关系属性
            // （BA_REL_ / BU_*_REL_ 等已在上面处理）
            if (line.StartsWith("CAT_DEF_") || line.StartsWith("CAT_") || line.StartsWith("FILTER") ||
                line.StartsWith("EV_DATA_") || line.StartsWith("ENVVAR_DATA_") || line.StartsWith("SGTYPE_") ||
                line.StartsWith("BA_SGTYPE_") || line.StartsWith("BA_DEF_SGTYPE_") || line.StartsWith("SIG_TYPE_REF_") ||
                line.StartsWith("SIGTYPE_VALTYPE_") || line.StartsWith("BA_REL_"))
                return;

            // 兜底：NS_ 段中声明过（以 _ 结尾）的关键字一律视为已识别，避免误报
            if (_db.NewSymbols.Any(s => line.StartsWith(s, StringComparison.Ordinal))) return;

            _db.Warnings.Add($"未知语句：{line.Substring(0, Math.Min(80, line.Length))}");
        }

        #endregion

        #region 基础语句

        void ParseVersion(String line)
        {
            var token = line.Substring("VERSION".Length).Trim().TrimEnd(';').Trim();
            _db.Version = Unquote(token);
        }

        void ParseNewSymbols(String line)
        {
            var rest = line.Substring("NS_".Length).Trim().TrimStart(':').Trim();
            foreach (var s in rest.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var t = s.Trim();
                if (t.EndsWith("_")) _db.NewSymbols.Add(t);
            }
        }

        void ParseBitTiming(String line)
        {
            // BS_: :    或    BS_: 500000 : 0,0
            var body = line.Substring("BS_".Length).Trim().TrimStart(':').Trim().TrimEnd(';').Trim();
            _db.BitTiming = body;
        }

        void ParseNodes(String line)
        {
            var rest = line.Substring("BU_".Length).Trim().TrimStart(':').Trim();
            foreach (var s in rest.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var name = s.Trim();
                if (name.Length == 0) continue;
                var node = _db.Nodes.FirstOrDefault(n => String.Equals(n.Name, name, StringComparison.Ordinal));
                if (node == null) _db.Nodes.Add(new DbcNode { Name = name });
            }
        }

        void ParseValueTable(String line)
        {
            // VAL_TABLE_ Armrest_Type 2 "LRT big armrest" 1 "..." 0 "..." ;
            var body = line.Substring("VAL_TABLE_".Length).Trim().TrimEnd(';').Trim();
            var sp = IndexOfWhitespace(body);
            if (sp < 0) return;

            var table = new DbcValueTable { Name = body.Substring(0, sp).Trim() };
            ParseValuePairs(body.Substring(sp + 1), table.Values);

            _db.ValueTables.Add(table);
            _valueTableMap[table.Name] = table;
        }

        /// <summary>解析 值 "描述" 值 "描述" 序列</summary>
        static void ParseValuePairs(String text, Dictionary<long, String> target)
        {
            var tokens = Tokenize(text);
            // 结构：数值 "文本" 数值 "文本" ...
            for (int i = 0; i + 1 < tokens.Count; i += 2)
            {
                var valToken = tokens[i];
                var descToken = tokens[i + 1];

                // 兼容紧挨着的键值（无空格）情形，这里按成对处理
                if (!TryParseLong(valToken, out var value)) continue;
                target[value] = descToken;
            }
        }

        #endregion

        #region 报文与信号

        void ParseMessage(String line)
        {
            // BO_ 2365530144 ARMREST_SwitchIn: 8 ARMREST
            var body = line.Substring("BO_".Length).Trim();
            var sp1 = IndexOfWhitespace(body);
            if (sp1 < 0) return;

            var idText = body.Substring(0, sp1).Trim();
            var rest = body.Substring(sp1 + 1).Trim();

            var colon = rest.IndexOf(':');
            if (colon < 0) return;

            var name = rest.Substring(0, colon).Trim();
            var tail = rest.Substring(colon + 1).Trim();

            var parts = tail.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            var msg = new DbcMessage
            {
                Id = ParseUInt32(idText),
                Name = name,
                Size = parts.Length > 0 ? (int)ParseDouble(parts[0]) : 8,
                Transmitter = parts.Length > 1 ? parts[1].Trim() : "",
            };

            _db.Messages.Add(msg);
            _currentMessage = msg;
        }

        void ParseSignal(String line)
        {
            if (_currentMessage == null)
            {
                _db.Warnings.Add($"信号出现在报文之前：{line.Substring(0, Math.Min(60, line.Length))}");
                return;
            }

            // SG_ COUNT_SOFTWARE_IDENTIFICATION_FI : 0|8@1+ (1,0) [5|5] "" Vector__XXX
            // SG_ ControlByte M : 0|8@1+ (1,0) [0|255] "" Vector__XXX
            var body = line.Substring("SG_".Length).Trim();

            var colon = body.IndexOf(':');
            if (colon < 0) return;

            var head = body.Substring(0, colon).Trim();
            var tail = body.Substring(colon + 1).Trim();

            var headParts = head.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (headParts.Length == 0) return;

            // 形如 "SG_ SigName M :" 或 "SG_ SigName m16 :"，第二段为多路复用标记
            var multiplex = "";
            if (headParts.Length > 1)
            {
                var t = headParts[1].Trim();
                if (t == "M" || (t.Length > 1 && (t[0] == 'm' || t[0] == 'M') && t.Skip(1).All(Char.IsDigit))) multiplex = t;
            }

            var sig = new DbcSignal
            {
                Name = headParts[0],
                Message = _currentMessage,
                Multiplex = multiplex,
            };

            // 起始位|长度@字节序符号 (factor,offset) [min|max] "unit" 接收节点...
            var tokens = tail.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0) return;

            // 1) 起始位|长度@1+
            var bitSpec = tokens[0];
            var at = bitSpec.IndexOf('@');
            var bar = bitSpec.IndexOf('|');
            if (bar < 0 || at < 0) return;

            sig.StartBit = (int)ParseDouble(bitSpec.Substring(0, bar));
            sig.Length = (int)ParseDouble(bitSpec.Substring(bar + 1, at - bar - 1));

            var orderText = bitSpec.Substring(at + 1);       // 形如 1+ / 0-
            sig.ByteOrder = orderText.StartsWith("0") ? DbcByteOrder_e.Motorola : DbcByteOrder_e.Intel;
            sig.ValueType = orderText.EndsWith("-") ? DbcValueType_e.Signed : DbcValueType_e.Unsigned;

            // 2) (factor,offset)
            var factorSpec = FindBetween(tail, '(', ')');
            if (!String.IsNullOrEmpty(factorSpec))
            {
                var comma = factorSpec.IndexOf(',');
                if (comma > 0)
                {
                    sig.Factor = ParseDouble(factorSpec.Substring(0, comma));
                    sig.Offset = ParseDouble(factorSpec.Substring(comma + 1));
                }
            }

            // 3) [min|max]
            var rangeSpec = FindBetween(tail, '[', ']');
            if (!String.IsNullOrEmpty(rangeSpec))
            {
                var bar2 = rangeSpec.IndexOf('|');
                if (bar2 > 0)
                {
                    sig.Minimum = ParseDouble(rangeSpec.Substring(0, bar2));
                    sig.Maximum = ParseDouble(rangeSpec.Substring(bar2 + 1));
                }
            }

            // 4) 单位与接收节点：引号内为单位，其后为接收节点（逗号分隔）
            var remain = tail;
            var unitStart = remain.IndexOf('"');
            if (unitStart >= 0)
            {
                var unitEnd = FindQuoteEnd(remain, unitStart);
                if (unitEnd > unitStart)
                {
                    sig.Unit = UnescapeText(remain.Substring(unitStart + 1, unitEnd - unitStart - 1));
                    remain = remain.Substring(unitEnd + 1);
                }
            }

            foreach (var r in remain.Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var name = r.Trim();
                if (name.Length == 0) continue;
                sig.Receivers.Add(name);
            }

            _currentMessage.Signals.Add(sig);
        }

        void ParseExtraTransmitters(String line)
        {
            // BO_TX_BU_ 2499682645 : SRC14_3431_HW,SRC14_3431_IO;
            var body = line.Substring("BO_TX_BU_".Length).Trim().TrimEnd(';').Trim();
            var colon = body.IndexOf(':');
            if (colon < 0) return;

            var id = ParseUInt32(body.Substring(0, colon).Trim());
            var msg = _db.FindMessageById(id);
            if (msg == null) return;

            foreach (var n in body.Substring(colon + 1).Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                msg.AdditionalTransmitters.Add(n.Trim());
            }
        }

        void ParseSignalValue(String line)
        {
            // VAL_ 2365530144 DIFFLOCK_SwitchAuto 3 "Not Available" 2 "Error" 1 "On" 0 "Off" ;
            var body = line.Substring("VAL_".Length).Trim().TrimEnd(';').Trim();
            var sp = IndexOfWhitespace(body);
            if (sp < 0) return;

            var id = ParseUInt32(body.Substring(0, sp).Trim());
            var rest = body.Substring(sp + 1).Trim();

            var sp2 = IndexOfWhitespace(rest);
            if (sp2 < 0) return;

            var signalName = rest.Substring(0, sp2).Trim();
            var pairs = rest.Substring(sp2 + 1);

            var values = new Dictionary<long, String>();
            ParseValuePairs(pairs, values);

            var msg = _db.FindMessageById(id);
            var sig = msg?.FindSignal(signalName);
            if (sig == null) return;   // 例如 VAL_ 只为取默认值表的场景，忽略

            foreach (var kv in values) sig.ValueDescriptions[kv.Key] = kv.Value;
        }

        void ParseSignalMultiplex(String line)
        {
            // SG_MUL_VAL_ 419392789 Name MuxName 1-1;
            var body = line.Substring("SG_MUL_VAL_".Length).Trim().TrimEnd(';').Trim();
            var parts = body.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3) return;

            var msg = _db.FindMessageById(ParseUInt32(parts[0]));
            msg?.MultiplexRanges.Add(body);
        }

        void ParseSignalValueType(String line)
        {
            // SIG_VALTYPE_ 1234 SignalName : 1;   1=float 2=double
            var body = line.Substring("SIG_VALTYPE_".Length).Trim().TrimEnd(';').Trim();
            var colon = body.IndexOf(':');
            if (colon <= 0) return;

            var left = body.Substring(0, colon).Trim()
                           .Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (left.Length < 2) return;

            var msg = _db.FindMessageById(ParseUInt32(left[0]));
            var sig = msg?.FindSignal(left[1]);
            if (sig == null) return;

            var v = ParseDouble(body.Substring(colon + 1).Trim());
            sig.Attributes["SIG_VALTYPE"] = v;   // 1=IEEE float，2=IEEE double
        }

        #endregion

        #region 注释 / 属性

        void ParseComment(String line)
        {
            // CM_ "全局注释" ;
            // CM_ BO_ 123 "注释" ;
            // CM_ BU_ Node "注释" ;
            // CM_ SG_ 123 Signal "注释" ;
            // CM_ EV_ EvName "注释" ;
            var body = line.Substring("CM_".Length).Trim().TrimEnd(';').Trim();

            // 数据库级全局注释（CM_ "..." ;）不参与对象关联，直接忽略
            if (body.StartsWith("\"")) return;

            var parts = SplitByWhitespaceRespectQuote(body);
            if (parts.Count < 2) return;

            switch (parts[0])
            {
                case "BO_":
                    if (parts.Count >= 3)
                    {
                        _db.FindMessageById(ParseUInt32(parts[1]))?.Let(m => m.Comment = parts[2]);
                    }
                    break;

                case "BU_":
                    if (parts.Count >= 3)
                    {
                        var node = _db.Nodes.FirstOrDefault(n => n.Name == parts[1]);
                        if (node == null) { node = new DbcNode { Name = parts[1] }; _db.Nodes.Add(node); }
                        node.Comment = parts[2];
                    }
                    break;

                case "SG_":
                    if (parts.Count >= 4)
                    {
                        var msg = _db.FindMessageById(ParseUInt32(parts[1]));
                        msg?.FindSignal(parts[2])?.Let(s => s.Comment = parts[3]);
                    }
                    break;

                case "EV_":
                    if (parts.Count >= 3)
                    {
                        _db.EnvVars.FirstOrDefault(e => e.Name == parts[1])?.Let(e => e.Comment = parts[2]);
                    }
                    break;

                case "VAL_TABLE_":
                    if (parts.Count >= 3 && _valueTableMap.TryGetValue(parts[1], out var t)) t.Comment = parts[2];
                    break;
            }
        }

        void ParseAttributeDefinition(String line)
        {
            // BA_DEF_ "BusType" STRING ;
            // BA_DEF_ BO_ "GenMsgCycleTime" INT 0 3600000;
            // BA_DEF_ SG_ "GenSigStartValue" FLOAT 0 100000000000;
            // BA_DEF_ BU_ "NodeLayerModules" STRING ;
            var body = line.Substring("BA_DEF_".Length).Trim().TrimEnd(';').Trim();

            var def = new DbcAttributeDefinition();

            var parts = SplitByWhitespaceRespectQuote(body);
            if (parts.Count == 0) return;

            var index = 0;
            switch (parts[0])
            {
                case "BO_": def.ObjectType = DbcObjectType_e.Message; index = 1; break;
                case "SG_": def.ObjectType = DbcObjectType_e.Signal; index = 1; break;
                case "BU_": def.ObjectType = DbcObjectType_e.Node; index = 1; break;
                case "EV_": def.ObjectType = DbcObjectType_e.EnvVar; index = 1; break;
                default: def.ObjectType = DbcObjectType_e.None; index = 0; break;
            }

            if (index >= parts.Count) return;
            def.Name = parts[index++];

            if (index < parts.Count)
            {
                var typeText = parts[index++].ToUpperInvariant();
                switch (typeText)
                {
                    case "INT": def.ValueType = DbcAttrType_e.Int; break;
                    case "HEX": def.ValueType = DbcAttrType_e.Hex; break;
                    case "FLOAT": def.ValueType = DbcAttrType_e.Float; break;
                    case "STRING": def.ValueType = DbcAttrType_e.String; break;
                    case "ENUM": def.ValueType = DbcAttrType_e.Enum; break;
                    default: def.ValueType = DbcAttrType_e.String; break;
                }

                if (def.ValueType == DbcAttrType_e.Int || def.ValueType == DbcAttrType_e.Hex || def.ValueType == DbcAttrType_e.Float)
                {
                    if (index < parts.Count) def.Minimum = ParseDouble(parts[index++]);
                    if (index < parts.Count) def.Maximum = ParseDouble(parts[index++]);
                }
                else if (def.ValueType == DbcAttrType_e.Enum)
                {
                    // 枚举候选值来自引号文本；属性名本身也用引号包裹，需跳过，
                    // 否则解析出的候选会比实际定义多一项，索引整体错位。
                    var quoted = ExtractQuoted(body);
                    var start = quoted.Count > 0 && quoted[0] == def.Name ? 1 : 0;
                    for (int i = start; i < quoted.Count; i++) def.EnumValues.Add(quoted[i]);
                }
            }

            _db.AttributeDefinitions.Add(def);
        }

        void ParseAttributeDefault(String line)
        {
            // BA_DEF_DEF_ "BusType" "";
            // BA_DEF_DEF_ "GenMsgCycleTime" 0;
            var body = line.Substring("BA_DEF_DEF_".Length).Trim().TrimEnd(';').Trim();

            var parts = SplitByWhitespaceRespectQuote(body);
            if (parts.Count < 2) return;

            var name = parts[0];
            var def = _db.AttributeDefinitions.FirstOrDefault(d => d.Name == name);
            if (def == null) return;

            def.DefaultValue = ConvertAttributeValue(def, parts[1]);
        }

        void ParseAttributeValue(String line)
        {
            // BA_ "BusType" "CAN";
            // BA_ "GenMsgCycleTime" BO_ 123 100;
            // BA_ "GenSigStartValue" SG_ 123 SignalName 0;
            // BA_ "NodeLayerModules" BU_ Node "CAN";
            // BA_ "Attr" EV_ EnvName 1;
            var body = line.Substring("BA_".Length).Trim().TrimEnd(';').Trim();

            var parts = SplitByWhitespaceRespectQuote(body);
            if (parts.Count < 2) return;

            var record = new DbcAttributeValue { Name = parts[0] };

            var def = _db.AttributeDefinitions.FirstOrDefault(d => d.Name == record.Name);

            var index = 1;
            if (index < parts.Count && IsObjectKeyword(parts[index]))
            {
                switch (parts[index])
                {
                    case "BO_": record.ObjectType = DbcObjectType_e.Message; break;
                    case "SG_": record.ObjectType = DbcObjectType_e.Signal; break;
                    case "BU_": record.ObjectType = DbcObjectType_e.Node; break;
                    case "EV_": record.ObjectType = DbcObjectType_e.EnvVar; break;
                }
                index++;
            }

            switch (record.ObjectType)
            {
                case DbcObjectType_e.Message:
                case DbcObjectType_e.Signal:
                    if (index >= parts.Count) return;
                    record.MessageId = ParseUInt32(parts[index++]);
                    if (record.ObjectType == DbcObjectType_e.Signal)
                    {
                        if (index >= parts.Count) return;
                        record.ObjectName = parts[index++];
                    }
                    break;

                case DbcObjectType_e.Node:
                case DbcObjectType_e.EnvVar:
                    if (index >= parts.Count) return;
                    record.ObjectName = parts[index++];
                    break;
            }

            if (index >= parts.Count) return;
            record.Value = ConvertAttributeValue(def, parts[index]);

            // 同步写入对应对象的属性字典
            switch (record.ObjectType)
            {
                case DbcObjectType_e.None:
                    _db.GlobalAttributes.Add(record);
                    break;

                case DbcObjectType_e.Message:
                    _db.FindMessageById(record.MessageId)?.Let(m => m.Attributes[record.Name] = record.Value);
                    break;

                case DbcObjectType_e.Signal:
                    _db.FindMessageById(record.MessageId)?.FindSignal(record.ObjectName)?.Let(s => s.Attributes[record.Name] = record.Value);
                    break;

                case DbcObjectType_e.Node:
                    _db.Nodes.FirstOrDefault(n => n.Name == record.ObjectName)?.Let(n => n.Attributes[record.Name] = record.Value);
                    break;

                case DbcObjectType_e.EnvVar:
                    _db.EnvVars.FirstOrDefault(e => e.Name == record.ObjectName)?.Let(e => e.Attributes[record.Name] = record.Value);
                    break;
            }
        }

        void ParseEnvVar(String line)
        {
            // EV_ EvName: 0 [0|100] "unit" 0 1 DUMMY_NODE_VECTOR0 Vector__XXX;
            var body = line.TrimEnd(';').Trim();
            if (!body.StartsWith("EV_")) return;

            body = body.Substring("EV_".Length).Trim();
            var colon = body.IndexOf(':');
            if (colon < 0) return;

            var ev = new DbcEnvVar { Name = body.Substring(0, colon).Trim() };
            var rest = body.Substring(colon + 1).Trim();

            var parts = SplitByWhitespaceRespectQuote(rest);
            var index = 0;

            if (index < parts.Count) ev.Type = (int)ParseDouble(parts[index++]);

            var range = FindBetween(rest, '[', ']');
            if (!String.IsNullOrEmpty(range))
            {
                var bar = range.IndexOf('|');
                if (bar > 0)
                {
                    ev.Minimum = ParseDouble(range.Substring(0, bar));
                    ev.Maximum = ParseDouble(range.Substring(bar + 1));
                }
            }

            // 单位：引号内文本
            var unitStart = rest.IndexOf('"');
            if (unitStart >= 0)
            {
                var unitEnd = FindQuoteEnd(rest, unitStart);
                if (unitEnd > unitStart) ev.Unit = UnescapeText(rest.Substring(unitStart + 1, unitEnd - unitStart - 1));
            }

            // 取数字序列：type [min|max] "unit" initial id accessType nodes
            var numericTail = partAfter(rest, '"');
            var nums = new List<double>();
            foreach (var t in numericTail.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (t.StartsWith("[") || t.Contains("|")) continue;
                if (TryParseDouble(t, out var d)) nums.Add(d);
            }

            if (nums.Count > 0) ev.InitialValue = nums[0];
            if (nums.Count > 1) ev.EnvVarId = (int)nums[1];

            var accessIdx = rest.IndexOf("DUMMY_NODE_VECTOR", StringComparison.Ordinal);
            if (accessIdx >= 0)
            {
                var tail = rest.Substring(accessIdx);
                var sp = IndexOfWhitespace(tail);
                ev.AccessType = sp < 0 ? tail.Trim() : tail.Substring(0, sp).Trim();
                if (sp > 0)
                {
                    foreach (var n in tail.Substring(sp + 1).Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        ev.AccessNodes.Add(n.Trim());
                }
            }

            _db.EnvVars.Add(ev);
        }

        #endregion

        #region 工具方法

        /// <summary>按属性定义把文本转为对应类型的值</summary>
        Object ConvertAttributeValue(DbcAttributeDefinition def, String text)
        {
            if (text == null) return null;

            if (def != null && def.ValueType == DbcAttrType_e.Enum)
            {
                var name = Unquote(text);
                var idx = def.EnumValues.FindIndex(v => String.Equals(v, name, StringComparison.Ordinal));
                if (idx >= 0) return idx;
                if (TryParseDouble(text, out var d)) return (int)d;
                return name;
            }

            if (def != null && def.ValueType == DbcAttrType_e.String) return Unquote(text);

            if (TryParseDouble(text, out var value)) return value;
            return Unquote(text);
        }

        static bool IsObjectKeyword(String s)
            => s == "BO_" || s == "SG_" || s == "BU_" || s == "EV_";

        static int IndexOfWhitespace(String s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (Char.IsWhiteSpace(s[i])) return i;
            }
            return -1;
        }

        /// <summary>取第一个 open..close 之间的文本（不含括号）</summary>
        static String FindBetween(String s, char open, char close)
        {
            if (s == null) return null;
            var a = s.IndexOf(open);
            if (a < 0) return null;
            var b = s.IndexOf(close, a + 1);
            if (b < 0) return null;
            return s.Substring(a + 1, b - a - 1);
        }

        static int FindQuoteEnd(String s, int quoteStart)
        {
            var escaped = false;
            for (int i = quoteStart + 1; i < s.Length; i++)
            {
                var c = s[i];
                if (c == '\\' && !escaped) { escaped = true; continue; }
                if (c == '"' && !escaped) return i;
                escaped = false;
            }
            return -1;
        }

        static String partAfter(String s, char c)
        {
            var i = s.IndexOf(c);
            return i < 0 ? s : s.Substring(i + 1);
        }

        /// <summary>去掉首尾引号并还原转义</summary>
        static String Unquote(String s)
        {
            if (String.IsNullOrEmpty(s)) return s;
            s = s.Trim();
            if (s.Length >= 2 && s[0] == '"' && s[s.Length - 1] == '"')
                return UnescapeText(s.Substring(1, s.Length - 2));
            return s;
        }

        static String UnescapeText(String s)
        {
            if (String.IsNullOrEmpty(s)) return s;
            if (s.IndexOf('\\') < 0) return s;

            var sb = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '\\' && i + 1 < s.Length)
                {
                    var n = s[i + 1];
                    switch (n)
                    {
                        case 'n': sb.Append(' '); break;   // 注释中的换行统一成空格
                        case 'r': break;
                        case 't': sb.Append(' '); break;
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        default: sb.Append(n); break;
                    }
                    i++;
                }
                else sb.Append(s[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 按空白切分，但保留引号内整体（引号本身剔除）。
        /// 用于 CM_ / BA_ / BA_DEF_ 这类「关键字 + 若干参数」的语句。
        /// </summary>
        static List<String> SplitByWhitespaceRespectQuote(String s)
        {
            var result = new List<String>();
            if (String.IsNullOrEmpty(s)) return result;

            var i = 0;
            while (i < s.Length)
            {
                while (i < s.Length && Char.IsWhiteSpace(s[i])) i++;
                if (i >= s.Length) break;

                var sb = new StringBuilder();
                if (s[i] == '"')
                {
                    i++;
                    while (i < s.Length && s[i] != '"')
                    {
                        if (s[i] == '\\' && i + 1 < s.Length)
                        {
                            sb.Append(s[i]).Append(s[i + 1]);
                            i += 2;
                            continue;
                        }
                        sb.Append(s[i++]);
                    }
                    if (i < s.Length) i++;   // 跳过收尾引号
                    result.Add(UnescapeText(sb.ToString()));
                }
                else
                {
                    while (i < s.Length && !Char.IsWhiteSpace(s[i])) sb.Append(s[i++]);
                    result.Add(sb.ToString());
                }
            }
            return result;
        }

        /// <summary>提取文本中所有引号内的字符串（不含引号）</summary>
        static List<String> ExtractQuoted(String s)
        {
            var list = new List<String>();
            var i = 0;
            while (i < s.Length)
            {
                if (s[i] == '"')
                {
                    var end = FindQuoteEnd(s, i);
                    if (end < 0) break;
                    list.Add(UnescapeText(s.Substring(i + 1, end - i - 1)));
                    i = end + 1;
                }
                else i++;
            }
            return list;
        }

        /// <summary>词法切分：引号内为整体，其余按空白切分（用于 VAL_TABLE_ / VAL_ 的值对）</summary>
        static List<String> Tokenize(String s)
        {
            var result = new List<String>();
            if (String.IsNullOrEmpty(s)) return result;

            var i = 0;
            while (i < s.Length)
            {
                while (i < s.Length && (Char.IsWhiteSpace(s[i]) || s[i] == ';')) i++;
                if (i >= s.Length) break;

                if (s[i] == '"')
                {
                    var end = FindQuoteEnd(s, i);
                    if (end < 0) { result.Add(UnescapeText(s.Substring(i + 1))); break; }
                    result.Add(UnescapeText(s.Substring(i + 1, end - i - 1)));
                    i = end + 1;
                }
                else
                {
                    var start = i;
                    while (i < s.Length && !Char.IsWhiteSpace(s[i]) && s[i] != ';') i++;
                    result.Add(s.Substring(start, i - start));
                }
            }
            return result;
        }

        static UInt32 ParseUInt32(String s)
        {
            s = s.Trim();
            if (s.Length == 0) return 0;

            // 支持 0x 前缀
            if (s.StartsWith("0x") || s.StartsWith("0X"))
            {
                if (UInt32.TryParse(s.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var hx)) return hx;
            }

            if (UInt32.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v)) return v;
            return (UInt32)ParseDouble(s);
        }

        static bool TryParseLong(String s, out long value)
        {
            value = 0;
            if (String.IsNullOrEmpty(s)) return false;

            s = s.Trim();
            if (s.StartsWith("0x") || s.StartsWith("0X"))
            {
                if (Int64.TryParse(s.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value)) return true;
            }
            return Int64.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        static bool TryParseDouble(String s, out double value)
            => Double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value);

        static double ParseDouble(String s)
            => TryParseDouble(s?.Trim(), out var v) ? v : 0;

        #endregion
    }

    /// <summary>链式调用的语法糖（内部使用，减少 null 判断）</summary>
    static class DbcObjectExtensions
    {
        public static void Let<T>(this T obj, Action<T> action) where T : class
        {
            if (obj != null) action(obj);
        }
    }
}
