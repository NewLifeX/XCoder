using ALib;
using NewLife.Collections;
using NewLife.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ALib.CX_CAN.CxControlCAN;
using ALib.Can;
using ALib.CAN;
using NewLife;

namespace ALib.CX_CAN
{
    public class CxCanPort : ICanPort
    {
        /// <summary>构造，指定枚举名称</summary>
        /// <param name="name">枚举时候的名字</param>
        public CxCanPort(String name) : base(name) { }

        #region DLL管理

        private static readonly string DllFileName = "ControlCAN.dll";

        /// <summary>
        /// 静态构造函数，确保DLL可用
        /// </summary>
        static CxCanPort()
        {
            // 使用通用DLL资源管理器确保DLL可用
            bool success = DllResourceManager.EnsureDllAvailable("ALib.CX_CAN", DllFileName);
            if (!success)
            {
                XTrace.WriteLine(L.T("Failed to load ControlCAN.dll."));
                throw new InvalidOperationException(L.T("Failed to load ControlCAN.dll."));
            }
        }

        #endregion

        #region 硬件相关

        // USBCAN2
        static UInt32 m_devtype = 4;

        /// <summary>CAN 分析仪下标</summary>
        public UInt32 DevId = 0;

        /// <summary>打开的</summary>
        public bool Opened = false;

        // 收数据包缓冲
        private VCI_CAN_OBJ[] m_recobj = new VCI_CAN_OBJ[1000];

        /// <summary>打开</summary>
        /// <returns></returns>
        public override bool Open()
        {
            if (Opened) return true;

            // 以下两行为多卡同机测试代码，用来获取序列号与对应的设备索引号，单卡可以不使用。
            // VCI_BOARD_INFO[] vbi2 = new VCI_BOARD_INFO[50];
            // uint num1 = VCI_FindUsbDevice2(ref vbi2[0]);

            try
            {
                // 打开（引用计数：一个设备有多个CAN口，仅首个口真正打开设备）
                if (!_OpenDevice(m_devtype, DevId))
                {
                    return false;
                }
                // 配置
                VCI_INIT_CONFIG cfg = new VCI_INIT_CONFIG();
                cfg.AccCode = 0;
                cfg.AccMask = UInt32.MaxValue;

                switch (Baudrate)
                {
                    case Baudrate_e.B1000K: cfg.Timing0 = 0; cfg.Timing1 = 0x14; break;
                    case Baudrate_e.B500K: cfg.Timing0 = 0; cfg.Timing1 = 0x1c; break;
                    case Baudrate_e.B250K: cfg.Timing0 = 1; cfg.Timing1 = 0x1c; break;
                    case Baudrate_e.B200K: cfg.Timing0 = 0x81; cfg.Timing1 = 0xFA; break;
                    case Baudrate_e.B125K: cfg.Timing0 = 3; cfg.Timing1 = 0x1c; break;
                    case Baudrate_e.B100K: cfg.Timing0 = 4; cfg.Timing1 = 0x1c; break;
                    default: break;
                }

                // 00  1c  500k    01  1c 250k
                // cfg.Timing0 = 0;
                // cfg.Timing1 = 0x1c;
                cfg.Filter = 1;
                cfg.Mode = 0;
                VCI_InitCAN(m_devtype, DevId, (UInt32)PortId, ref cfg);
                // 启动
                CxControlCAN.VCI_StartCAN(m_devtype, DevId, (UInt32)PortId);
            }
            catch
            {
                // 初始化异常，回退设备引用计数
                _CloseDevice(m_devtype, DevId);
                return false;
            }

            Opened = true;
            // 复位停止标志，恢复正常收发
            _ResumeReceive();
            _StartRx();

            return true;
        }

