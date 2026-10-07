using NewLife;
using NewLife.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALib.ComPort
{
    public abstract class Com_WithUnpack : DisposeBase
    {
        /// <summary>消息命令</summary>
        public CircularQueue<byte[]> RxMsg { get; set; } = new CircularQueue<byte[]>(128);
        /// <summary>收数据缓冲</summary>
        public CircularQueue<byte> RxCache { get; set; } = new CircularQueue<byte>(256);

        private SerialTransport _Port = new SerialTransport();

        public Com_WithUnpack(String comName, int baudrate = 115200)
        {
            _Port.PortName = comName;
            _Port.BaudRate = baudrate;
        }

        public Com_WithUnpack() { }

        public void Init(String comName, int baudrate = 115200)
        {
            _Port.PortName = comName;
            _Port.BaudRate = baudrate;
        }


        #region 开关析构

        private void _Port_Disconnected(object sender, EventArgs e)
        {
            // throw new NotImplementedException();
            RxCache.Clear();
            WriteLog("串口断开连接");
        }

        public bool Open()
        {
            _Port.Disconnected += _Port_Disconnected;
            _Port.Received += _Port_Received;
            return _Port.Open();
        }

        public void Close()
        {
            _Port.Received -= _Port_Received;
            _Port.Disconnected -= _Port_Disconnected;
            _Port.Close();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Close();
        }

        /// <summary>收数据</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Port_Received(object sender, NewLife.Net.ReceivedEventArgs e)
        {
            var bs = e.GetBytes();
            WriteLog(" RX " + bs.ToHex());

            RxCache.WriteArray(bs, true);
            while (true)
            {
                var pk = TryUnpack(RxCache);
                if (pk == null) break;
                if (pk.Length == 0) break;

                if (RxMsg.Remain() < 1) RxMsg.Seek(1);
                RxMsg.Enqueue(pk);

                // 所有消息都处理抛
                MsgReceived?.Invoke(this, pk);
            }

            if (RxCache.IsEmpty()) RxCache.Clear();
        }

        #endregion

        /// <summary>
        /// 拆包,需要丢弃不合法数据，别被塞死。
        /// 返回有效包，返回null表示一条完整的都没有，结束拆包。
        /// 需要回复ACK的时候直接Send发送。
        /// </summary>
        public abstract byte[] TryUnpack(CircularQueue<byte> queue);

        /// <summary>收到消息事件</summary>
        public event EventHandler<byte[]> MsgReceived;

        /// <summary>清空接收数据包</summary>
        public void RxMsgClear() => RxMsg.Clear();

        /// <summary>发送</summary>
        /// <param name="msg"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool Send(byte[] msg, bool log = true)
        {
            if (log) WriteLog(" TX " + msg.ToHex());
            lock (_Port) { return _Port.Send(msg) == msg.Length; }
        }

        #region 日志

        /// <summary>日志前缀</summary>
        public String LogPre = "  ";

        public ILog Log { get; set; }

        void WriteLog(String format, params Object[] args) => Log?.Write(LogLevel.Info, LogPre + format, args);

        #endregion
    }
}
