using ALib.Can;
using NewLife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ALib.CanOpen
{
    public class SDO_Base : CanPacket
    {
        #region byte 0

        /// <summary>数据包类型</summary>
        public enum xCs_e
        {
            Error = 4,
            /// <summary>向SDO上传附带数据(快速)。 发送2，应答2</summary>
            UploadExpedited = 2,
            /// <summary>向SDO上传 常规模式。 发送3，应答0</summary>
            UploadNormal = 3,
            /// <summary>SDO应答 常规模式。 发送3，应答0</summary>
            UploadRes = 0,
        }

        /// <summary>bit7 6 5。 xCs_e </summary>
        public int xCs = 1;
        /// <summary>bit4 。翻转位，根上一个数据包相关，收发需要一致</summary>
        public bool T = false;

        /// <summary>bit 2,3 。值 = 4 - Payload.Length </summary>
        public int BytesNotData = 0;
        /// <summary>bit1。 传输类型, 正常(后续包附带) / 加速(本数据包携带)</summary>
        public bool TransferTypeFast = false;
        /// <summary>bit0。 包内含 Payload 数据</summary>
        public bool SizeIndicator = false;

        /// <summary>处理 byte0</summary>
        /// <returns></returns>
        protected byte Byte0
        {
            get
            {
                int _byte0 = 0;
                _byte0 |= (xCs << 5);
                if (T) _byte0 |= 0x10;
                // if (TransferTypeFast) 
                _byte0 |= ((BytesNotData & 3) << 2);
                if (TransferTypeFast) _byte0 |= 0x02;
                if (SizeIndicator) _byte0 |= 0x01;

                return (byte)_byte0;
            }
            set
            {
                xCs = value >> 5;
                T = (value & 0x10) > 0;
                BytesNotData = (value >> 2) & 3;
                TransferTypeFast = (value & 2) > 0;
                SizeIndicator = (value & 1) > 0;
            }
        }

        #endregion

        #region byte 1 2 3

        /// <summary>索引, ushort 类型</summary>
        public int Index = 0;
        /// <summary>子索引, byte 类型</summary>
        public int SubIndex = 0;

        #endregion

        #region other

        /// <summary>TransferTypeFast=true。 
        /// 负载数据,最大4字节</summary>
        public byte[] Payload = new byte[4];
        /// <summary>TransferTypeFast=false,SizeIndicator=true。
        /// 给定大小，后续数据包负责传输</summary>
        public int Length;

        #endregion

        protected override void OnReadPkt(CanPacket pkt)
        {
            Byte0 = Data[0];
            Index = Data[2] * 256 + Data[1];
            SubIndex = Data[3];

            // if (TransferTypeFast && SizeIndicator)
            // {
            //     Payload = Data.ReadBytes(4, 4 - BytesNotData);
            // }
            // if (SizeIndicator && (!TransferTypeFast))
            // {
            //     Length = Data.ReadBytes(4, 4).ToInt();
            // }
        }

        protected override void WritePkt()
        {
            byte _byte0 = Byte0;

            // 60   70  读数据的 ACK 包
            if ((_byte0 == 0x60) || (_byte0 == 0x70))
            {
                Data = new byte[4];
                Data[0] = _byte0;

                base.WritePkt();
                return;
            }

            // 正常传输内容
            var ms = new MemoryStream(8);
            ms.WriteByte(_byte0);
            ms.WriteByte((byte)Index);
            ms.WriteByte((byte)(Index >> 8));
            ms.WriteByte((byte)(SubIndex));
            if (TransferTypeFast && SizeIndicator)
            {
                if (BytesNotData < 4) { ms.Write(Payload.ReadBytes(0, 4 - BytesNotData)); }
            }

            Data = ms.ToArray();
            base.WritePkt();
        }


        /// <summary>启动SDO读</summary>
        /// <param name="nodId"></param>
        /// <param name="index"></param>
        /// <param name="subidx"></param>
        /// <returns></returns>
        public static SDO_Base SDO_ReadInit(int nodId, int index, int subidx)
        {
            var pkt = new SDO_Base();
            pkt.COB_ID = 0x600 + nodId;
            // xCs=1
            pkt.Byte0 = 0x40;
            pkt.Index = index;
            pkt.SubIndex = subidx;

            return pkt;
        }

        /// <summary>读操作，分片传输的中间ACK（客户端发送）</summary>
        /// <param name="nodId"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static SDO_Base SDO_ReadContinue(int nodId, bool t)
        {
            var pkt = new SDO_Base();

            pkt.COB_ID = 0x600 + nodId;
            // xCs=3  T=t
            pkt.Byte0 = (byte)(t ? 0x60 : 0x70);
            return pkt;
        }
    }
}
