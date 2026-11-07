using System.ComponentModel;
using XCoder.Tools;

namespace XCoder.Tests;

/// <summary>网络语音播报报文解析</summary>
public class VoiceNotifyMessageTests
{
    [Fact]
    [DisplayName("纯文本报文直接作为播报内容")]
    public void ParsePlainText()
    {
        var msg = VoiceNotifyMessage.Parse("  A2 烧写完成，自检通过  ");

        Assert.NotNull(msg);
        Assert.Equal("A2 烧写完成，自检通过", msg!.Text);
        Assert.Equal("", msg.Device);
    }

    [Fact]
    [DisplayName("JSON 报文提取 text/device/type")]
    public void ParseJsonText()
    {
        var msg = VoiceNotifyMessage.Parse("""{"text":"全量自检通过","device":"A2-4B003A","type":"pass"}""");

        Assert.NotNull(msg);
        Assert.Equal("全量自检通过", msg!.Text);
        Assert.Equal("A2-4B003A", msg.Device);
        Assert.Equal("pass", msg.Type);
    }

    [Fact]
    [DisplayName("JSON 字段名大小写不敏感")]
    public void ParseJsonCaseInsensitive()
    {
        var msg = VoiceNotifyMessage.Parse("""{"Text":"测试语音","Device":"A2"}""");

        Assert.NotNull(msg);
        Assert.Equal("测试语音", msg!.Text);
        Assert.Equal("A2", msg.Device);
    }

    [Fact]
    [DisplayName("兼容 devnotify 自检通过报文")]
    public void ParseDevnotifyPass()
    {
        var msg = VoiceNotifyMessage.Parse("""{"name":"A2-4B003A","ok":true,"fail":0,"secs":12}""");

        Assert.NotNull(msg);
        Assert.Equal("A2-4B003A 自检通过，可以换下一台了", msg!.Text);
        Assert.Equal("A2-4B003A", msg.Device);
        Assert.Equal("pass", msg.Type);
    }

    [Fact]
    [DisplayName("兼容 devnotify 自检失败报文")]
    public void ParseDevnotifyFail()
    {
        var msg = VoiceNotifyMessage.Parse("""{"name":"A2-4B003A","ok":false,"fail":3}""");

        Assert.NotNull(msg);
        Assert.Equal("A2-4B003A 自检未通过（3 项），请检查", msg!.Text);
        Assert.Equal("fail", msg.Type);
    }

    [Theory]
    [DisplayName("空报文或无效 JSON 返回空")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("{\"foo\":1}")]
    [InlineData("{bad json")]
    public void ParseInvalid(String payload)
    {
        Assert.Null(VoiceNotifyMessage.Parse(payload));
    }
}
