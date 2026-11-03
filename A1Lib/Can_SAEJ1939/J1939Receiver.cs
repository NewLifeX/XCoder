using ALib.CAN;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALib.Can_SAEJ1939
{
    #region 结果载体

    /// <summary>
    /// 一帧非 TP 的 J1939 报文（已由 <see cref="J1939Receiver"/> 拆解）。
    /// </summary>
    public class J1939Frame
    {
        /// <summary>原始 CAN 帧</summary>
        public CanMsg Raw;

        /// <summary>拆解后的 29bit J1939 ID（含优先级 / PF / PS / SA）</summary>
        public J1939Id Id;

        /// <summary>参数组编号（18bit）</summary>
        public UInt32 Pgn;

        /// <summary>源地址 SA</summary>
        public byte SourceAddress => Id?.SourceAddress ?? 0;

        /// <summary>目标地址 DA（PDU1 时为 PS，PDU2 时为全局 0xFF）</summary>
        public byte DestinationAddress => Id?.DestinationAddress ?? 0xFF;

        /// <summary>
        /// 按 PGN 解析出的固定结构消息对象（如 <see cref="Msg_Request"/> / <see cref="Msg_DM1"/> 等）。
        /// 未知 PGN 时为 null —— 调用方据此判断是否需要自行按原始数据区解析。
        /// </summary>
        public Object Parsed;

        public override String ToString()
        {
            var pgn = Pgn.ToString("X5");
            var sa = SourceAddress.ToString("X2");
            var da = DestinationAddress.ToString("X2");
            var kind = Parsed?.GetType().Name ?? "Raw";
            return $"J1939 PGN=0x{pgn} SA=0x{sa} DA=0x{da} [{kind}]";
        }
    }

    #endregion

    #region 接收器

    /// <summary>
    /// J1939 应用层 / 网络管理报文接收器（传输协议 <see cref="J1939Tp"/> 之外的所有 J1939 帧）。
    /// <para>与 <see cref="J1939Tp"/> 的分工：TP.CM(0xEC00) 与 TP.DT(0xEB00) 由 <see cref="J1939Tp"/> 认领并重组；
    /// 其余 29bit J1939 报文（地址声明、请求、应答、DM1~DM13、EEC1/CCVS 等应用 PG，以及未知 PGN）全部由本接收器承接。</para>
    /// <para>典型用法（结合端口级单归属，避免同一帧被多协议重复解析）：
    /// <code>
    /// var rx = port.GetOrAdd(J1939Receiver.KeyItems, () => new J1939Receiver(port));
    /// rx.MessageReceived += (s, e) => { /* e.Parsed 为已知类型，否则按 e.Raw.Data 自行解析 */ };
    /// rx.SetPgnWhitelist(new[] { J1939Pgn.DM1, J1939Pgn.Request });  // 只关心这两个 PGN
    /// port.SingleOwner = true;   // 多协议并存时建议开启
    /// </code></para>
    /// <para>线程模型：<see cref="Process"/> 由接收线程回调，<see cref="MessageReceived"/> 也在接收线程触发，
    /// 订阅者请勿在其中做阻塞 / 耗时操作，也不要调用 <see cref="J1939Tp"/> 的发送接口（否则死锁）。</para>
    /// </summary>
    public class J1939Receiver : ICanRx
    {
        /// <summary>在 <see cref="ICanPort"/> 上挂载本对象的字典键</summary>
        public const String KeyItems = "J1939Receiver";

        /// <summary>本对象所属的 CAN 端口</summary>
        public ICanPort Port { get; }

        /// <summary>是否把 TP.CM / TP.DT 也纳入本接收器（默认 false：交给 <see cref="J1939Tp"/> 处理）</summary>
        public bool IncludeTpFrames { get; set; } = false;

        #region PGN 白名单过滤

        /// <summary>
        /// PGN 白名单（18bit）。为空表示不过滤，接收除 TP 之外的所有 J1939 帧；
        /// 非空时仅接收列表内的 PGN（PDU1 的 PGN 不含目标地址 DA，按 18bit PGN 匹配）。
        /// 写时整体替换引用、读取无锁，可在接收线程安全访问。
        /// </summary>
        private volatile HashSet<UInt32> _pgnWhitelist = new HashSet<UInt32>();

        /// <summary>是否已启用 PGN 白名单（列表非空）</summary>
        public bool HasPgnWhitelist => _pgnWhitelist.Count > 0;

        /// <summary>当前白名单快照（只读）</summary>
        public IReadOnlyCollection<UInt32> PgnWhitelist => _pgnWhitelist.ToArray();

        /// <summary>
        /// 覆盖式设置 PGN 白名单。传入 null / 空集合表示关闭过滤，接收除 TP 之外的所有 J1939 帧。
        /// 每个 PGN 会自动掩码到 18bit（&amp; 0x3FFFF）。
        /// </summary>
        public void SetPgnWhitelist(IEnumerable<UInt32> pgns)
        {
            var set = new HashSet<UInt32>();
            if (pgns != null)
            {
                foreach (var p in pgns) set.Add(p & 0x3FFFFu);
            }
            _pgnWhitelist = set;
        }

        /// <summary>向白名单追加一个 PGN（自动掩码到 18bit）</summary>
        public void AddPgn(UInt32 pgn) => _pgnWhitelist = new HashSet<UInt32>(_pgnWhitelist) { pgn & 0x3FFFFu };

        /// <summary>从白名单移除一个 PGN（自动掩码到 18bit），返回是否实际发生了移除</summary>
        public bool RemovePgn(UInt32 pgn)
        {
            var mask = pgn & 0x3FFFFu;
            var old = _pgnWhitelist;
            if (!old.Contains(mask)) return false;
            _pgnWhitelist = new HashSet<UInt32>(old.Where(x => x != mask));
            return true;
        }

        /// <summary>清空白名单（恢复为不过滤）</summary>
        public void ClearPgnWhitelist() => _pgnWhitelist = new HashSet<UInt32>();

        #endregion

        /// <summary>收到一帧非 TP 的 J1939 报文（已知 PGN 时 <see cref="J1939Frame.Parsed"/> 非 null）</summary>
        public event EventHandler<J1939Frame> MessageReceived;

        /// <summary>收到一帧 PGN 不在固定结构表中的未知 J1939 报文（<see cref="J1939Frame.Parsed"/> 为 null）</summary>
        public event EventHandler<J1939Frame> UnknownMessage;

        /// <summary>构造并注册到端口。建议通过 <c>port.GetOrAdd(J1939Receiver.KeyItems, () =&gt; new J1939Receiver(port))</c> 获取。</summary>
        public J1939Receiver(ICanPort port)
        {
            if (port == null) throw new ArgumentNullException(nameof(port));
            Port = port;

            // 端口不再做 CAN ID 匹配，本处理器会收到该端口所有帧，
            // 由 Match / Process 内部按 J1939 规则过滤 TP 之外的报文。
            if (!port.Register(this)) throw new InvalidOperationException("该端口已注册 J1939 接收器");
        }

        /// <summary>
        /// 快速预匹配：仅认领 J1939 帧（29bit 扩展帧），且 PGN 不属于 TP.CM / TP.DT。
        /// 供端口层在分发前过滤无关帧（UDS / CANopen / 普通 11bit 报文），避免对每个帧都做完整解析。
        /// 若已设置 <see cref="PgnWhitelist"/>，则仅放行列表内的非 TP PGN。最终是否消费仍以 <see cref="Process"/> 为准。
        /// <para>注意：ISO-TP(UDS) 在 29bit 诊断寻址（如 0x18DAxxxx）下也会是 29bit 帧，
        /// 若同一端口同时挂 UDS 且未用 <see cref="ICanPort.SingleOwner"/> 或不同 ID 区分，可能与之重叠；
        /// 此时应让本接收器仅认领 J1939 网络上的 PGN 范围，或确保 UDS 的 ID 与 J1939 不冲突。</para>
        /// </summary>
        public override bool Match(CanMsg msg)
        {
            if (msg == null || !msg.ExternFlag) return false;   // J1939 固定使用 29bit 扩展帧

            var pgn = J1939Id.GetPgn((UInt32)msg.ID);
            if (pgn == J1939Pgn.TpCm || pgn == J1939Pgn.TpDt) return IncludeTpFrames;

            // PGN 白名单：非空时仅放行列表内的 PGN
            var wl = _pgnWhitelist;
            if (wl.Count > 0 && !wl.Contains(pgn)) return false;

            return true;
        }

        /// <summary>
        /// 接收线程回调：解析 J1939 报文并触发 <see cref="MessageReceived"/>。
        /// 仅认领 TP.CM / TP.DT 之外的 J1939 帧，避免与 <see cref="J1939Tp"/> 重复处理；
        /// 同时受 <see cref="PgnWhitelist"/> 约束（与 <see cref="Match"/> 行为一致，作为直接调用时的安全阀）。
        /// </summary>
        public override bool Process(CanMsg msg, ICanPort port)
        {
            if (msg == null) return false;

            var id = J1939Id.FromCanMsg(msg);
            var pgn = id.PGNValue;

            // 安全阀：TP 帧交给 J1939Tp
            if (pgn == J1939Pgn.TpCm || pgn == J1939Pgn.TpDt) return false;

            // 安全阀：白名单约束（与 Match 一致）
            var wl = _pgnWhitelist;
            if (wl.Count > 0 && !wl.Contains(pgn)) return false;

            var parsed = J1939Message.Parse(msg);   // 已知 PGN 返回类型化对象，未知返回 null
            var frame = new J1939Frame
            {
                Raw = msg,
                Id = id,
                Pgn = pgn,
                Parsed = parsed,
            };

            MessageReceived?.Invoke(this, frame);
            if (parsed == null) UnknownMessage?.Invoke(this, frame);

            return true;
        }

        /// <summary>从端口注销并释放资源</summary>
        public void Close()
        {
            try { Port?.Unregister(this); } catch { }
            try { Port?.RemoveItem(KeyItems); } catch { }
        }
    }

    #endregion
}
