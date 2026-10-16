using ALib.CAN;
using NewLife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALib.Can_SAEJ1939
{
    #region 枚举

    /// <summary>
    /// TP.CM 控制字节（byte1），决定连接管理报文的具体结构
    /// </summary>
    public enum TpControl_e : byte
    {
        /// <summary>RTS 请求发送（点对点）</summary>
        Rts = 0x10,

        /// <summary>CTS 允许发送（点对点）</summary>
        Cts = 0x11,

        /// <summary>EOM 发送结束（点对点）</summary>
        EndOfMsg = 0x13,

        /// <summary>BAM 广播发送</summary>
        Bam = 0x20,

        /// <summary>Abort 中止</summary>
        Abort = 0xFF,
    }

    /// <summary>
    /// TP 中止原因
    /// </summary>
    public enum TpAbortReason_e : byte
    {
        /// <summary>已达最大重发次数</summary>
        MaxRetransmit = 1,

        /// <summary>资源超时</summary>
        Timeout = 2,

        /// <summary>连接异常中止</summary>
        AbortConnection = 3,

        /// <summary>暂存区不足</summary>
        NoBuffer = 4,
    }

    #endregion

    #region TP.CM 连接管理

    /// <summary>
    /// TP.CM 连接管理，PGN 60416 (0x00EC00)。
    /// 数据区 8 字节，byte1 为控制字节，其余按控制字节解释：
    /// <code>
    /// RTS (0x10)   : [ctrl][总长 L][总长 H][包数][最大包长][PGN 4字节]
    /// CTS (0x11)   : [ctrl][可发包数][下一包序号][保留 2][PGN 4字节]
    /// EOM (0x13)   : [ctrl][总长 L][总长 H][包数][保留 1][PGN 4字节]
    /// BAM (0x20)   : [ctrl][总长 L][总长 H][包数][保留 1][PGN 4字节]
    /// Abort (0xFF) : [ctrl][原因][保留 3][PGN 4字节]
    /// </code>
    /// </summary>
    public class Msg_TpCm
    {
        /// <summary>目标 PGN</summary>
        public const UInt32 PGN = J1939Pgn.TpCm;

        /// <summary>控制字节</summary>
        public TpControl_e Control = TpControl_e.Bam;

        /// <summary>报文总长度（字节），RTS/EOM/BAM 有效</summary>
        public UInt16 TotalLength = 0;

        /// <summary>总包数（每包 7 字节），RTS/EOM/BAM 有效</summary>
        public byte TotalPackets = 0;

        /// <summary>最大包长（即每包可容纳字节数），仅 RTS 有效，通常为 7</summary>
        public byte MaxPacketSize = 7;

        /// <summary>可发送的包数，仅 CTS 有效</summary>
        public byte PacketsToSend = 0;

        /// <summary>下一包序号，仅 CTS 有效（从 1 开始）</summary>
        public byte NextPacketNumber = 1;

        /// <summary>中止原因，仅 Abort 有效</summary>
        public TpAbortReason_e AbortReason = TpAbortReason_e.AbortConnection;

        /// <summary>本次传输所对应的报文 PGN（18bit）</summary>
        public UInt32 MessagePgn = 0;

        /// <summary>目标地址 DA，BAM 时为 0xFF</summary>
        public byte DestinationAddress = J1939Addr.Global;

        /// <summary>源地址 SA</summary>
        public byte SourceAddress = 0;

        /// <summary>优先级，TP.CM 固定为 7</summary>
        public int Priority = 7;

        public Msg_TpCm() { }

        /// <summary>构造一个 BAM 连接管理报文</summary>
        public static Msg_TpCm CreateBam(UInt32 messagePgn, UInt16 totalLength, byte sourceAddress)
        {
            return new Msg_TpCm
            {
                Control = TpControl_e.Bam,
                MessagePgn = messagePgn,
                TotalLength = totalLength,
                TotalPackets = (byte)((totalLength + 6) / 7),
                DestinationAddress = J1939Addr.Global,
                SourceAddress = sourceAddress,
            };
        }

        /// <summary>构造一个 RTS 连接管理报文</summary>
        public static Msg_TpCm CreateRts(UInt32 messagePgn, UInt16 totalLength, byte destinationAddress, byte sourceAddress)
        {
            return new Msg_TpCm
            {
                Control = TpControl_e.Rts,
                MessagePgn = messagePgn,
                TotalLength = totalLength,
                TotalPackets = (byte)((totalLength + 6) / 7),
                MaxPacketSize = 7,
                DestinationAddress = destinationAddress,
                SourceAddress = sourceAddress,
            };
        }

        /// <summary>构造 CTS 报文</summary>
        public static Msg_TpCm CreateCts(UInt32 messagePgn, byte packetsToSend, byte nextPacketNumber, byte destinationAddress, byte sourceAddress)
        {
            return new Msg_TpCm
            {
                Control = TpControl_e.Cts,
                MessagePgn = messagePgn,
                PacketsToSend = packetsToSend,
                NextPacketNumber = nextPacketNumber,
                DestinationAddress = destinationAddress,
                SourceAddress = sourceAddress,
            };
        }

        /// <summary>构造 EOM 报文</summary>
        public static Msg_TpCm CreateEom(UInt32 messagePgn, UInt16 totalLength, byte destinationAddress, byte sourceAddress)
        {
            return new Msg_TpCm
            {
                Control = TpControl_e.EndOfMsg,
                MessagePgn = messagePgn,
                TotalLength = totalLength,
                TotalPackets = (byte)((totalLength + 6) / 7),
                DestinationAddress = destinationAddress,
                SourceAddress = sourceAddress,
            };
        }

        /// <summary>构造 Abort 报文</summary>
        public static Msg_TpCm CreateAbort(UInt32 messagePgn, TpAbortReason_e reason, byte destinationAddress, byte sourceAddress)
        {
            return new Msg_TpCm
            {
                Control = TpControl_e.Abort,
                MessagePgn = messagePgn,
                AbortReason = reason,
                DestinationAddress = destinationAddress,
                SourceAddress = sourceAddress,
            };
        }

        /// <summary>构造 29bit ID（PDU2，PS = 0xEC；点对点时为 DA，需 PDU1 语义）</summary>
        public J1939Id ToId()
        {
            return new J1939Id
            {
                Priority = Priority,
                PF = 0xEC,
                PS = DestinationAddress,
                SourceAddress = SourceAddress,
            };
        }

        /// <summary>序列化为 8 字节数据区</summary>
        public byte[] ToData()
        {
            var bs = new byte[8];
            bs[0] = (byte)Control;

            switch (Control)
            {
                case TpControl_e.Rts:
                    bs[1] = (byte)TotalLength;
                    bs[2] = (byte)(TotalLength >> 8);
                    bs[3] = TotalPackets;
                    bs[4] = MaxPacketSize;
                    break;

                case TpControl_e.Cts:
                    bs[1] = PacketsToSend;
                    bs[2] = NextPacketNumber;
                    bs[3] = 0xFF;
                    bs[4] = 0xFF;
                    break;

                case TpControl_e.EndOfMsg:
                case TpControl_e.Bam:
                    bs[1] = (byte)TotalLength;
                    bs[2] = (byte)(TotalLength >> 8);
                    bs[3] = TotalPackets;
                    bs[4] = 0xFF;
                    break;

                case TpControl_e.Abort:
                    bs[1] = (byte)AbortReason;
                    bs[2] = 0xFF;
                    bs[3] = 0xFF;
                    bs[4] = 0xFF;
                    break;
            }

            bs[5] = (byte)MessagePgn;
            bs[6] = (byte)(MessagePgn >> 8);
            bs[7] = (byte)(MessagePgn >> 16);

            return bs;
        }

        /// <summary>转为 CAN 帧</summary>
        public CanMsg ToCanMsg()
        {
            return ToId().ToCanMsg(ToData());
        }

        /// <summary>从 CAN 帧解析</summary>
        public static Msg_TpCm Parse(CanMsg msg)
        {
            if (msg == null || msg.Data == null || msg.Data.Length < 8) return null;

            var id = J1939Id.FromCanMsg(msg);
            var d = msg.Data;

            var pkt = new Msg_TpCm
            {
                Control = (TpControl_e)d[0],
                MessagePgn = (UInt32)(d[5] | (d[6] << 8) | (d[7] << 16)),
                DestinationAddress = id.PS,
                SourceAddress = id.SourceAddress,
                Priority = id.Priority,
            };

            switch (pkt.Control)
            {
                case TpControl_e.Rts:
                    pkt.TotalLength = (UInt16)(d[1] | (d[2] << 8));
                    pkt.TotalPackets = d[3];
                    pkt.MaxPacketSize = d[4];
                    break;

                case TpControl_e.Cts:
                    pkt.PacketsToSend = d[1];
                    pkt.NextPacketNumber = d[2];
                    break;

                case TpControl_e.EndOfMsg:
                case TpControl_e.Bam:
                    pkt.TotalLength = (UInt16)(d[1] | (d[2] << 8));
                    pkt.TotalPackets = d[3];
                    break;

                case TpControl_e.Abort:
                    pkt.AbortReason = (TpAbortReason_e)d[1];
                    break;
            }

            return pkt;
        }

        public override String ToString()
        {
            switch (Control)
            {
                case TpControl_e.Rts:
                    return $"TP.CM RTS PGN=0x{MessagePgn:X5} Len={TotalLength} Pkts={TotalPackets} DA=0x{DestinationAddress:X2} SA=0x{SourceAddress:X2}";
                case TpControl_e.Cts:
                    return $"TP.CM CTS PGN=0x{MessagePgn:X5} Count={PacketsToSend} Next={NextPacketNumber} DA=0x{DestinationAddress:X2} SA=0x{SourceAddress:X2}";
                case TpControl_e.EndOfMsg:
                    return $"TP.CM EOM PGN=0x{MessagePgn:X5} Len={TotalLength} Pkts={TotalPackets} DA=0x{DestinationAddress:X2} SA=0x{SourceAddress:X2}";
                case TpControl_e.Bam:
                    return $"TP.CM BAM PGN=0x{MessagePgn:X5} Len={TotalLength} Pkts={TotalPackets} SA=0x{SourceAddress:X2}";
                case TpControl_e.Abort:
                    return $"TP.CM ABORT PGN=0x{MessagePgn:X5} Reason={AbortReason} DA=0x{DestinationAddress:X2} SA=0x{SourceAddress:X2}";
                default:
                    return $"TP.CM Unknown(0x{(byte)Control:X2})";
            }
        }
    }

    #endregion

    #region TP.DT 数据传输

    /// <summary>
    /// TP.DT 数据传输，PGN 60160 (0x00EB00)。
    /// 数据区 8 字节：byte1 为包序号（1~1785），byte2~8 为 7 字节负载数据。
    /// 最后一包不足 7 字节时用 0xFF 填充。
    /// </summary>
    public class Msg_TpDt
    {
        /// <summary>目标 PGN</summary>
        public const UInt32 PGN = J1939Pgn.TpDt;

        /// <summary>每包携带的有效负载字节数</summary>
        public const int PayloadPerPacket = 7;

        /// <summary>包序号，从 1 开始</summary>
        public byte PacketNumber = 1;

        /// <summary>本包负载（最多 7 字节）</summary>
        public byte[] Payload = new byte[PayloadPerPacket];

        /// <summary>目标地址 DA，BAM 时为 0xFF</summary>
        public byte DestinationAddress = J1939Addr.Global;

        /// <summary>源地址 SA</summary>
        public byte SourceAddress = 0;

        /// <summary>优先级，TP.DT 固定为 7</summary>
        public int Priority = 7;

        public Msg_TpDt() { }

        public Msg_TpDt(byte packetNumber, byte[] payload, byte da, byte sa)
        {
            PacketNumber = packetNumber;
            DestinationAddress = da;
            SourceAddress = sa;

            if (payload != null)
            {
                var len = Math.Min(payload.Length, PayloadPerPacket);
                for (int i = 0; i < len; i++) Payload[i] = payload[i];
            }
        }

        /// <summary>从一段数据中切分出第 n 包（n 从 1 开始）</summary>
        public static Msg_TpDt FromChunk(byte[] data, int packetNumber, byte da, byte sa)
        {
            var offset = (packetNumber - 1) * PayloadPerPacket;
            var bs = new byte[PayloadPerPacket];
            for (int i = 0; i < PayloadPerPacket; i++)
            {
                var idx = offset + i;
                bs[i] = idx < data.Length ? data[idx] : (byte)0xFF;
            }

            return new Msg_TpDt((byte)packetNumber, bs, da, sa);
        }

        /// <summary>构造 29bit ID</summary>
        public J1939Id ToId()
        {
            return new J1939Id
            {
                Priority = Priority,
                PF = 0xEB,
                PS = DestinationAddress,
                SourceAddress = SourceAddress,
            };
        }

        /// <summary>序列化为 8 字节数据区</summary>
        public byte[] ToData()
        {
            var bs = new byte[8];
            bs[0] = PacketNumber;
            for (int i = 0; i < PayloadPerPacket; i++) bs[i + 1] = Payload[i];
            return bs;
        }

        /// <summary>转为 CAN 帧</summary>
        public CanMsg ToCanMsg()
        {
            return ToId().ToCanMsg(ToData());
        }

        /// <summary>从 CAN 帧解析</summary>
        public static Msg_TpDt Parse(CanMsg msg)
        {
            if (msg == null || msg.Data == null || msg.Data.Length < 8) return null;

            var id = J1939Id.FromCanMsg(msg);
            var pkt = new Msg_TpDt
            {
                PacketNumber = msg.Data[0],
                DestinationAddress = id.PS,
                SourceAddress = id.SourceAddress,
                Priority = id.Priority,
            };

            for (int i = 0; i < PayloadPerPacket; i++) pkt.Payload[i] = msg.Data[i + 1];

            return pkt;
        }

        public override String ToString()
        {
            return $"TP.DT #{PacketNumber} DA=0x{DestinationAddress:X2} SA=0x{SourceAddress:X2} [{Payload.ToHex(" ")}]";
        }
    }

    #endregion

    #region TP 接收侧重组

    /// <summary>
    /// 完成一次 TP 多包传输后的结果
    /// </summary>
    public class J1939TpResult
    {
        /// <summary>原始报文 PGN</summary>
        public UInt32 MessagePgn;

        /// <summary>源地址</summary>
        public byte SourceAddress;

        /// <summary>拼接完成的完整数据</summary>
        public byte[] Data;
    }

    /// <summary>
    /// J1939 传输协议（TP）接收侧重组件。
    /// 处理 TP.CM(BAM/RTS) 与 TP.DT，按「源地址 + 目标 PGN」缓存并拼包，
    /// 拼满后触发 <see cref="Completed"/>。
    /// 用法：把收到的每一帧都喂给 <see cref="Process"/>。
    /// </summary>
    public class J1939TpReassembler
    {
        /// <summary>单包负载字节数</summary>
        public const int PayloadPerPacket = 7;

        /// <summary>单次拼包会话</summary>
        class Session
        {
            public byte SourceAddress;
            public byte DestinationAddress;
            public UInt32 MessagePgn;
            public int TotalLength;
            public int TotalPackets;
            public int NextPacket;          // 期望收到的下一包序号
            public byte[] Buffer;
            public DateTime LastTick;
        }

        readonly Dictionary<String, Session> _sessions = new Dictionary<String, Session>();
        readonly Object _lock = new Object();

        /// <summary>会话超时时间（毫秒），超时自动丢弃</summary>
        public int TimeoutMs = 1000;

        /// <summary>拼包完成事件</summary>
        public event EventHandler<J1939TpResult> Completed;

        /// <summary>异常/中止事件</summary>
        public event EventHandler<String> Aborted;

        static String KeyOf(byte sa, byte da, UInt32 pgn) => $"{sa:X2}-{da:X2}-{pgn:X5}";

        /// <summary>
        /// 处理一帧。若是 TP 相关帧则纳入拼包流程，返回到目前为止是否处理了该帧。
        /// </summary>
        public bool Process(CanMsg msg)
        {
            if (msg == null) return false;

            var id = J1939Id.FromCanMsg(msg);
            var pgn = id.PGNValue;
            var data = msg.Data;
            if (data == null || data.Length < 1) return false;

            if (pgn == J1939Pgn.TpCm) return ProcessTpCm(id, data);
            if (pgn == J1939Pgn.TpDt) return ProcessTpDt(id, data);

            return false;
        }

        bool ProcessTpCm(J1939Id id, byte[] data)
        {
            var cm = Msg_TpCm.Parse(new CanMsg((Int32)id.ID, data, false, true));
            if (cm == null) return false;

            lock (_lock)
            {
                Cleanup();

                var key = KeyOf(cm.SourceAddress, cm.DestinationAddress, cm.MessagePgn);

                switch (cm.Control)
                {
                    case TpControl_e.Bam:
                    case TpControl_e.Rts:
                        _sessions[key] = new Session
                        {
                            SourceAddress = cm.SourceAddress,
                            DestinationAddress = cm.DestinationAddress,
                            MessagePgn = cm.MessagePgn,
                            TotalLength = cm.TotalLength,
                            TotalPackets = cm.TotalPackets,
                            NextPacket = 1,
                            Buffer = new byte[cm.TotalLength],
                            LastTick = DateTime.Now,
                        };
                        break;

                    case TpControl_e.EndOfMsg:
                        // 发送方正常结束
                        _sessions.Remove(key);
                        break;

                    case TpControl_e.Abort:
                        _sessions.Remove(key);
                        Aborted?.Invoke(this, $"TP 中止: PGN=0x{cm.MessagePgn:X5} SA=0x{cm.SourceAddress:X2} 原因={cm.AbortReason}");
                        break;
                }
            }

            return true;
        }

        bool ProcessTpDt(J1939Id id, byte[] data)
        {
            lock (_lock)
            {
                Cleanup();
                foreach (var kv in _sessions.ToArray())
                {
                    var s = kv.Value;
                    if (s.SourceAddress != id.SourceAddress) continue;

                    var seq = data[0];
                    if (seq != s.NextPacket) continue;

                    // 拷贝 7 字节有效负载
                    var offset = (seq - 1) * PayloadPerPacket;
                    for (int i = 0; i < PayloadPerPacket; i++)
                    {
                        var idx = offset + i;
                        if (idx < s.Buffer.Length) s.Buffer[idx] = data[i + 1];
                    }

                    s.NextPacket++;
                    s.LastTick = DateTime.Now;

                    // 全部收齐
                    if (s.NextPacket > s.TotalPackets)
                    {
                        var result = new J1939TpResult
                        {
                            MessagePgn = s.MessagePgn,
                            SourceAddress = s.SourceAddress,
                            Data = s.Buffer,
                        };
                        _sessions.Remove(kv.Key);
                        Completed?.Invoke(this, result);
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>清理超时会话</summary>
        void Cleanup()
        {
            var now = DateTime.Now;
            foreach (var kv in _sessions.ToArray())
            {
                if ((now - kv.Value.LastTick).TotalMilliseconds > TimeoutMs)
                {
                    _sessions.Remove(kv.Key);
                    Aborted?.Invoke(this, $"TP 超时: PGN=0x{kv.Value.MessagePgn:X5} SA=0x{kv.Value.SourceAddress:X2}");
                }
            }
        }

        /// <summary>清空所有会话</summary>
        public void Clear()
        {
            lock (_lock) _sessions.Clear();
        }
    }

    #endregion

    #region TP 发送侧

    /// <summary>
    /// TP 发送结果
    /// </summary>
    public enum TpSendState_e
    {
        /// <summary>空闲</summary>
        Idle = 0,

        /// <summary>发送中</summary>
        Sending = 1,

        /// <summary>发送完成</summary>
        Ok = 2,

        /// <summary>忙（已有会话进行中）</summary>
        Busy = 3,

        /// <summary>超时失败</summary>
        Timeout = 4,

        /// <summary>被对方中止</summary>
        Aborted = 5,

        /// <summary>参数非法</summary>
        Error = 6,
    }

    /// <summary>
    /// J1939 传输协议（TP）发送侧组件。
    /// 支持两种模式：
    /// <list type="bullet">
    /// <item>BAM 广播：无需对方应答，按 50ms 间隔主动发完所有 TP.DT。</item>
    /// <item>RTS/CTS 点对点：发送 RTS 后等待对方 CTS，按 CTS 给定的「包数 + 起始序号」发送 TP.DT，
    ///       全部发完后再等对方的 EOM 确认。</item>
    /// </list>
    /// 线程模型：<see cref="SendBam"/> / <see cref="SendRts"/> 会在**调用线程上阻塞**直到完成或超时；
    /// 因此调用方必须在**非接收线程**上发起（例如业务/UI 线程），
    /// 而接收线程需把收到的帧喂给 <see cref="Process"/> 以推进状态机。
    /// BAM 模式全在调用线程完成，不依赖 <see cref="Process"/>。
    /// 用法：
    /// <list type="number">
    /// <item><see cref="Bind"/> 绑定 CAN 端口。</item>
    /// <item>接收线程把每帧喂给 <see cref="Process"/>。</item>
    /// <item>业务线程调用 <see cref="SendBam"/> 或 <see cref="SendRts"/> 发起传输。</item>
    /// </list>
    /// </summary>
    public class J1939TpSender
    {
        /// <summary>单包负载字节数</summary>
        public const int PayloadPerPacket = 7;

        /// <summary>BAM 模式下每包之间的最小间隔（毫秒），协议要求 50ms 左右</summary>
        public int BamIntervalMs = 50;

        /// <summary>等待对方 CTS / EOM 的超时时间（毫秒）</summary>
        public int ReceiveTimeoutMs = 1000;

        /// <summary>单次 CTS 允许发送的最大包数（协议 1~16，常用 16）</summary>
        public byte MaxPacketsPerCts = 16;

        /// <summary>发送状态</summary>
        public TpSendState_e State { get; private set; } = TpSendState_e.Idle;

        /// <summary>待发送的完整数据</summary>
        byte[] _data;

        /// <summary>当前报文 PGN</summary>
        UInt32 _messagePgn;

        /// <summary>目标地址 DA，BAM 为 0xFF</summary>
        byte _destinationAddress;

        /// <summary>本机源地址</summary>
        byte _sourceAddress;

        /// <summary>是否使用 RTS/CTS 点对点模式</summary>
        bool _useRts;

        /// <summary>下一包待发序号（1 起始）</summary>
        int _nextPacket;

        /// <summary>总包数</summary>
        int _totalPackets;

        /// <summary>最近一次收发时刻，用于超时判定</summary>
        DateTime _lastTick;

        /// <summary>完成信号量，RTS 模式阻塞等待用</summary>
        readonly System.Threading.ManualResetEventSlim _done = new System.Threading.ManualResetEventSlim(false);

        readonly Object _lock = new Object();

        /// <summary>当前使用的 CAN 端口</summary>
        ICanPort _transport;

        /// <summary>发送完成/失败事件</summary>
        public event EventHandler<TpSendState_e> Finished;

        /// <summary>绑定 CAN 端口</summary>
        public void Bind(ICanPort transport) => _transport = transport;

        /// <summary>发起一次 BAM 广播传输（阻塞直到发完）</summary>
        /// <param name="messagePgn">被传输报文的 PGN</param>
        /// <param name="data">完整数据</param>
        /// <param name="sourceAddress">本机源地址</param>
        public TpSendState_e SendBam(UInt32 messagePgn, byte[] data, byte sourceAddress)
        {
            return Send(messagePgn, data, sourceAddress, J1939Addr.Global, false);
        }

        /// <summary>发起一次 BAM 广播传输（阻塞直到发完），并绑定端口</summary>
        public TpSendState_e SendBam(ICanPort transport, UInt32 messagePgn, byte[] data, byte sourceAddress)
        {
            Bind(transport);
            return Send(messagePgn, data, sourceAddress, J1939Addr.Global, false);
        }

        /// <summary>发起一次 RTS/CTS 点对点传输（阻塞直到完成或超时）</summary>
        public TpSendState_e SendRts(ICanPort transport, UInt32 messagePgn, byte[] data, byte sourceAddress, byte destinationAddress)
        {
            Bind(transport);
            return Send(messagePgn, data, sourceAddress, destinationAddress, true);
        }

        TpSendState_e Send(UInt32 messagePgn, byte[] data, byte sourceAddress, byte destinationAddress, bool useRts)
        {
            lock (_lock)
            {
                if (State == TpSendState_e.Sending) return TpSendState_e.Busy;
                if (_transport == null || data == null || data.Length == 0 || data.Length > 1785) return TpSendState_e.Error;

                _data = data;
                _messagePgn = messagePgn;
                _sourceAddress = sourceAddress;
                _destinationAddress = destinationAddress;
                _useRts = useRts;
                _totalPackets = (data.Length + PayloadPerPacket - 1) / PayloadPerPacket;
                _nextPacket = 1;
                _lastTick = DateTime.Now;
                _done.Reset();
                State = TpSendState_e.Sending;
            }

            try
            {
                if (useRts) SendRtsCore();
                else SendBamCore();

                var res = State;
                Finished?.Invoke(this, res);
                return res;
            }
            finally
            {
                lock (_lock)
                {
                    if (State == TpSendState_e.Sending) State = TpSendState_e.Idle;
                }
            }
        }

        /// <summary>BAM：无需应答，按序发完所有 TP.DT</summary>
        void SendBamCore()
        {
            var cm = Msg_TpCm.CreateBam(_messagePgn, (UInt16)_data.Length, _sourceAddress);
            _transport.Send(cm.ToCanMsg());

            for (int seq = 1; seq <= _totalPackets; seq++)
            {
                var dt = Msg_TpDt.FromChunk(_data, seq, J1939Addr.Global, _sourceAddress);
                _transport.Send(dt.ToCanMsg());
                System.Threading.Thread.Sleep(BamIntervalMs);
            }

            lock (_lock) State = TpSendState_e.Ok;
        }

        /// <summary>RTS/CTS：发 RTS → 等 CTS（可能多轮）→ 发 DT → 等 EOM</summary>
        void SendRtsCore()
        {
            // 1. 发 RTS
            var rts = Msg_TpCm.CreateRts(_messagePgn, (UInt16)_data.Length, _destinationAddress, _sourceAddress);
            _transport.Send(rts.ToCanMsg());

            // 2. 轮询等待状态机推进（Process 由接收线程调用）。
            //    用小块超时轮询而非一次性死等，便于会话超时后发 Abort。
            var deadline = DateTime.Now.AddMilliseconds(ReceiveTimeoutMs);
            while (DateTime.Now < deadline)
            {
                var st = State;
                if (st != TpSendState_e.Sending) return;

                _done.Wait(10);
                _done.Reset();

                // 数据已全部发完，只剩等 EOM
                if (_nextPacket > _totalPackets)
                {
                    // 继续等 EOM
                }
            }

            lock (_lock)
            {
                if (State == TpSendState_e.Sending)
                {
                    var abort = Msg_TpCm.CreateAbort(_messagePgn, TpAbortReason_e.Timeout, _destinationAddress, _sourceAddress);
                    _transport.Send(abort.ToCanMsg());
                    State = TpSendState_e.Timeout;
                }
            }
        }

        /// <summary>
        /// 处理接收到的帧，推进 RTS/CTS 状态机。
        /// 返回是否为本组件相关帧（TP.CM 的 CTS/EOM/Abort）。
        /// </summary>
        public bool Process(CanMsg msg)
        {
            if (msg == null) return false;
            if (State != TpSendState_e.Sending) return false;
            if (!_useRts) return false;

            var id = J1939Id.FromCanMsg(msg);
            if (id.PGNValue != J1939Pgn.TpCm) return false;

            var cm = Msg_TpCm.Parse(msg);
            if (cm == null) return false;
            if (cm.MessagePgn != _messagePgn) return false;
            if (cm.SourceAddress != _destinationAddress) return false;

            _lastTick = DateTime.Now;

            switch (cm.Control)
            {
                case TpControl_e.Cts:
                    // 对方允许发送若干包，从指定序号开始
                    SendWindow(cm.PacketsToSend, cm.NextPacketNumber);
                    _done.Set();
                    return true;

                case TpControl_e.EndOfMsg:
                    // 对方确认收齐
                    lock (_lock) State = TpSendState_e.Ok;
                    _done.Set();
                    return true;

                case TpControl_e.Abort:
                    lock (_lock) State = TpSendState_e.Aborted;
                    _done.Set();
                    return true;
            }

            return false;
        }

        /// <summary>按 CTS 许可发送一个窗口的 TP.DT</summary>
        void SendWindow(byte packetsToSend, byte nextPacketNumber)
        {
            // CTS 的包数 0 表示请求停止发送
            if (packetsToSend == 0)
            {
                lock (_lock) State = TpSendState_e.Aborted;
                return;
            }

            var start = nextPacketNumber;
            var count = Math.Min(packetsToSend, MaxPacketsPerCts);

            for (int i = 0; i < count; i++)
            {
                var seq = start + i;
                if (seq < 1 || seq > _totalPackets) break;

                var dt = Msg_TpDt.FromChunk(_data, seq, _destinationAddress, _sourceAddress);
                _transport?.Send(dt.ToCanMsg());
                _nextPacket = seq + 1;
            }
        }

        /// <summary>中止当前传输并通知对方</summary>
        public void Abort(TpAbortReason_e reason = TpAbortReason_e.AbortConnection)
        {
            lock (_lock)
            {
                if (State != TpSendState_e.Sending) return;

                var abort = Msg_TpCm.CreateAbort(_messagePgn, reason, _destinationAddress, _sourceAddress);
                _transport?.Send(abort.ToCanMsg());

                State = TpSendState_e.Aborted;
            }

            _done.Set();
        }

        /// <summary>释放资源</summary>
        public void Close()
        {
            _done.Set();
            _done.Dispose();
        }
    }

    #endregion

    #region TP 门面（每端口单例）

    /// <summary>
    /// J1939 传输协议（TP）门面类。
    /// 把「接收侧重组」与「发送侧会话」合为一个 <see cref="ICanRx"/> 处理器，
    /// 构造时自动注册到指定 <see cref="ICanPort"/>，并在该端口上按 key 挂载为端口级单例：
    /// <code>
    /// var tp = port.GetOrAdd(J1939Tp.KeyItems, () => new J1939Tp(port));
    /// tp.Completed += (s, e) => { /* 收到完整大包 */ };
    /// tp.SendBam(0x00FECA, data, sa: 0x00);
    /// </code>
    /// 线程模型：
    /// <list type="bullet">
    /// <item><see cref="Process"/> 由接收线程回调，负责收包重组与推进发送侧 CTS/EOM 状态机。</item>
    /// <item><see cref="SendBam"/> / <see cref="SendRts"/> 在调用线程阻塞，
    ///       **禁止在接收线程或 <see cref="Completed"/> 事件中调用**，否则会死锁。</item>
    /// </list>
    /// </summary>
    public class J1939Tp : ICanRx
    {
        /// <summary>在 <see cref="ICanPort"/> 上挂载本对象所使用的字典键</summary>
        public const String KeyItems = "J1939Tp";

        /// <summary>本对象所属的 CAN 端口</summary>
        public ICanPort Port { get; }

        /// <summary>接收侧重组器</summary>
        public J1939TpReassembler Reassembler { get; }

        /// <summary>发送侧会话管理器</summary>
        public J1939TpSender Sender { get; }

        /// <summary>
        /// 构造并注册到端口。
        /// 建议通过 <c>port.GetOrAdd(J1939Tp.KeyItems, () =&gt; new J1939Tp(port))</c> 获取，
        /// 以保证同一端口只有一个实例。
        /// </summary>
        /// <param name="port">CAN 端口</param>
        public J1939Tp(ICanPort port)
        {
            if (port == null) throw new ArgumentNullException(nameof(port));

            Port = port;

            // 接收侧：TP.CM + TP.DT
            Reassembler = new J1939TpReassembler();
            // 转发重组事件（含完整大包与中止/超时）
            Reassembler.Completed += (s, e) => Completed?.Invoke(this, e);
            Reassembler.Aborted += (s, e) => Aborted?.Invoke(this, e);

            // 发送侧：绑定端口，其 Process(CanMsg) 由本门面的 Process 转发
            Sender = new J1939TpSender();
            Sender.Bind(port);

            // 端口不再做 CAN ID 匹配，本处理器会收到该端口所有帧，
            // 由 Process 内部按 PGN 过滤 TP.CM / TP.DT。
            if (!port.Register(this)) throw new InvalidOperationException("该端口已注册 J1939 TP 处理器");
        }

        /// <summary>收到完整 TP 多包报文（重组完成）</summary>
        public event EventHandler<J1939TpResult> Completed;

        /// <summary>TP 中止 / 超时事件</summary>
        public event EventHandler<String> Aborted;

        /// <summary>
        /// 接收线程回调：把帧分发给接收侧重组器与发送侧状态机。
        /// 仅当确实属于本机制（TP.CM / TP.DT）时返回 true，避免端口写"未识别"日志。
        /// </summary>
        public override bool Process(CanMsg msg, ICanPort port)
        {
            if (msg == null) return false;

            var id = J1939Id.FromCanMsg(msg);
            var pgn = id.PGNValue;

            // 非 TP 帧：不处理（可能被端口日志记录，属正常）
            if (pgn != J1939Pgn.TpCm && pgn != J1939Pgn.TpDt) return false;

            // 发送侧优先：我们在等 CTS/EOM/Abort 时，先让发送侧状态机消费
            if (Sender.Process(msg)) return true;

            // 否则作为接收侧（对方向我们发起 RTS/BAM，或发来 TP.DT）
            return Reassembler.Process(msg);
        }

        /// <summary>发起一次 BAM 广播传输（阻塞）</summary>
        /// <param name="messagePgn">被传输报文的 PGN</param>
        /// <param name="data">完整数据</param>
        /// <param name="sourceAddress">本机源地址</param>
        public TpSendState_e SendBam(UInt32 messagePgn, byte[] data, byte sourceAddress)
        {
            return Sender.SendBam(messagePgn, data, sourceAddress);
        }

        /// <summary>发起一次 RTS/CTS 点对点传输（阻塞）</summary>
        /// <param name="messagePgn">被传输报文的 PGN</param>
        /// <param name="data">完整数据</param>
        /// <param name="sourceAddress">本机源地址</param>
        /// <param name="destinationAddress">目标地址</param>
        public TpSendState_e SendRts(UInt32 messagePgn, byte[] data, byte sourceAddress, byte destinationAddress)
        {
            return Sender.SendRts(Port, messagePgn, data, sourceAddress, destinationAddress);
        }

        /// <summary>当前发送状态</summary>
        public TpSendState_e State => Sender.State;

        /// <summary>接收侧会话超时时间（毫秒）</summary>
        public int TimeoutMs
        {
            get => Reassembler.TimeoutMs;
            set => Reassembler.TimeoutMs = value;
        }

        /// <summary>BAM 每包发送间隔（毫秒）</summary>
        public int BamIntervalMs
        {
            get => Sender.BamIntervalMs;
            set => Sender.BamIntervalMs = value;
        }

        /// <summary>从端口注销并释放资源</summary>
        public void Close()
        {
            try { Port?.Unregister(this); } catch { }
            try { Port?.RemoveItem(KeyItems); } catch { }

            Sender?.Close();
            Reassembler?.Clear();
        }
    }

    #endregion
}
