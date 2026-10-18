using ALib.CAN;
using NewLife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALib.Can_SAEJ1939
{
    /// <summary>
    /// 故障指示灯状态（2bit 编码）
    /// </summary>
    public enum LampStatus_e : byte
    {
        /// <summary>灯灭 / 未使用</summary>
        Off = 0,

        /// <summary>灯亮（一般报警）</summary>
        On = 1,

        /// <summary>未使用</summary>
        NotAvailable = 2,

        /// <summary>灯不适用</summary>
        NotApplicable = 3,
    }

    /// <summary>
    /// 故障类型 FMI（Failure Mode Identifier），5bit
    /// </summary>
    public enum Fmi_e : byte
    {
        /// <summary>高于正常范围（最严重）</summary>
        AboveNormalMostSevere = 0,
        /// <summary>低于正常范围（最严重）</summary>
        BelowNormalMostSevere = 1,
        /// <summary>数据间断/不稳定</summary>
        DataErratic = 2,
        /// <summary>电压高于正常范围或短路到电源</summary>
        VoltageAboveNormal = 3,
        /// <summary>电压低于正常范围或短路到地</summary>
        VoltageBelowNormal = 4,
        /// <summary>电流低于正常范围或断路</summary>
        CurrentBelowNormal = 5,
        /// <summary>电流高于正常范围或短路到地</summary>
        CurrentAboveNormal = 6,
        /// <summary>机械系统无响应或失调</summary>
        MechanicalNoResponse = 7,
        /// <summary>异常频率</summary>
        AbnormalFrequency = 8,
        /// <summary>异常更新速率</summary>
        AbnormalUpdateRate = 9,
        /// <summary>异常变化率</summary>
        AbnormalRateOfChange = 10,
        /// <summary>根本原因未知</summary>
        RootCauseUnknown = 11,
        /// <summary>部件故障</summary>
        BadComponent = 12,
        /// <summary>超出标定范围</summary>
        OutOfCalibration = 13,
        /// <summary>特殊说明</summary>
        SpecialInstructions = 14,
        /// <summary>数据高于正常范围（最轻微）</summary>
        AboveNormalLeastSevere = 15,
        /// <summary>数据低于正常范围（最轻微）</summary>
        BelowNormalLeastSevere = 16,
        /// <summary>数据有效但低于正常范围</summary>
        DataValidButBelowNormal = 17,
        /// <summary>数据有效但高于正常范围</summary>
        DataValidButAboveNormal = 18,
        /// <summary>接收网络数据错误</summary>
        ReceivedNetworkDataError = 19,
        /// <summary>数据漂移过高</summary>
        DataDriftedHigh = 20,
        /// <summary>数据漂移过低</summary>
        DataDriftedLow = 21,
        /// <summary>与其他数据不一致</summary>
        Inconsistent = 31,
    }

    /// <summary>
    /// 单个故障码（DTC，4 字节）。SPN 18bit + FMI 5bit + OC 7bit + CM 1bit。
    /// </summary>
    public class J1939Dtc
    {
        /// <summary>可疑参数编号 SPN（19bit，标准用 18bit + 1bit 转换方式）</summary>
        public UInt32 SPN = 0;

        /// <summary>故障类型 FMI</summary>
        public Fmi_e FMI = Fmi_e.RootCauseUnknown;

        /// <summary>故障发生次数（Occurrence Count），7bit</summary>
        public byte OccurrenceCount = 0;

        /// <summary>SPN 转换方式（Conversion Method），1bit</summary>
        public bool ConversionMethod = false;

        /// <summary>解析 4 字节 DTC</summary>
        public static J1939Dtc Parse(byte[] data, int offset)
        {
            // byte1: SPN bit7..0 ; byte2: SPN bit15..8 ; byte3: FMI(5) + SPN bit17..16(2) + CM(1) ; byte4: OC(7)+1
            var spnLow = data[offset];
            var spnMid = data[offset + 1];
            var b3 = data[offset + 2];
            var fmi = (byte)(b3 & 0x1F);
            var spnHigh = (byte)((b3 >> 5) & 0x03);
            var cm = (b3 & 0x80) != 0;   // bit7
            var spn = (UInt32)(spnLow | (spnMid << 8) | (spnHigh << 16)) | (cm ? 0x80000u : 0u);

            return new J1939Dtc
            {
                SPN = spn,
                FMI = (Fmi_e)fmi,
                OccurrenceCount = (byte)(data[offset + 3] & 0x7F),
                ConversionMethod = cm,
            };
        }

        /// <summary>写出 4 字节 DTC</summary>
        public void WriteTo(byte[] data, int offset)
        {
            data[offset] = (byte)(SPN & 0xFF);
            data[offset + 1] = (byte)((SPN >> 8) & 0xFF);

            byte b3 = (byte)(((byte)FMI) & 0x1F);
            b3 |= (byte)(((SPN >> 16) & 0x03) << 5);
            if (ConversionMethod) b3 |= 0x80;
            data[offset + 2] = b3;

            data[offset + 3] = (byte)(OccurrenceCount & 0x7F);
        }

        /// <summary>是否为填充占位（SPN=0 且 FMI=0）</summary>
        public bool IsEmpty => SPN == 0 && FMI == 0;

        public override String ToString()
        {
            return $"SPN={SPN} FMI={FMI}({(byte)FMI}) OC={OccurrenceCount}";
        }
    }

    /// <summary>
    /// DM1 活动故障码 / DM2 历史故障码，PGN 65226 / 65227。
    /// 数据区结构（多包传输时按 7 字节一包拼接后解析）：
    /// byte1~2 灯具状态（各 2bit），byte3 起每 4 字节一个 DTC。
    /// </summary>
    public class Msg_DM1
    {
        /// <summary>DM1 PGN</summary>
        public const UInt32 DM1Pgn = J1939Pgn.DM1;

        /// <summary>DM2 PGN</summary>
        public const UInt32 DM2Pgn = J1939Pgn.DM2;

        /// <summary>保护灯 Protect Lamp</summary>
        public LampStatus_e ProtectLamp = LampStatus_e.Off;

        /// <summary>琥珀色警告灯 Amber Warning Lamp</summary>
        public LampStatus_e AmberWarningLamp = LampStatus_e.Off;

        /// <summary>红色停止灯 Red Stop Lamp</summary>
        public LampStatus_e RedStopLamp = LampStatus_e.Off;

        /// <summary>故障指示灯 MIL（Malfunction Indicator Lamp）</summary>
        public LampStatus_e MalfunctionIndicatorLamp = LampStatus_e.Off;

        /// <summary>故障码列表</summary>
        public List<J1939Dtc> Dtcs = new List<J1939Dtc>();

        /// <summary>源地址 SA</summary>
        public byte SourceAddress = 0;

        /// <summary>优先级，DM1/DM2 固定为 6</summary>
        public int Priority = 6;

        /// <summary>本次报文对应 PGN（DM1 或 DM2）</summary>
        public UInt32 Pgn = DM1Pgn;

        public Msg_DM1() { }

        /// <summary>灯具状态字节合成</summary>
        byte LampByte1()
        {
            byte b = 0;
            b |= (byte)((byte)ProtectLamp & 0x03);
            b |= (byte)(((byte)AmberWarningLamp & 0x03) << 2);
            b |= (byte)(((byte)RedStopLamp & 0x03) << 4);
            b |= (byte)(((byte)MalfunctionIndicatorLamp & 0x03) << 6);
            return b;
        }

        /// <summary>序列化为数据区（字节数组），长度 = 2 + 4*N</summary>
        public byte[] ToData()
        {
            var len = 2 + Dtcs.Count * 4;
            // 至少 2 字节，且 TP 传输时按 7 字节对齐也无妨
            var bs = new byte[len];
            bs[0] = LampByte1();
            bs[1] = 0xFF;   // byte2 保留，填 0xFF

            for (int i = 0; i < Dtcs.Count; i++)
            {
                Dtcs[i].WriteTo(bs, 2 + i * 4);
            }

            return bs;
        }

        /// <summary>构造 29bit ID（PDU2 广播）</summary>
        public J1939Id ToId()
        {
            return new J1939Id
            {
                Priority = Priority,
                PF = (byte)((Pgn >> 8) & 0xFF),
                PS = (byte)(Pgn & 0xFF),
                SourceAddress = SourceAddress,
            };
        }

        /// <summary>转为 CAN 帧（单帧，最多 1 个 DTC；超过需拆分/TP 传输）</summary>
        public CanMsg ToCanMsg()
        {
            return ToId().ToCanMsg(ToData());
        }

        /// <summary>从已拼接的完整数据区解析</summary>
        public static Msg_DM1 ParseData(byte[] data, UInt32 pgn, byte sa, int priority = 6)
        {
            if (data == null || data.Length < 2) return null;

            var msg = new Msg_DM1
            {
                Pgn = pgn,
                SourceAddress = sa,
                Priority = priority,
                ProtectLamp = (LampStatus_e)(data[0] & 0x03),
                AmberWarningLamp = (LampStatus_e)((data[0] >> 2) & 0x03),
                RedStopLamp = (LampStatus_e)((data[0] >> 4) & 0x03),
                MalfunctionIndicatorLamp = (LampStatus_e)((data[0] >> 6) & 0x03),
            };

            // byte2 为保留字节，DTC 自 byte3（索引 2）开始，每 4 字节一个
            for (int i = 2; i + 3 < data.Length; i += 4)
            {
                var dtc = J1939Dtc.Parse(data, i);
                if (dtc.IsEmpty && data[i] == 0xFF && data[i + 1] == 0xFF) continue; // 跳过填充 0xFF
                msg.Dtcs.Add(dtc);
            }

            return msg;
        }

        /// <summary>从单帧 CAN 报文解析</summary>
        public static Msg_DM1 Parse(CanMsg canMsg)
        {
            if (canMsg == null) return null;

            var id = J1939Id.FromCanMsg(canMsg);
            return ParseData(canMsg.Data, id.PGNValue, id.SourceAddress, id.Priority);
        }

        /// <summary>
        /// 结合 DBC 定义解读故障码：把每个 DTC 的 SPN 映射到具体信号与所属报文，
        /// 并校验 FMI 是否落在信号声明的 FMI 集合内。
        /// </summary>
        /// <param name="db">DBC 数据库，为 null 时返回空列表</param>
        /// <returns>故障码与 DBC 定义的对应关系</returns>
        public List<DtcMapping> MapToDbc(DbcDatabase db)
        {
            var result = new List<DtcMapping>();
            if (db == null) return result;

            foreach (var dtc in Dtcs)
            {
                var sigs = db.ResolveDtc(dtc);
                result.Add(new DtcMapping
                {
                    Dtc = dtc,
                    Signals = sigs,
                    FmiSupported = sigs.Count > 0 && sigs.All(s => DbcDatabase.IsFmiSupported(s, (int)dtc.FMI)),
                });
            }

            return result;
        }

        public override String ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"DM PGN=0x{Pgn:X5} SA=0x{SourceAddress:X2} ");
            sb.Append($"MIL={MalfunctionIndicatorLamp} RSL={RedStopLamp} AWL={AmberWarningLamp} PL={ProtectLamp}");
            if (Dtcs.Count == 0) sb.Append(" [无故障]");
            else
            {
                sb.Append($" [{Dtcs.Count}]");
                foreach (var d in Dtcs) sb.Append(" " + d);
            }
            return sb.ToString();
        }
    }

    /// <summary>故障码与 DBC 定义的对应关系</summary>
    public class DtcMapping
    {
        /// <summary>故障码</summary>
        public J1939Dtc Dtc;

        /// <summary>DBC 中承载该 SPN 的信号（可能为空，表示 DBC 未定义）</summary>
        public List<DbcSignal> Signals = new List<DbcSignal>();

        /// <summary>FMI 是否属于信号声明的 FMI 集合（信号未声明 FMI 时视为支持）</summary>
        public bool FmiSupported = true;

        /// <summary>DBC 中是否定义了该 SPN</summary>
        public bool IsDefined => Signals.Count > 0;

        /// <summary>承载信号的报文（首个）</summary>
        public DbcMessage Message => Signals.Count > 0 ? Signals[0].Message : null;

        public override String ToString()
        {
            var spn = DbcDatabase.FormatSpn(Dtc.SPN);
            if (!IsDefined) return $"{spn} FMI={Dtc.FMI} → DBC 未定义";
            return $"{spn} FMI={Dtc.FMI} → {Message?.Name}.{Signals[0].Name}" + (FmiSupported ? "" : " (FMI 不在声明集合)");
        }
    }
}
