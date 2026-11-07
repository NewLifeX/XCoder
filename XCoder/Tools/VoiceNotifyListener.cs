using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using NewLife;

namespace XCoder.Tools;

/// <summary>网络语音播报监听器</summary>
/// <remarks>
/// 绑定 UDP 端口并加入组播组，收到文本后通过 <see cref="Received"/> 事件抛出。
/// 同时可接收发往本机端口的受限广播。
/// </remarks>
public class VoiceNotifyListener : IDisposable
{
    #region 属性
    /// <summary>组播地址</summary>
    public String Group { get; set; } = "239.255.42.99";

    /// <summary>监听端口</summary>
    public Int32 Port { get; private set; }

    /// <summary>收到消息事件。在后台线程触发</summary>
    public event EventHandler<VoiceNotifyEventArgs> Received;
    #endregion

    #region 方法
    /// <summary>启动监听</summary>
    /// <param name="port">监听端口，0 表示由系统分配空闲端口</param>
    public void Start(Int32 port)
    {
        if (_udp != null) throw new InvalidOperationException("监听器已启动");

        var udp = new UdpClient(AddressFamily.InterNetwork);
        try
        {
            udp.Client.Bind(new IPEndPoint(IPAddress.Any, port));
            Port = ((IPEndPoint)udp.Client.LocalEndPoint).Port;

            if (!Group.IsNullOrEmpty() && IPAddress.TryParse(Group, out var group))
            {
                foreach (var address in GetLocalAddresses())
                {
                    try { udp.JoinMulticastGroup(group, address); } catch { }
                }
            }
        }
        catch
        {
            udp.Dispose();
            throw;
        }

        _udp = udp;
        _cts = new CancellationTokenSource();
        _task = Task.Run(() => ReceiveLoop(udp, _cts.Token));
    }

    /// <summary>停止监听</summary>
    public void Stop()
    {
        var cts = _cts;
        var udp = _udp;
        var task = _task;
        _cts = null;
        _udp = null;
        _task = null;

        try { cts?.Cancel(); } catch { }
        try { udp?.Dispose(); } catch { }

        // 等收包循环退出后再释放令牌，避免循环仍在使用已释放的 CancellationTokenSource
        if (task != null)
        {
            try { task.Wait(1000); } catch { }
        }
        cts?.Dispose();
    }

    async Task ReceiveLoop(UdpClient udp, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            UdpReceiveResult rs;
            try
            {
                rs = await udp.ReceiveAsync(token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) { break; }
            catch (ObjectDisposedException) { break; }
            catch (SocketException) { break; }
            catch { continue; }

            var text = Encoding.UTF8.GetString(rs.Buffer);
            var msg = VoiceNotifyMessage.Parse(text);
            if (msg == null) continue;

            try { Received?.Invoke(this, new VoiceNotifyEventArgs(rs.RemoteEndPoint, msg)); }
            catch { }
        }
    }

    static IPAddress[] GetLocalAddresses()
    {
        try
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(e => e.OperationalStatus == OperationalStatus.Up)
                .SelectMany(e => e.GetIPProperties().UnicastAddresses)
                .Select(e => e.Address)
                .Where(e => e.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(e))
                .Distinct()
                .ToArray();
        }
        catch
        {
            return [IPAddress.Any];
        }
    }
    #endregion

    #region 释放
    private UdpClient _udp;
    private CancellationTokenSource _cts;
    private Task _task;

    /// <summary>释放资源</summary>
    public void Dispose() => Stop();
    #endregion
}

/// <summary>网络语音播报事件参数</summary>
public class VoiceNotifyEventArgs : EventArgs
{
    /// <summary>远端地址</summary>
    public IPEndPoint RemoteEndPoint { get; }

    /// <summary>消息</summary>
    public VoiceNotifyMessage Message { get; }

    /// <summary>实例化</summary>
    /// <param name="remoteEndPoint">远端地址</param>
    /// <param name="message">消息</param>
    public VoiceNotifyEventArgs(IPEndPoint remoteEndPoint, VoiceNotifyMessage message)
    {
        RemoteEndPoint = remoteEndPoint;
        Message = message;
    }
}
