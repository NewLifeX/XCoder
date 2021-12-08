using System;
using System.Net.Sockets;
using System.Threading;
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
        public override Byte[] SendCommand(Byte host, Byte code, UInt16 address, UInt16 value)
        {
            Open();

            var tid = Interlocked.Increment(ref _transactionId);

            var cmd = new Byte[12];
            cmd[0] = (Byte)(tid >> 8);
            cmd[1] = (Byte)(tid & 0xFF);
            cmd[2] = (Byte)(ProtocolId >> 8);
            cmd[3] = (Byte)(ProtocolId & 0xFF);
            cmd[4] = 0;
            cmd[5] = 1 + 1 + 2 + 2;
            cmd[6] = host;
            cmd[7] = code;
            cmd[8] = (Byte)(address >> 8);
            cmd[9] = (Byte)(address & 0xFF);
            cmd[10] = (Byte)(value >> 8);
            cmd[11] = (Byte)(value & 0xFF);

            {
                using var span = Tracer?.NewSpan("modbus:SendCommand", cmd.ToHex());

                WriteLog("{0}=> {1}", Server, cmd.ToHex("-"));
                _stream.Write(cmd, 0, cmd.Length);
                Thread.Sleep(10);
            }

            try
            {
                using var span = Tracer?.NewSpan("modbus:ReceiveCommand");

                var rs = new Byte[32];
                var c = _stream.Read(rs, 0, rs.Length);
                rs = rs.ReadBytes(0, c);
                WriteLog("{0}<= {1}", Server, rs.ToHex("-"));

                if (span != null) span.Tag = rs.ToHex();

                if (rs.Length < 8) return null;

                return rs;
            }
            catch (TimeoutException) { return null; }
        }
        #endregion
    }
}