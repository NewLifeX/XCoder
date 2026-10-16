
using ALib.Can;
using ALib.CAN;
using NewLife;
using NewLife.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALib.CanOpen
{
    /// <summary>
    /// 发送 cobid = 0x600+nodeid, byte0 = 0x40
    /// 接收 cobid = 0x580+nodeid
    /// </summary>
    public class SDO_ReadPacket : CanPacket
    {
        #region byte 0

        /// <summary>第一个字节，控制字节</summary>
        public byte Byte0 = 0;

        #endregion

        #region byte 1 2 3

        /// <summary>索引, ushort 类型</summary>
        public int Index = 0;
        /// <summary>子索引, byte 类型</summary>
        public int SubIndex = 0;

        #endregion

        #region 两种负载

        /// <summary>单包负载</summary>
        public byte[] Payload = new byte[0];
        /// <summary>TransferTypeFast=false,SizeIndicator=true。
        /// 给定大小，后续数据包负责传输</summary>
        public int Length = 0;

        #endregion

        public SDO_ReadPacket() { }

        public SDO_ReadPacket(int nodId, int index, int subidx)
        {
            COB_ID = 0x600 + nodId;
            Byte0 = 0x40;

            Index = index;
            SubIndex = subidx;
        }

        /// <summary>读操作，分片传输的中间ACK（客户端发送）</summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public void SDO_ReadContinue(bool t)
        {
            // xCs=3  T=t
            Byte0 = (byte)(t ? 0x60 : 0x70);
        }

        /// <summary>下一个包</summary>
        /// <returns></returns>
        public SDO_ReadPacket NextPkt()
        {
            if (IsEndPacket) return null;
            if (Error) return null;

            var pkt = new SDO_ReadPacket(Node_Id, Index, SubIndex);

            if (Byte0 == 0x41)
            {
                // 第一个 60/70 包是  60
                pkt.Byte0 = 0x60;
            }
            else
            {
                // T bit4 需要翻转
                if ((Byte0 & 0x10) == 0) pkt.Byte0 = 0x70;
                else pkt.Byte0 = 0x60;
            }

            pkt.Func = 0x600 >> 7;
            pkt.Node_Id = Node_Id;
            pkt.RTR = RTR;

            return pkt;
        }

        /// <summary>这是结尾包</summary>
        public bool IsEndPacket = false;
        /// <summary>出错,错误结束，数据无效</summary>
        public bool Error = false;

        /// <summary>错误码</summary>
        public UInt32 ErrCode = 0;


        protected override void OnReadPkt(CanPacket pkt)
        {
            // 判断 COB_ID 是不是 580 基底
            if (Func != 0xB)
            {
                IsEndPacket = true;
                Error = true;

                return;
            }

            var byte0 = Data[0];
            Byte0 = byte0;
            if ((byte0 & 0x43) == 0x43)
            {
                // byte0 = 43 + (BytesNotData << 2)
                // 快速包。   负载最大 4 byte
                var BytesNotData = (byte0 >> 2) & 0x03;
                Payload = Data.ReadBytes(4, 4 - BytesNotData);
                IsEndPacket = true;
            }
            else if (byte0 == 0x41)
            {
                // 分片数据包,首包。   负载为数据长度。
                Length = Data.ReadBytes(4, 4).ToInt();
            }
            else if ((byte0 & 0xef) == 0)
            {
                // byte0 =  00 or 10
                // 分片数据包,后续包。   负载 7 字节。
                Payload = Data.ReadBytes(1, 7);
            }
            else if ((byte0 & 0xe1) == 0x01)
            {
                // byte0 = 01 or 11  +  (bytesNotData << 1)
                // 分片数据包,尾包。    负载最大 7 字节。
                var bytesNotData = (byte0 >> 1) & 0x07;
                Payload = Data.ReadBytes(1, 7 - bytesNotData);
                IsEndPacket = true;
            }
            else if (byte0 == 0x80)
            {
                // 错误数据，有错误码可以提取
                ErrCode = Data.ReadBytes(4, 4).ToUInt32();
                IsEndPacket = true;
                Error = true;
            }
            else
            {
                IsEndPacket = true;
                Error = true;
            }
        }

        protected override void WritePkt()
        {
            byte _byte0 = Byte0;

            // 60   70  读数据的 ACK 包
            if ((_byte0 == 0x60) || (_byte0 == 0x70))
            {
                Data = new byte[8];
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
            ms.Write(new byte[4]);

            Data = ms.ToArray();
            base.WritePkt();
        }
    }

    /*
    用法示例

    var reader = new SDO_Read(_Port, 2);

    // 12字节负载，普通读
    // var data = reader.Read(0x100b, 0, 500);

    // 4字节负载，快速读
    // var data = reader.Read(0x100a, 3, 500);

    WriteLog(data?.ToHex());

    */

    /// <summary>SDO 读数据，常规模式/快速模式</summary>
    public class SDO_Read : ICanRx
    {
        ICanPort _Port;

        public int Node_Id;
        /// <summary>索引, ushort 类型</summary>
        public int Index = 0;
        /// <summary>子索引, byte 类型</summary>
        public int SubIndex = 0;

        public SDO_Read(ICanPort port, int nodId)
        {
            _Port = port;
            Node_Id = nodId;
        }

        /// <summary>收数据流</summary>
        public MemoryStream ObjMs = new MemoryStream(0);

        public enum State_e
        {
            /// <summary>初始化</summary>
            Init = 0,
            /// <summary>工作中</summary>
            Working = 1,
            /// <summary>完成</summary>
            OK = 6,
            /// <summary>超时失败</summary>
            Timeout = 7,
            /// <summary>错误</summary>
            Error = 8,
        }

        /// <summary>状态</summary>
        public State_e State = State_e.Init;

        public byte[] Read(int idx, int subidx, int timeout)
        {
            Index = idx;
            SubIndex = subidx;

            if (_Port == null)
            { 
                State = State_e.Error;
                return new byte[0];
            }

            // 发送启动包
            {
                var pkt = new SDO_ReadPacket(Node_Id, Index, subidx);
                _Port?.Send(pkt.ToCanMsg());
                State = State_e.Working;
            }

            // 等待回复
            _Port?.Register(this);

            var tend = DateTime.Now.AddMilliseconds(timeout);
            while (true)
            {
                Task.Delay(5).Wait();

                if (State != State_e.Working) break;

                // 超时判断
                if (DateTime.Now > tend)
                {
                    State = State_e.Timeout;
                    break;
                }
            }

            _Port?.Unregister(this);

            if (State == State_e.OK) return ObjMs.ToArray();
            else return new byte[0];
        }

        public override bool Process(CanMsg cmsg, ICanPort port)
        {
            var msg = new CanPacket(cmsg);
            // base.Process(msg, port);
            var pkt = msg.ToPacket<SDO_ReadPacket>();
            if (pkt.Error)
            {
                State = State_e.Error;
                return true;
            }

            if (pkt.IsEndPacket)
            {
                if (ObjMs == null)
                {
                    ObjMs = new MemoryStream(pkt.Payload.Length);
                    ObjMs.Write(pkt.Payload);
                }
                else
                {
                    ObjMs.Write(pkt.Payload);
                }

                State = State_e.OK;
                return true;
            }
            else
            {
                if (pkt.Length > 0)
                {
                    ObjMs = new MemoryStream(pkt.Length);
                }
                else
                {
                    if (ObjMs == null)
                    {
                        State = State_e.Error;
                        return true;
                    }

                    ObjMs.Write(pkt.Payload);
                }

                port.Send(pkt.NextPkt().ToCanMsg());
                return true;
            }
        }


        #region 日志

        /// <summary>日志</summary>
        public ILog Log { get; set; } = Logger.Null;

        /// <summary>写日志</summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteLog(String format, params Object[] args) => Log?.Info($"SDO_Read {format}", args);

        #endregion
    }
}
