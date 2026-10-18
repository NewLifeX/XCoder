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
    /// 应答控制字节
    /// </summary>
    public enum AckControl_e : byte
    {
        /// <summary>肯定应答 ACK</summary>
        Ack = 0,

        /// <summary>否定应答 NACK</summary>
        Nack = 1,

        /// <summary>拒绝访问 Denied / Access Denied</summary>
        AccessDenied = 2,

        /// <summary>无法应答 Cannot Respond</summary>
        CannotRespond = 3,
    }

    /// <summary>
    /// 应答 Acknowledgement，PGN 59392 (0x00E800)。
    /// 数据区 8 字节：
    /// byte1 控制字节、byte2 被应答消息的组功能值(PF)、byte3 被应答消息的地址、
    /// byte4~7 被应答消息的 PGN（18bit 小端，高位补 0）。
    /// </summary>
    public class Msg_Acknowledge
    {
        /// <summary>目标 PGN</summary>
        public const UInt32 PGN = J1939Pgn.Acknowledge;

        /// <summary>控制字节</summary>
        public AckControl_e Control = AckControl_e.Ack;

        /// <summary>被应答消息的组功能值（PF）</summary>
        public byte GroupFunction = 0;

        /// <summary>被应答消息的地址。PDU1 为 DA，PDU2 为 0xFF</summary>
        public byte AddressAcknowledged = J1939Addr.Global;

        /// <summary>被应答消息的 PGN（18bit）</summary>
        public UInt32 AcknowledgedPgn = 0;

        /// <summary>目标地址 DA，0xFF 表示广播</summary>
        public byte DestinationAddress = J1939Addr.Global;

        /// <summary>源地址 SA</summary>
        public byte SourceAddress = 0;

        /// <summary>优先级，应答固定为 6</summary>
        public int Priority = 6;

        public Msg_Acknowledge() { }

        public Msg_Acknowledge(AckControl_e control, UInt32 ackPgn, byte sa = 0)
        {
            Control = control;
            AcknowledgedPgn = ackPgn;
            GroupFunction = (byte)((ackPgn >> 8) & 0xFF);
            // PDU1 消息的 PF < 240，其地址为 DA；PDU2 用 0xFF 填充
            AddressAcknowledged = GroupFunction < 240 ? (byte)0x00 : J1939Addr.Global;
            SourceAddress = sa;
        }

        /// <summary>构造 29bit ID</summary>
        public J1939Id ToId()
        {
            return new J1939Id
            {
                Priority = Priority,
                PF = 0xE8,
                PS = DestinationAddress,
                SourceAddress = SourceAddress,
            };
        }

        /// <summary>序列化为 8 字节数据区</summary>
        public byte[] ToData()
        {
            var bs = new byte[8];
            bs[0] = (byte)Control;
            bs[1] = GroupFunction;
            bs[2] = AddressAcknowledged;
            bs[3] = (byte)AcknowledgedPgn;
            bs[4] = (byte)(AcknowledgedPgn >> 8);
            bs[5] = (byte)(AcknowledgedPgn >> 16);
            bs[6] = 0;
            bs[7] = 0;
            return bs;
        }

        /// <summary>转为 CAN 帧</summary>
        public CanMsg ToCanMsg()
        {
            return ToId().ToCanMsg(ToData());
        }

        /// <summary>从 CAN 帧解析</summary>
        public static Msg_Acknowledge Parse(CanMsg msg)
        {
            if (msg == null || msg.Data == null || msg.Data.Length < 8) return null;

            var id = J1939Id.FromCanMsg(msg);
            var d = msg.Data;
            var pgn = (UInt32)(d[3] | (d[4] << 8) | (d[5] << 16));

            return new Msg_Acknowledge
            {
                Control = (AckControl_e)d[0],
                GroupFunction = d[1],
                AddressAcknowledged = d[2],
                AcknowledgedPgn = pgn,
                DestinationAddress = id.DestinationAddress,
                SourceAddress = id.SourceAddress,
                Priority = id.Priority,
            };
        }

        public override String ToString()
        {
            return $"ACK {Control} PGN=0x{AcknowledgedPgn:X5} PF=0x{GroupFunction:X2} DA=0x{DestinationAddress:X2} SA=0x{SourceAddress:X2}";
        }
    }
}
