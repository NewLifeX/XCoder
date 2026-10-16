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
    /// DM3 历史故障码清除，PGN 65228 (0x00FECC)。
    /// 数据区为空（DLC = 0），用于请求清空已存储的历史故障码。
    /// </summary>
    public class Msg_DM3
    {
        /// <summary>目标 PGN</summary>
        public const UInt32 PGN = J1939Pgn.DM3;

        /// <summary>目标地址 DA，0xFF 为广播</summary>
        public byte DestinationAddress = J1939Addr.Global;

        /// <summary>源地址 SA</summary>
        public byte SourceAddress = 0;

        /// <summary>优先级，DM3 固定为 6</summary>
        public int Priority = 6;

        public Msg_DM3() { }

        public Msg_DM3(byte da = J1939Addr.Global, byte sa = 0)
        {
            DestinationAddress = da;
            SourceAddress = sa;
        }

        /// <summary>构造 29bit ID</summary>
        public J1939Id ToId()
        {
            return new J1939Id
            {
                Priority = Priority,
                PF = 0xFE,
                PS = DestinationAddress,
                SourceAddress = SourceAddress,
            };
        }

        /// <summary>转为 CAN 帧（空数据区，DLC=0）</summary>
        public CanMsg ToCanMsg()
        {
            var msg = ToId().ToCanMsg(null);
            msg.Data = new byte[0];
            return msg;
        }

        /// <summary>从 CAN 帧解析</summary>
        public static Msg_DM3 Parse(CanMsg msg)
        {
            if (msg == null) return null;

            var id = J1939Id.FromCanMsg(msg);
            return new Msg_DM3
            {
                DestinationAddress = id.DestinationAddress,
                SourceAddress = id.SourceAddress,
                Priority = id.Priority,
            };
        }

        public override String ToString()
        {
            return $"DM3 清除历史故障 DA=0x{DestinationAddress:X2} SA=0x{SourceAddress:X2}";
        }
    }

    /// <summary>
    /// DM13 停止/开始广播，PGN 65237 (0x00FED5)。
    /// 数据区固定 8 字节：
    /// byte1~3 为被请求的 PGN（18bit，小端），byte4 保留 0xFF，byte5~8 保留 0xFF。
    /// </summary>
    public class Msg_DM13
    {
        /// <summary>目标 PGN</summary>
        public const UInt32 PGN = J1939Pgn.DM13;

        /// <summary>被请求停止/开始广播的 PGN（18bit）</summary>
        public UInt32 RequestPgn = 0;

        /// <summary>目标地址 DA，0xFF 为广播</summary>
        public byte DestinationAddress = J1939Addr.Global;

        /// <summary>源地址 SA</summary>
        public byte SourceAddress = 0;

        /// <summary>优先级，DM13 固定为 6</summary>
        public int Priority = 6;

        public Msg_DM13() { }

        public Msg_DM13(UInt32 requestPgn, byte da = J1939Addr.Global, byte sa = 0)
        {
            RequestPgn = requestPgn;
            DestinationAddress = da;
            SourceAddress = sa;
        }

        /// <summary>构造 29bit ID</summary>
        public J1939Id ToId()
        {
            return new J1939Id
            {
                Priority = Priority,
                PF = 0xFE,
                PS = DestinationAddress,
                SourceAddress = SourceAddress,
            };
        }

        /// <summary>序列化为 8 字节数据区</summary>
        public byte[] ToData()
        {
            var bs = new byte[8] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            bs[0] = (byte)RequestPgn;
            bs[1] = (byte)(RequestPgn >> 8);
            bs[2] = (byte)(RequestPgn >> 16);
            return bs;
        }

        /// <summary>转为 CAN 帧</summary>
        public CanMsg ToCanMsg()
        {
            return ToId().ToCanMsg(ToData());
        }

        /// <summary>从 CAN 帧解析</summary>
        public static Msg_DM13 Parse(CanMsg msg)
        {
            if (msg == null || msg.Data == null || msg.Data.Length < 3) return null;

            var id = J1939Id.FromCanMsg(msg);
            return new Msg_DM13
            {
                RequestPgn = (UInt32)(msg.Data[0] | (msg.Data[1] << 8) | (msg.Data[2] << 16)),
                DestinationAddress = id.DestinationAddress,
                SourceAddress = id.SourceAddress,
                Priority = id.Priority,
            };
        }

        public override String ToString()
        {
            return $"DM13 PGN=0x{RequestPgn:X5} DA=0x{DestinationAddress:X2} SA=0x{SourceAddress:X2}";
        }
    }
}
