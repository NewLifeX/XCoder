using System;
using System.Net.Sockets;
using System.Threading;
using NewLife.Data;
using NewLife.Net;

namespace NewLife.IoT.Protocols
{
    /// <summary>ModbusTCP网口通信</summary>
    public class ModbusTcp : Modbus
    {
        #region 属性
        /// <summary>服务端地址。127.0.0.1:502</summary>
        public String Server { get; set; }

        /// <summary>协议标识。默认0</summary>
        public UInt16 ProtocolId { get; set; }

        /// <summary>缓冲区大小。默认1024</summary>
        public Int32 BufferSize { get; set; } = 1024;

        private Int32 _transactionId;
        private TcpClient _client;
        private NetworkStream _stream;
        #endregion

        #region 方法
        /// <summary>初始化。传入配置字符串</summary>
        /// <param name="config"></param>
        public override void Init(String config)
        {
            var ss = config.SplitAsDictionary("=", ";", true);
            if (ss.TryGetValue("Server", out var str))
                Server = str;
            else if (ss.TryGetValue("Address", out str))
                Server = str;

            if (ss.TryGetValue("ProtocolId", out str)) ProtocolId = (UInt16)str.ToInt();
        }

        /// <summary>打开</summary>
        public override void Open()
        {
            if (_client == null || !_client.Connected)
            {
                var uri = new NetUri(Server);
                if (uri.Port == 0) uri.Port = 502;

                var client = new TcpClient
                {
                    SendTimeout = 3_000,
                    ReceiveTimeout = 3_000
                };
                client.Connect(uri.Address, uri.Port);

                _client = client;
                _stream = client.GetStream();

                WriteLog("Open {0}:{1}", uri.Host, uri.Port);
            }
        }

        /// <summary>发送两字节命令，并接收返回</summary>
        /// <param name="host"></param>
        /// <param name="code"></param>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Byte[] SendCommand(Byte host, FunctionCodes code, UInt16 address, Object value)
        {
            Open();

            var tid = Interlocked.Increment(ref _transactionId);

            var msg = new ModbusMessage
            {
                TransactionId = (UInt16)tid,
                ProtocolId = ProtocolId,
                Host = host,
                Code = code,
                Address = address,
                //Count = value
            };
            if (value is UInt16 v)
                msg.Count = v;
            else if (value is Packet pk)
                msg.Payload = pk;
            else if (value is Byte[] buf)
                msg.Payload = buf;

            WriteLog("=> {0}", msg);
            var cmd = msg.ToPacket().ToArray();

            {
                using var span = Tracer?.NewSpan("modbus:SendCommand", cmd.ToHex());

                _stream.Write(cmd, 0, cmd.Length);
                Thread.Sleep(10);
            }

            using var span2 = Tracer?.NewSpan("modbus:ReceiveCommand");
            try
            {

                var buf = new Byte[BufferSize];
                var c = _stream.Read(buf, 0, buf.Length);
                buf = buf.ReadBytes(0, c);

                if (span2 != null) span2.Tag = buf.ToHex();

                var rs = ModbusMessage.Read((ArrayPacket)buf, true);
                if (rs == null) return null;

                WriteLog("<= {0}", rs);

                return rs.Payload?.ToArray();
            }
            catch (Exception ex)
            {
                span2?.SetError(ex, null);
                if (ex is TimeoutException) return null;
                throw;
            }
        }
        #endregion
    }
}