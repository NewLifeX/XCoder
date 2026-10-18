using NewLife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALib.CAN
{
    public class CanMsg
    {
        /// <summary>时间戳</summary>
        public TimeSpan TimeStamp;

        /// <summary>标准帧11bit，扩展帧29bit</summary>
        public Int32 ID;

        /// <summary>扩展帧标志</summary>
        public bool ExternFlag;

        /// <summary>RTR 远程帧标志</summary>
        public bool RemoteFlag;

        /// <summary>负载数据，最大8字节</summary>
        public byte[] Data = new byte[8];

        /// <summary>64位数据，用于一次读写 Data</summary>
        public UInt64 Data64
        {
            get { return BitConverter.ToUInt64(Data); }
            set { Data = value.GetBytes(); }
        }

        public int DataLen => Data.Length;

        public CanMsg(Int32 id, byte[] data, bool rtr, bool ext)
        {
            ID = id;
            Data = data;
            RemoteFlag = rtr;
            ExternFlag = ext;
        }

        public CanMsg(Int32 id, UInt64 data, bool rtr, bool ext)
        {
            ID = id;
            Data64 = data;
            RemoteFlag = rtr;
            ExternFlag = ext;
        }

        /// <summary>格式化字符串（输出格式与 yCanTool 保持一致）</summary>
        /// <returns></returns>
        public override String ToString()
        {
            var sb = new StringBuilder();

            sb.Append(TimeStamp.ToString("hh\\:mm\\:ss\\.fff"));

            // CANID，扩展帧 8 位十六进制，标准帧低 2 字节十六进制
            if (ExternFlag)
            {
                var id = ID.GetBytes(false).ReadBytes(0, 4).ToHex();
                sb.Append("  " + id + "  ");
            }
            else
            {
                var id = ID.GetBytes(false).ReadBytes(2, 2).ToHex();
                sb.Append("  " + id + " ");
            }

            var R = RemoteFlag ? "R " : "  ";
            sb.Append(R);

            var E = ExternFlag ? "E " : "  ";
            sb.Append(E);

            var data = Data;
            var len = data?.Length ?? 0;
            if (len > 8) len = 8;
            sb.Append("[" + len + "] ");
            if (data != null && len > 0)
            {
                sb.Append(" " + data.ReadBytes(0, len).ToHex(" "));
            }

            return sb.ToString();
        }
    }
}
