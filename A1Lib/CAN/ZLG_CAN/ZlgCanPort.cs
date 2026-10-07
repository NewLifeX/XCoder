using ALib;
using ALib.Can;
using ALib.CAN;
using NewLife.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using static ALib.ZLG_CAN.ZlgCanApi;

namespace ALib.ZLG_CAN
{
    /// <summary>
    /// ZLG（致远）CAN 接口卡端口，基于 zlgcan.dll（第二代句柄式 API）。
    /// 每个实例对应设备上的一个 CAN 通道。
    /// </summary>
    public class ZlgCanPort : ICanPort
    {
        /// <summary>构造，指定枚举名称</summary>
        /// <param name="name">枚举时候的名字</param>
        public ZlgCanPort(String name) : base(name) { }

        #region DLL管理

        private static readonly String DllFileName = "zlgcan.dll";

        /// <summary>
        /// 静态构造函数，确保DLL可用
        /// </summary>
        static ZlgCanPort() => _Prepare();

        /// <summary>
        /// 一次性完成运行环境准备，保证「先释放驱动目录、再加载 zlgcan.dll」的严格顺序。
        /// 顺序不可颠倒：zlgcan.dll 加载时会探测 kerneldlls 并缓存结果，
        /// 若此时驱动尚未就位，本次进程内 ZCAN_OpenDevice 将一直返回 0（表现为枚举为空）。
        /// </summary>
        private static void _Prepare()
        {
            // 幂等：静态构造函数、EnumDevices()、Open() 都可能触发，只做一次
            if (_prepared) return;
            lock (_prepareLock)
            {
                if (_prepared) return;

                try
                {
                    // 1）先把 kerneldlls 释放到位（zlgcan.dll 初始化时依赖它）
                    // 释放失败必须直接返回：一旦 zlgcan.dll 被加载，它会把「驱动缺失」
                    // 的结果缓存到本进程，之后即便 kerneldlls 补齐，枚举也仍为空。
                    if (!_EnsureKernelDlls())
                    {
                        XTrace.WriteLine("ZLG 驱动目录未就位，跳过 zlgcan.dll 加载");
                        return;
                    }

                    // 2）再确保 zlgcan.dll 可用（资源名 ALib.ZLG_CAN.zlgcan.dll，见 A1Lib.csproj）
                    var success = DllResourceManager.EnsureDllAvailable("ALib.ZLG_CAN", DllFileName);
                    if (!success)
                    {
                        XTrace.WriteLine($"{DllFileName} 加载失败");
                        throw new InvalidOperationException($"{DllFileName} 加载失败");
                    }

                    _prepared = true;
                }
                catch (InvalidOperationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    // 无硬件等场景下不应因准备失败而中断，仅记录
                    XTrace.WriteLine("ZLG 运行环境准备失败: " + ex.Message);
                }
            }
        }

        /// <summary>运行环境是否已准备（驱动目录 + zlgcan.dll）</summary>
        private static Boolean _prepared;

        /// <summary>准备过程锁</summary>
        private static readonly Object _prepareLock = new Object();

        /// <summary>
        /// 确保 zlgcan.dll 能找到同目录下的 kerneldlls 驱动目录。
        /// zlgcan.dll 运行时按「自身同级目录\kerneldlls\xxx.dll」加载各设备内核驱动
        /// （USBCAN.dll、USBCANFD.dll 等），并读取 kerneldlls\dll_cfg.ini 决定设备类型到驱动的映射。
        /// 缺少该目录时 ZCAN_OpenDevice 一律返回 0，表现为「枚举不到任何设备」。
        /// </summary>
        /// <returns>kerneldlls 目录是否就位</returns>
        private static Boolean _EnsureKernelDlls()
        {
            try
            {
                var appDir = AppContext.BaseDirectory;
                if (String.IsNullOrEmpty(appDir)) return false;

                var dir = Path.Combine(appDir, KernelDllExtractor.DirName);

                // 已存在且含配置文件的目录直接复用，避免重复遍历全部嵌入资源
                if (File.Exists(Path.Combine(dir, KernelDllExtractor.CfgFileName))) return true;

                return KernelDllExtractor.Extract(appDir);
            }
            catch (Exception ex)
            {
                XTrace.WriteLine("释放 ZLG kerneldlls 失败: " + ex.Message);
                return false;
            }
        }

