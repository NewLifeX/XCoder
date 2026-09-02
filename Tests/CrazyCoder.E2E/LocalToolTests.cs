using System.Security.Cryptography;
using System.Text;
using Xunit.Abstractions;

namespace CrazyCoder.E2E;

/// <summary>纯本地工具功能断言：驱动真实 UI 操作并校验结果</summary>
[Collection("CrazyCoder")]
public class LocalToolTests
{
    readonly CrazyCoderSession _s;
    readonly ITestOutputHelper _log;

    public LocalToolTests(CrazyCoderSession session, ITestOutputHelper log)
    {
        _s = session;
        _log = log;
    }

    /// <summary>打开工具并返回子窗体</summary>
    FlaUI.Core.AutomationElements.AutomationElement Open(String category, String name)
    {
        _s.Start();
        var child = _s.OpenTool(new ToolDef(category, name));
        Assert.NotNull(child);
        Thread.Sleep(800);
        return child!;
    }

    /// <summary>取首行（RichTextBox 换行可能是 \r 或 \n）</summary>
    static String? FirstLine(String? text)
    {
        if (text == null) return null;
        return text.Split(['\r', '\n']).FirstOrDefault(x => !String.IsNullOrWhiteSpace(x))?.Trim();
    }

    void Shot(FlaUI.Core.AutomationElements.AutomationElement? child, String name)
    {
        if (child == null) return;
        try { _s.Capture(child, name); } catch { }
    }

    [Fact]
    public void 加密解密工具_MD5计算正确()
    {
        var child = Open("安全工具", "加密解密");
        var ops = new UiOps(_s);
        try
        {
            ops.SelectRadio(child, "rbString");          // 原文=字符串
            ops.SetCheck(child, "cbString", false);      // 结果只要 HEX
            ops.SetCheck(child, "cbBase64", false);
            ops.SetCheck(child, "cbHex", true);
            ops.SetText(child, "rtSource", "123456");
            Thread.Sleep(200);
            ops.Click(child, "btnMD5");                  // MD5_32
            Thread.Sleep(600);

            var result = ops.Read(child, "rtResult");
            var first = FirstLine(result);
            var expected = Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes("123456")));
            Assert.Equal(expected, first);
            _log.WriteLine($"MD5(123456)={first}");
        }
        catch
        {
            Shot(child, "失败-加密解密-MD5");
            throw;
        }
        finally { _s.CloseAllChildren(); }
    }

    [Fact]
    public void 对称加密_AES加解密往返一致()
    {
        var child = Open("安全工具", "对称加密");
        var ops = new UiOps(_s);
        try
        {
            const String plain = "Hello XCoder 2026!";
            const String pass = "NewLife@2026";

            // 输入=字符串、密码=字符串，结果=Base64
            ops.SelectRadio(child, "rbString");
            ops.SelectRadio(child, "rbString2");
            ops.SetCheck(child, "cbString", false);
            ops.SetCheck(child, "cbHex", false);
            ops.SetCheck(child, "cbBase64", true);
            ops.SetText(child, "rtSource", plain);
            ops.SetText(child, "rtPass", pass);
            Thread.Sleep(200);

            // AES 加密
            ops.Click(child, "btnAES");
            Thread.Sleep(600);
            var encB64 = FirstLine(ops.Read(child, "rtResult"));
            Assert.False(String.IsNullOrEmpty(encB64), "AES 加密结果为空");
            _log.WriteLine($"AES 加密密文(Base64)={encB64}");

            // 回填密文并以 Base64 解析原文格式，再解密
            ops.SetText(child, "rtSource", encB64!);
            Thread.Sleep(200);
            ops.SelectRadio(child, "rbBase64");
            Thread.Sleep(200);
            ops.Click(child, "btnAES2");                 // AES 解密
            Thread.Sleep(600);
            var decB64 = FirstLine(ops.Read(child, "rtResult"));
            var expected = Convert.ToBase64String(Encoding.UTF8.GetBytes(plain));
            Assert.Equal(expected, decB64);
            _log.WriteLine($"AES 解密明文(Base64)={decB64}");
        }
        catch
        {
            Shot(child, "失败-对称加密-AES");
            throw;
        }
        finally { _s.CloseAllChildren(); }
    }

    [Fact]
    public void 正则表达式工具_匹配计数正确()
    {
        var child = Open("工具(T)", "正则表达式工具");
        var ops = new UiOps(_s);
        try
        {
            ops.SelectRadio(child, "radioButton1");      // 切到「匹配」模式
            ops.SetText(child, "txtPattern", @"\d+");
            ops.SetText(child, "txtContent", "ab12cd345ef678");
            Thread.Sleep(300);
            ops.Click(child, "button2");                 // 正则匹配
            Thread.Sleep(1200);

            var content = _s.GetTexts(child);
            _log.WriteLine("匹配后状态/内容片段: " + content[..Math.Min(200, content.Length)]);
            Assert.Contains("成功匹配 3 项", content);
        }
        catch
        {
            Shot(child, "失败-正则表达式-匹配");
            throw;
        }
        finally { _s.CloseAllChildren(); }
    }

    [Fact]
    public void GPS辅助_HEX坐标转浮点()
    {
        var child = Open("地理信息", "GPS辅助");
        var ops = new UiOps(_s);
        try
        {
            var lat = 39.9042f;
            var lng = 116.4074f;
            var latHex = Convert.ToHexString(BitConverter.GetBytes(lat));
            var lngHex = Convert.ToHexString(BitConverter.GetBytes(lng));

            ops.SetText(child, "txt_16_lat", latHex);
            ops.SetText(child, "txt_16_long", lngHex);
            Thread.Sleep(200);
            ops.Click(child, "btn_latlong");
            Thread.Sleep(500);

            var actual = ops.Read(child, "txt_latlong");
            var expected = $"{lat},{lng}";
            Assert.Equal(expected, actual);
            _log.WriteLine($"GPS 转换: {actual}");
        }
        catch
        {
            Shot(child, "失败-GPS-Hex转换");
            throw;
        }
        finally { _s.CloseAllChildren(); }
    }
}
