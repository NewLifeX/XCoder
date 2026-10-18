using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALib.Can_SAEJ1939
{
    /// <summary>字节序（DBC 信号定义中的 @1 / @0）</summary>
    public enum DbcByteOrder_e
    {
        /// <summary>@1 Intel 小端，起始位为最低位</summary>
        Intel = 1,

        /// <summary>@0 Motorola 大端，起始位为最高位</summary>
        Motorola = 0,
    }

    /// <summary>信号取值类型（DBC @1+ / @1-）</summary>
    public enum DbcValueType_e
    {
        /// <summary>无符号</summary>
        Unsigned = 0,

        /// <summary>有符号（补码）</summary>
        Signed = 1,
    }

    /// <summary>属性值类型（BA_DEF_ 中声明）</summary>
    public enum DbcAttrType_e
    {
        Int,
        Hex,
        Float,
        String,
        Enum,
    }

    /// <summary>属性作用对象</summary>
    public enum DbcObjectType_e
    {
        None,
        Node,
        Message,
        Signal,
        EnvVar,
    }

    /// <summary>DBC 中一个信号的完整定义</summary>
    public class DbcSignal
    {
        /// <summary>信号名（Unique Signal Name），同一报文内唯一</summary>
        public String Name;

        /// <summary>多路复用标记：M=多路复用器，mN=属于第 N 路复用，空=普通信号</summary>
        public String Multiplex;

        /// <summary>该信号是否是多路复用器本身（M）</summary>
        public bool IsMultiplexer => String.Equals(Multiplex, "M", StringComparison.OrdinalIgnoreCase);

        /// <summary>所属复用路号，非复用信号返回 -1</summary>
        public int MultiplexValue
        {
            get
            {
                if (String.IsNullOrEmpty(Multiplex) || IsMultiplexer) return -1;
                if (Multiplex.Length > 1 && (Multiplex[0] == 'm' || Multiplex[0] == 'M'))
                {
                    if (int.TryParse(Multiplex.Substring(1), out var v)) return v;
                }
                return -1;
            }
        }

        /// <summary>起始位（0 基，与 DBC 文本一致）</summary>
        public int StartBit;

        /// <summary>位长度</summary>
        public int Length;

        /// <summary>字节序</summary>
        public DbcByteOrder_e ByteOrder = DbcByteOrder_e.Intel;

        /// <summary>取值类型</summary>
        public DbcValueType_e ValueType = DbcValueType_e.Unsigned;

        /// <summary>原始值缩放因子（物理值 = 原始值 * Factor + Offset）</summary>
        public double Factor = 1;

        /// <summary>原始值偏移量</summary>
        public double Offset = 0;

        /// <summary>原始值最小值</summary>
        public double Minimum = 0;

        /// <summary>原始值最大值</summary>
        public double Maximum = 0;

        /// <summary>物理单位，如 "%"、"rpm"</summary>
        public String Unit = "";

        /// <summary>接收节点列表</summary>
        public List<String> Receivers = new List<String>();

        /// <summary>所属报文</summary>
        public DbcMessage Message;

        /// <summary>信号注释（CM_ SG_）</summary>
        public String Comment;

        /// <summary>信号属性（BA_ ... SG_），键为属性名</summary>
        public Dictionary<String, Object> Attributes = new Dictionary<String, Object>(StringComparer.Ordinal);

        /// <summary>信号取值枚举（VAL_ 报文ID 信号名 值 "描述" ...），键为原始值</summary>
        public Dictionary<long, String> ValueDescriptions = new Dictionary<long, String>();

        /// <summary>引用的全局取值表名（若存在同名 ValueTable，则由解析器自动合并）</summary>
        public String ValueTableName;

        /// <summary>原始值（未缩放）最大值</summary>
        public long RawMaximum => Length >= 63 ? Int64.MaxValue : (1L << Length) - 1;

        /// <summary>原始值（未缩放）最小值</summary>
        public long RawMinimum => ValueType == DbcValueType_e.Signed ? -(1L << (Length - 1)) : 0;

        /// <summary>物理值最大值</summary>
        public double PhysicalMaximum => RawMaximum * Factor + Offset;

        /// <summary>物理值最小值</summary>
        public double PhysicalMinimum => RawMinimum * Factor + Offset;

        /// <summary>原始值转物理值</summary>
        public double RawToPhysical(double raw) => raw * Factor + Offset;

        /// <summary>物理值转原始值（四舍五入）</summary>
        public double PhysicalToRaw(double physical) => (physical - Offset) / Factor;

        /// <summary>取某个原始值对应的描述文本（无则返回 null）</summary>
        public String GetValueDescription(double raw)
        {
            var key = (long)Math.Round(raw);
            if (ValueDescriptions.Count == 0) return null;
            return ValueDescriptions.TryGetValue(key, out var s) ? s : null;
        }

        #region SPN / FMI

        /// <summary>所属 SPN（可疑参数编号）。无 <c>BA_ "SPN" SG_</c> 时返回 null</summary>
        public long? Spn => DbcSpn.ParseNumber(_AttributesGet(DbcSpnHelper.AttrSpn))?.LongValue;

        /// <summary>该信号涉及的 FMI 列表。无 <c>BA_ "FMI" SG_</c> 时为空列表</summary>
        public List<int> FmiList => DbcSpn.ParseIntList(_AttributesGet(DbcSpnHelper.AttrFmi));

        /// <summary>FMI 原文，例如 "5;6"</summary>
        public String FmiText => _AttributesGet(DbcSpnHelper.AttrFmi) as String;

        /// <summary>SPN 文本，形如 "SPN234 (0xEA)"，无 SPN 时为空串</summary>
        public String SpnText
        {
            get
            {
                var spn = Spn;
                return spn == null ? "" : $"SPN{spn} (0x{spn.Value:X})";
            }
        }

        /// <summary>是否为 DTC 载体信号（声明了有效的非 0 SPN）</summary>
        public bool HasSpn => Spn != null && Spn.Value > 0;

        Object _AttributesGet(String key)
        {
            if (Attributes == null || key == null) return null;
            return Attributes.TryGetValue(key, out var v) ? v : null;
        }

        #endregion

        /// <summary>
        /// 从报文字节中解析信号原始值（整数）。
        /// 位序规则：Intel 起始位为最低位向高位延伸；Motorola 起始位为最高位向低位延伸，字节按网络序拼接。
        /// </summary>
        /// <param name="data">报文数据</param>
        /// <param name="offset">数据起始偏移</param>
        public long GetRawValue(byte[] data, int offset = 0)
        {
            if (data == null || Length <= 0) return 0;

            UInt64 raw = 0;

            if (ByteOrder == DbcByteOrder_e.Intel)
            {
                // 小端：位号 b 对应 data[(offset+b)/8] 的第 (b%8) 位，起始位即最低位
                for (int i = 0; i < Length; i++)
                {
                    var bit = StartBit + i;
                    var index = offset + (bit >> 3);
                    if (index < 0 || index >= data.Length) continue;
                    if ((data[index] & (1 << (bit & 7))) != 0) raw |= 1UL << i;
                }
            }
            else
            {
                // 大端：从起始位所在的最高位开始，依次向“下一个低位”推进，
                // 每跨过一个字节边界进入下一个字节；
                // 位号 b = index*8 + (7 - 位内序号)，即 b 是字节内从高到低的序号（b/8=字节号，b%8=7-序号）。
                int index = StartBit >> 3;
                int pos = StartBit & 7;
                for (int i = 0; i < Length; i++)
                {
                    var cur = offset + index;
                    if (cur >= 0 && cur < data.Length)
                    {
                        if ((data[cur] & (1 << pos)) != 0) raw |= 1UL << (Length - 1 - i);
                    }

                    if (pos == 0) { index++; pos = 7; }
                    else pos--;
                }
            }

            // 有符号整数的符号扩展
            if (ValueType == DbcValueType_e.Signed && Length < 64)
            {
                var mask = 1UL << (Length - 1);
                if ((raw & mask) != 0) return (long)(raw | ~((1UL << Length) - 1));
            }

            return (long)raw;
        }

        /// <summary>从报文字节中解析信号物理值</summary>
        public double GetValue(byte[] data, int offset = 0) => RawToPhysical(GetRawValue(data, offset));

        /// <summary>把整数原始值写入报文字节（修改 data）</summary>
        public void SetRawValue(byte[] data, long value, int offset = 0)
        {
            if (data == null || Length <= 0) return;

            var raw = (UInt64)value;
            if (Length < 64) raw &= ((1UL << Length) - 1);

            if (ByteOrder == DbcByteOrder_e.Intel)
            {
                for (int i = 0; i < Length; i++)
                {
                    var bit = StartBit + i;
                    var index = offset + (bit >> 3);
                    if (index < 0 || index >= data.Length) continue;
                    if ((raw & (1UL << i)) != 0) data[index] |= (byte)(1 << (bit & 7));
                    else data[index] &= (byte)~(1 << (bit & 7));
                }
            }
            else
            {
                int index = StartBit >> 3;
                int pos = StartBit & 7;
                for (int i = 0; i < Length; i++)
                {
                    var cur = offset + index;
                    if (cur >= 0 && cur < data.Length)
                    {
                        if ((raw & (1UL << (Length - 1 - i))) != 0) data[cur] |= (byte)(1 << pos);
                        else data[cur] &= (byte)~(1 << pos);
                    }

                    if (pos == 0) { index++; pos = 7; }
                    else pos--;
                }
            }
        }

        /// <summary>把物理值写入报文字节（修改 data）</summary>
        public void SetValue(byte[] data, double physical, int offset = 0)
        {
            SetRawValue(data, (long)Math.Round(PhysicalToRaw(physical)), offset);
        }

        public override String ToString() => $"{Name} [{StartBit}|{Length}@{(int)ByteOrder}{(ValueType == DbcValueType_e.Unsigned ? "+" : "-")}] ({Factor},{Offset}) [{Minimum}|{Maximum}] \"{Unit}\"";
    }

    /// <summary>DBC 中一条报文（BO_）</summary>
    public class DbcMessage
    {
        /// <summary>报文 ID，即 29bit 扩展帧 ID（J1939 特性：CANdb++ 导出的 ID 已含优先级/数据页/源地址）</summary>
        public UInt32 Id;

        /// <summary>报文名</summary>
        public String Name;

        /// <summary>数据长度 DLC</summary>
        public int Size;

        /// <summary>发送节点</summary>
        public String Transmitter;

        /// <summary>信号列表，顺序与 DBC 文件一致</summary>
        public List<DbcSignal> Signals = new List<DbcSignal>();

        /// <summary>报文注释（CM_ BO_）</summary>
        public String Comment;

        /// <summary>报文属性（BA_ ... BO_）</summary>
        public Dictionary<String, Object> Attributes = new Dictionary<String, Object>(StringComparer.Ordinal);

        /// <summary>附加发送节点（BO_TX_BU_）</summary>
        public List<String> AdditionalTransmitters = new List<String>();

        /// <summary>信号复用描述（SG_MUL_VAL_）</summary>
        public List<String> MultiplexRanges = new List<String>();

        /// <summary>解析得到的 J1939 ID 视图</summary>
        public J1939Id J1939Id
        {
            get
            {
                var id = new J1939Id(Id);
                // DBC 导出的 ID 未包含 SA，J1939 报文中 SA 由接收方填充，故这里保留 0
                return id;
            }
        }

        /// <summary>优先级（ID bit28..26）</summary>
        public int Priority => (int)((Id >> 26) & 0x07);

        /// <summary>数据页 DP（ID bit24）</summary>
        public bool DataPage => (Id & (1u << 24)) != 0;

        /// <summary>扩展数据页 EDP（ID bit25）</summary>
        public bool ExtendedDataPage => (Id & (1u << 25)) != 0;

        /// <summary>PDU 格式 PF</summary>
        public byte PF => (byte)((Id >> 16) & 0xFF);

        /// <summary>PDU 特定 PS（PDU1 时为目标地址，PDU2 时为组扩展）</summary>
        public byte PS => (byte)((Id >> 8) & 0xFF);

        /// <summary>源地址 SA（ID bit7..0），DBC 中通常为 0</summary>
        public byte SourceAddress => (byte)(Id & 0xFF);

        /// <summary>是否 PDU1（点对点）格式</summary>
        public bool IsPDU1 => PF < 240;

        /// <summary>PGN 值（18bit）</summary>
        public UInt32 Pgn
        {
            get
            {
                UInt32 pgn = (UInt32)PF << 8;
                if (!IsPDU1) pgn |= PS;
                if (DataPage) pgn |= 0x10000;
                if (ExtendedDataPage) pgn |= 0x20000;
                return pgn;
            }
        }

        /// <summary>PGN 十进制文本，便于与 J1939 文档比对</summary>
        public String PgnText => Pgn.ToString();

        /// <summary>按名称查找信号（忽略大小写）</summary>
        public DbcSignal FindSignal(String name)
        {
            if (String.IsNullOrEmpty(name)) return null;
            return Signals.FirstOrDefault(s => String.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        #region SPN

        /// <summary>报文级通信错误 DTC 的 FMI 列表（<c>BA_ "CommunicationErrorDTCFMI" BO_</c>）</summary>
        public List<int> CommunicationErrorFmi
            => Attributes != null && Attributes.TryGetValue(DbcSpnHelper.AttrCommErrorFmi, out var v)
                ? DbcSpn.ParseIntList(v)
                : new List<int>();

        /// <summary>本报文内所有带 SPN 的信号（DTC 载体）</summary>
        public List<DbcSignal> SpnSignals => Signals.Where(s => s.HasSpn).ToList();

        /// <summary>按 SPN 在本报文内查找信号</summary>
        public DbcSignal FindSignalBySpn(long spn)
            => Signals.FirstOrDefault(s => s.Spn == spn);

        /// <summary>本报文涉及的 SPN 列表（升序）</summary>
        public List<long> SpnList => SpnSignals.Select(s => s.Spn.Value).Distinct().OrderBy(x => x).ToList();

        #endregion

        /// <summary>
        /// 按 J1939 ID 规则把信号原始值组装成报文字节数组（长度 = Size，最多 8 字节）。
        /// </summary>
        public byte[] BuildData(IDictionary<String, long> rawValues)
        {
            var len = Math.Min(Size <= 0 ? 8 : Size, 8);
            var data = new byte[len == 0 ? 8 : len];
            if (rawValues == null) return data;

            foreach (var kv in rawValues)
            {
                var sig = FindSignal(kv.Key);
                sig?.SetRawValue(data, kv.Value);
            }

            return data;
        }

        public override String ToString() => $"BO_ {Id} {Name}: {Size} {Transmitter}  PGN={PgnText} 信号数={Signals.Count}";
    }

    /// <summary>DBC 中一个节点（BU_）</summary>
    public class DbcNode
    {
        /// <summary>节点名</summary>
        public String Name;

        /// <summary>节点注释（CM_ BU_）</summary>
        public String Comment;

        /// <summary>节点属性（BA_ ... BU_）</summary>
        public Dictionary<String, Object> Attributes = new Dictionary<String, Object>(StringComparer.Ordinal);

        public override String ToString() => $"BU_ {Name}";
    }

    /// <summary>DBC 中一个全局取值表（VAL_TABLE_）</summary>
    public class DbcValueTable
    {
        /// <summary>取值表名</summary>
        public String Name;

        /// <summary>取值表注释（CM_ VAL_TABLE_）</summary>
        public String Comment;

        /// <summary>值到描述文本的映射</summary>
        public Dictionary<long, String> Values = new Dictionary<long, String>();

        public override String ToString() => $"VAL_TABLE_ {Name} ({Values.Count} 项)";
    }

    /// <summary>DBC 中一个环境变量（EV_）</summary>
    public class DbcEnvVar
    {
        /// <summary>环境变量名</summary>
        public String Name;

        /// <summary>类型（0=整型 1=浮点 2=字符串）</summary>
        public int Type;

        /// <summary>最小值</summary>
        public double Minimum;

        /// <summary>最大值</summary>
        public double Maximum;

        /// <summary>单位</summary>
        public String Unit = "";

        /// <summary>初始值</summary>
        public double InitialValue;

        /// <summary>环境变量 ID</summary>
        public int EnvVarId;

        /// <summary>访问类型（DUMMY_NODE_VECTOR0..3）</summary>
        public String AccessType;

        /// <summary>访问节点</summary>
        public List<String> AccessNodes = new List<String>();

        /// <summary>注释</summary>
        public String Comment;

        /// <summary>属性</summary>
        public Dictionary<String, Object> Attributes = new Dictionary<String, Object>(StringComparer.Ordinal);

        /// <summary>值描述</summary>
        public Dictionary<long, String> ValueDescriptions = new Dictionary<long, String>();

        public override String ToString() => $"EV_ {Name}";
    }

    /// <summary>DBC 属性定义（BA_DEF_）</summary>
    public class DbcAttributeDefinition
    {
        /// <summary>属性名</summary>
        public String Name;

        /// <summary>作用对象</summary>
        public DbcObjectType_e ObjectType = DbcObjectType_e.None;

        /// <summary>值类型</summary>
        public DbcAttrType_e ValueType = DbcAttrType_e.Int;

        /// <summary>整型/浮点最小值</summary>
        public double Minimum;

        /// <summary>整型/浮点最大值</summary>
        public double Maximum;

        /// <summary>枚举候选值（ENUM）</summary>
        public List<String> EnumValues = new List<String>();

        /// <summary>默认值（BA_DEF_DEF_）</summary>
        public Object DefaultValue;

        public override String ToString() => $"BA_DEF_ {ObjectType} \"{Name}\" {ValueType}";
    }

    /// <summary>DBC 属性赋值（BA_）</summary>
    public class DbcAttributeValue
    {
        /// <summary>属性名</summary>
        public String Name;

        /// <summary>作用对象</summary>
        public DbcObjectType_e ObjectType = DbcObjectType_e.None;

        /// <summary>作用对象所属报文 ID（信号/报文属性时有效）</summary>
        public UInt32 MessageId;

        /// <summary>作用对象名称（信号属性时为信号名）</summary>
        public String ObjectName;

        /// <summary>属性值（Int/Hex/Float 为 double，String 为 String，Enum 为索引 int）</summary>
        public Object Value;

        public override String ToString() => $"BA_ \"{Name}\" {ObjectType} {ObjectName} = {Value}";
    }

    /// <summary>
    /// 一个 DBC 文件的完整解析结果。
    /// 语法覆盖 Vector CANdb++ 导出格式：VERSION / NS_ / BS_ / BU_ / VAL_TABLE_ /
    /// BO_ / SG_ / EV_ / BO_TX_BU_ / CM_ / BA_DEF_ / BA_DEF_DEF_ / BA_ / VAL_ /
    /// SG_MUL_VAL_ / SIG_GROUP_ / SIG_VALTYPE_ / CAT_DEF_ / CAT_ / FILTER / EV_DATA_ / ENVVAR_DATA_ / SGTYPE_*。
    /// </summary>
    public class DbcDatabase
    {
        /// <summary>版本号（VERSION）</summary>
        public String Version = "";

        /// <summary>新符号列表（NS_）</summary>
        public List<String> NewSymbols = new List<String>();

        /// <summary>位定时配置原文（BS_）</summary>
        public String BitTiming;

        /// <summary>网络节点（BU_）</summary>
        public List<DbcNode> Nodes = new List<DbcNode>();

        /// <summary>全局取值表（VAL_TABLE_）</summary>
        public List<DbcValueTable> ValueTables = new List<DbcValueTable>();

        /// <summary>报文列表，顺序与文件一致</summary>
        public List<DbcMessage> Messages = new List<DbcMessage>();

        /// <summary>环境变量（EV_）</summary>
        public List<DbcEnvVar> EnvVars = new List<DbcEnvVar>();

        /// <summary>属性定义（BA_DEF_）</summary>
        public List<DbcAttributeDefinition> AttributeDefinitions = new List<DbcAttributeDefinition>();

        /// <summary>全局属性赋值（BA_ 无对象类型，作用于整个数据库）</summary>
        public List<DbcAttributeValue> GlobalAttributes = new List<DbcAttributeValue>();

        /// <summary>信号组（SIG_GROUP_）</summary>
        public List<String> SignalGroups = new List<String>();

        /// <summary>解析过程中的异常恢复记录，便于定位文件异常</summary>
        public List<String> Warnings = new List<String>();

        /// <summary>源文件路径</summary>
        public String FilePath;

        /// <summary>解析器集合</summary>
        readonly DbcParser _parser;

        /// <summary>SPN 索引（SPN → 信号列表），解析完成后自动构建</summary>
        Dictionary<long, List<DbcSignal>> _spnMap;

        /// <summary>PGN 索引（PGN → 报文列表），解析完成后自动构建</summary>
        Dictionary<UInt32, List<DbcMessage>> _pgnMap;

        public DbcDatabase()
        {
            _parser = new DbcParser(this);
        }

        /// <summary>
        /// 解析完成回调，由 <see cref="DbcParser"/> 在解析结束时调用，用于构建各类索引。
        /// 手工往 <see cref="Messages"/> 里添加对象后，可再调用 <see cref="BuildIndex"/> 重建索引。
        /// </summary>
        internal void BuildIndex()
        {
            _spnMap = new Dictionary<long, List<DbcSignal>>();
            _pgnMap = new Dictionary<UInt32, List<DbcMessage>>();

            foreach (var msg in Messages)
            {
                if (!_pgnMap.TryGetValue(msg.Pgn, out var mlist))
                {
                    mlist = new List<DbcMessage>();
                    _pgnMap[msg.Pgn] = mlist;
                }
                mlist.Add(msg);

                foreach (var sig in msg.Signals)
                {
                    var spn = sig.Spn;

                    // SPN=0 是该 DBC 里 BA_DEF_DEF_ "SPN" 的默认值，表示未定义，不入索引
                    if (spn == null || spn.Value <= 0) continue;

                    if (!_spnMap.TryGetValue(spn.Value, out var slist))
                    {
                        slist = new List<DbcSignal>();
                        _spnMap[spn.Value] = slist;
                    }
                    slist.Add(sig);
                }
            }
        }

        void _EnsureIndex()
        {
            if (_spnMap == null) BuildIndex();
        }

        /// <summary>按报文名查找（忽略大小写）</summary>
        public DbcMessage FindMessage(String name)
        {
            if (String.IsNullOrEmpty(name)) return null;
            return Messages.FirstOrDefault(m => String.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>按 29bit 报文 ID 查找</summary>
        public DbcMessage FindMessageById(UInt32 id) => Messages.FirstOrDefault(m => m.Id == id);

        /// <summary>按 PGN 查找全部报文</summary>
        public List<DbcMessage> FindMessagesByPgn(UInt32 pgn) => Messages.Where(m => m.Pgn == pgn).ToList();

        #region SPN 解读

        /// <summary>全部带 SPN 的信号（DTC 载体信号）</summary>
        public List<DbcSignal> SpnSignals
        {
            get
            {
                _EnsureIndex();
                return _spnMap.Values.SelectMany(x => x).ToList();
            }
        }

        /// <summary>全部 SPN（升序去重）</summary>
        public List<long> AllSpns
        {
            get
            {
                _EnsureIndex();
                return _spnMap.Keys.OrderBy(x => x).ToList();
            }
        }

        /// <summary>SPN 总数</summary>
        public int SpnCount
        {
            get
            {
                _EnsureIndex();
                return _spnMap.Count;
            }
        }

        /// <summary>按 SPN 查找全部信号（同一 SPN 可能出现在多条报文中）</summary>
        public List<DbcSignal> FindSignalsBySpn(long spn)
        {
            _EnsureIndex();
            return _spnMap.TryGetValue(spn, out var list) ? list : new List<DbcSignal>();
        }

        /// <summary>按 SPN 查找首个信号</summary>
        public DbcSignal FindSignalBySpn(long spn) => FindSignalsBySpn(spn).FirstOrDefault();

        /// <summary>按 SPN 查找承载它的报文（去重）</summary>
        public List<DbcMessage> FindMessagesBySpn(long spn)
            => FindSignalsBySpn(spn).Select(s => s.Message).Where(m => m != null).Distinct().ToList();

        /// <summary>SPN 索引快照（SPN → 信号列表）</summary>
        public Dictionary<long, List<DbcSignal>> SpnMap
        {
            get
            {
                _EnsureIndex();
                return _spnMap;
            }
        }

        /// <summary>
        /// 把 J1939 故障码 DTC 定位到 DBC 中的具体信号。
        /// 用于 DM1/DM2 等诊断报文解析结果与 DBC 定义建立对应关系。
        /// </summary>
        /// <param name="dtc">已解析的故障码</param>
        /// <returns>承载该 SPN 的信号列表，未定义时为空列表</returns>
        public List<DbcSignal> ResolveDtc(J1939Dtc dtc)
        {
            if (dtc == null) return new List<DbcSignal>();

            // DTC 的 SPN 为 19bit（含转换方式位），DBC 属性为 19bit 上限，直接取值比对
            var list = FindSignalsBySpn(dtc.SPN);

            // 兜底：部分工具导出时 SPN 未含第 19 位，去掉最高位再试
            if (list.Count == 0 && dtc.SPN > 0x3FFFF) list = FindSignalsBySpn(dtc.SPN & 0x3FFFF);

            return list;
        }

        /// <summary>该 FMI 是否属于信号声明支持的 FMI 集合</summary>
        /// <param name="sig">信号</param>
        /// <param name="fmi">故障类型</param>
        public static bool IsFmiSupported(DbcSignal sig, int fmi)
        {
            if (sig == null) return false;
            var list = sig.FmiList;
            return list.Count == 0 || list.Contains(fmi);
        }

        /// <summary>SPN 简要文本，形如 "SPN234 (0xEA)"</summary>
        public static String FormatSpn(long spn) => $"SPN{spn} (0x{spn:X})";

        #endregion

        /// <summary>把文件（或文本）解析为数据库对象</summary>
        /// <param name="text">DBC 文件全文</param>
        public void Parse(String text) => _parser.Parse(text);

        /// <summary>解析文件内容，自动识别编码（UTF-8 / GB2312）</summary>
        public void ParseFile(String path)
        {
            FilePath = path;
            String text;
            var bytes = System.IO.File.ReadAllBytes(path);

            // DBC 常见编码：UTF-8 与 GB2312，带 BOM 时按 BOM 判定
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            {
                text = Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
            }
            else
            {
                try
                {
                    var strict = new UTF8Encoding(false, true);
                    text = strict.GetString(bytes);
                }
                catch (DecoderFallbackException)
                {
                    try { text = Encoding.GetEncoding("GB2312").GetString(bytes); }
                    catch (Exception) { text = Encoding.Default.GetString(bytes); }
                }
            }

            Parse(text);
        }

        /// <summary>从文件加载</summary>
        public static DbcDatabase LoadFile(String path)
        {
            var db = new DbcDatabase();
            db.ParseFile(path);
            return db;
        }

        /// <summary>从文本加载</summary>
        public static DbcDatabase Load(String text)
        {
            var db = new DbcDatabase();
            db.Parse(text);
            return db;
        }

        public override String ToString()
        {
            _EnsureIndex();

            return $"DBC 版本=\"{Version}\" 节点={Nodes.Count} 报文={Messages.Count} " +
                   $"信号={Messages.Sum(m => m.Signals.Count)} SPN={_spnMap.Count} " +
                   $"取值表={ValueTables.Count} 属性定义={AttributeDefinitions.Count}";
        }
    }
}
