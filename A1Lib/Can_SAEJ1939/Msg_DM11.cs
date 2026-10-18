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
    /// DM11 清除活动故障码，PGN 65235 (0x00FED3)。
    /// 数据区为空（DLC = 0），用于请求清空当前活动故障码。
    /// </summary>
    public class Msg_DM11
    {
        /// <summary>目标 PGN</summary>
        public const UInt32 PGN = J1939Pgn.DM11;

        /// <summary>目标地址 DA，0xFF 为广播</summary>
        public byte DestinationAddress = J1939Addr.Global;

        /// <summary>源地址 SA</summary>
        public byte SourceAddress = 0;

        /// <summary>优先级，DM11 固定为 6</summary>
        public int Priority = 6;

        public Msg_DM11() { }

        public Msg_DM11(byte da = J1939Addr.Global, byte sa = 0)
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
        public static Msg_DM11 Parse(CanMsg msg)
        {
            if (msg == null) return null;

            var id = J1939Id.FromCanMsg(msg);
            return new Msg_DM11
            {
                DestinationAddress = id.DestinationAddress,
                SourceAddress = id.SourceAddress,
                Priority = id.Priority,
            };
        }

        public override String ToString()
        {
            return $"DM11 清除活动故障 DA=0x{DestinationAddress:X2} SA=0x{SourceAddress:X2}";
        }
    }
}