        #endregion

        #region 硬件相关

        /// <summary>设备类型，见 ZlgCanApi.ZCAN_USBCAN* 常量</summary>
        public UInt32 DeviceType = ZCAN_USBCAN2;

        /// <summary>同类型设备索引，从 0 开始</summary>
        public UInt32 DeviceIndex = 0;

        /// <summary>设备句柄（由首个打开的通道创建，引用计数归零时关闭）</summary>
        public IntPtr DeviceHandle = IntPtr.Zero;

        /// <summary>通道句柄（本实例独占）</summary>
        public IntPtr ChannelHandle = IntPtr.Zero;

        /// <summary>打开标记</summary>
        public Boolean Opened = false;

        /// <summary>接收缓冲区可容纳的帧数</summary>
        private const Int32 RxBufferSize = 1000;

        /// <summary>接收等待超时，用于周期性检查退出标志</summary>
        private const Int32 RxWaitTimeoutMs = 100;

        #endregion

        #region 设备枚举

        /// <summary>枚举当前计算机上所有可用的 ZLG CAN 口（未打开，仅列出）</summary>
        /// <returns>key = "ZLG-{设备类型}-{设备索引}-{通道号}"，value = 可配置的 ICanPort 实例</returns>
        public new static Dictionary<String, ICanPort> EnumDevices()
        {
            var dict = new Dictionary<String, ICanPort>();

            // 必须最先执行：枚举内部会立即调用 ZCAN_OpenDevice，
            // 而它要求 kerneldlls 已就位且 zlgcan.dll 已在驱动目录就绪之后加载。
            // 未就绪时直接返回空表：此时调用 ZCAN_OpenDevice 会因找不到驱动而失败，
            // 且可能让 zlgcan.dll 把故障状态缓存下来。
            _Prepare();
            if (!_prepared) return dict;

            // 物理设备唯一键 -> 已选中的探测结果。
            // 同一个 ZCAN_USBCAN1(3) 与 ZCAN_USBCAN2(4) 在 dll_cfg.ini 里指向同一个
            // USBCAN.dll，两种类型去打开同一台设备都会成功，若不去重会重复列出。
            var devices = new Dictionary<String, DeviceCandidate>();

            // zlgcan.dll 没有类似 VCI_FindUsbDevice 的枚举接口，
            // 采用「按设备类型逐索引试探打开」的方式枚举：打不开说明该索引没有设备。
            foreach (var devtype in ScanDeviceTypes)
            {
                // 单类型设备索引通常不超过 16 个
                for (UInt32 index = 0; index < MaxDeviceIndex; index++)
                {
                    IntPtr dev = IntPtr.Zero;
                    try
                    {
                        dev = ZCAN_OpenDevice(devtype, index, 0);
                        if (dev == IntPtr.Zero) break;

                        var info = new ZCAN_DEVICE_INFO();
                        UInt32 canNum = 0;
                        if (ZCAN_GetDeviceInf(dev, ref info) == STATUS_OK)
                        {
                            canNum = info.can_Num;
                        }
                        if (canNum == 0) canNum = 1;

                        var serial = info.str_Serial_Num == null
                            ? ""
                            : System.Text.Encoding.Default.GetString(info.str_Serial_Num).TrimEnd('\0', ' ');
                        var hwType = info.str_hw_Type == null
                            ? ""
                            : System.Text.Encoding.Default.GetString(info.str_hw_Type).TrimEnd('\0', ' ');

                        // 序列号为空的设备无法作为唯一键，退化为「类型+索引」以免误合并不同设备
                        var key = String.IsNullOrEmpty(serial)
                            ? $"{hwType}|{devtype}|{index}"
                            : $"{hwType}|{serial}";

                        var cur = new DeviceCandidate
                        {
                            DeviceType = devtype,
                            DeviceIndex = index,
                            CanNum = canNum,
                            Serial = serial,
                            HwType = hwType,
                        };

                        if (devices.TryGetValue(key, out var old))
                        {
                            // 同一物理设备被多种类型识别：保留更贴切的一个。
                            // 例：USBCAN-II 是双通道，can_Num=2，应归到 ZCAN_USBCAN2 而非单通道的 USBCAN1。
                            if (!IsBetterType(cur, old))
                            {
                                continue;
                            }
                        }

                        devices[key] = cur;
                    }
                    catch (Exception ex)
                    {
                        // 输出完整异常（含 DllNotFoundException / EntryPointNotFoundException 等类型），便于定位
                        XTrace.WriteLine($"枚举ZLG设备失败(type={devtype}, index={index}): {ex}");
                        break;
                    }
                    finally
                    {
                        // 枚举只是探测，立刻关闭，避免占用设备导致后续真正 Open 失败
                        if (dev != IntPtr.Zero)
                        {
                            try { ZCAN_CloseDevice(dev); } catch { }
                        }
                    }
                }
            }

            // 去重后按设备生成各通道
            foreach (var d in devices.Values)
            {
                for (UInt32 ch = 0; ch < d.CanNum; ch++)
                {
                    var name = $"ZLG-{d.DeviceType}-{d.DeviceIndex}-{ch}";
                    var port = new ZlgCanPort(name)
                    {
                        DeviceType = d.DeviceType,
                        DeviceIndex = d.DeviceIndex,
                        PortId = (Int32)ch,
                    };
                    port._Serial = d.Serial;
                    port._HwType = d.HwType;
                    dict[name] = port;
                }
            }

            return dict;
        }

