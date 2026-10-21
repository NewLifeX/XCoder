using NewLife;
using NewLife.Data;
using NewLife.Log;
using NewLife.Threading;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ALib.ComPort
{
    public abstract class Com_WithUnpack : DisposeBase
    {
        #region 构造，初始化，析构

        /// <summary>串口对象</summary>
        public SerialPort _Port = new SerialPort();

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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Close();
        }

        #endregion

        #region 开关，离线

        public bool Open()
        {
            if (!_Port.IsOpen)
            {
                _Port.DataReceived += _Port_DataReceived;
                _Port.Open();

                StartCheck();
            }

            return true;
        }

        public void Close()
        {
            StopCheck();

            try
            {
                if (_Port != null)
                {
                    _Port.DataReceived -= _Port_DataReceived;
                    if (_Port.IsOpen) _Port.Close();
                }
            }
            catch (Exception ex)
            {
                WriteLog("关闭串口出错 " + ex.Message);
            }
        }

        #region 检查离线

        /// <summary>串口断开。可能是人为关闭，也可能是设备被拔出</summary>
        public event EventHandler Disconnected;

        private TimerX _checkTimer;

        /// <summary>检测间隔。默认2000ms</summary>
        public Int32 CheckPeriod { get; set; } = 2000;

        void StartCheck()
        {
            if (_checkTimer != null) return;
            if (CheckPeriod <= 0) return;

            _checkTimer = new TimerX(CheckDisconnect, null, CheckPeriod, CheckPeriod) { Async = true };
        }

        void StopCheck()
        {
            var timer = _checkTimer;
            _checkTimer = null;
            timer?.Dispose();
        }

        /// <summary>
        /// 检查串口是否已经断开。
        /// FX 串口异步操作有泄漏缺陷，硬件长时间断开时 IsOpen 检测不到，需要对比系统端口列表。
        /// </summary>
        void CheckDisconnect(Object state)
        {
            try
            {
                var name = _Port?.PortName;
                if (name.IsNullOrEmpty()) return;

                // 端口已经不存在，或者已经不在打开状态
                if (!SerialPort.GetPortNames().Contains(name, StringComparer.OrdinalIgnoreCase)) { }
                else if (_Port.IsOpen) return;

                StopCheck();

                WriteLog("串口{0}断开连接", name);

                RxCache.Clear();
                RxMsg.Clear();

                Disconnected?.Invoke(this, EventArgs.Empty);
            }
            catch { }
        }

        #endregion

        #endregion

        #region 收包解包

        /// <summary>收数据缓冲</summary>
        public CircularQueue<byte> RxCache { get; set; } = new CircularQueue<byte>(256);

        /// <summary>收数据</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var sp = sender as SerialPort;

                // 直接读走缓冲区现有数据，不做等待
                while (sp.BytesToRead > 0)
                {
                    var count = sp.BytesToRead;
                    var buf = new Byte[count];
                    var len = sp.Read(buf, 0, count);
                    if (len <= 0) break;

                    var bs = len == buf.Length ? buf : buf.ReadBytes(0, len);
                    WriteLog(" RX " + bs.ToHex());

                    RxCache.WriteArray(bs, true);
                }

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
            catch (Exception ex)
            {
                WriteLog("收数据出错 " + ex.Message);
            }
        }

        /// <summary>
        /// 拆包,需要丢弃不合法数据，别被塞死。
        /// 返回有效包，返回null表示一条完整的都没有，结束拆包。
        /// 需要回复ACK的时候直接Send发送。
        /// </summary>
        public abstract byte[] TryUnpack(CircularQueue<byte> queue);

        /// <summary>收到消息事件</summary>
        public event EventHandler<byte[]> MsgReceived;


        /// <summary>消息命令</summary>
        public CircularQueue<byte[]> RxMsg { get; set; } = new CircularQueue<byte[]>(128);

        /// <summary>清空接收数据包</summary>
        public void RxMsgClear() => RxMsg.Clear();

        #endregion

        /// <summary>发送</summary>
        /// <param name="msg"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool Send(byte[] msg, bool log = true)
        {
            if (log) WriteLog(" TX " + msg.ToHex());

            try
            {
                lock (_Port)
                {
                    if (!_Port.IsOpen) Open();

                    _Port.Write(msg, 0, msg.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                WriteLog("发送出错 " + ex.Message);
                return false;
            }
        }

        #region 日志

        /// <summary>日志前缀</summary>
        public String LogPre = "  ";

        public ILog Log { get; set; }

        void WriteLog(String format, params Object[] args) => Log?.Write(LogLevel.Info, LogPre + format, args);

        #endregion
    }
}
