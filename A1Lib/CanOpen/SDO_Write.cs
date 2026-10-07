
using ALib.Can;
using ALib.CAN;
using NewLife;
using NewLife.Log;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ALib.CanOpen
{
    public class SDO_WritePacket : CanPacket
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

        #region 负载

        /// <summary>总负载数据，可能需要分包</summary>
        public MemoryStream ObjMs = null;

        #endregion


        public SDO_WritePacket() { }

        public SDO_WritePacket(int nodId, int index, int subidx, byte[] data)
        {
            COB_ID = 0x600 + nodId;
            Byte0 = 0x21;

            Index = index;
            SubIndex = subidx;

            if (data != null) ObjMs = new MemoryStream(data);
        }

        public bool First()
        {
            if (ObjMs.Capacity <= 4)
            {
                int len = ObjMs.Capacity;
                int bytesNotData = 4 - len;
                Byte0 = (byte)(0x23 + (bytesNotData << 2));

                Data = new byte[8];
                var ms = new MemoryStream(Data);
                ms.Write(Byte0);
                ms.WriteByte((byte)Index);
                ms.WriteByte((byte)(Index >> 8));
                ms.WriteByte((byte)(SubIndex));
                ms.Write(ObjMs.ReadBytes(len));
            }
            else
            {
                int len = ObjMs.Capacity;
                int bytesNotData = 4 - len;
                Byte0 = (byte)0x21;

                Data = new byte[8];
                var ms = new MemoryStream(Data);
                ms.Write(Byte0);
                ms.WriteByte((byte)Index);
                ms.WriteByte((byte)(Index >> 8));
                ms.WriteByte((byte)(SubIndex));
                ms.Write(len.GetBytes());
            }

            return true;
        }

        private bool T;

        public bool Continue(bool t)
        {
            T = t;
            var remian = ObjMs.Capacity - ObjMs.Position;
            if (remian < 1) return false;

            if (remian > 7)
            {
                Byte0 = (byte)(t ? 0x10 : 0x00);

                Data = new byte[8];
                var ms = new MemoryStream(Data);
                ms.WriteByte(Byte0);
                ms.Write(ObjMs.ReadBytes(7));
            }
            else
            {
                int bytesNotData = (int)(7 - remian);
                Byte0 = 0x01;
                Byte0 += (byte)(bytesNotData << 1);
                if (t) Byte0 += 0x10;

                Data = new byte[8];
                var ms = new MemoryStream(Data);
                ms.WriteByte(Byte0);
                ms.Write(ObjMs.ReadBytes(remian));
            }

            return true;
        }

        /// <summary>出错,错误结束，数据无效</summary>
        public bool Error = false;
        /// <summary>错误码</summary>
        public Int32 ErrCode = 0;
        /// <summary>这是结尾包</summary>
        public bool IsEndPacket = false;

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

            if (byte0 == 0x80)
            {
                // 快速包，异常
                // 分段包，异常
                Error = true;
                ErrCode = Data.ReadBytes(4, 4).ToInt();
                IsEndPacket = true;
                return;
            }
            else if (byte0 == 0x60)
            {
                // 快速包，结束
                IsEndPacket = true;
                return;
            }
            else if ((byte0 & 0xef) == 0x20)
            {
                // 0x20  0x30
                // 分段包，中间的包,结尾的包
            }
            else
            {
                IsEndPacket = true;
                Error = true;
            }
        }
    }

    public class SDO_Write : ICanRx
    {
        ICanPort _Port;

        public int Node_Id;
        /// <summary>索引, ushort 类型</summary>
        public int Index = 0;
        /// <summary>子索引, byte 类型</summary>
        public int SubIndex = 0;

        private SDO_WritePacket Packet;

        private bool T = false;

        public SDO_Write(ICanPort port, int nodId)
        {
            _Port = port;
            Node_Id = nodId;

            CanIdMask = -1;
            CanId = 0x580 + nodId;
        }

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

        public bool Write(int idx, int subidx, byte[] data, int timeout)
        {
            Index = idx;
            SubIndex = subidx;

            if (_Port == null)
            {
                State = State_e.Error;
                return false;
            }

            var pkt = new SDO_WritePacket(Node_Id, Index, subidx, data);
            Packet = pkt;

            // 等待回复
            _Port?.Register(this);
            // 发送启动包
            {
                pkt.First();
                _Port?.Send(pkt.ToCanMsg());
                State = State_e.Working;
            }


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
            return State == State_e.OK;
        }


        public override bool Process(CanMsg cmsg, ICanPort port)
        {
            var msg = new CanPacket(cmsg);
            // base.Process(msg, port);
            var pkt = msg.ToPacket<SDO_WritePacket>();
            if (pkt.Error)
            {
                State = State_e.Error;
                return true;
            }

            if (!Packet.Continue(T))
            {
                State = State_e.OK;
                return true;
            }

            T = !T;
            _Port?.Send(Packet.ToCanMsg());
            return true;
        }


        #region 日志

        /// <summary>日志</summary>
        public ILog Log { get; set; } = Logger.Null;

        /// <summary>写日志</summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteLog(String format, params Object[] args) => Log?.Info($"Upgrade {format}", args);

        #endregion
    }
}
