using ALib.Can;
using NewLife;
using NewLife.Log;
using NewLife.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ALib.CAN
{
    /// <summary>
    /// CAN 帧处理器。
    /// 注册后会收到端口上的**所有**帧（不再做 CAN ID 匹配），
    /// 由各实现自行判断是否关心该帧。
    /// </summary>
    public abstract class ICanRx
    {
        /// <summary>
        /// 数据处理。会收到端口上的所有帧，需自行过滤。
        /// 返回 true 表示已消费该帧（端口据此抑制「未识别帧」日志）。
        /// </summary>
        /// <param name="msg">can 帧</param>
        /// <param name="port">所属端口</param>
        /// <returns>是否已处理</returns>
        public virtual bool Process(CanMsg msg, ICanPort port)
        {
            // XTrace.WriteLine(MsgToString(msg));
            return false;
        }
    }

    public abstract class ICanPort : DisposeBase
    {
        /// <summary>由子类构造时指定名称</summary>
        /// <param name="name">枚举时候的名字</param>
        protected ICanPort(String name) { Name = name; }

        public enum Baudrate_e
        {
            B1000K,
            B500K,
            B250K,
            B200K,
            B125K,
            B100K,
        };

        /// <summary>
        /// 枚举各 ICanPort 实现（ZLG/CX/PCAN 等）的设备并汇总输出。
        /// 注意：这里显式调用各实现的静态 EnumDevices()，不使用反射 Invoke。
        /// 原因：部分实现内部调用含 unsafe/fixed 结构体 ref 参数的 P/Invoke，
        /// 反射 Invoke 会走通用封送路径，导致传给原生库的指针错误而原生崩溃（0xC0000409）。
        /// </summary>
        /// <returns>key = 描述性名称，value = ICanPort 实例</returns>
        public static Dictionary<String, ICanPort> EnumDevices()
        {
            var dict = new Dictionary<String, ICanPort>();

            // 新增 ICanPort 实现时，在此追加一行
            _Collect(dict, () => ALib.ZLG_CAN.ZlgCanPort.EnumDevices());
            _Collect(dict, () => ALib.CX_CAN.CxCanPort.EnumDevices());
            _Collect(dict, () => ALib.PEAK_CAN.PeakCanPort.EnumDevices());

            return dict;
        }

        /// <summary>调用某个实现的静态 EnumDevices()，异常不影响其它实现</summary>
        /// <param name="dict">汇总字典</param>
        /// <param name="enumFunc">实现的枚举委托</param>
        static void _Collect(Dictionary<String, ICanPort> dict, Func<Dictionary<String, ICanPort>> enumFunc)
        {
            try
            {
                var sub = enumFunc();
                if (sub == null) return;

                foreach (var kv in sub)
                {
                    dict[kv.Key] = kv.Value;
                }
            }
            catch (Exception ex)
            {
                NewLife.Log.XTrace.WriteLine("枚举CAN设备失败: " + ex.ToString());
            }
        }

        /// <summary>枚举时候的名字，由子类构造时传入，创建后不可修改</summary>
        public String Name { get; }

        /// <summary>CAN 分析仪多个CAN口，下标</summary>
        public Int32 PortId = 0;

        /// <summary>波特率，默认 250K</summary>
        public Baudrate_e Baudrate = Baudrate_e.B250K;

        /// <summary>打开</summary>
        /// <returns></returns>
        public abstract bool Open();

        /// <summary>关闭</summary>
        public abstract void Close();

        /// <summary>释放资源（硬件设备，确保关闭）</summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            // 仅在显式释放时关闭硬件。
            // 终结器/进程退出阶段（disposing == false）不得触碰原生 DLL 与等待句柄，
            // 否则在 CLR 关闭期间操作已失效的原生资源会导致进程崩溃（0xC0000409）。
            if (disposing)
            {
                try { Close(); }
                catch { }
            }

            base.Dispose(disposing);
        }

        public abstract void Send(UInt32 id, byte[] data, bool remote = false, bool ext = false);

        /// <summary>发送消息</summary>
        /// <param name="pkt"></param>
        public void Send(CanMsg pkt) { Send((UInt32)pkt.ID, pkt.Data, pkt.RemoteFlag, pkt.ExternFlag); }

        public void Send(CanPacket pkt) { Send(pkt.ToCanMsg()); }

        /// <summary>
        /// 收发停止标志。关闭/停止接收时置位，
        /// 接收线程据此立即停止把帧分发给订阅者（<see cref="Received"/> 与 <see cref="ICanRx"/>），
        /// 避免关闭过程中 UI 线程等待接收线程、而接收线程正阻塞在 UI 线程 Invoke 上的双向死等。
        /// </summary>
        private volatile Boolean _rxStopped = false;

        /// <summary>是否已停止收发（接收线程与分发逻辑的短路依据）</summary>
        public Boolean RxStopped => _rxStopped;

        /// <summary>置位收发停止标志，之后 <see cref="ProcessReceived"/> 不再分发消息</summary>
        protected void _StopReceive() => _rxStopped = true;

        /// <summary>复位收发停止标志，用于重新打开端口前恢复工作</summary>
        protected void _ResumeReceive() => _rxStopped = false;

        /// <summary>数据到达事件</summary>
        public event EventHandler<CanMsg> Received;

        /// <summary>消息处理器</summary>
        protected readonly List<ICanRx> _RxProcess = new List<ICanRx>();

        #region 端口级附加对象（每端口单例的挂载点）

        /// <summary>
        /// 端口级附加对象字典。
        /// 用于挂载「每个端口一份」的协议处理器（如 J1939 TP 收发器），
        /// 避免做成进程级单例：一个进程可同时打开多个 CAN 端口，
        /// 每个端口各自持有独立实例，互不干扰。
        /// </summary>
        readonly Dictionary<String, Object> _Items = new Dictionary<String, Object>();

        /// <summary>
        /// 端口级附加对象索引器。
        /// 读取时若不存在返回 null；写入 null 表示移除该项。
        /// <code>
        /// var tp = port["J1939Tp"] as J1939Tp;
        /// if (tp == null) port["J1939Tp"] = tp = new J1939Tp(port);
        /// </code>
        /// </summary>
        /// <param name="key">对象键，建议使用协议名/类型名</param>
        /// <returns>对象实例，不存在时为 null</returns>
        public Object this[String key]
        {
            get
            {
                if (key == null) return null;

                lock (_Items)
                {
                    return _Items.TryGetValue(key, out var value) ? value : null;
                }
            }
            set
            {
                if (key == null) return;

                lock (_Items)
                {
                    if (value == null) _Items.Remove(key);
                    else _Items[key] = value;
                }
            }
        }

        /// <summary>
        /// 获取端口级附加对象，不存在时用 factory 创建并挂载。
        /// 同一 key 只会创建一次，多线程并发安全。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="key">对象键</param>
        /// <param name="factory">首次访问时的创建委托</param>
        public T GetOrAdd<T>(String key, Func<T> factory) where T : class
        {
            if (key == null || factory == null) return null;

            lock (_Items)
            {
                if (_Items.TryGetValue(key, out var value) && value is T t) return t;

                var obj = factory();
                if (obj != null) _Items[key] = obj;
                return obj;
            }
        }

        /// <summary>已挂载的附加对象键集合（快照）</summary>
        public String[] ItemKeys
        {
            get
            {
                lock (_Items) return _Items.Keys.ToArray();
            }
        }

        /// <summary>移除端口级附加对象</summary>
        /// <param name="key">对象键</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveItem(String key)
        {
            if (key == null) return false;

            lock (_Items) return _Items.Remove(key);
        }

        /// <summary>清空所有端口级附加对象</summary>
        public void ClearItems()
        {
            lock (_Items) _Items.Clear();
        }

        #endregion


        /// <summary>注册数据处理器</summary>
        /// <param name="proc"></param>
        /// <returns></returns>
        public virtual bool Register(ICanRx proc)
        {
            if (proc == null) return false;

            lock (_RxProcess)
            {
                // 排除重复：同一实例只注册一次
                if (_RxProcess.Contains(proc)) return false;

                _RxProcess.Add(proc);
                return true;
            }
        }

        /// <summary>反注册</summary>
        /// <param name="proc"></param>
        public virtual void Unregister(ICanRx proc)
        {
            lock (_RxProcess) _RxProcess.Remove(proc);
        }

        /// <summary>处理收到的 CAN 帧（分发到已注册的处理器，无处理器处理时写日志）</summary>
        /// <param name="pkt">已转换的 CAN 帧</param>
        /// <returns>是否至少有一个处理器处理了该帧</returns>
        protected bool ProcessReceived(CanMsg pkt)
        {
            // 已停止接收：直接丢弃本帧。
            // 关闭端口时 UI 线程会同步等待接收线程退出，若此处仍回调订阅者，
            // 订阅者会 Invoke 回正在等待的 UI 线程，形成双向死等导致关闭卡顿。
            if (_rxStopped) return false;

            Received?.Invoke(this, pkt);

            bool res = false;
            lock (_RxProcess)
            {
                foreach (var item in _RxProcess)
                {
                    // 不做 CAN ID 匹配，全量回调；处理器自行过滤
                    if (item.Process(pkt, this)) res = true;
                }
            }

            if (!res)
            {
                WriteLog(pkt.ToString());
            }

            return res;
        }

        /// <summary>日志</summary>
        public ILog Log { get; set; } = Logger.Null;

        /// <summary>写日志</summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected void WriteLog(String format, params Object[] args) => Log?.Info(LogPre + format, args);

        /// <summary>日志前缀，由各实现指定（如 "ZLG " / "CX " / "PCAN "）</summary>
        protected virtual String LogPre => "CAN ";
    }
}
