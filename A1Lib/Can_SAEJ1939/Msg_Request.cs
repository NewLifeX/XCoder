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
    /// 请求 Request，PGN 59904 (0x00EA00)。
    /// 数据区恒定 3 字节 = 被请求的 PGN（18bit，小端）。
    /// 目标地址为 0xFF 时向全网广播请求，否则点对点请求某个 SA。
    /// </summary>
    public class Msg_Request
    {
        /// <summary>目标 PGN</summary>
        public const UInt32 PGN = J1939Pgn.Request;

        /// <summary>被请求的 PGN（18bit）</summary>
        public UInt32 RequestPgn = 0;

        /// <summary>目标地址 DA，0xFF 为广播</summary>
        public byte DestinationAddress = J1939Addr.Global;

        /// <summary>源地址 SA</summary>
        public byte SourceAddress = 0;

        /// <summary>优先级，请求固定为 6</summary>
        public int Priority = 6;

        public Msg_Request() { }

        public Msg_Request(UInt32 requestPgn, byte da = J1939Addr.Global, byte sa = 0)
        {
            RequestPgn = requestPgn;
            DestinationAddress = da;
            SourceAddress = sa;
        }

        /// <summary>构造 29bit ID（PDU1，PS 为目标地址）</summary>
        public J1939Id ToId()
        {
            return new J1939Id
            {
                Priority = Priority,
                PF = 0xEA,
                PS = DestinationAddress,
                SourceAddress = SourceAddress,
            };
        }

        /// <summary>序列化为 3 字节数据区</summary>
        public byte[] ToData()
        {
            var bs = new byte[3];
            bs[0] = (byte)RequestPgn;
            bs[1] = (byte)(RequestPgn >> 8);
            bs[2] = (byte)(RequestPgn >> 16);
            return bs;
        }

        /// <summary>转为 CAN 帧</summary>
        public CanMsg ToCanMsg()
        {
            var msg = ToId().ToCanMsg(ToData());
            // 请求报文长度固定 3 字节
            msg.Data = ToData();
            return msg;
        }

        /// <summary>从 CAN 帧解析</summary>
        public static Msg_Request Parse(CanMsg msg)
        {
            if (msg == null || msg.Data == null || msg.Data.Length < 3) return null;

            var id = J1939Id.FromCanMsg(msg);
            var pgn = (UInt32)(msg.Data[0] | (msg.Data[1] << 8) | (msg.Data[2] << 16));

            return new Msg_Request
            {
                RequestPgn = pgn,
                DestinationAddress = id.DestinationAddress,
                SourceAddress = id.SourceAddress,
                Priority = id.Priority,
            };
        }

        public override String ToString()
        {
            return $"Request PGN=0x{RequestPgn:X5} DA=0x{DestinationAddress:X2} SA=0x{SourceAddress:X2}";
        }
    }
}
