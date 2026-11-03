using ALib.CAN;
using NewLife;
using NewLife.Collections;
using NewLife.Log;
using NewLife.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ALib.Can_Uds
{
    #region 时间参数

    /// <summary>
    /// UDS 应用层定时参数（单位毫秒）
    /// </summary>
    public class UdsTiming
    {
        /// <summary>P2_server：服务器收到请求到给出响应的最大时间（默认 50ms）</summary>
        public int P2Server = UdsConstants.DefaultP2Server;

        /// <summary>P2*_server：收到 0x78 后允许延长的最大时间（默认 5000ms）</summary>
        public int P2StarServer = UdsConstants.DefaultP2StarServer;

        /// <summary>S3_server：非默认会话保活周期（默认 5000ms）</summary>
        public int S3Server = UdsConstants.DefaultS3Server;

        /// <summary>S3 保活发送间隔（默认 2000ms），应小于 S3Server</summary>
        public int TesterPresentInterval = 2000;

        /// <summary>收到 0x78 后最多重试次数（默认 10 次），防止死循环等待</summary>
        public int MaxResponsePending = 10;
    }

    #endregion

    /// <summary>
    /// UDS 诊断客户端（Tester 侧）。
    /// <para>在 <see cref="IsoTpChannel"/> 之上提供同步的「发请求 → 等响应」语义，
    /// 封装了常用诊断服务：会话控制、复位、读/写 DID、安全访问、例程控制、DTC 读写、刷写等。</para>
    /// <para>线程模型：所有 <c>Xxx()</c> 诊断方法在调用线程阻塞直到收到响应或超时，
    /// <b>禁止在接收线程（<see cref="IsoTpChannel.MessageReceived"/>）内调用</b>，否则死锁。</para>
    /// <para>用法：
    /// <code>
    /// var uds = port.GetOrAdd(UdsClient.KeyItems, () =&gt; new UdsClient(port, 0x7E0, 0x7E8));
    /// var data = uds.ReadDataByIdentifier(0xF190);   // 读 VIN
    /// uds.DiagnosticSessionControl(UdsSession_e.Extended);
    /// uds.SecurityAccess(UdsSecurityLevel_e.RequestSeedLevel1, seed =&gt; KeyAlgo(seed));
    /// </code>
    /// </para>
    /// </summary>
    public class UdsClient
    {
        /// <summary>在 <see cref="ICanPort"/> 上挂载本对象的字典键</summary>
        public const String KeyItems = "UdsClient";

        #region 属性

        /// <summary>所属 CAN 端口</summary>
        public ICanPort Port { get; }

        /// <summary>ISO-TP 传输层通道</summary>
        public IsoTpChannel Transport { get; }

        /// <summary>时间参数</summary>
        public UdsTiming Timing { get; set; } = new UdsTiming();

        /// <summary>会话上下文</summary>
        public UdsSession Session { get; } = new UdsSession();

        /// <summary>日志</summary>
        public ILog Log { get; set; } = Logger.Null;

        /// <summary>是否启用 S3 会话保活（进非默认会话后自动周期发 0x3E 80）</summary>
        public Boolean KeepAliveEnabled = true;

        /// <summary>收到完整的肯定/否定响应</summary>
        public event EventHandler<UdsResponse> ResponseReceived;

        /// <summary>收到完整响应（原始字节）</summary>
        public event EventHandler<Byte[]> DataReceived;

        #endregion

        #region 内部字段

        /// <summary>等待响应的信号量</summary>
        readonly ManualResetEventSlim _respEvent = new ManualResetEventSlim(false);

        /// <summary>最近一次接收到的响应</summary>
        UdsResponse _lastResponse;

        /// <summary>等待中的请求 SID</summary>
        UdsService_e _waitSid;
        volatile Boolean _waiting = false;

        /// <summary>S3 保活定时器</summary>
        TimerX _keepAliveTimer;

        #endregion

        /// <summary>构造：在指定端口上创建 ISO-TP 与 UDS 客户端</summary>
        /// <param name="port">CAN 端口</param>
        /// <param name="requestId">物理请求 CAN ID</param>
        /// <param name="responseId">物理响应 CAN ID</param>
        public UdsClient(ICanPort port, UInt32 requestId, UInt32 responseId)
        {
            Port = port ?? throw new ArgumentNullException(nameof(port));

            Transport = port.GetOrAdd(IsoTpChannel.KeyItems, () => new IsoTpChannel(port, requestId, responseId));
            Transport.Log = Log;
            Transport.MessageReceived += OnMessageReceived;
        }

        /// <summary>按 ECU 序号构造（0x7E0/0x7E8 起）</summary>
        public static UdsClient CreateByEcu(ICanPort port, Int32 ecuIndex)
        {
            ecuIndex &= 0x0F;
            return new UdsClient(port, (UInt32)(0x7E0 + 2 * ecuIndex), (UInt32)(0x7E8 + 2 * ecuIndex));
        }

        #region 接收分发

        /// <summary>ISO-TP 收到完整消息 → UDS 响应处理</summary>
        void OnMessageReceived(Object sender, IsoTpMessage msg)
        {
            if (msg?.Data == null || msg.Data.Length == 0) return;

            DataReceived?.Invoke(this, msg.Data);

            // 仅接收本客户端等待中的响应
            if (!_waiting) return;

            var res = UdsResponse.Parse(msg.Data, _waitSid);
            if (res == null) return;

            _lastResponse = res;
            _respEvent.Set();
        }

        #endregion

        #region 底层收发

        /// <summary>
        /// 发送请求并等待响应（同步阻塞）。
        /// </summary>
        /// <param name="request">请求字节</param>
        /// <param name="sid">请求服务 SID，用于校验响应</param>
        /// <param name="function">是否功能寻址（广播）</param>
        /// <returns>响应对象；超时返回 null</returns>
        public UdsResponse Request(Byte[] request, UdsService_e sid, Boolean function = false)
        {
            if (request == null || request.Length == 0) throw new ArgumentNullException(nameof(request));

            _waitSid = sid;
            _lastResponse = null;
            _respEvent.Reset();
            _waiting = true;

            try
            {
                Log?.Info($"[UDS] >>> {request.ToHex(" ")}");

                if (!Transport.Send(request, function))
                {
                    Log?.Info("[UDS] 发送失败（传输层拒绝）");
                    return null;
                }

                // 功能寻址：无肯定响应，只可能收到否定响应
                if (function)
                {
                    // 广播时可能有多个 ECU 回否定响应；短暂接收后返回
                    var waitMs = Math.Max(1, Math.Min(Timing.P2Server, 200));
                    if (_respEvent.Wait(waitMs))
                    {
                        var neg = _lastResponse;
                        Log?.Info($"[UDS] <<< {neg}");
                        return neg;
                    }

                    return new UdsResponse { Raw = Array.Empty<Byte>(), Service = sid, ResponseSid = (Byte)((Byte)sid + 0x40) };
                }

                // 等待响应，处理 0x78 挂起
                var pendingCount = 0;
                var deadline = DateTime.Now.AddMilliseconds(Timing.P2Server);

                while (true)
                {
                    var remain = deadline - DateTime.Now;
                    if (remain <= TimeSpan.Zero || !_respEvent.Wait(remain))
                    {
                        Log?.Info($"[UDS] 等待响应超时（P2={Timing.P2Server}ms）");
                        return null;
                    }

                    _respEvent.Reset();
                    var res = _lastResponse;
                    _lastResponse = null;
                    if (res == null) continue;

                    // 收到 0x78：延长等待
                    if (res.IsNegative && res.Nrc == UdsNrc_e.ResponsePending)
                    {
                        pendingCount++;
                        if (pendingCount > Timing.MaxResponsePending)
                        {
                            Log?.Info("[UDS] 响应挂起次数超限，放弃等待");
                            return res;
                        }

                        Log?.Info($"[UDS] <<< NRC 0x78 响应挂起，继续等待（第 {pendingCount} 次）");
                        deadline = DateTime.Now.AddMilliseconds(Timing.P2StarServer);
                        continue;
                    }

                    Log?.Info($"[UDS] <<< {res.Raw.ToHex(" ")}");
                    ResponseReceived?.Invoke(this, res);
                    return res;
                }
            }
            finally
            {
                _waiting = false;
            }
        }

        /// <summary>发送请求，响应为否定时抛出 <see cref="UdsException"/>，否则返回响应</summary>
        /// <param name="request">请求数据</param>
        /// <param name="sid">请求 SID</param>
        /// <param name="function">是否功能寻址</param>
        public UdsResponse RequestThrow(Byte[] request, UdsService_e sid, Boolean function = false)
        {
            var res = Request(request, sid, function);
            if (res == null) throw new UdsException(sid, UdsNrc_e.GeneralReject);

            if (res.IsNegative && res.Nrc != UdsNrc_e.ResponsePending)
                throw new UdsException(sid, res.Nrc);

            return res;
        }

        #endregion

        #region 诊断和通信管理

        /// <summary>诊断会话控制 0x10</summary>
        /// <param name="session">目标会话</param>
        /// <param name="suppress">是否抑制肯定响应</param>
        public UdsResponse DiagnosticSessionControl(UdsSession_e session, Boolean suppress = false)
        {
            var req = new UdsRequest(UdsService_e.DiagnosticSessionControl)
            {
                SubFunction = (Byte)session,
                SuppressResponse = suppress,
            };

            var res = Request(req.ToArray(), UdsService_e.DiagnosticSessionControl);
            if (res != null && res.IsPositive) Session.SwitchTo(session);

            return res;
        }

        /// <summary>ECU 复位 0x11</summary>
        /// <param name="reset">复位类型</param>
        /// <param name="suppress">是否抑制肯定响应</param>
        public UdsResponse ECUReset(UdsReset_e reset, Boolean suppress = false)
        {
            var req = new UdsRequest(UdsService_e.ECUReset)
            {
                SubFunction = (Byte)reset,
                SuppressResponse = suppress,
            };

            // 复位后 ECU 掉线，肯定响应可能收不到，不作为错误
            var res = Request(req.ToArray(), UdsService_e.ECUReset);
            if (res != null && res.IsPositive) Session.SwitchTo(UdsSession_e.Default);

            return res;
        }

        /// <summary>链路保活 0x3E（抑制肯定响应，固定发 3E 80）</summary>
        public Boolean TesterPresent(Boolean suppress = true)
        {
            var req = new UdsRequest(UdsService_e.TesterPresent)
            {
                SubFunction = 0x00,
                SuppressResponse = suppress,
            };

            var res = Request(req.ToArray(), UdsService_e.TesterPresent, suppress);
            return res != null;
        }

        /// <summary>通信控制 0x28</summary>
        /// <param name="control">控制类型</param>
        /// <param name="type">通信类型</param>
        public UdsResponse CommunicationControl(UdsCommunicationControl_e control, UdsCommunicationType_e type = UdsCommunicationType_e.NormalCommunicationMessages)
        {
            var req = new UdsRequest(UdsService_e.CommunicationControl)
            {
                SubFunction = (Byte)control,
                Data = new[] { (Byte)type },
            };

            var res = Request(req.ToArray(), UdsService_e.CommunicationControl);
            if (res != null && res.IsPositive)
                Session.CommunicationEnabled = control == UdsCommunicationControl_e.EnableRxAndTx;

            return res;
        }

        /// <summary>DTC 设置控制 0x85</summary>
        /// <param name="setting">开/关</param>
        /// <param name="suppress">是否抑制肯定响应</param>
        public UdsResponse ControlDTCSetting(UdsDtcSetting_e setting, Boolean suppress = false)
        {
            var req = new UdsRequest(UdsService_e.ControlDTCSetting)
            {
                SubFunction = (Byte)setting,
                SuppressResponse = suppress,
            };

            var res = Request(req.ToArray(), UdsService_e.ControlDTCSetting);
            if (res != null && res.IsPositive) Session.DtcSettingEnabled = setting == UdsDtcSetting_e.On;

            return res;
        }

        #endregion

        #region 安全访问

        /// <summary>请求种子 0x27（奇数子功能）</summary>
        /// <param name="level">安全等级子功能（如 RequestSeedLevel1）</param>
        /// <returns>ECU 返回的种子；已解锁时返回空数组</returns>
        public Byte[] SecurityAccessRequestSeed(UdsSecurityLevel_e level)
        {
            if (((Byte)level & 0x01) == 0) throw new ArgumentException("请求种子必须使用奇数子功能", nameof(level));

            var req = new UdsRequest(UdsService_e.SecurityAccess) { SubFunction = (Byte)level };
            var res = RequestThrow(req.ToArray(), UdsService_e.SecurityAccess);

            // 响应格式：67 <子功能> <种子...>
            return res.Data ?? Array.Empty<Byte>();
        }

        /// <summary>发送密钥 0x27（偶数子功能）</summary>
        /// <param name="level">与请求种子配对的安全等级子功能</param>
        /// <param name="key">密钥</param>
        public UdsResponse SecurityAccessSendKey(UdsSecurityLevel_e level, Byte[] key)
        {
            if (((Byte)level & 0x01) != 0) throw new ArgumentException("发送密钥必须使用偶数子功能", nameof(level));
            if (key == null || key.Length == 0) throw new ArgumentNullException(nameof(key));

            var req = new UdsRequest(UdsService_e.SecurityAccess)
            {
                SubFunction = (Byte)level,
                Data = key,
            };

            var res = RequestThrow(req.ToArray(), UdsService_e.SecurityAccess);
            if (res != null && res.IsPositive) Session.SecurityLevel = (Byte)((Byte)level - 1);

            return res;
        }

        /// <summary>
        /// 完整安全访问流程：请求种子 → 由 keyAlgorithm 计算密钥 → 发送密钥。
        /// </summary>
        /// <param name="level">配对的安全等级子功能（应为请求种子，如 RequestSeedLevel1）</param>
        /// <param name="keyAlgorithm">种子 → 密钥 的算法；为空则使用 <see cref="UdsSession.KeyAlgorithm"/></param>
        /// <returns>是否解锁成功</returns>
        public Boolean SecurityAccess(UdsSecurityLevel_e level, Func<Byte[], Byte[]> keyAlgorithm = null)
        {
            var algo = keyAlgorithm ?? Session.KeyAlgorithm;
            if (algo == null) throw new InvalidOperationException("未提供密钥算法");

            var seed = SecurityAccessRequestSeed(level);
            Log?.Info($"[UDS] 种子：{seed.ToHex(" ")}");

            var key = algo(seed);
            if (key == null) return false;

            var sendLevel = (UdsSecurityLevel_e)((Byte)level + 1);
            SecurityAccessSendKey(sendLevel, key);
            return true;
        }

        #endregion

        #region 数据传输

        /// <summary>按 DID 读数据 0x22</summary>
        /// <param name="did">数据标识符</param>
        /// <returns>数据部分（不含 DID 回显）</returns>
        public Byte[] ReadDataByIdentifier(UInt16 did)
        {
            var req = new UdsRequest(UdsService_e.ReadDataByIdentifier) { Did = did };
            var res = RequestThrow(req.ToArray(), UdsService_e.ReadDataByIdentifier);

            // 响应：62 <DID 高> <DID 低> <数据...>
            var data = res.Data;
            if (data == null || data.Length < 2) return Array.Empty<Byte>();
            return data.ReadBytes(2, data.Length - 2);
        }

        /// <summary>按 DID 写数据 0x2E</summary>
        /// <param name="did">数据标识符</param>
        /// <param name="data">数据</param>
        public UdsResponse WriteDataByIdentifier(UInt16 did, Byte[] data)
        {
            var req = new UdsRequest(UdsService_e.WriteDataByIdentifier)
            {
                Did = did,
                Data = data ?? Array.Empty<Byte>(),
            };

            return RequestThrow(req.ToArray(), UdsService_e.WriteDataByIdentifier);
        }

        /// <summary>按地址读内存 0x23（地址与长度均为 4 字节）</summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">长度</param>
        /// <param name="addressLength">地址字节数（1~4，默认 4）</param>
        /// <param name="lengthLength">长度字节数（1~4，默认 4）</param>
        public Byte[] ReadMemoryByAddress(UInt32 address, UInt32 length, Int32 addressLength = 4, Int32 lengthLength = 4)
        {
            // ALFID：地址长度高 4 位，长度长度低 4 位
            var alfid = (Byte)(((addressLength & 0x0F) << 4) | (lengthLength & 0x0F));
            var body = new List<Byte> { alfid };
            body.AddRange(ToBigEndian(address, addressLength));
            body.AddRange(ToBigEndian(length, lengthLength));

            var req = new UdsRequest(UdsService_e.ReadMemoryByAddress) { Data = body.ToArray() };
            var res = RequestThrow(req.ToArray(), UdsService_e.ReadMemoryByAddress);
            return res.Data ?? Array.Empty<Byte>();
        }

        /// <summary>按地址写内存 0x3D</summary>
        /// <param name="address">起始地址</param>
        /// <param name="data">写入数据</param>
        /// <param name="addressLength">地址字节数（默认 4）</param>
        /// <param name="lengthLength">长度字节数（默认 4）</param>
        public UdsResponse WriteMemoryByAddress(UInt32 address, Byte[] data, Int32 addressLength = 4, Int32 lengthLength = 4)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var alfid = (Byte)(((addressLength & 0x0F) << 4) | (lengthLength & 0x0F));
            var body = new List<Byte> { alfid };
            body.AddRange(ToBigEndian(address, addressLength));
            body.AddRange(ToBigEndian((UInt32)data.Length, lengthLength));
            body.AddRange(data);

            var req = new UdsRequest(UdsService_e.WriteMemoryByAddress) { Data = body.ToArray() };
            return RequestThrow(req.ToArray(), UdsService_e.WriteMemoryByAddress);
        }

        /// <summary>按 DID 控制输入输出 0x2F</summary>
        /// <param name="did">数据标识符</param>
        /// <param name="control">控制参数</param>
        /// <param name="data">控制数据（可空）</param>
        public UdsResponse InputOutputControlByIdentifier(UInt16 did, UdsIoControl_e control, Byte[] data = null)
        {
            var body = new List<Byte> { (Byte)(did >> 8), (Byte)did, (Byte)control };
            if (data != null && data.Length > 0) body.AddRange(data);

            var req = new UdsRequest(UdsService_e.InputOutputControlByIdentifier) { Data = body.ToArray() };
            return RequestThrow(req.ToArray(), UdsService_e.InputOutputControlByIdentifier);
        }

        #endregion

        #region DTC

        /// <summary>清除诊断信息 0x14（三个 0xFF 表示清除全部）</summary>
        /// <param name="groupOfDtc">DTC 组，默认 0xFFFFFF 全部</param>
        public UdsResponse ClearDiagnosticInformation(UInt32 groupOfDtc = 0xFFFFFF)
        {
            var body = new[] { (Byte)(groupOfDtc >> 16), (Byte)(groupOfDtc >> 8), (Byte)groupOfDtc };
            var req = new UdsRequest(UdsService_e.ClearDiagnosticInformation) { Data = body };

            return RequestThrow(req.ToArray(), UdsService_e.ClearDiagnosticInformation);
        }

        /// <summary>
        /// 读取 DTC 信息 0x19。
        /// </summary>
        /// <param name="subFunction">子功能</param>
        /// <param name="data">附加参数（如状态掩码 0xFF）</param>
        public UdsResponse ReadDTCInformation(UdsReadDtcSubFunction_e subFunction, params Byte[] data)
        {
            var req = new UdsRequest(UdsService_e.ReadDTCInformation)
            {
                SubFunction = (Byte)subFunction,
                Data = data ?? Array.Empty<Byte>(),
            };

            return RequestThrow(req.ToArray(), UdsService_e.ReadDTCInformation);
        }

        /// <summary>
        /// 按状态掩码读取 DTC 列表（子功能 0x02）。
        /// </summary>
        /// <param name="statusMask">状态掩码，默认 0xFF 全部</param>
        public List<UdsDtc> ReadDtcByStatusMask(Byte statusMask = 0xFF)
        {
            var res = ReadDTCInformation(UdsReadDtcSubFunction_e.ReportDTCByStatusMask, statusMask);

            // 响应：59 02 <状态可用性掩码> [<DTC 3字节><状态1字节>]...
            var list = new List<UdsDtc>();
            var d = res.Data;
            if (d == null || d.Length < 2) return list;

            // 跳过子功能回显(0x02)与状态可用性掩码
            for (var i = 2; i + 3 < d.Length; i += 4)
            {
                var dtc = new UdsDtc
                {
                    Dtc = new[] { d[i], d[i + 1], d[i + 2] },
                    Status = (UdsDtcStatus_e)d[i + 3],
                };
                list.Add(dtc);
            }

            return list;
        }

        /// <summary>按状态掩码读取 DTC 数量（子功能 0x01）</summary>
        /// <param name="statusMask">状态掩码</param>
        public Int32 ReadDtcCount(Byte statusMask = 0xFF)
        {
            var res = ReadDTCInformation(UdsReadDtcSubFunction_e.ReportNumberOfDTCByStatusMask, statusMask);

            // 响应：59 01 <状态可用性掩码> <DTC 格式标识> <数量高> <数量低>
            var d = res.Data;
            if (d == null || d.Length < 5) return 0;

            return (d[3] << 8) | d[4];
        }

        #endregion

        #region 例程控制

        /// <summary>例程控制 0x31</summary>
        /// <param name="control">启动/停止/请求结果</param>
        /// <param name="routineId">例程标识符（2 字节）</param>
        /// <param name="data">例程参数</param>
        public UdsResponse RoutineControl(UdsRoutineControl_e control, UInt16 routineId, Byte[] data = null)
        {
            var body = new List<Byte> { (Byte)(routineId >> 8), (Byte)routineId };
            if (data != null && data.Length > 0) body.AddRange(data);

            var req = new UdsRequest(UdsService_e.RoutineControl)
            {
                SubFunction = (Byte)control,
                Data = body.ToArray(),
            };

            return RequestThrow(req.ToArray(), UdsService_e.RoutineControl);
        }

        /// <summary>启动例程</summary>
        public UdsResponse StartRoutine(UInt16 routineId, Byte[] data = null)
            => RoutineControl(UdsRoutineControl_e.StartRoutine, routineId, data);

        /// <summary>停止例程</summary>
        public UdsResponse StopRoutine(UInt16 routineId, Byte[] data = null)
            => RoutineControl(UdsRoutineControl_e.StopRoutine, routineId, data);

        /// <summary>请求例程结果</summary>
        public UdsResponse RequestRoutineResults(UInt16 routineId)
            => RoutineControl(UdsRoutineControl_e.RequestRoutineResults, routineId);

        #endregion

        #region 上传下载（刷写）

        /// <summary>请求下载 0x34</summary>
        /// <param name="address">目标地址</param>
        /// <param name="size">数据总长度</param>
        /// <param name="compression">压缩/加密方法（高 4 位压缩，低 4 位加密），默认 0x00</param>
        /// <param name="addressLength">地址字节数，默认 4</param>
        /// <param name="sizeLength">长度字节数，默认 4</param>
        /// <returns>ECU 允许的每块最大字节数（响应中的 maxNumberOfBlockLength）</returns>
        public Int32 RequestDownload(UInt32 address, UInt32 size, Byte compression = 0x00, Int32 addressLength = 4, Int32 sizeLength = 4)
        {
            var alfid = (Byte)(((addressLength & 0x0F) << 4) | (sizeLength & 0x0F));
            var body = new List<Byte> { compression, alfid };
            body.AddRange(ToBigEndian(address, addressLength));
            body.AddRange(ToBigEndian(size, sizeLength));

            var req = new UdsRequest(UdsService_e.RequestDownload) { Data = body.ToArray() };
            var res = RequestThrow(req.ToArray(), UdsService_e.RequestDownload);

            // 响应：74 <长度格式标识> <maxNumberOfBlockLength 若干字节>
            var d = res.Data;
            if (d == null || d.Length < 2) return 0;

            var len = 0;
            for (var i = 1; i < d.Length; i++) len = (len << 8) | d[i];
            return len;
        }

        /// <summary>传输数据 0x36</summary>
        /// <param name="blockSequenceCounter">块序号（从 1 开始，循环 0~FF）</param>
        /// <param name="data">本块数据</param>
        public UdsResponse TransferData(Byte blockSequenceCounter, Byte[] data)
        {
            var body = new List<Byte> { blockSequenceCounter };
            if (data != null && data.Length > 0) body.AddRange(data);

            var req = new UdsRequest(UdsService_e.TransferData) { Data = body.ToArray() };
            return RequestThrow(req.ToArray(), UdsService_e.TransferData);
        }

        /// <summary>请求结束传输 0x37</summary>
        /// <param name="data">附加参数（如校验和）</param>
        public UdsResponse RequestTransferExit(Byte[] data = null)
        {
            var req = new UdsRequest(UdsService_e.RequestTransferExit) { Data = data ?? Array.Empty<Byte>() };
            return RequestThrow(req.ToArray(), UdsService_e.RequestTransferExit);
        }

        /// <summary>请求上传 0x35</summary>
        /// <param name="address">源地址</param>
        /// <param name="size">长度</param>
        /// <param name="compression">压缩/加密方法</param>
        /// <param name="addressLength">地址字节数</param>
        /// <param name="sizeLength">长度字节数</param>
        public Int32 RequestUpload(UInt32 address, UInt32 size, Byte compression = 0x00, Int32 addressLength = 4, Int32 sizeLength = 4)
        {
            var alfid = (Byte)(((addressLength & 0x0F) << 4) | (sizeLength & 0x0F));
            var body = new List<Byte> { compression, alfid };
            body.AddRange(ToBigEndian(address, addressLength));
            body.AddRange(ToBigEndian(size, sizeLength));

            var req = new UdsRequest(UdsService_e.RequestUpload) { Data = body.ToArray() };
            var res = RequestThrow(req.ToArray(), UdsService_e.RequestUpload);

            var d = res.Data;
            if (d == null || d.Length < 2) return 0;

            var len = 0;
            for (var i = 1; i < d.Length; i++) len = (len << 8) | d[i];
            return len;
        }

        /// <summary>
        /// 完整下载流程：请求下载 → 分块传输 → 结束传输。
        /// </summary>
        /// <param name="address">目标地址</param>
        /// <param name="data">完整数据</param>
        /// <param name="blockSize">每块大小，0 表示使用 ECU 返回的最大块长</param>
        /// <param name="progress">进度回调（已发送字节数 / 总字节数）</param>
        /// <returns>是否成功</returns>
        public Boolean Download(UInt32 address, Byte[] data, Int32 blockSize = 0, Action<Int32, Int32> progress = null)
        {
            if (data == null || data.Length == 0) return false;

            var maxBlock = RequestDownload(address, (UInt32)data.Length);
            if (maxBlock <= 0) maxBlock = 1024;

            // ECU 返回的最大块长包含 SID 与块序号两个字节，实际数据需扣除
            var payloadMax = maxBlock - 2;
            if (payloadMax <= 0) payloadMax = 1022;

            var block = blockSize > 0 ? blockSize : payloadMax;
            if (block > payloadMax) block = payloadMax;

            var offset = 0;
            Byte seq = 1;

            while (offset < data.Length)
            {
                var len = Math.Min(block, data.Length - offset);
                var chunk = new Byte[len];
                Array.Copy(data, offset, chunk, 0, len);

                TransferData(seq, chunk);

                offset += len;
                seq++;
                if (seq == 0) seq = 1;  // 序号 0 保留给首块之外的循环，从 1 起

                progress?.Invoke(offset, data.Length);
            }

            RequestTransferExit();
            return true;
        }

        #endregion

        #region 会话保活

        /// <summary>启动 S3 保活定时器</summary>
        public void StartKeepAlive()
        {
            if (!KeepAliveEnabled) return;
            if (_keepAliveTimer != null) return;

            lock (this)
            {
                if (_keepAliveTimer != null) return;

                var period = Timing.TesterPresentInterval;
                if (period <= 0) period = 2000;

                _keepAliveTimer = new TimerX(OnKeepAliveTick, null, period, period) { Async = true };
            }
        }

        /// <summary>停止 S3 保活</summary>
        public void StopKeepAlive()
        {
            lock (this)
            {
                _keepAliveTimer?.Dispose();
                _keepAliveTimer = null;
            }
        }

        void OnKeepAliveTick(Object state)
        {
            // 仅非默认会话需要保活；且不在等待响应时穿插发送
            if (Session.IsDefault) return;
            if (_waiting) return;

            try { TesterPresent(true); }
            catch (Exception ex) { Log?.Info("[UDS] S3 保活失败: " + ex.Message); }
        }

        #endregion

        #region 工具

        /// <summary>把数值转换为指定字节数的大端字节数组</summary>
        static Byte[] ToBigEndian(UInt32 value, Int32 length)
        {
            if (length <= 0) length = 4;
            if (length > 4) length = 4;

            var bs = new Byte[length];
            for (var i = 0; i < length; i++)
            {
                bs[length - 1 - i] = (Byte)(value >> (8 * i));
            }
            return bs;
        }

        /// <summary>从端口注销并释放资源</summary>
        public void Close()
        {
            StopKeepAlive();

            try { Port?.RemoveItem(KeyItems); } catch { }

            try
            {
                Transport.MessageReceived -= OnMessageReceived;
                Transport.Close();
            }
            catch { }

            try { _respEvent.Dispose(); } catch { }
        }

        #endregion
    }
}
