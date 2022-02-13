using System;
using System.IO;
using System.Runtime.Serialization;
using NewLife.Data;
using NewLife.Serialization;

namespace NewLife.IoT.Protocols
{
    public class ModbusMessage : IAccessor
    {
        #region 属性
        /// <summary>是否响应</summary>
        [IgnoreDataMember]
        public Boolean Reply { get; set; }

        /// <summary>事务编号</summary>
        public UInt16 TransactionId { get; set; }

        /// <summary>协议</summary>
        public UInt16 ProtocolId { get; set; }

        /// <summary>站号</summary>
        public Byte Host { get; set; }

        /// <summary>操作码</summary>
        public FunctionCodes Code { get; set; }

        /// <summary>地址。请求中使用</summary>
        public UInt16 Address { get; set; }

        /// <summary>个数</summary>
        public UInt16 Count { get; set; }

        /// <summary>负载数据。响应中使用</summary>
        [IgnoreDataMember]
        public Packet Payload { get; set; }
        #endregion

        #region 构造
        public override String ToString()
        {
            if (!Reply)
            {
                if (Payload == null)
                    return $"{Code} ({Address}, {Count})";
                else
                    return $"{Code} ({Address}={Payload?.ToHex()})";
            }
            else
                return $"{Code} {Payload?.ToHex()}";
        }
        #endregion

        #region 方法
        /// <summary>读取</summary>
        /// <param name="stream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Boolean Read(Stream stream, Object context)
        {
            var binary = context as Binary ?? new Binary { Stream = stream, IsLittleEndian = false };

            TransactionId = binary.Read<UInt16>();
            ProtocolId = binary.Read<UInt16>();
            var len = binary.Read<UInt16>();
            if (len < 1 + 1 + 1) return false;

            Host = binary.ReadByte();
            Code = (FunctionCodes)binary.ReadByte();

            len -= 2;
            if (!Reply)
            {
                Address = binary.Read<UInt16>();
                if (len == 4)
                    Count = binary.Read<UInt16>();
                else
                    Payload = binary.ReadBytes(len - 2);
            }
            else if (len >= 1)
            {
                var len2 = binary.ReadByte();
                if (len2 <= len - 1) Payload = binary.ReadBytes(len2);
            }

            return true;
        }

        /// <summary>解析消息</summary>
        /// <param name="pk"></param>
        /// <returns></returns>
        public static ModbusMessage Read(Packet pk, Boolean reply = false)
        {
            var msg = new ModbusMessage { Reply = reply };
            if (msg.Read(pk.GetStream(), null)) return msg;

            return null;
        }

        /// <summary>写入消息到数据流</summary>
        /// <param name="stream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Boolean Write(Stream stream, Object context)
        {
            var binary = context as Binary ?? new Binary { Stream = stream, IsLittleEndian = false };

            binary.Write(TransactionId);
            binary.Write(ProtocolId);

            var pk = Payload;
            var len = 1 + 1;
            if (!Reply)
                len += 2 + (pk?.Total ?? 2);
            else
                len += 1 + (pk?.Total ?? 0);
            binary.Write((UInt16)len);

            binary.Write(Host);
            binary.Write((Byte)Code);

            if (!Reply)
            {
                binary.Write(Address);
                if (pk != null)
                    binary.Write(pk.Data, pk.Offset, pk.Count);
                else
                    binary.Write(Count);
            }
            else
            {
                var len2 = (pk?.Total ?? 0);
                binary.Write((Byte)len2);
                if (pk != null)
                    binary.Write(pk.Data, pk.Offset, pk.Count);
            }

            return true;
        }

        /// <summary>消息转数据包</summary>
        /// <returns></returns>
        public Packet ToPacket()
        {
            var ms = new MemoryStream();
            Write(ms, null);

            ms.Position = 0;
            return new Packet(ms);
        }

        public ModbusMessage CreateReply()
        {
            if (Reply) throw new InvalidOperationException();

            var msg = new ModbusMessage
            {
                Reply = true,
                TransactionId = TransactionId,
                ProtocolId = ProtocolId,
                Host = Host,
                Code = Code,
            };

            return msg;
        }
        #endregion
    }
}