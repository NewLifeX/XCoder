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
    /// 统一的固定结构消息解析入口。
    /// 依据 29bit ID 中的 PGN 判定消息类型并解析。
    /// </summary>
    public static class J1939Message
    {
        /// <summary>
        /// 尝试把 CAN 帧解析为对应的固定结构消息对象。
        /// 未知 PGN 返回 null。
        /// </summary>
        /// <param name="msg">CAN 帧</param>
        public static Object Parse(CanMsg msg)
        {
            if (msg == null) return null;

            var id = J1939Id.FromCanMsg(msg);
            var pgn = id.PGNValue;

            switch (pgn)
            {
                case J1939Pgn.AddressClaimed: return Msg_AddressClaimed.Parse(msg);
                case J1939Pgn.Request: return Msg_Request.Parse(msg);
                case J1939Pgn.Acknowledge: return Msg_Acknowledge.Parse(msg);
                case J1939Pgn.TpCm: return Msg_TpCm.Parse(msg);
                case J1939Pgn.TpDt: return Msg_TpDt.Parse(msg);
                case J1939Pgn.DM1:
                case J1939Pgn.DM2: return Msg_DM1.Parse(msg);
                case J1939Pgn.DM3: return Msg_DM3.Parse(msg);
                case J1939Pgn.DM11: return Msg_DM11.Parse(msg);
                case J1939Pgn.DM13: return Msg_DM13.Parse(msg);
                default: return null;
            }
        }

        /// <summary>是否为固定结构消息</summary>
        public static bool IsFixedMessage(UInt32 pgn)
        {
            switch (pgn)
            {
                case J1939Pgn.AddressClaimed:
                case J1939Pgn.Request:
                case J1939Pgn.Acknowledge:
                case J1939Pgn.TpCm:
                case J1939Pgn.TpDt:
                case J1939Pgn.DM1:
                case J1939Pgn.DM2:
                case J1939Pgn.DM3:
                case J1939Pgn.DM11:
                case J1939Pgn.DM13:
                    return true;
                default:
                    return false;
            }
        }
    }
}
