using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using XCoder.Tools;

namespace XCoder.Tests;

/// <summary>网络语音播报监听器</summary>
public class VoiceNotifyListenerTests
{
    [Fact]
    [DisplayName("UDP 监听器收到 UTF-8 文本")]
    public async Task ReceivePlainText()
    {
        using var listener = new VoiceNotifyListener();
        var received = new TaskCompletionSource<VoiceNotifyEventArgs>(TaskCreationOptions.RunContinuationsAsynchronously);
        listener.Received += (s, e) => received.TrySetResult(e);
        listener.Start(0);

        Assert.InRange(listener.Port, 1, 65535);

        var payload = Encoding.UTF8.GetBytes("A2 全量自检全部通过");
        using var udp = new UdpClient(AddressFamily.InterNetwork);
        udp.Send(payload, payload.Length, new IPEndPoint(IPAddress.Loopback, listener.Port));

        var args = await received.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal("A2 全量自检全部通过", args.Message.Text);
        Assert.Equal(IPAddress.Loopback, args.RemoteEndPoint.Address);
    }

    [Fact]
    [DisplayName("UDP 监听器解析 JSON 报文")]
    public async Task ReceiveJson()
    {
        using var listener = new VoiceNotifyListener();
        var received = new TaskCompletionSource<VoiceNotifyEventArgs>(TaskCreationOptions.RunContinuationsAsynchronously);
        listener.Received += (s, e) => received.TrySetResult(e);
        listener.Start(0);

        var payload = Encoding.UTF8.GetBytes("""{"text":"测试语音","device":"A2"}""");
        using var udp = new UdpClient(AddressFamily.InterNetwork);
        udp.Send(payload, payload.Length, new IPEndPoint(IPAddress.Loopback, listener.Port));

        var args = await received.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal("测试语音", args.Message.Text);
        Assert.Equal("A2", args.Message.Device);
    }
}
