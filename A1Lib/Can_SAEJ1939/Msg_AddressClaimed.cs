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
    /// 64bit NAME（名称），J1939 中唯一标识一个 ECU。
    /// 位域（bit63..0）：bit63..61 保留(1) ... 具体见标准 NAME 结构。
    /// 这里按字节小端存储，索引 0 为最低有效字节。
    /// </summary>
    public class J1939Name
    {
        /// <summary>64bit NAME 原始值</summary>
        public UInt64 Value;

        public J1939Name() { }

        public J1939Name(UInt64 value) => Value = value;

        public J1939Name(byte[] data, int offset)
        {
            for (int i = 0; i < 8; i++) Value |= (UInt64)data[offset + i] << (8 * i);
        }

        /// <summary>任意制造商代码，11bit（bit63..53）</summary>
        public UInt16 ManufacturerCode
        {
            get => (UInt16)((Value >> 53) & 0x07FF);
            set => Value = (Value & ~(0x7FFUL << 53)) | ((UInt64)(value & 0x07FF) << 53);
        }

        /// <summary>功能实例，5bit（bit52..48）</summary>
        public byte FunctionInstance
        {
            get => (byte)((Value >> 48) & 0x1F);
            set => Value = (Value & ~(0x1FUL << 48)) | ((UInt64)(value & 0x1F) << 48);
        }

        /// <summary>ECU 实例，3bit（bit47..45）</summary>
        public byte EcuInstance
        {
            get => (byte)((Value >> 45) & 0x07);
            set => Value = (Value & ~(0x07UL << 45)) | ((UInt64)(value & 0x07) << 45);
        }

        /// <summary>功能，8bit（bit44..37）</summary>
        public byte Function
        {
            get => (byte)((Value >> 37) & 0xFF);
            set => Value = (Value & ~(0xFFUL << 37)) | ((UInt64)value << 37);
        }

        /// <summary>车辆系统，7bit（bit36..30）</summary>
        public byte VehicleSystem
        {
            get => (byte)((Value >> 30) & 0x7F);
            set => Value = (Value & ~(0x7FUL << 30)) | ((UInt64)(value & 0x7F) << 30);
        }

        /// <summary>行业组，3bit（bit29..27）</summary>
        public byte IndustryGroup
        {
            get => (byte)((Value >> 27) & 0x07);
            set => Value = (Value & ~(0x07UL << 27)) | ((UInt64)(value & 0x07) << 27);
        }

        /// <summary>任意地址能力/自配置，1bit（bit24..21 区域内，标准为 bit25 ArbitraryAddressCapable）</summary>
        public bool ArbitraryAddressCapable
        {
            get => ((Value >> 24) & 1) != 0;
            set => Value = (Value & ~(1UL << 24)) | ((value ? 1UL : 0UL) << 24);
        }

        /// <summary>更低字节保留（bit23..0），标准未使用</summary>
        public UInt32 Reserved
        {
            get => (UInt32)(Value & 0x00FFFFFF);
            set => Value = (Value & 0xFFFFFFFFFF000000UL) | ((UInt64)value & 0x00FFFFFF);
        }

        /// <summary>写入字节数组（小端，8 字节）</summary>
        public void WriteTo(byte[] data, int offset = 0)
        {
            for (int i = 0; i < 8; i++) data[offset + i] = (byte)(Value >> (8 * i));
        }

        public override String ToString() => Value.ToString("X16");
    }

    /// <summary>
    /// 地址声明 Address Claimed，PGN 60928 (0x00EE00)。
    /// 数据区 8 字节 = 64bit NAME（小端）。
    /// 源地址冲突时，SA 与 PGN 相同但 NAME 更高者放弃声明。
    /// </summary>
    public class Msg_AddressClaimed
    {
        /// <summary>目标 PGN</summary>
        public const UInt32 PGN = J1939Pgn.AddressClaimed;

        /// <summary>64bit NAME</summary>
        public J1939Name Name = new J1939Name();

        /// <summary>源地址 SA（声明者占用的地址）</summary>
        public byte SourceAddress = J1939Addr.Null;

        /// <summary>优先级，地址声明固定为 6</summary>
        public int Priority = 6;

        public Msg_AddressClaimed() { }

        public Msg_AddressClaimed(J1939Name name, byte sa)
        {
            Name = name;
            SourceAddress = sa;
        }

        /// <summary>构造 29bit ID</summary>
        public J1939Id ToId()
        {
            return new J1939Id
            {
                Priority = Priority,
                Reserved = false,
                DataPage = false,
                PF = 0xEE,
                PS = 0x00,
                SourceAddress = SourceAddress,
            };
        }

        /// <summary>序列化为 8 字节数据区</summary>
        public byte[] ToData()
        {
            var bs = new byte[8];
            Name.WriteTo(bs);
            return bs;
        }

        /// <summary>转为 CAN 帧</summary>
        public CanMsg ToCanMsg()
        {
            return ToId().ToCanMsg(ToData());
        }

        /// <summary>从 CAN 帧解析</summary>
        public static Msg_AddressClaimed Parse(CanMsg msg)
        {
            if (msg == null || msg.Data == null || msg.Data.Length < 8) return null;

            var id = J1939Id.FromCanMsg(msg);
            return new Msg_AddressClaimed
            {
                Name = new J1939Name(msg.Data, 0),
                SourceAddress = id.SourceAddress,
                Priority = id.Priority,
            };
        }

        public override String ToString()
        {
            return $"AddressClaimed SA=0x{SourceAddress:X2} NAME={Name}";
        }
    }
}
