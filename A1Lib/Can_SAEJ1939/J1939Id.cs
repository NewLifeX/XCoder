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
    /// 参数组编号 PGN，J1939 中用于标识一组相关参数。
    /// 由扩展数据页 EDP、数据页 DP、PDU 格式 PF、PDU 特定 PS(PDU1 时部分位) 组成。
    /// </summary>
    public class PGN
    {
        /// <summary>扩展数据页，1bit（bit25）</summary>
        public bool ExtendDataPage = false;

        /// <summary>数据页，1bit（bit24）</summary>
        public bool DataPage = false;

        /// <summary>PDU 格式 PF（bit23..16）</summary>
        public byte PF = 0;

        /// <summary>PDU 特定 PS（bit15..8）</summary>
        public byte PS = 0;

        /// <summary>PDU 格式 &lt; 240 时为 PDU1（点对点），否则为 PDU2（广播）</summary>
        public bool IsPDU1 => PF < 240;

        /// <summary>目标地址 DA，仅 PDU1 有效（即 PS）</summary>
        public byte DA => PS;

        /// <summary>组扩展 GE，仅 PDU2 有效（即 PS）</summary>
        public byte GroupExtension => PS;

        public PGN() { }

        public PGN(byte pf, byte ps = 0, bool dp = false, bool edp = false)
        {
            PF = pf;
            PS = ps;
            DataPage = dp;
            ExtendDataPage = edp;
        }

        /// <summary>PGN 的值（18bit）。PDU1 时目标地址 DA 不属于 PGN，置 0</summary>
        public UInt32 Value
        {
            get
            {
                UInt32 pgn = (UInt32)PF << 8;
                if (!IsPDU1) pgn |= PS;
                if (DataPage) pgn |= 0x10000;
                if (ExtendDataPage) pgn |= 0x20000;
                return pgn;
            }
            set
            {
                ExtendDataPage = (value & 0x20000) != 0;
                DataPage = (value & 0x10000) != 0;
                PF = (byte)((value >> 8) & 0xFF);
                PS = IsPDU1 ? (byte)0 : (byte)(value & 0xFF);
            }
        }

        /// <summary>输出十六进制 PGN 文本</summary>
        public override String ToString() => Value.ToString("X5");
    }

    /// <summary>
    /// J1939 使用扩展帧（29bit ID），把 ID 拆解为多个分段。
    /// 位域结构（bit28..0）：
    /// <code>
    /// bit28..26  Priority(3)  bit25  Reserved/EDP  bit24  DataPage  bit23..16  PF  bit15..8  PS  bit7..0  SA
    /// PDU1(PF&lt;240)  PS 为目标地址 DA；PDU2(PF&gt;=240) PS 为组扩展 GE
    /// </code>
    /// </summary>
    public class J1939Id
    {
        /// <summary>优先级，3bit，取值 0~7，数值越小优先级越高，默认 6</summary>
        public int Priority = 6;

        /// <summary>保留位 / 扩展数据页 EDP，1bit</summary>
        public bool Reserved = false;

        /// <summary>数据页 DP，1bit</summary>
        public bool DataPage = false;

        /// <summary>PDU 格式 PF，8bit</summary>
        public byte PF = 0;

        /// <summary>PDU 特定 PS，8bit。PDU1 时为目标地址，PDU2 时为组扩展</summary>
        public byte PS = 0;

        /// <summary>源地址 SA，8bit</summary>
        public byte SourceAddress = 0;

        public J1939Id() { }

        public J1939Id(UInt32 id29)
        {
            ID = id29;
        }

        public J1939Id(int id29)
        {
            ID = (UInt32)id29;
        }

        /// <summary>PDU 格式 &lt; 240 时为 PDU1（点对点），否则为 PDU2（广播）</summary>
        public bool IsPDU1 => PF < 240;

        /// <summary>目标地址 DA，仅 PDU1 有意义（即 PS）</summary>
        public byte DestinationAddress => PS;

        /// <summary>组扩展 GE，仅 PDU2 有意义（即 PS）</summary>
        public byte GroupExtension => PS;

        /// <summary>参数组编号</summary>
        public PGN PGN
        {
            get => new PGN(PF, PS, DataPage, Reserved);
            set
            {
                if (value == null) return;
                Reserved = value.ExtendDataPage;
                DataPage = value.DataPage;
                PF = value.PF;
                // PDU1 时 PS 代表目标地址，不由 PGN 决定，保留原值
                if (!IsPDU1) PS = value.PS;
            }
        }

        /// <summary>PGN 原始值（18bit）</summary>
        public UInt32 PGNValue
        {
            get => PGN.Value;
            set => PGN = new PGN { Value = value };
        }

        /// <summary>
        /// 29bit 扩展 ID。
        /// bit28..26 Priority | bit25 Reserved | bit24 DataPage | bit23..16 PF | bit15..8 PS | bit7..0 SA
        /// </summary>
        public UInt32 ID
        {
            get
            {
                UInt32 id = 0;
                id |= (UInt32)(Priority & 0x07) << 26;
                if (Reserved) id |= 1u << 25;
                if (DataPage) id |= 1u << 24;
                id |= (UInt32)PF << 16;
                id |= (UInt32)PS << 8;
                id |= SourceAddress;
                return id;
            }
            set
            {
                Priority = (int)((value >> 26) & 0x07);
                Reserved = (value & (1u << 25)) != 0;
                DataPage = (value & (1u << 24)) != 0;
                PF = (byte)((value >> 16) & 0xFF);
                PS = (byte)((value >> 8) & 0xFF);
                SourceAddress = (byte)(value & 0xFF);
            }
        }

        /// <summary>转换为 CAN 帧（J1939 固定使用扩展帧）</summary>
        /// <param name="data">负载数据，最多 8 字节</param>
        /// <param name="rtr">是否远程帧</param>
        public CanMsg ToCanMsg(byte[] data = null, bool rtr = false)
        {
            var bs = new byte[8];
            var len = data == null ? 0 : Math.Min(data.Length, 8);
            if (len > 0) Array.Copy(data, bs, len);

            return new CanMsg((Int32)ID, bs, rtr, true);
        }

        /// <summary>从 CAN 帧构造</summary>
        public static J1939Id FromCanMsg(CanMsg msg)
        {
            if (msg == null) return null;
            return new J1939Id((UInt32)msg.ID);
        }

        /// <summary>复制当前值</summary>
        public J1939Id Clone()
        {
            return new J1939Id
            {
                Priority = Priority,
                Reserved = Reserved,
                DataPage = DataPage,
                PF = PF,
                PS = PS,
                SourceAddress = SourceAddress,
            };
        }

        /// <summary>输出格式：Priority.PGN.SA （如 6.FEF1.00）</summary>
        public override String ToString()
        {
            return $"{Priority}.{PGN:X5}.{SourceAddress:X2}";
        }
    }
}