        /// <summary>关闭</summary>
        public override void Close()
        {
            // 先置停止标志并发出取消，接收线程不会再分发消息，也会尽快退出
            _StopRx();

            // 必须等接收线程真正退出后再释放设备，否则线程仍在 VCI_Receive 中访问已关闭的设备。
            // 此处不会长时间阻塞：停止标志已置位，线程最多跑完当前一次 VCI_Receive 便会退出。
            var thr = _thrRx;
            if ((thr != null) && thr.IsAlive)
            {
                try { thr.Join(1000); } catch { }
            }

            if (Opened)
            {
                try
                {
                    // 停止本CAN口
                    CxControlCAN.VCI_ResetCAN(m_devtype, DevId, (UInt32)PortId);
                }
                catch { }

                // 关闭设备（引用计数：一个设备多个CAN口，需所有口都关闭后才真正关闭设备）
                _CloseDevice(m_devtype, DevId);
            }

            Opened = false;
        }

        #region 设备引用计数（一个CAN工具有多个CAN口，需所有口都关闭才能关闭设备）

        /// <summary>设备打开引用计数，key = (设备类型 &lt;&lt; 32) | 设备索引</summary>
        private static readonly Dictionary<UInt64, Int32> _devRefCount = new Dictionary<UInt64, Int32>();

        /// <summary>引用计数锁</summary>
        private static readonly Object _devLock = new Object();

        /// <summary>设备key</summary>
        private static UInt64 _DevKey(UInt32 devtype, UInt32 devid) => ((UInt64)devtype << 32) | devid;

        /// <summary>打开设备（引用计数，仅首个CAN口真正调用 VCI_OpenDevice）</summary>
        private static bool _OpenDevice(UInt32 devtype, UInt32 devid)
        {
            lock (_devLock)
            {
                var key = _DevKey(devtype, devid);
                if (_devRefCount.TryGetValue(key, out var cnt) && cnt > 0)
                {
                    // 设备已打开，仅增加引用
                    _devRefCount[key] = cnt + 1;
                    return true;
                }

                // 首次打开设备
                if (CxControlCAN.VCI_OpenDevice(devtype, devid, 0) == 0)
                {
                    return false;
                }
                _devRefCount[key] = 1;
                return true;
            }
        }

        /// <summary>关闭设备（引用计数，仅最后一个CAN口真正调用 VCI_CloseDevice）</summary>
        private static void _CloseDevice(UInt32 devtype, UInt32 devid)
        {
            lock (_devLock)
            {
                var key = _DevKey(devtype, devid);
                if (!_devRefCount.TryGetValue(key, out var cnt) || cnt <= 0)
                {
                    return;
                }

                cnt--;
                if (cnt > 0)
                {
                    // 还有其它CAN口在用，仅减少引用
                    _devRefCount[key] = cnt;
                    return;
                }

                // 最后一个CAN口，真正关闭设备
                _devRefCount.Remove(key);
                try { CxControlCAN.VCI_CloseDevice(devtype, devid); } catch { }
            }
        }

        #endregion


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

        #region 设备枚举

        /// <summary>枚举当前计算机上所有可用的 CX CAN 口（未打开，仅列出）</summary>
        /// <returns>key = "CX-{设备类型}-{DevId}-{口序号}"，value = 可配置的 ICanPort 实例</returns>
        public new static Dictionary<String, ICanPort> EnumDevices()
        {
            var dict = new Dictionary<String, ICanPort>();
            try
            {
                // 注意：VCI_FindUsbDevice2 会按设备数量向传入缓冲区连续写入 VCI_BOARD_INFO，
                // 必须传入足够大的数组（官方示例为 50 个元素）。
                // 若只传单个元素的引用，多设备时驱动会越界写栈，导致进程原生崩溃
                // （0xC0000409 STATUS_STACK_BUFFER_OVERRUN，表现为“在这行跑飞”）。
                VCI_BOARD_INFO[] infos = new VCI_BOARD_INFO[50];
                UInt32 count = CxControlCAN.VCI_FindUsbDevice2(ref infos[0]);
                if (count <= 0) return dict;
                if (count > infos.Length) count = (UInt32)infos.Length;

                // 为每个 USB-CAN 适配器（DevId = 索引）枚举其所有 CAN 口
                for (UInt32 devId = 0; devId < count; devId++)
                {
                    byte canNum = infos[devId].can_Num;
                    if (canNum == 0) canNum = 1;

                    for (byte ch = 0; ch < canNum; ch++)
                    {
                        var name = $"CX-{m_devtype}-{devId}-{ch}";
                        var port = new CxCanPort(name);
                        port.DevId = devId;
                        port.PortId = ch;
                        dict[name] = port;
                    }
                }
            }
            catch (Exception ex)
            {
                // 输出完整异常（含 DllNotFoundException / EntryPointNotFoundException 等类型），便于定位
                XTrace.WriteLine("枚举CX设备失败: " + ex.ToString());
            }
            return dict;
        }

