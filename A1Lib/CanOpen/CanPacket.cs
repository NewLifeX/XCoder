using ALib.CAN;
using NewLife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALib.Can
{
    /// <summary>Can报文格式</summary>
    public class CanPacket
    {
        /// <summary>功能码,4bit</summary>
        public byte Func;
        /// <summary>节点ID  7bit</summary>
        public int Node_Id = 0;

        public int COB_ID
        {
            get { return ((int)Func << 7) + Node_Id; }
            set { Func = (byte)(value >> 7); Node_Id = value & 0x7f; }
        }

        /// <summary>远程帧，默认false</summary>
        public bool RTR = false;
        /// <summary>报文长度</summary>
        public int DLC => Data.Length;
        /// <summary>负载数据</summary>
        public byte[] Data = new byte[0];

        public CanPacket() { }

        public CanPacket(int cobId, byte[] py)
        {
            COB_ID = cobId;

            RTR = false;
            if (py.Length > 8) Data = py.ReadBytes(0, 8);
            else Data = py;
        }

        public CanPacket(byte func, int nodeId, byte[] py)
        {
            Func = func;
            Node_Id = nodeId;

            RTR = false;
            if (py.Length > 8) Data = py.ReadBytes(0, 8);
            else Data = py;
        }

        public CanPacket(CanMsg msg) 
        {
            COB_ID = msg.ID;
            RTR = msg.RemoteFlag;
            Data = msg.Data;               
        }

        protected virtual void OnReadPkt(CanPacket pkt) { }

        /// <summary>copy 内容，子类拿数据用</summary>
        /// <param name="pkt"></param>
        public void ReadPkt(CanPacket pkt)
        {
            this.Func = pkt.Func;
            this.Node_Id = pkt.Node_Id;

            this.RTR = pkt.RTR;
            this.Data = (byte[])pkt.Data.Clone();

            OnReadPkt(pkt);
        }

        protected virtual void WritePkt() { }

        /// <summary>转化为子类，具体类型的数据包</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ToPacket<T>() where T : CanPacket, new()
        {
            var obj = new T();
            obj.ReadPkt(this);
            return obj;
        }

        /// <summary>序列化为驱动要求的包，用于发送数据（会先按子类规则填充 Data）</summary>
        /// <returns></returns>
        public CanMsg ToCanMsg()
        {
            // 必须先 WritePkt()，否则 Data 为空（默认 byte[0]），会发出 DLC=0 的空帧
            WritePkt();

            return new CanMsg(COB_ID, Data, RTR, false);
        }
    }
}