        /// <summary>探测到的物理设备候选（同一设备可能被多种设备类型识别）</summary>
        private struct DeviceCandidate
        {
            /// <summary>设备类型</summary>
            public UInt32 DeviceType;

            /// <summary>同类型设备索引</summary>
            public UInt32 DeviceIndex;

            /// <summary>通道数</summary>
            public UInt32 CanNum;

            /// <summary>序列号</summary>
            public String Serial;

            /// <summary>硬件类型</summary>
            public String HwType;
        }

        /// <summary>
        /// 比较同一物理设备下两种设备类型的贴切程度。
        /// 规则：通道数能对上硬件实际的类型优先（USBCAN1 为单通道型号，
        /// 双通道设备应归 ZCAN_USBCAN2）；其次按类型码排序保证结果稳定。
        /// </summary>
        /// <param name="cur">当前候选</param>
        /// <param name="old">已记录候选</param>
        /// <returns>cur 是否更优</returns>
        private static Boolean IsBetterType(DeviceCandidate cur, DeviceCandidate old)
        {
            // 单通道设备类型若声称的通道数与实际不符，说明是别名，判为劣
            var curFit = _ChannelFit(cur.DeviceType, cur.CanNum);
            var oldFit = _ChannelFit(old.DeviceType, old.CanNum);
            if (curFit != oldFit) return curFit > oldFit;

            // 通道数一致时取较大的类型码（USBCAN2=4 优于 USBCAN1=3）
            return cur.DeviceType > old.DeviceType;
        }

