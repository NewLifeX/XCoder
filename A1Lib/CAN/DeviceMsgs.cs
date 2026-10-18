using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ALib.CAN
{
    public class MsgBitsTag
    {
        #region

        public int CanId
        {
            get { return (int)(_tag >> 32); }
            set
            {
                UInt64 mask = 0x0000_0000_ffff_ffff;
                _tag &= mask;
                _tag |= ((UInt64)value << 32);
            }
        }

        public byte BitOffset
        {
            get { return (byte)(_tag >> 24); }
            set
            {
                UInt64 mask = 0xffff_ffff_00ff_ffff;
                _tag &= mask;
                _tag |= ((UInt64)value << 24);
            }
        }

        public byte BitLenght
        {
            get { return (byte)(_tag >> 16); }
            set
            {
                UInt64 mask = 0xffff_ffff_ff00_ffff;
                _tag &= mask;
                _tag |= ((UInt64)value << 16);
            }
        }

        public ushort Other
        {
            get { return (ushort)(_tag); }
            set
            {
                UInt64 mask = 0xffff_ffff_ffff_0000;
                _tag &= mask;
                _tag |= ((UInt64)value);
            }
        }

        private UInt64 _tag;

        public UInt64 Tag { get { return _tag; } set { _tag = value; } }

        #endregion

        public MsgBitsTag(UInt64 tag) { Tag = tag; }

        public MsgBitsTag(int canid, byte offset, byte len, ushort other = 0)
        {
            // Tag = 0;
            CanId = canid;
            BitOffset = offset;
            BitLenght = len;
            Other = other;
        }

        /// <summary>从消息获取数据</summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public UInt64 GetDataFrom(UInt64 msg)
        {
            msg >>= BitOffset;
            var mask = ~(UInt64.MaxValue << BitLenght);

            return msg & mask;
        }

        /// <summary>设置数据到小消息</summary>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public UInt64 SetDataTo(UInt64 msg, UInt64 data)
        {
            // cnt 位宽掩码（低 cnt 位为 1）
            UInt64 bitMask = ~(UInt64.MaxValue << BitLenght);

            // 清掉目标位区间，再写入新值
            msg &= ~(bitMask << BitOffset);
            msg |= ((UInt64)data & bitMask) << BitOffset;

            return msg;
        }
    }

    /// <summary>设备消息汇总</summary>
    public class DeviceMsgs
    {
        /// <summary>Msgs对应的canid</summary>
        public int[] CandIds { get; private set; }

        /// <summary>canid,data</summary>
        private Dictionary<Int32, UInt64> Msgs;

        /// <summary>构造</summary>
        public DeviceMsgs(int[] canIds)
        {
            CandIds = canIds;

            Msgs = new Dictionary<Int32, UInt64>(CandIds.Length);
            foreach (var id in CandIds) { Msgs.Add(id, 0); }
        }

        #region 整条消息读写

        /// <summary></summary>
        /// <param name="canid"></param>
        /// <param name="msg"></param>
        public void Update(Int32 canid, UInt64? msg)
        {
            if (msg == null) return;
            if (!CandIds.Contains(canid)) return;
            Msgs[canid] = msg.Value;
        }

        public void Update(Int32 canid, byte[] msg)
        {
            if (msg == null) return;
            if (msg.Length != 8) return;

            Update(canid, BitConverter.ToUInt64(msg));
        }

        public void Update(CanMsg msg) => Update(msg.ID, msg.Data);
        
        /// <summary>索引器：以 canid 直接获取/设置对应消息内容（不存在的 canid 返回 null）</summary>
        public UInt64? this[Int32 canid]
        {
            get => Msgs.TryGetValue(canid, out var data) ? data : null;
            set => Update(canid, value);
        }

        /// <summary>清空所有消息到0</summary>
        public void CleanAll()
        {
            foreach (var id in CandIds) { Update(id, 0); }
        }

        #endregion

        #region 消息内读写

        /// <summary>读取 tag 指定的位域数据，消息不存在返回 null</summary>
        /// <param name="tag">位域标签</param>
        /// <returns></returns>
        public UInt64? GetData(MsgBitsTag tag)
        {
            if (tag == null) return null;

            var msg = this[tag.CanId];
            if (msg == null) return null;

            return tag.GetDataFrom(msg.Value);
        }

        /// <summary>写入 tag 指定的位域数据，返回写入之前的原始值</summary>
        /// <param name="tag">位域标签</param>
        /// <param name="value">写入的值</param>
        /// <returns>返回写入之前的数值</returns>
        public UInt64 SetData(MsgBitsTag tag, UInt64 value)
        {
            if (tag == null) return 0;

            var oldVal = GetData(tag) ?? 0;

            var msg = this[tag.CanId] ?? 0;
            this[tag.CanId] = tag.SetDataTo(msg, value);

            return oldVal;
        }

        #endregion

        #region 比较

        /// <summary>逐条比较两个设备消息的所有 canid 数据是否完全一致，最后一字节seq不参与比较</summary>
        public static bool operator ==(DeviceMsgs a, DeviceMsgs b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;

            if (a.Msgs.Count != b.Msgs.Count) return false;

            foreach (var kvp in a.Msgs)
            {
                if (!b.Msgs.TryGetValue(kvp.Key, out var otherData)) return false;

                var data = kvp.Value << 8;
                var other = otherData << 8;

                if (data != other) return false;
            }

            return true;
        }

        /// <summary>逐条比较两个设备消息的所有 canid 数据是否完全一致，最后一字节seq不参与比较</summary>
        public static bool operator !=(DeviceMsgs a, DeviceMsgs b) { return !(a == b); }

        /// <summary>逐条比较两个设备消息的所有 canid 数据是否完全一致，最后一字节seq不参与比较</summary>
        public override bool Equals(object obj) { return obj is DeviceMsgs other && this == other; }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var kvp in Msgs)
            {
                hash.Add(kvp.Key);
                hash.Add(kvp.Value);
            }
            return hash.ToHashCode();
        }

        #endregion
    }
}
