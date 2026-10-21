using ALib.CAN;
using NewLife;
using NewLife.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ALib.Can_Uds
{
    #region 服务处理接口

    /// <summary>
    /// UDS 服务处理器接口。实现后注册到 <see cref="UdsServer"/>，即可响应对应的诊断请求。
    /// </summary>
    public interface IUdsServiceHandler
    {
        /// <summary>本处理器负责的服务 SID</summary>
        UdsService_e Service { get; }

        /// <summary>
        /// 处理请求。
        /// </summary>
        /// <param name="server">所属服务端</param>
        /// <param name="request">请求数据（完整字节，含 SID）</param>
        /// <param name="session">当前会话上下文</param>
        /// <returns>响应数据（不含 SID+0x40，由服务端自动补充）；返回 null 表示不回响应（抑制）</returns>
        /// <exception cref="UdsException">抛出以回送否定响应</exception>
        Byte[] Handle(UdsServer server, Byte[] request, UdsSession session);
    }

    /// <summary>
    /// 基于委托的服务处理器，便于快速注册简单服务。
    /// </summary>
    public class UdsDelegateHandler : IUdsServiceHandler
    {
        /// <summary>服务 SID</summary>
        public UdsService_e Service { get; }

        /// <summary>处理委托</summary>
        public Func<UdsServer, Byte[], UdsSession, Byte[]> Handler;

        public UdsDelegateHandler(UdsService_e service, Func<UdsServer, Byte[], UdsSession, Byte[]> handler)
        {
            Service = service;
            Handler = handler;
        }

        public Byte[] Handle(UdsServer server, Byte[] request, UdsSession session) => Handler?.Invoke(server, request, session);
    }

    #endregion

    #region 常用服务默认实现

    /// <summary>
    /// 通用基础服务处理器：会话控制、保活、复位、DTC 设置等。
    /// 便于快速搭起一个可被诊断的仿真 ECU。
    /// </summary>
    public class UdsBasicHandler : IUdsServiceHandler
    {
        /// <summary>负责的服务 SID</summary>
        public virtual UdsService_e Service => throw new NotImplementedException();

        /// <summary>处理请求</summary>
        public virtual Byte[] Handle(UdsServer server, Byte[] request, UdsSession session) => throw new NotImplementedException();

        /// <summary>取子功能（含抑制位剥离）</summary>
        protected static Byte SubOf(Byte[] req) => req.Length >= 2 ? (Byte)(req[1] & 0x7F) : (Byte)0;

        /// <summary>是否抑制肯定响应</summary>
        protected static Boolean Suppressed(Byte[] req) => req.Length >= 2 && (req[1] & 0x80) != 0;

        /// <summary>构造肯定响应头（SID + 0x40）</summary>
        protected static List<Byte> Ok(UdsService_e sid) => new List<Byte> { (Byte)((Byte)sid + UdsConstants.PositiveResponseOffset) };
    }

    /// <summary>诊断会话控制 0x10 处理器</summary>
    public class UdsSessionControlHandler : UdsBasicHandler
    {
        /// <summary>P2 服务器超时（毫秒），响应中返回</summary>
        public UInt16 P2Server = 50;

        /// <summary>P2* 服务器超时（10ms 为单位），响应中返回</summary>
        public UInt16 P2StarServer = 500;

        public override UdsService_e Service => UdsService_e.DiagnosticSessionControl;

        public override Byte[] Handle(UdsServer server, Byte[] request, UdsSession session)
        {
            if (request.Length != 2) throw new UdsException(Service, UdsNrc_e.IncorrectMessageLengthOrInvalidFormat);

            var sub = SubOf(request);
            var type = (UdsSession_e)sub;

            // 仅支持 01/02/03/04
            if (sub < 0x01 || sub > 0x04) throw new UdsException(Service, UdsNrc_e.SubFunctionNotSupported);

            session.SwitchTo(type);

            var body = Ok(Service);
            body.Add(sub);

            // 返回 P2/P2* 时间参数（高字节在前）
            body.Add((Byte)(P2Server >> 8));
            body.Add((Byte)P2Server);
            body.Add((Byte)(P2StarServer >> 8));
            body.Add((Byte)P2StarServer);

            return body.ToArray();
        }
    }

    /// <summary>链路保活 0x3E 处理器</summary>
    public class UdsTesterPresentHandler : UdsBasicHandler
    {
        public override UdsService_e Service => UdsService_e.TesterPresent;

        public override Byte[] Handle(UdsServer server, Byte[] request, UdsSession session)
        {
            if (request.Length != 2) throw new UdsException(Service, UdsNrc_e.IncorrectMessageLengthOrInvalidFormat);

            session.LastActive = DateTime.Now;

            // 抑制肯定响应时不回包
            if (Suppressed(request)) return null;

            return new[] { (Byte)((Byte)Service + UdsConstants.PositiveResponseOffset), SubOf(request) };
        }
    }

    /// <summary>ECU 复位 0x11 处理器</summary>
    public class UdsEcuResetHandler : UdsBasicHandler
    {
        /// <summary>复位动作回调，参数为复位类型</summary>
        public Action<UdsReset_e> OnReset;

        public override UdsService_e Service => UdsService_e.ECUReset;

        public override Byte[] Handle(UdsServer server, Byte[] request, UdsSession session)
        {
            if (request.Length != 2) throw new UdsException(Service, UdsNrc_e.IncorrectMessageLengthOrInvalidFormat);

            var sub = SubOf(request);
            if (sub < 0x01 || sub > 0x05) throw new UdsException(Service, UdsNrc_e.SubFunctionNotSupported);

            session.SwitchTo(UdsSession_e.Default);

            // 先回肯定响应，再执行复位动作
            var body = new List<Byte> { (Byte)((Byte)Service + UdsConstants.PositiveResponseOffset), sub };

            if (OnReset != null)
            {
                var reset = (UdsReset_e)sub;
                Task.Run(() =>
                {
                    Thread.Sleep(100);
                    try { OnReset(reset); } catch { }
                });
            }

            return Suppressed(request) ? null : body.ToArray();
        }
    }

    /// <summary>DTC 设置控制 0x85 处理器</summary>
    public class UdsDtcSettingHandler : UdsBasicHandler
    {
        public override UdsService_e Service => UdsService_e.ControlDTCSetting;

        public override Byte[] Handle(UdsServer server, Byte[] request, UdsSession session)
        {
            if (request.Length != 2) throw new UdsException(Service, UdsNrc_e.IncorrectMessageLengthOrInvalidFormat);

            var sub = SubOf(request);
            if (sub != 0x01 && sub != 0x02) throw new UdsException(Service, UdsNrc_e.SubFunctionNotSupported);

            session.DtcSettingEnabled = sub == 0x01;

            if (Suppressed(request)) return null;

            return new[] { (Byte)((Byte)Service + UdsConstants.PositiveResponseOffset), sub };
        }
    }

    #endregion

    #region UDS 服务端

    /// <summary>
    /// UDS 诊断服务端（ECU 侧）。接收诊断请求，分发给注册的 <see cref="IUdsServiceHandler"/>，
    /// 自动回送肯定/否定响应。主要用于仿真 ECU、上下位机联调与协议自测。
    /// <para>用法：
    /// <code>
    /// var srv = port.GetOrAdd(UdsServer.KeyItems, () =&gt; new UdsServer(port, 0x7E0, 0x7E8));
    /// srv.Register(new UdsSessionControlHandler());
    /// srv.Register(new UdsTesterPresentHandler());
    /// srv.Register(new UdsDelegateHandler(UdsService_e.ReadDataByIdentifier, (s, req, sess) =&gt; ...));
    /// </code>
    /// </para>
    /// </summary>
    public class UdsServer
    {
        /// <summary>在 <see cref="ICanPort"/> 上挂载本对象的字典键</summary>
        public const String KeyItems = "UdsServer";

        #region 属性

        /// <summary>所属 CAN 端口</summary>
        public ICanPort Port { get; }

        /// <summary>ISO-TP 传输层通道</summary>
        public IsoTpChannel Transport { get; }

        /// <summary>会话上下文</summary>
        public UdsSession Session { get; } = new UdsSession();

        /// <summary>日志</summary>
        public ILog Log { get; set; } = Logger.Null;

        /// <summary>是否对未注册的服务自动回 NRC 0x11（服务不支持）</summary>
        public Boolean RejectUnsupportedService = true;

        /// <summary>收到请求时触发，可用于日志或监控</summary>
        public event EventHandler<Byte[]> RequestReceived;

        /// <summary>发出响应时触发</summary>
        public event EventHandler<Byte[]> ResponseSent;

        #endregion

        /// <summary>已注册的处理器，key 为 SID</summary>
        readonly Dictionary<UdsService_e, IUdsServiceHandler> _handlers = new Dictionary<UdsService_e, IUdsServiceHandler>();

        readonly Object _lock = new Object();

        /// <summary>构造：在指定端口上创建 ISO-TP 与服务端</summary>
        /// <param name="port">CAN 端口</param>
        /// <param name="requestId">接收请求的 CAN ID</param>
        /// <param name="responseId">发出响应的 CAN ID</param>
        public UdsServer(ICanPort port, UInt32 requestId, UInt32 responseId)
        {
            Port = port ?? throw new ArgumentNullException(nameof(port));

            Transport = port.GetOrAdd(IsoTpChannel.KeyItems, () => new IsoTpChannel(port, responseId, requestId));
            Transport.Log = Log;
            Transport.MessageReceived += OnMessageReceived;
        }

        #region 注册

        /// <summary>注册服务处理器（同名 SID 覆盖）</summary>
        public void Register(IUdsServiceHandler handler)
        {
            if (handler == null) return;

            lock (_lock) _handlers[handler.Service] = handler;
        }

        /// <summary>注册委托式处理器</summary>
        public void Register(UdsService_e service, Func<UdsServer, Byte[], UdsSession, Byte[]> handler)
            => Register(new UdsDelegateHandler(service, handler));

        /// <summary>注销服务处理器</summary>
        public Boolean Unregister(UdsService_e service)
        {
            lock (_lock) return _handlers.Remove(service);
        }

        /// <summary>注销全部处理器</summary>
        public void Clear() { lock (_lock) _handlers.Clear(); }

        #endregion

        #region 接收与应答

        void OnMessageReceived(Object sender, IsoTpMessage msg)
        {
            if (msg?.Data == null || msg.Data.Length < 1) return;

            var request = msg.Data;
            Log?.Info($"[UDS-S] <<< {request.ToHex(" ")}");
            RequestReceived?.Invoke(this, request);

            var sid = (UdsService_e)request[0];
            var suppress = request.Length >= 2 && (request[1] & UdsConstants.SuppressPositiveResponseMask) != 0;

            Byte[] response;

            try
            {
                IUdsServiceHandler handler;
                lock (_lock) _handlers.TryGetValue(sid, out handler);

                if (handler == null)
                {
                    if (!RejectUnsupportedService) return;
                    response = BuildNegative(sid, UdsNrc_e.ServiceNotSupported);
                }
                else
                {
                    var body = handler.Handle(this, request, Session);

                    // null 表示抑制响应
                    if (body == null) return;

                    // 若处理器已包含 SID，则视为完整响应；否则补 SID+0x40
                    response = body.Length > 0 && body[0] == (Byte)((Byte)sid + UdsConstants.PositiveResponseOffset)
                        ? body
                        : Prepend((Byte)((Byte)sid + UdsConstants.PositiveResponseOffset), body);
                }
            }
            catch (UdsException ex)
            {
                Log?.Info($"[UDS-S] 服务 0x{(Byte)sid:X2} 被拒: {ex.Message}");
                response = BuildNegative(sid, ex.Nrc);
            }
            catch (Exception ex)
            {
                Log?.Info($"[UDS-S] 处理异常: {ex}");
                response = BuildNegative(sid, UdsNrc_e.GeneralReject);
            }

            if (response == null || response.Length == 0) return;

            try
            {
                // 抑制肯定响应：仅否定响应需要回送
                if (suppress && response[0] != UdsConstants.NegativeResponseSid) return;

                Log?.Info($"[UDS-S] >>> {response.ToHex(" ")}");
                Transport.Send(response);
                ResponseSent?.Invoke(this, response);
            }
            catch (Exception ex)
            {
                Log?.Info($"[UDS-S] 发送响应失败: {ex.Message}");
            }
        }

        /// <summary>构造否定响应：7F &lt;SID&gt; &lt;NRC&gt;</summary>
        public static Byte[] BuildNegative(UdsService_e sid, UdsNrc_e nrc)
        {
            return new[] { UdsConstants.NegativeResponseSid, (Byte)sid, (Byte)nrc };
        }

        static Byte[] Prepend(Byte head, Byte[] body)
        {
            var n = body?.Length ?? 0;
            var bs = new Byte[n + 1];
            bs[0] = head;
            if (n > 0) Array.Copy(body, 0, bs, 1, n);
            return bs;
        }

        #endregion

        /// <summary>从端口注销并释放资源</summary>
        public void Close()
        {
            try { Port?.RemoveItem(KeyItems); } catch { }

            try
            {
                Transport.MessageReceived -= OnMessageReceived;
                Transport.Close();
            }
            catch { }
        }
    }

    #endregion
}