        /// <summary>设备类型与通道数的匹配度，越大越贴切</summary>
        /// <param name="devtype">设备类型</param>
        /// <param name="canNum">实际通道数</param>
        /// <returns>匹配度</returns>
        private static Int32 _ChannelFit(UInt32 devtype, UInt32 canNum)
        {
            // 已知单通道型号
            if (devtype == ZCAN_USBCAN1 || devtype == ZCAN_USBCAN_E_U) return canNum == 1 ? 2 : 0;

            // 已知双通道型号
            if (devtype == ZCAN_USBCAN2 || devtype == ZCAN_USBCAN_2E_U) return canNum == 2 ? 2 : 0;

            // 其它类型不做通道数假设
            return 1;
        }

        /// <summary>枚举时探测的设备类型（仅 USB 系列，PCI/网络系列按需追加）</summary>
        private static readonly UInt32[] ScanDeviceTypes =
        {
            ZCAN_USBCAN1,
            ZCAN_USBCAN2,
            ZCAN_USBCAN_E_U,
            ZCAN_USBCAN_2E_U,
            ZCAN_USBCANFD_200U,
            ZCAN_USBCANFD_100U,
            ZCAN_USBCANFD_MINI,
        };

        /// <summary>单类型最多探测的设备索引数</summary>
        private const UInt32 MaxDeviceIndex = 16;

        /// <summary>序列号（枚举时读取，仅用于展示）</summary>
        private String _Serial;

        /// <summary>硬件类型（枚举时读取，仅用于展示）</summary>
        private String _HwType;

        #endregion

        #region 设备引用计数（一个CAN工具有多个CAN口，需所有口都关闭才能关闭设备）

        /// <summary>设备打开引用计数，key = (设备类型 &lt;&lt; 32) | 设备索引</summary>
        private static readonly Dictionary<UInt64, Int32> _devRefCount = new Dictionary<UInt64, Int32>();

        /// <summary>设备句柄缓存，key 同上</summary>
        private static readonly Dictionary<UInt64, IntPtr> _devHandles = new Dictionary<UInt64, IntPtr>();

        /// <summary>引用计数锁</summary>
        private static readonly Object _devLock = new Object();

        /// <summary>设备key</summary>
        private static UInt64 _DevKey(UInt32 devtype, UInt32 devindex) => ((UInt64)devtype << 32) | devindex;

        /// <summary>打开设备（引用计数，仅首个CAN口真正调用 ZCAN_OpenDevice）</summary>
        private static IntPtr _OpenDevice(UInt32 devtype, UInt32 devindex)
        {
            lock (_devLock)
            {
                var key = _DevKey(devtype, devindex);
                if (_devRefCount.TryGetValue(key, out var cnt) && cnt > 0)
                {
                    // 设备已打开，仅增加引用
                    _devRefCount[key] = cnt + 1;
                    return _devHandles[key];
                }

                // 首次打开设备
                var handle = ZCAN_OpenDevice(devtype, devindex, 0);
                if (handle == IntPtr.Zero) return IntPtr.Zero;

                _devRefCount[key] = 1;
                _devHandles[key] = handle;
                return handle;
            }
        }

        /// <summary>关闭设备（引用计数，仅最后一个CAN口真正调用 ZCAN_CloseDevice）</summary>
        private static void _CloseDevice(UInt32 devtype, UInt32 devindex)
        {
            lock (_devLock)
            {
                var key = _DevKey(devtype, devindex);
                if (!_devRefCount.TryGetValue(key, out var cnt) || cnt <= 0) return;

                cnt--;
                if (cnt > 0)
                {
                    // 还有其它CAN口在用，仅减少引用
                    _devRefCount[key] = cnt;
                    return;
                }

                // 最后一个CAN口，真正关闭设备
                _devRefCount.Remove(key);
                if (_devHandles.TryGetValue(key, out var handle))
                {
                    _devHandles.Remove(key);
                    try { ZCAN_CloseDevice(handle); } catch { }
                }
            }
        }

        #endregion

        #region 打开/关闭

