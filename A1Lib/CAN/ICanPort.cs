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
    public abstract class ICanRx
    {
        /// <summary>数据包CAN ID 通过掩码之后的值，如果匹配则代表处理器正确</summary>
        public int CanId = 0;

        /// <summary>CAN ID 掩码</summary>
        public int CanIdMask = 0;

        /// <summary>数据处理</summary>
        /// <param name="bs">can 帧</param>
        /// <returns></returns>
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

        /// <summary>注册数据处理器</summary>
        /// <param name="proc"></param>
        /// <returns></returns>
        public virtual bool Register(ICanRx proc)
        {
            lock (_RxProcess)
            {
                // 排除重复
                foreach (var item in _RxProcess)
                {
                    if ((item.CanIdMask == proc.CanIdMask) && (item.CanId == proc.CanId))
                    {
                        return false;
                    }
                }

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
                    // 用于匹配处理器的 CAN ID
                    var id = pkt.ID;
                    if ((id & (UInt32)item.CanIdMask) == (UInt32)item.CanId)
                    {
                        if (item.Process(pkt, this)) res = true;
                    }
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
