using ALib.CAN;
using NewLife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ALib.Can_Uds
{
    /*
    CAN报文:  ID  R  E  DATA[8]
    ISOTP报文： TA/SA   PCI   DATA
    
    TA/SA 来源于：
    1.扩展帧+普通寻址： CANID(29bit)截取
    2.标准帧+普通寻址： CANID(11bit)截取N，用N查表得到（不查表也行，就用N表示）
    3.标准帧+扩展寻址： DATA[0]
    4.扩展帧+扩展寻址： CANID(29bit)截取+DATA[0]，极不常用
    TA是目的地址，SA是源地址。

    PCI 来源于：
    1.普通寻址：DATA[0]
    2.扩展寻址：DATA[1]
    组成： 帧类型4bit(IsoTpFrameType_e) + 参数4bit
    4种帧类型
    SF，参数：数据长度（1-7）
    FF，参数：和下1自己组成包长度（0-4095）
    CF，参数：包序列号（0-15循环）
    FC，参数：流控状态（IsoTpFlowStatus_e）

    ISOTP 不存在远程帧 R 类型。
    */
    #region 枚举

    /// <summary>
    /// 寻址模式，决定 CAN ID 的分配方式与数据区地址字节的布局
    /// </summary>
    public enum IsoTpAddressing_e
    {
        /// <summary>普通寻址：CAN ID 区分收发与目标，数据区首字节即 PCI</summary>
        Normal = 0,

        /// <summary>扩展寻址：数据区首字节为地址（TA/SA），其后才是 PCI</summary>
        Extended = 1,

        /// <summary>混合寻址：29bit ID 含地址信息，数据区首字节为地址扩展</summary>
        Mixed = 2,
    }

    /// <summary>
    /// PCI 协议控制信息类型（高 4 位）
    /// </summary>
    public enum IsoTpFrameType_e : byte
    {
        /// <summary>单帧 SF (0x0)</summary>
        SingleFrame = 0x0,

        /// <summary>首帧 FF (0x1)</summary>
        FirstFrame = 0x1,

        /// <summary>连续帧 CF (0x2)</summary>
        ConsecutiveFrame = 0x2,

        /// <summary>流控帧 FC (0x3)</summary>
        FlowControl = 0x3,
    }

    /// <summary>
    /// 流控帧状态 FS
    /// </summary>
    public enum IsoTpFlowStatus_e : byte
    {
        /// <summary>继续发送 ContinueToSend</summary>
        ContinueToSend = 0,

        /// <summary>等待 Wait</summary>
        Wait = 1,

        /// <summary>溢出 OverFlow</summary>
        OverFlow = 2,
    }

    #endregion

    #region 定时参数

    /// <summary>
    /// ISO 15765-2 网络层定时参数（单位毫秒）。
    /// 默认值取标准推荐值，可按 ECU 实际情况调整。
    /// </summary>
    public class IsoTpTiming
    {
        /// <summary>发送方发送一帧到收到流控帧的最大时间 N_Bs（默认 1000ms）</summary>
        public int N_Bs = 1000;

        /// <summary>接收方收到首帧到发出流控帧的时间 N_Br（默认 0，仅约束实现，用于日志）</summary>
        public int N_Br = 0;

        /// <summary>接收方可等待下一连续帧的最大时间 N_Cr（默认 1000ms）</summary>
        public int N_Cr = 1000;

        /// <summary>连续帧之间的最小间隔 STmin（默认 0ms）</summary>
        public int STmin = 0;

        /// <summary>单次请求（含多帧收发）整体超时（默认 5000ms）</summary>
        public int Timeout = 5000;
    }

    #endregion

    #region 消息单元

    /// <summary>
    /// ISO-TP 收发的一个完整消息单元（已重组的多帧报文，或单帧报文）
    /// </summary>
    public class IsoTpMessage
    {
        /// <summary>发送方 CAN ID</summary>
        public UInt32 SourceId;

        /// <summary>接收方 CAN ID</summary>
        public UInt32 TargetId;

        /// <summary>完整数据（不含 PCI 与地址字节）</summary>
        public byte[] Data;

        /// <summary>是否使用扩展寻址（数据区首字节为地址）</summary>
        public bool Extended;

        /// <summary>扩展寻址时的地址字节值</summary>
        public byte Address;

        /// <summary>是否远程帧</summary>
        public bool Remote;

        /// <summary>是否为扩展帧（29bit ID）</summary>
        public bool ExternId;

        public override String ToString()
        {
            var hex = Data == null ? "" : Data.ToHex(" ");
            return $"ID=0x{SourceId:X8} Data=[{Data?.Length ?? 0}] {hex}";
        }
    }

    #endregion

    #region ISO-TP 通道

    /// <summary>
    /// ISO 15765-2（ISO-TP）传输层通道，基于 <see cref="ICanPort"/> 之上实现多帧拆包与重组。
    /// <para>支持普通寻址与扩展寻址；<b>不支持混合寻址</b>（混合寻址需要 29bit ID 解析地址映射表，实际项目少见）。</para>
    /// <para>线程模型：</para>
    /// <list type="bullet">
    /// <item><see cref="Process"/> 由接收线程回调，负责接收侧重组与发送侧流控状态机推进。</item>
    /// <item><see cref="Send"/>/<see cref="SendMultiFrame"/> 在调用线程阻塞，<b>禁止在接收线程或 <see cref="MessageReceived"/> 事件中调用</b>，否则死锁。</item>
    /// </list>
    /// <para>用法：<c>var tp = port.GetOrAdd(IsoTpChannel.KeyItems, () =&gt; new IsoTpChannel(port, 0x7E0, 0x7E8));</c></para>
    /// </summary>
    public class IsoTpChannel : ICanRx
    {
        /// <summary>在 <see cref="ICanPort"/> 上挂载本对象的字典键</summary>
        public const String KeyItems = "IsoTp";

        /// <summary>经典 CAN 单帧最大数据字节数</summary>
        public const int CanFrameDataSize = 8;

        /// <summary>普通寻址下 FF/SF 单帧可携带的最大数据长度（经典 CAN）</summary>
        public const int SingleFrameMaxLength = 7;

        /// <summary>普通寻址下 CF 每帧可携带的数据长度</summary>
        public const int MultiFramePayloadPerPacket = 7;

        /// <summary>经典 CAN 多帧最大总长度（12bit 长度域上限 4095）</summary>
        public const int MaxMessageLength = 4095;

        /// <summary>所属 CAN 端口</summary>
        public ICanPort Port { get; }

        /// <summary>物理寻址：发送请求所用的 CAN ID（本机 → ECU）</summary>
        public UInt32 RequestId { get; set; }

        /// <summary>物理寻址：接收响应所用的 CAN ID（ECU → 本机）</summary>
        public UInt32 ResponseId { get; set; }

        /// <summary>功能寻址 CAN ID，默认 0x7DF，0 表示不启用</summary>
        public UInt32 FunctionId { get; set; } = 0x7DF;

        /// <summary>寻址模式，默认普通寻址</summary>
        public IsoTpAddressing_e Addressing = IsoTpAddressing_e.Normal;

        /// <summary>定时参数</summary>
        public IsoTpTiming Timing { get; set; } = new IsoTpTiming();

        /// <summary>是否使用扩展帧（29bit ID），默认 false 使用标准帧</summary>
        public bool ExternId = false;

        /// <summary>发送连续帧时使用的块大小 BS（0 表示不限）</summary>
        public byte BlockSize = 0;

        /// <summary>发送连续帧时使用的最小间隔 STmin（毫秒）</summary>
        public int SendSTmin = 0;

        /// <summary>接收方发给发送方的流控参数：块大小 BS</summary>
        public byte RxBlockSize = 0;

        /// <summary>接收方发给发送方的流控参数：最小间隔 STmin（毫秒）</summary>
        public int RxSTmin = 0;

        /// <summary>接收缓冲区大小（单次组包的最大字节数）</summary>
        public int RxBufferSize = 4095;

        /// <summary>日志</summary>
        public NewLife.Log.ILog Log { get; set; } = NewLife.Log.Logger.Null;

        /// <summary>收到一个完整的（已重组的）消息单元</summary>
        public event EventHandler<IsoTpMessage> MessageReceived;

        /// <summary>协议异常/超时事件</summary>
        public event EventHandler<String> Error;

        #region 内部字段

        /// <summary>接收侧组包会话（普通模式为响应 ID，扩展模式为响应 ID + 地址）</summary>
        class RxSession
        {
            public UInt32 Id;
            public byte Address;
            public byte[] Buffer;
            public int TotalLength;
            public int TotalPackets;
            public int NextPacket;
            public DateTime LastTick;
        }

        readonly Dictionary<String, RxSession> _rxSessions = new Dictionary<String, RxSession>();
        readonly Object _lock = new Object();

        /// <summary>发送侧当前等待的流控帧</summary>
        readonly ManualResetEventSlim _fcEvent = new ManualResetEventSlim(false);

        IsoTpFlowStatus_e _fcStatus = IsoTpFlowStatus_e.ContinueToSend;
        byte _fcBlockSize = 0;
        int _fcSTmin = 0;
        volatile bool _sending = false;
        volatile bool _abortSend = false;
        int _sendNextPacket = 1;
        bool _sendExtended = false;
        byte _sendAddress = 0;

        #endregion

        /// <summary>构造并注册到端口</summary>
        /// <param name="port">CAN 端口</param>
        /// <param name="requestId">物理请求 CAN ID（如 0x7E0）</param>
        /// <param name="responseId">物理响应 CAN ID（如 0x7E8）</param>
        public IsoTpChannel(ICanPort port, UInt32 requestId, UInt32 responseId)
        {
            Port = port ?? throw new ArgumentNullException(nameof(port));
            RequestId = requestId;
            ResponseId = responseId;

            if (!port.Register(this)) throw new InvalidOperationException("该端口已注册 ISO-TP 处理器");
        }

        /// <summary>按端口自动分配 ID（ECU 序号 0~15）：请求 0x7E0+2n，响应 0x7E8+2n</summary>
        /// <param name="port">CAN 端口</param>
        /// <param name="ecuIndex">ECU 序号</param>
        public static IsoTpChannel CreateByEcu(ICanPort port, int ecuIndex)
        {
            ecuIndex &= 0x0F;
            return new IsoTpChannel(port, (UInt32)(0x7E0 + 2 * ecuIndex), (UInt32)(0x7E8 + 2 * ecuIndex));
        }

        #region 帧收发

        /// <summary>发送一帧原始 CAN 帧</summary>
        void SendFrame(UInt32 id, byte[] data)
        {
            var len = data == null ? 0 : Math.Min(data.Length, CanFrameDataSize);
            var bs = new byte[len];
            if (len > 0) Array.Copy(data, bs, len);

            Port.Send(id, bs, false, ExternId);
        }

        /// <summary>协议异常</summary>
        void RaiseError(String message)
        {
            Log?.Info("[ISO-TP] " + message);
            Error?.Invoke(this, message);
        }

        #endregion

        #region 接收

        /// <summary>
        /// 接收线程回调：处理本通道关心的所有 ISO-TP 帧。
        /// </summary>
        public override bool Process(CanMsg msg, ICanPort port)
        {
            if (msg == null) return false;

            var id = (UInt32)msg.ID;

            // 忽略远程帧
            if (msg.RemoteFlag) return false;

            // 发送侧：等待流控帧（流控由对方发回，故使用响应 ID，与请求 ID 相同的特殊映射除外）
            if (_sending && (id == ResponseId || id == RequestId))
            {
                var d = msg.Data;
                if (d != null && d.Length >= 3)
                {
                    var (pci, addr) = SplitAddress(d);
                    if ((pci >> 4) == (byte)IsoTpFrameType_e.FlowControl)
                    {
                        var off = PayloadIndex;
                        OnFlowControl((IsoTpFlowStatus_e)(pci & 0x0F), d[off + 1], d[off + 2]);
                        return true;
                    }
                }
            }

            // 接收侧：物理响应 ID 或功能寻址 ID
            var isResponse = id == ResponseId;
            var isFunction = FunctionId != 0 && id == FunctionId;
            if (!isResponse && !isFunction) return false;

            return ProcessRx(msg, d: msg.Data);
        }

        /// <summary>
        /// 快速预匹配：忽略远程帧，仅当帧 ID 属于本通道关切的
        /// 请求 / 响应 / 功能寻址之一时才认领，供端口层在分发前过滤无关帧
        /// （如 J1939、CANopen 报文），避免 IsoTpChannel 对每个帧都做 ID 比对与状态机查询。
        /// 最终是否消费仍以 <see cref="Process"/> 为准（例如发送侧流控帧仅在 <c>_sending</c> 时认领）。
        /// </summary>
        public override bool Match(CanMsg msg)
        {
            if (msg == null) return false;
            if (msg.RemoteFlag) return false;                    // ISO-TP 不存在远程帧

            var id = (UInt32)msg.ID;
            if (id == RequestId) return true;
            if (id == ResponseId) return true;
            if (FunctionId != 0 && id == FunctionId) return true;
            return false;
        }

        bool ProcessRx(CanMsg msg, byte[] d)
        {
            if (d == null || d.Length < 1) return false;

            var (pci, addr) = SplitAddress(d);
            var type = (IsoTpFrameType_e)(pci >> 4);

            switch (type)
            {
                case IsoTpFrameType_e.SingleFrame:
                    return OnSingleFrame(msg, pci, addr, d);

                case IsoTpFrameType_e.FirstFrame:
                    return OnFirstFrame(msg, pci, addr, d);

                case IsoTpFrameType_e.ConsecutiveFrame:
                    return OnConsecutiveFrame(msg, addr, d);

                case IsoTpFrameType_e.FlowControl:
                    // 接收侧收到流控帧：非本通道语义，忽略
                    return false;
            }

            return false;
        }

        /// <summary>按寻址模式拆分地址字节与 PCI 字节</summary>
        (byte Pci, byte Addr) SplitAddress(byte[] data)
        {
            if (Addressing == IsoTpAddressing_e.Extended)
            {
                if (data.Length < 2) return (0, 0);
                return (data[1], data[0]);
            }

            return (data[0], 0);
        }

        /// <summary>扩展寻址下数据区首字节为地址，返回实际负载起始下标</summary>
        int PayloadIndex => Addressing == IsoTpAddressing_e.Extended ? 1 : 0;

        /// <summary>单帧最大数据长度（扣除地址字节）</summary>
        int SfMaxLength => CanFrameDataSize - PayloadIndex - 1;

        /// <summary>多帧每包数据长度（扣除地址字节与序号字节）</summary>
        int PacketPayloadLength => CanFrameDataSize - PayloadIndex - 1;

        bool OnSingleFrame(CanMsg msg, byte pci, byte addr, byte[] d)
        {
            var len = pci & 0x0F;
            var off = PayloadIndex + 1;

            // 长度超界时按实际可用长度截断
            var avail = d.Length - off;
            if (len > avail) len = avail < 0 ? 0 : avail;

            var data = new byte[len];
            if (len > 0) Array.Copy(d, off, data, 0, len);

            var unit = new IsoTpMessage
            {
                SourceId = (UInt32)msg.ID,
                TargetId = RequestId,
                Data = data,
                Extended = Addressing == IsoTpAddressing_e.Extended,
                Address = addr,
                ExternId = msg.ExternFlag,
            };

            Log?.Info("[ISO-TP] 收到单帧 " + unit);
            MessageReceived?.Invoke(this, unit);
            return true;
        }

        bool OnFirstFrame(CanMsg msg, byte pci, byte addr, byte[] d)
        {
            var off = PayloadIndex;

            // 12bit 长度：PCI 低 4 位为高 4 位，下一字节为低 8 位
            if (d.Length < off + 2) return false;

            var total = ((pci & 0x0F) << 8) | d[off + 1];
            if (total <= SfMaxLength || total > MaxMessageLength)
            {
                RaiseError($"首帧长度非法：{total}");
                return false;
            }

            var firstLen = PacketPayloadLength;
            var buf = new byte[total];
            var copy = Math.Min(firstLen, total);
            Array.Copy(d, off + 2, buf, 0, copy);

            var key = KeyOf((UInt32)msg.ID, addr);
            lock (_lock)
            {
                Cleanup();
                _rxSessions[key] = new RxSession
                {
                    Id = (UInt32)msg.ID,
                    Address = addr,
                    Buffer = buf,
                    TotalLength = total,
                    TotalPackets = (total - firstLen + PacketPayloadLength - 1) / PacketPayloadLength + 1,
                    NextPacket = 1,
                    LastTick = DateTime.Now,
                };
            }

            // 回送流控帧：允许继续
            SendFlowControl(addr, IsoTpFlowStatus_e.ContinueToSend, RxBlockSize, RxSTmin);

            Log?.Info($"[ISO-TP] 收到首帧 总长={total} 来自 0x{(UInt32)msg.ID:X} 已回送流控");
            return true;
        }

        bool OnConsecutiveFrame(CanMsg msg, byte addr, byte[] d)
        {
            var off = PayloadIndex;
            if (d.Length < off + 1) return false;

            var seq = (byte)(d[off] & 0x0F);
            var key = KeyOf((UInt32)msg.ID, addr);

            RxSession s;
            lock (_lock)
            {
                Cleanup();
                if (!_rxSessions.TryGetValue(key, out s)) return false;

                // 序号不连续：丢弃整个会话
                var expect = (byte)(s.NextPacket & 0x0F);
                if (seq != expect)
                {
                    _rxSessions.Remove(key);
                    RaiseError($"连续帧序号错误：期望 {expect}，实际 {seq}");
                    return true;
                }

                var offset = PacketPayloadLength * s.NextPacket;
                var copy = Math.Min(PacketPayloadLength, s.Buffer.Length - offset);
                if (copy > 0) Array.Copy(d, off + 1, s.Buffer, offset, copy);

                s.NextPacket++;
                s.LastTick = DateTime.Now;

                if (offset + copy >= s.Buffer.Length)
                {
                    _rxSessions.Remove(key);

                    var unit = new IsoTpMessage
                    {
                        SourceId = (UInt32)msg.ID,
                        TargetId = RequestId,
                        Data = s.Buffer,
                        Extended = Addressing == IsoTpAddressing_e.Extended,
                        Address = addr,
                        ExternId = msg.ExternFlag,
                    };

                    Log?.Info($"[ISO-TP] 多帧重组完成 总长={s.Buffer.Length}");
                    MessageReceived?.Invoke(this, unit);
                }
            }

            return true;
        }

        /// <summary>发送流控帧</summary>
        void SendFlowControl(byte addr, IsoTpFlowStatus_e fs, byte blockSize, int stmin)
        {
            var bs = new byte[CanFrameDataSize];
            var off = PayloadIndex;

            if (Addressing == IsoTpAddressing_e.Extended) bs[0] = addr;

            bs[off] = (byte)(((byte)IsoTpFrameType_e.FlowControl << 4) | (byte)fs);
            bs[off + 1] = blockSize;
            bs[off + 2] = EncodeSTmin(stmin);

            SendFrame(ResponseId, bs);
        }

        /// <summary>把毫秒转换为 STmin 编码值</summary>
        static byte EncodeSTmin(int ms)
        {
            if (ms <= 0) return 0x00;
            if (ms <= 0x7F) return (byte)ms;
            if (ms <= 900) return (byte)(0xF0 + (ms / 100));
            return 0x7F;
        }

        /// <summary>把 STmin 编码值转换为毫秒</summary>
        static int DecodeSTmin(byte v)
        {
            if (v <= 0x7F) return v;
            if (v >= 0xF1 && v <= 0xF9) return (v & 0x0F) * 100;
            return 0;
        }

        String KeyOf(UInt32 id, byte addr)
        {
            return Addressing == IsoTpAddressing_e.Extended ? $"{id:X8}-{addr:X2}" : $"{id:X8}";
        }

        /// <summary>清理超时会话</summary>
        void Cleanup()
        {
            var now = DateTime.Now;
            foreach (var kv in _rxSessions.ToArray())
            {
                if ((now - kv.Value.LastTick).TotalMilliseconds > Timing.N_Cr)
                {
                    _rxSessions.Remove(kv.Key);
                    RaiseError($"接收组包超时：ID=0x{kv.Value.Id:X8}");
                }
            }
        }

        #endregion

        #region 发送

        /// <summary>
        /// 发送一段完整数据（自动选择单帧或流控多帧），阻塞直到对方确认或超时。
        /// <b>不可在接收线程调用。</b>
        /// </summary>
        /// <param name="data">完整数据</param>
        /// <param name="function">是否使用功能寻址（广播）</param>
        /// <param name="address">扩展寻址时使用的地址字节</param>
        /// <returns>是否发送成功</returns>
        public bool Send(byte[] data, bool function = false, byte address = 0)
        {
            if (data == null || data.Length == 0) return false;
            if (data.Length > MaxMessageLength) throw new ArgumentOutOfRangeException(nameof(data), $"数据长度超限 {data.Length} > {MaxMessageLength}");

            // 简单互斥：同一时刻只允许一次发送（单帧/多帧共用该标志）
            if (Interlocked.CompareExchange(ref _sendLock, 1, 0) != 0) return false;

            try
            {
                return SendCore(data, function, address);
            }
            finally
            {
                Interlocked.Exchange(ref _sendLock, 0);
            }
        }

        Int32 _sendLock = 0;

        bool SendCore(byte[] data, bool function, byte address)
        {
            var id = function ? FunctionId : RequestId;
            if (id == 0) return false;

            _sendExtended = Addressing == IsoTpAddressing_e.Extended;
            _sendAddress = address;

            var off = PayloadIndex;
            var sfMax = SfMaxLength;

            // 单帧：省去流控交互
            if (data.Length <= sfMax)
            {
                var bs = new byte[CanFrameDataSize];
                if (_sendExtended) bs[0] = address;
                bs[off] = (byte)(((byte)IsoTpFrameType_e.SingleFrame << 4) | data.Length);
                Array.Copy(data, 0, bs, off + 1, data.Length);

                SendFrame(id, bs);
                return true;
            }

            // 多帧：功能寻址不允许流控多帧（多个接收方无法统一流控）
            if (function)
            {
                RaiseError("功能寻址不支持多帧发送");
                return false;
            }

            return SendMultiFrame(id, data);
        }

        /// <summary>发送多帧数据，按 ISO-TP 规范等待流控。</summary>
        bool SendMultiFrame(UInt32 id, byte[] data)
        {
            _fcBlockSize = 0;
            _fcSTmin = 0;
            _fcStatus = IsoTpFlowStatus_e.ContinueToSend;
            _abortSend = false;
            _fcEvent.Reset();

            var off = PayloadIndex;
            var per = PacketPayloadLength;

            // 1. 发送首帧（FF），携带 12bit 总长度 + 首段数据
            var ff = new byte[CanFrameDataSize];
            if (_sendExtended) ff[0] = _sendAddress;
            ff[off] = (byte)(((byte)IsoTpFrameType_e.FirstFrame << 4) | ((data.Length >> 8) & 0x0F));
            ff[off + 1] = (byte)(data.Length & 0xFF);
            Array.Copy(data, 0, ff, off + 2, per);

            // 先置位再发送，避免对方流控帧先到而被丢弃
            _sending = true;
            SendFrame(id, ff);

            // 已发送的数据偏移与待发序号（首帧带走了第 0 段，序号从 1 开始）
            var offset = per;
            _sendNextPacket = 1;

            try
            {
                // 2. 等待流控帧
                if (!_fcEvent.Wait(Timing.N_Bs))
                {
                    RaiseError($"等待流控帧超时（N_Bs={Timing.N_Bs}ms）");
                    return false;
                }

                // 3. 按流控参数循环发送连续帧，直到数据发完
                while (offset < data.Length)
                {
                    if (_abortSend) return false;

                    // FS=OverFlow 直接失败；FS=Wait 时等待下一个流控帧
                    if (_fcStatus == IsoTpFlowStatus_e.OverFlow)
                    {
                        RaiseError("对方流控指示溢出 OverFlow");
                        return false;
                    }

                    if (_fcStatus == IsoTpFlowStatus_e.Wait)
                    {
                        _fcEvent.Reset();
                        if (!_fcEvent.Wait(Timing.N_Bs))
                        {
                            RaiseError("等待流控帧超时（FS=Wait）");
                            return false;
                        }
                        continue;
                    }

                    // FS=CTS：BS=0 表示不限量，一次发完；否则按 BS 发一批
                    var remainPackets = (data.Length - offset + per - 1) / per;
                    var batch = _fcBlockSize == 0 ? remainPackets : _fcBlockSize;
                    if (batch > remainPackets) batch = remainPackets;

                    for (int i = 0; i < batch && offset < data.Length; i++)
                    {
                        if (_abortSend) return false;

                        var cf = new byte[CanFrameDataSize];
                        if (_sendExtended) cf[0] = _sendAddress;
                        cf[off] = (byte)(((byte)IsoTpFrameType_e.ConsecutiveFrame << 4) | (_sendNextPacket & 0x0F));

                        var copy = Math.Min(per, data.Length - offset);
                        Array.Copy(data, offset, cf, off + 1, copy);

                        SendFrame(id, cf);

                        offset += copy;
                        _sendNextPacket = (_sendNextPacket + 1) & 0x0F;

                        var st = _fcSTmin > 0 ? _fcSTmin : SendSTmin;
                        if (st > 0 && offset < data.Length) Thread.Sleep(st);
                    }

                    // 数据尚未发完（BS 到期），继续等下一帧流控
                    if (offset < data.Length && _fcBlockSize != 0)
                    {
                        _fcEvent.Reset();
                        if (!_fcEvent.Wait(Timing.N_Bs))
                        {
                            RaiseError("等待流控帧超时（续帧）");
                            return false;
                        }
                    }
                }

                Log?.Info($"[ISO-TP] 多帧发送完成 总长={data.Length}");
                return true;
            }
            finally
            {
                _sending = false;
            }
        }

        /// <summary>接收线程回调：处理发送侧的流控帧</summary>
        void OnFlowControl(IsoTpFlowStatus_e fs, byte blockSize, byte stminEncoded)
        {
            _fcStatus = fs;
            _fcBlockSize = blockSize;
            _fcSTmin = DecodeSTmin(stminEncoded);

            _fcEvent.Set();
        }

        #endregion

        /// <summary>从端口注销并释放资源</summary>
        public void Close()
        {
            _abortSend = true;
            _fcEvent.Set();

            try { Port?.Unregister(this); } catch { }
            try { Port?.RemoveItem(KeyItems); } catch { }

            try { _fcEvent.Dispose(); } catch { }

            lock (_lock) _rxSessions.Clear();
        }
    }

    #endregion
}
