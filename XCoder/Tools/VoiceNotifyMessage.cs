using System.Text.Json;
using NewLife;

namespace XCoder.Tools;

/// <summary>网络语音播报消息</summary>
/// <remarks>
/// UDP 报文为 UTF-8 文本。纯文本直接播报；JSON 支持
/// <c>{"text":"...","device":"...","type":"..."}</c>，
/// 以及 devnotify 的 <c>{"name":"A2-4B003A","ok":true,"fail":0}</c> 兼容格式。
/// </remarks>
public class VoiceNotifyMessage
{
    /// <summary>播报文本</summary>
    public String Text { get; set; } = "";

    /// <summary>设备名称</summary>
    public String Device { get; set; } = "";

    /// <summary>消息类型。pass/fail/note</summary>
    public String Type { get; set; } = "";

    /// <summary>解析 UDP 报文。支持纯文本与 JSON</summary>
    /// <param name="payload">UTF-8 文本</param>
    /// <returns>消息对象，无法识别时返回 null</returns>
    public static VoiceNotifyMessage Parse(String payload)
    {
        if (payload.IsNullOrEmpty()) return null;

        payload = payload.Trim().Trim('\uFEFF').Trim();
        if (payload.Length == 0) return null;

        if (payload[0] == '{') return ParseJson(payload);

        return new VoiceNotifyMessage { Text = payload };
    }

    static VoiceNotifyMessage ParseJson(String payload)
    {
        try
        {
            using var doc = JsonDocument.Parse(payload);
            var root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Object) return null;

            var text = GetString(root, "text") ?? GetString(root, "message") ?? GetString(root, "msg");
            var device = GetString(root, "device") ?? GetString(root, "name") ?? "";
            var type = GetString(root, "type") ?? "";

            // 兼容 devnotify 的设备上报格式
            if (text.IsNullOrEmpty() && !device.IsNullOrEmpty() && TryGet(root, "ok", out var ok))
            {
                var pass = ok.ValueKind == JsonValueKind.True;
                var fail = 0;
                if (TryGet(root, "fail", out var failEl) && failEl.ValueKind == JsonValueKind.Number) failEl.TryGetInt32(out fail);

                text = pass
                    ? $"{device} 自检通过，可以换下一台了"
                    : $"{device} 自检未通过{(fail > 0 ? $"（{fail} 项）" : "")}，请检查";
                type = pass ? "pass" : "fail";
            }

            if (text.IsNullOrEmpty()) return null;

            return new VoiceNotifyMessage
            {
                Text = text.Trim(),
                Device = device.Trim(),
                Type = type.Trim(),
            };
        }
        catch (JsonException)
        {
            return null;
        }
    }

    static String GetString(JsonElement root, String name) => TryGet(root, name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;

    static Boolean TryGet(JsonElement root, String name, out JsonElement value)
    {
        if (root.TryGetProperty(name, out value)) return true;

        foreach (var item in root.EnumerateObject())
        {
            if (item.Name.EqualIgnoreCase(name))
            {
                value = item.Value;
                return true;
            }
        }

        return false;
    }
}
