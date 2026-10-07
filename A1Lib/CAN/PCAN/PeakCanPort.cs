using ALib;
using ALib.Can;
using ALib.CAN;
using NewLife.Log;
using Peak.Can.Basic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ALib.PEAK_CAN
{
    public class PeakCanPort : ICanPort
    {
        /// <summary>构造，指定枚举名称</summary>
        /// <param name="name">枚举时候的名字</param>
        public PeakCanPort(String name) : base(name) { }

        #region DLL管理

        private static readonly string DllFileName = "PCANBasic.dll";

        /// <summary>
        /// 静态构造函数，确保DLL可用
        /// </summary>
        static PeakCanPort()
        {
            // 使用通用DLL资源管理器确保DLL可用（资源名 ALib.PCAN.PCANBasic.dll，见 A1Lib.csproj）
            bool success = DllResourceManager.EnsureDllAvailable("ALib.PCAN", DllFileName);
            if (!success)
            {
                XTrace.WriteLine($"PCANBasic.dll 加载失败");
                throw new InvalidOperationException("PCANBasic.dll 加载失败");
            }
        }

        #endregion

        #region 硬件相关

        /// <summary>PCAN 通道句柄，默认 USB 通道1（可按需改为 PCI/局域网通道）</summary>
        public UInt16 Channel = PCANBasic.PCAN_USBBUS1;

        /// <summary>打开标记</summary>
        public bool Opened = false;

        #endregion

        #region 设备枚举

        /// <summary>枚举当前计算机上所有可用的 PCAN 通道（未打开，仅列出）</summary>
        /// <returns>key = "PCAN-{设备名}-{控制器号}"，value = 可配置的 ICanPort 实例</returns>
        public new static Dictionary<String, ICanPort> EnumDevices()
        {
            var dict = new Dictionary<String, ICanPort>();
            try
            {
                TPCANChannelInformation[] buffer = new TPCANChannelInformation[16];
                var st = PCANBasic.GetValue(PCANBasic.PCAN_NONEBUS, TPCANParameter.PCAN_ATTACHED_CHANNELS, buffer);
                if (st != TPCANStatus.PCAN_ERROR_OK) return dict;

                foreach (var ci in buffer)
                {
                    if (ci.channel_handle == PCANBasic.PCAN_NONEBUS) continue;
                    // 跳过不可用通道（PCAN_CHANNEL_UNAVAILABLE = 0）
                    if (ci.channel_condition == 0) continue;

                    var name = $"PCAN-{ci.device_name}-{ci.controller_number}";
                    var port = new PeakCanPort(name);
                    port.Channel = ci.channel_handle;
                    dict[name] = port;
                }
            }
            catch (Exception ex)
            {
                // 输出完整异常，便于定位（如 DllNotFoundException / EntryPointNotFoundException）
                XTrace.WriteLine("枚举PCAN设备失败: " + ex.ToString());
            }
            return dict;
        }

        #endregion

        #region 打开/关闭

        /// <summary>打开</summary>
        /// <returns></returns>
        public override bool Open()
        {
            try
            {
                var st = PCANBasic.Initialize(Channel, MapBaudrate());
                if (st != TPCANStatus.PCAN_ERROR_OK)
                {
                    WriteLog("PCAN 初始化失败: " + GetErrorText(st));
                    return false;
                }

                Opened = true;
                // 复位停止标志，恢复正常收发
                _ResumeReceive();
                // 先注册接收事件，再启动线程，避免线程读到空的 _evtRx 而退化为轮询
                _RegisterRxEvent();
                // 注册失败不阻断打开流程：线程侧会自动退化为定时轮询
                _StartRx();

                return true;
            }
            catch (Exception ex)
            {
                WriteLog("PCAN 打开异常: " + ex.Message);
                return false;
            }
        }

        /// <summary>关闭</summary>
        public override void Close()
        {
            // 先置停止标志并发出取消，接收线程不会再分发消息，也会尽快退出
            _StopRx();

            // 等接收线程真正退出后再卸载通道，避免线程仍在 Read 中访问已卸载的通道。
            // 停止标志已置位，线程最多跑完当前一次 Read 便会退出，不会长时间阻塞。
            var thr = _thrRx;
            if ((thr != null) && thr.IsAlive)
            {
                try { thr.Join(1000); } catch { }
            }

            // 线程已退出，此时解除事件注册并释放句柄是安全的
            _UnregisterRxEvent();

            try
            {
                PCANBasic.Uninitialize(Channel);
            }
            catch { }

            Opened = false;
        }

        /// <summary>释放资源</summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            // 仅显式释放时清理；终结器/进程退出阶段不触碰 _ctsRx（等待句柄）与原生 DLL，
            // 否则会导致进程崩溃（0xC0000409 STATUS_STACK_BUFFER_OVERRUN）
            if (disposing)
            {
                _ctsRx?.Dispose();
                _ctsRx = null;
            }

            base.Dispose(disposing);   // 会调用 Close() -> _StopRx()（仅 disposing 时真正关闭硬件）
        }

        /// <summary>波特率映射</summary>
        /// <returns></returns>
        private TPCANBaudrate MapBaudrate()
        {
            switch (Baudrate)
            {
                case Baudrate_e.B1000K: return TPCANBaudrate.PCAN_BAUD_1M;
                case Baudrate_e.B500K: return TPCANBaudrate.PCAN_BAUD_500K;
                case Baudrate_e.B250K: return TPCANBaudrate.PCAN_BAUD_250K;
                case Baudrate_e.B125K: return TPCANBaudrate.PCAN_BAUD_125K;
                case Baudrate_e.B100K: return TPCANBaudrate.PCAN_BAUD_100K;
                case Baudrate_e.B200K:
                    WriteLog("PCAN 无 200K 标准波特率，回退 250K");
                    return TPCANBaudrate.PCAN_BAUD_250K;
                default: return TPCANBaudrate.PCAN_BAUD_250K;
            }
        }

        #endregion

        #region 接收线程

        /// <summary>后台接收线程</summary>
        private Thread _thrRx;

        /// <summary>终止线程</summary>
        private CancellationTokenSource _ctsRx = new CancellationTokenSource();

        /// <summary>PCAN 接收事件句柄（驱动在有报文时置位，由接收线程等待）</summary>
        private ManualResetEvent _evtRx;

        /// <summary>等待事件超时时间（毫秒），仅用于周期性检查退出标志，避免漏停</summary>
        private const Int32 RxWaitTimeoutMs = 200;

        /// <summary>启动接收线程</summary>
        private void _StartRx()
        {
            if ((_thrRx != null) && (_thrRx.IsAlive))
            {
                _StopRx();
                // 旧线程已置停止标志，会很快退出；短暂等待避免新旧线程同时访问通道
                _thrRx.Join(1000);
                _thrRx = null;
            }

            _ctsRx?.Dispose();
            _ctsRx = new CancellationTokenSource();
            _thrRx = new Thread(() => _RxThreadFunc());
            _thrRx.IsBackground = true;
            _thrRx.Start();
        }

        /// <summary>
        /// 停止接收线程。
        /// 只发出停止信号并置位停止标志，不在调用线程上阻塞等待。
        /// 原因：关闭通常由 UI 线程发起，接收线程可能正阻塞在向 UI 线程 Invoke 上，
        /// 若此处同步等待线程退出，会与 UI 线程形成双向死等，表现为主窗口关闭卡顿。
        /// </summary>
        private void _StopRx()
        {
            // 先置位停止标志，后续收到的帧不再分发（也不会再 Invoke 到 UI 线程）
            _StopReceive();

            if (_thrRx == null) return;
            if (_ctsRx == null) return;

            try { _ctsRx.Cancel(); } catch { }

            // 唤醒可能正阻塞在 WaitOne 上的接收线程，使其立即检查停止标志
            try { _evtRx?.Set(); } catch { }
        }

        /// <summary>
        /// 注册 PCAN 接收事件。驱动在接收队列非空时置位该事件，
        /// 接收线程据此阻塞等待，避免轮询空转。
        /// </summary>
        /// <returns>是否注册成功</returns>
        private bool _RegisterRxEvent()
        {
            _UnregisterRxEvent();

            try
            {
                // ManualReset：一次通知可覆盖多帧，线程被唤醒后抽干队列，读完自行 Reset
                _evtRx = new ManualResetEvent(false);
                var h = _evtRx.SafeWaitHandle.DangerousGetHandle();
                var st = PCANBasic.SetValue(
                    Channel,
                    TPCANParameter.PCAN_RECEIVE_EVENT,
                    h,
                    (UInt32)IntPtr.Size);

                if (st != TPCANStatus.PCAN_ERROR_OK)
                {
                    WriteLog("PCAN 注册接收事件失败: " + GetErrorText(st));
                    _UnregisterRxEvent();
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteLog("PCAN 注册接收事件异常: " + ex.Message);
                _UnregisterRxEvent();
                return false;
            }
        }

        /// <summary>注销 PCAN 接收事件</summary>
        private void _UnregisterRxEvent()
        {
            try
            {
                // 传 NULL 句柄即注销，BufferLength 必须为 0
                PCANBasic.SetValue(Channel, TPCANParameter.PCAN_RECEIVE_EVENT, IntPtr.Zero, 0);
            }
            catch { }

            try { _evtRx?.Dispose(); } catch { }
            _evtRx = null;
        }

        /// <summary>接收线程函数（事件驱动 + 批量抽干队列）</summary>
        private void _RxThreadFunc()
        {
            var cts = _ctsRx;
            var evt = _evtRx;

            try
            {
                while (true)
                {
                    if (cts == null || cts.Token.IsCancellationRequested) break;
                    // 收到停止信号立即退出
                    if (RxStopped) break;

                    try
                    {
                        // 阻塞等待驱动通知（带超时以便定期检查退出标志）。
                        // 事件缺失时退化为普通轮询，保证仍能收帧。
                        if (evt != null) evt.WaitOne(RxWaitTimeoutMs);
                        else Thread.Sleep(RxWaitTimeoutMs);

                        if (cts.Token.IsCancellationRequested) break;
                        if (RxStopped) break;

                        // 一次唤醒，抽干队列：状态帧/错误帧不入队
                        TPCANMsg msg;
                        TPCANTimestamp ts;
                        while (PCANBasic.Read(Channel, out msg, out ts) == TPCANStatus.PCAN_ERROR_OK)
                        {
                            // 状态帧/错误帧不向上分发
                            if ((msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_STATUS) != 0)
                                continue;
                            if ((msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_ERRFRAME) != 0)
                                continue;

                            RxMsgProcess(msg, ts);
                        }
                    }
                    catch
                    {
                        Thread.Sleep(10);
                    }
                    finally
                    {
                        // 必须在读完后复位，否则下一批报文将不再触发通知（漏帧）
                        try { evt?.Reset(); } catch { }
                    }
                }
            }
            finally
            {
                // 线程退出前解除事件注册，避免驱动继续向已释放的句柄置位
                try { _UnregisterRxEvent(); } catch { }
            }
        }

        #endregion

        #region 发送

        /// <summary>发送数据</summary>
        /// <param name="id">CANID</param>
        /// <param name="data">数据</param>
        /// <param name="remote">远程帧</param>
        /// <param name="ext">扩展帧</param>
        public override void Send(UInt32 id, byte[] data, bool remote = false, bool ext = false)
        {   
            if (!Opened)
            {
                WriteLog("发送失败，端口未打开");
                return;
            }

            var msg = new TPCANMsg();
            msg.ID = (uint)id;
            msg.MSGTYPE = ext
                ? TPCANMessageType.PCAN_MESSAGE_EXTENDED
                : TPCANMessageType.PCAN_MESSAGE_STANDARD;
            if (remote) msg.MSGTYPE |= TPCANMessageType.PCAN_MESSAGE_RTR;
            msg.LEN = (byte)data.Length;
            if (msg.LEN > 8) msg.LEN = 8;
            msg.DATA = new byte[8];
            Array.Copy(data, msg.DATA, msg.LEN);

            WriteLog(MsgToString(msg));

            var st = PCANBasic.Write(Channel, ref msg);
            if (st != TPCANStatus.PCAN_ERROR_OK)
            {
                WriteLog("PCAN 发送失败: " + GetErrorText(st));
            }
        }

        #endregion

        #region 处理消息，软件逻辑

        /// <summary>处理消息</summary>
        /// <param name="msg"></param>
        /// <param name="ts"></param>
        /// <returns></returns>
        private bool RxMsgProcess(TPCANMsg msg, TPCANTimestamp ts)
        {
            return ProcessReceived(ToCanMsg(msg, ts));
        }

        /// <summary>TPCANMsg -> CanMsg</summary>
        /// <param name="msg"></param>
        /// <param name="ts"></param>
        /// <returns></returns>
        private CanMsg ToCanMsg(TPCANMsg msg, TPCANTimestamp ts)
        {
            var ext = (msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_EXTENDED) != 0;
            var rtr = (msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_RTR) != 0;
            var len = msg.LEN > 8 ? 8 : msg.LEN;
            var data = new byte[len];
            Array.Copy(msg.DATA, data, len);

            var pkt = new CanMsg((Int32)msg.ID, data, rtr, ext);
            // 总微秒 = micros + 1000*millis + 0x100000000*1000*millis_overflow
            var micros = ts.micros + 1000UL * ts.millis + 0x100000000UL * 1000UL * ts.millis_overflow;
            pkt.TimeStamp = TimeSpan.FromMilliseconds(micros / 1000.0);
            return pkt;
        }

        #endregion

        #region 日志

        /// <summary>日志前缀</summary>
        protected override String LogPre => "PCAN ";

        /// <summary>格式化字符串</summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private String MsgToString(TPCANMsg msg)
        {
            var sb = new StringBuilder();

            if ((msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_EXTENDED) != 0)
                sb.Append(msg.ID.ToString("X8") + " ");
            else
                sb.Append(msg.ID.ToString("X3") + " ");

            var R = (msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_RTR) != 0 ? "R " : "  ";
            sb.Append(R);

            var E = (msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_EXTENDED) != 0 ? "E " : "  ";
            sb.Append(E);

            sb.Append("[" + msg.LEN + "] ");
            for (int i = 0; i < msg.LEN; i++)
            {
                sb.Append(msg.DATA[i].ToString("X2") + " ");
            }
            return sb.ToString();
        }

        /// <summary>错误描述</summary>
        /// <param name="st"></param>
        /// <returns></returns>
        private String GetErrorText(TPCANStatus st)
        {
            var sb = new StringBuilder(256);
            if (PCANBasic.GetErrorText(st, 0, sb) == TPCANStatus.PCAN_ERROR_OK)
                return sb.ToString();
            return st.ToString();
        }

        #endregion
    }
}