        #endregion

        #region 接收线程

        /// <summary>后台接收线程</summary>
        private Thread _thrRx;

        /// <summary>终止线程</summary>
        private CancellationTokenSource _ctsRx = new CancellationTokenSource();

        /// <summary>启动接收线程</summary>
        private void _StartRx()
        {
            if ((_thrRx != null) && (_thrRx.IsAlive))
            {
                _StopRx();
                // 旧线程已置停止标志，会很快退出；短暂等待避免新旧线程同时访问硬件
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
        /// 置位停止标志后，接收线程不再分发消息，也会立即退出等待。
        /// </summary>
        private void _StopRx()
        {
            // 先置位停止标志，后续收到的帧不再分发（也不会再 Invoke 到 UI 线程）
            _StopReceive();

            if (_thrRx == null) return;
            if (_ctsRx == null) return;

            try { _ctsRx.Cancel(); } catch { }
        }

        /// <summary>接收线程函数</summary>
        private void _RxThreadFunc()
        {
            var cts = _ctsRx;
            while (true)
            {
                if (cts == null || cts.Token.IsCancellationRequested) break;
                // 收到停止信号立即退出，不再处理本批剩余帧
                if (RxStopped) break;

                try
                {
                    UInt32 res = new UInt32();
                    res = VCI_Receive(m_devtype, DevId, (UInt32)PortId, ref m_recobj[0], 1000, 100);
                    if (res == 0xFFFFFFFF) res = 0;

                    for (int i = 0; i < res; i++)
                    {
                        // 停止过程中丢弃剩余帧，避免继续向 UI 线程 Invoke
                        if (RxStopped) break;

                        RxMsgProcess(m_recobj[i]);
                    }
                }
                catch { }

                if (RxStopped) break;

                Thread.Sleep(10);
            }
        }

        #endregion

        /// <summary>发送消息</summary>
        /// <param name="msg"></param>
        public void Send(VCI_CAN_OBJ msg)
        {
            if (!Opened)
            {
                WriteLog("发送失败，端口未打开");
                return;
            }

            if (VCI_Transmit(m_devtype, DevId, (UInt32)PortId, ref msg, 1) == 0) { }
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

            VCI_CAN_OBJ msg = new VCI_CAN_OBJ();
            msg.ID = id;
            msg.RemoteFlag = (byte)(remote ? 1 : 0);
            msg.ExternFlag = (byte)(ext ? 1 : 0);
            var len = data.Length;
            if (len > 8) len = 8;
            msg.DataLen = (byte)len;

            unsafe
            {
                for (int i = 0; i < len; i++)
                {
                    msg.Data[i] = data[i];
                }
            }

            WriteLog(msg.ToString() + " TX");
            Send(msg);
        }

        #endregion

        #region 处理消息，软件逻辑

        /// <summary>处理消息</summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool RxMsgProcess(VCI_CAN_OBJ msg)
        {
            return ProcessReceived(msg.ToCanMsg());
        }

        #endregion

        #region 日志

        /// <summary>日志前缀</summary>
        protected override String LogPre => "CX ";

        #endregion
    }
}