        /// <summary>打开</summary>
        /// <returns></returns>
        public override Boolean Open()
        {
            if (Opened) return true;

            try
            {
                // 枚举得到的实例可能被直接 Open（未经 EnumDevices），此处兜底准备
                _Prepare();
                if (!_prepared)
                {
                    WriteLog("ZLG 运行环境未就绪，无法打开端口");
                    return false;
                }

                // 打开（引用计数：一个设备有多个CAN口，仅首个口真正打开设备）
                var dev = _OpenDevice(DeviceType, DeviceIndex);
                if (dev == IntPtr.Zero)
                {
                    WriteLog("打开ZLG设备失败");
                    return false;
                }
                DeviceHandle = dev;

                // 配置通道：can_type 只取决于硬件类型，非 CANFD 设备必须传 TYPE_CAN
                var cfg = new ZCAN_CHANNEL_INIT_CONFIG
                {
                    can_type = TYPE_CAN,
                    acc_code = 0,
                    acc_mask = 0xFFFFFFFF,
                };

                if (!TryGetTiming((Int32)Baudrate, out var t0, out var t1))
                {
                    WriteLog($"不支持的波特率 {Baudrate}，回退 250K");
                }
                cfg.timing0 = t0;
                cfg.timing1 = t1;
                cfg.filter = 1;
                cfg.mode = 0;

                var ch = ZCAN_InitCAN(dev, (UInt32)PortId, ref cfg);
                if (ch == IntPtr.Zero)
                {
                    WriteLog("ZCAN_InitCAN 失败");
                    _CloseDevice(DeviceType, DeviceIndex);
                    DeviceHandle = IntPtr.Zero;
                    return false;
                }
                ChannelHandle = ch;

                if (ZCAN_StartCAN(ch) != STATUS_OK)
                {
                    WriteLog("ZCAN_StartCAN 失败");
                    _CloseDevice(DeviceType, DeviceIndex);
                    DeviceHandle = IntPtr.Zero;
                    ChannelHandle = IntPtr.Zero;
                    return false;
                }

                // 丢弃打开前的残留报文
                try { ZCAN_ClearBuffer(ch); } catch { }
            }
            catch (Exception ex)
            {
                WriteLog("ZLG 打开异常: " + ex.Message);
                _CloseDevice(DeviceType, DeviceIndex);
                DeviceHandle = IntPtr.Zero;
                ChannelHandle = IntPtr.Zero;
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

            // 必须等接收线程真正退出后再释放通道，否则线程仍在 ZCAN_Receive 中访问已复位的通道。
            // 此处不会长时间阻塞：停止标志已置位，线程最多跑完当前一次 ZCAN_Receive 便会退出。
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
                    ZCAN_ResetCAN(ChannelHandle);
                }
                catch { }

                // 关闭设备（引用计数：一个设备多个CAN口，需所有口都关闭后才真正关闭设备）
                _CloseDevice(DeviceType, DeviceIndex);

                ChannelHandle = IntPtr.Zero;
                DeviceHandle = IntPtr.Zero;
            }

            Opened = false;
        }

        /// <summary>释放资源</summary>
        /// <param name="disposing"></param>
        protected override void Dispose(Boolean disposing)
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
        /// </summary>
        private void _StopRx()
        {
            // 先置位停止标志，后续收到的帧不再分发（也不会再 Invoke 到 UI 线程）
            _StopReceive();

            if (_thrRx == null) return;
            if (_ctsRx == null) return;

            try { _ctsRx.Cancel(); } catch { }
        }

        /// <summary>接收线程函数（阻塞式批量接收）</summary>
        private void _RxThreadFunc()
        {
            var cts = _ctsRx;

            // ZCAN_Receive 需要一段连续的非托管缓冲，一次性分配并复用。
            // 每帧结构体大小固定，按 ZCAN_Receive_Data 布局写入后按同样布局读回。
            var frameSize = Marshal.SizeOf<ZCAN_Receive_Data>();
            var bufSize = frameSize * RxBufferSize;
            var buf = Marshal.AllocHGlobal(bufSize);

            try
            {
                while (true)
                {
                    if (cts == null || cts.Token.IsCancellationRequested) break;
                    // 收到停止信号立即退出，不再处理本批剩余帧
                    if (RxStopped) break;

                    var ch = ChannelHandle;
                    if (ch == IntPtr.Zero) break;

                    try
                    {
                        // wait_time 给超时值，既能及时响应退出，也能在超时内被数据唤醒
                        var res = ZCAN_Receive(ch, buf, RxBufferSize, RxWaitTimeoutMs);
                        if (res == 0xFFFFFFFF) res = 0;
                        if (res > RxBufferSize) res = RxBufferSize;

                        for (UInt32 i = 0; i < res; i++)
                        {
                            // 停止过程中丢弃剩余帧，避免继续向 UI 线程 Invoke
                            if (RxStopped) break;

                            var pkt = Marshal.PtrToStructure<ZCAN_Receive_Data>(buf + (Int32)i * frameSize);

                            // 错误帧不向上分发
                            if (pkt.frame.IsError) continue;

                            ProcessReceived(pkt.ToCanMsg());
                        }
                    }
                    catch
                    {
                        Thread.Sleep(10);
                    }

                    if (RxStopped) break;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buf);
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

            var len = data?.Length ?? 0;
            if (len > CAN_MAX_DLEN) len = CAN_MAX_DLEN;

            var tx = new ZCAN_Transmit_Data
            {
                frame = new can_frame
                {
                    can_id = can_frame.MakeCanId(id, ext, remote, false),
                    can_dlc = (byte)len,
                    data = new byte[CAN_MAX_DLEN],
                },
                transmit_type = TRANSMIT_NORMAL,
            };
            if (len > 0) Array.Copy(data, tx.frame.data, len);

            WriteLog(tx.ToCanMsg().ToString() + " TX");

            // ZCAN_Transmit 需要连续的结构体地址，用非托管内存承载，避免取托管结构体地址
            var size = Marshal.SizeOf<ZCAN_Transmit_Data>();
            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(tx, ptr, false);
                var n = ZCAN_Transmit(ChannelHandle, ptr, 1);
                if (n != 1) WriteLog("ZLG 发送失败");
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        #endregion

        #region 日志

        /// <summary>日志前缀</summary>
        protected override String LogPre => "ZLG ";

        #endregion
    }

    /// <summary>ZLG 帧与通用 CanMsg 的转换辅助</summary>
    internal static class ZlgCanConvert
    {
        /// <summary>接收数据 -> CanMsg</summary>
        /// <param name="pkt">ZLG 接收数据</param>
        /// <returns></returns>
        public static CanMsg ToCanMsg(this ZCAN_Receive_Data pkt)
        {
            var f = pkt.frame;
            var len = f.can_dlc > CAN_MAX_DLEN ? CAN_MAX_DLEN : f.can_dlc;
            var data = new byte[len];
            if (len > 0 && f.data != null) Array.Copy(f.data, data, len);

            var msg = new CanMsg((Int32)f.PureId, data, f.IsRemote, f.IsExtend);
            // ZLG 时间戳单位为微秒
            if (pkt.timestamp > 0) msg.TimeStamp = TimeSpan.FromMilliseconds(pkt.timestamp / 1000.0);
            return msg;
        }

        /// <summary>发送数据 -> CanMsg（仅用于日志格式化）</summary>
        /// <param name="pkt">ZLG 发送数据</param>
        /// <returns></returns>
        public static CanMsg ToCanMsg(this ZCAN_Transmit_Data pkt)
        {
            var f = pkt.frame;
            var len = f.can_dlc > CAN_MAX_DLEN ? CAN_MAX_DLEN : f.can_dlc;
            var data = new byte[len];
            if (len > 0 && f.data != null) Array.Copy(f.data, data, len);

            return new CanMsg((Int32)f.PureId, data, f.IsRemote, f.IsExtend);
        }
    }
}
