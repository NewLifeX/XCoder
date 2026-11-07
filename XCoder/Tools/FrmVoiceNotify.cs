using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Speech.Synthesis;
using NewLife;
using NewLife.Log;

namespace XCoder.Tools;

/// <summary>网络语音播报</summary>
/// <remarks>
/// 监听 UDP 端口，接收局域网设备广播的文字并调用 Windows TTS 朗读。
/// 设备侧可通过 A2 镜像内置的 <c>ttsnotify</c> 命令发送，也可直接发送 UTF-8 文本。
/// </remarks>
[Category("工具")]
[DisplayName("网络语音播报")]
public partial class FrmVoiceNotify : Form, IXForm
{
    #region 窗体
    private readonly List<VoiceInfo> _voices = [];
    private VoiceNotifyListener _listener;
    private SpeechSynthesizer _speech;
    private Int32 _count;

    /// <summary>实例化</summary>
    public FrmVoiceNotify()
    {
        InitializeComponent();
    }

    private void FrmVoiceNotify_Load(Object sender, EventArgs e)
    {
        var cfg = XConfig.Current;
        numPort.Value = cfg.VoiceNotifyPort is > 0 and <= 65535 ? cfg.VoiceNotifyPort : 5502;
        numVolume.Value = Math.Clamp(cfg.VoiceNotifyVolume, 0, 100);
        numRate.Value = Math.Clamp(cfg.VoiceNotifyRate, -10, 10);
        cbAutoStart.Checked = cfg.VoiceNotifyAutoStart;

        LoadVoices(cfg.VoiceNotifyVoice);

        if (cbAutoStart.Checked) StartListen();
    }

    private void FrmVoiceNotify_FormClosing(Object sender, FormClosingEventArgs e)
    {
        StopListen();

        try { _speech?.SpeakAsyncCancelAll(); } catch { }
        _speech?.Dispose();
        _speech = null;

        SaveConfig();
    }
    #endregion

    #region 监听
    private void btnListen_Click(Object sender, EventArgs e)
    {
        if (_listener == null)
            StartListen();
        else
            StopListen();
    }

    private void StartListen()
    {
        if (_listener != null) return;

        var listener = new VoiceNotifyListener();
        listener.Received += OnReceived;

        try
        {
            listener.Start((Int32)numPort.Value);
        }
        catch (Exception ex)
        {
            listener.Dispose();

            SetStatus("启动失败", Color.Firebrick);
            AddMessage(DateTime.Now, "系统", $"监听 UDP {(Int32)numPort.Value} 失败：{ex.Message}");
            XTrace.WriteLine($"[网络语音播报] 启动监听失败：{ex}");

            return;
        }

        _listener = listener;

        numPort.Enabled = false;
        btnListen.Text = "停止监听";
        SetStatus($"监听中 :{listener.Port}", Color.SeaGreen);
        XTrace.WriteLine($"[网络语音播报] 开始监听 UDP {listener.Port}（组播 {listener.Group}）");

        SaveConfig();
    }

    private void StopListen()
    {
        var listener = _listener;
        if (listener == null) return;

        _listener = null;
        listener.Received -= OnReceived;
        listener.Dispose();

        numPort.Enabled = true;
        btnListen.Text = "开始监听";
        SetStatus("已停止", Color.DimGray);
        XTrace.WriteLine("[网络语音播报] 停止监听");

        SaveConfig();
    }

    private void lnkFirewall_LinkClicked(Object sender, LinkLabelLinkClickedEventArgs e)
    {
        var port = (Int32)numPort.Value;
        var exe = Application.ExecutablePath;
        var name = "XCoder VoiceNotify";
        var script = $"Remove-NetFirewallRule -DisplayName '{name}' -ErrorAction SilentlyContinue; " +
            $"New-NetFirewallRule -DisplayName '{name}' -Direction Inbound -Program '{exe.Replace("'", "''")}' -Protocol UDP -LocalPort {port} -Action Allow -Profile Any | Out-Null";

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script}\"",
                UseShellExecute = true,
                Verb = "runas",
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            Process.Start(psi);

            MessageBox.Show($"已请求管理员放行 UDP {port}。\r\n若已取消，可手动执行：\r\nnetsh advfirewall firewall add rule name=\"{name}\" dir=in action=allow program=\"{exe}\" protocol=UDP localport={port}",
                "放行防火墙", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"放行失败：{ex.Message}\r\n\r\n可手动以管理员身份执行：\r\nnetsh advfirewall firewall add rule name=\"{name}\" dir=in action=allow program=\"{exe}\" protocol=UDP localport={port}",
                "放行防火墙", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void OnReceived(Object sender, VoiceNotifyEventArgs e)
    {
        if (IsDisposed) return;

        try
        {
            BeginInvoke(new Action(() => HandleMessage(e)));
        }
        catch (InvalidOperationException) { }
    }

    private void HandleMessage(VoiceNotifyEventArgs e)
    {
        if (IsDisposed) return;

        var msg = e.Message;
        var ip = e.RemoteEndPoint?.Address + "";
        var source = msg.Device.IsNullOrEmpty() ? ip : $"{msg.Device} ({ip})";

        AddMessage(DateTime.Now, source, msg.Text);
        Speak(msg.Text);
    }
    #endregion

    #region TTS
    private void LoadVoices(String voiceName)
    {
        try
        {
            using var speech = new SpeechSynthesizer();
            _voices.AddRange(speech.GetInstalledVoices().Where(e => e.Enabled).Select(e => e.VoiceInfo));
        }
        catch (Exception ex)
        {
            XTrace.WriteLine($"[网络语音播报] 读取语音列表失败：{ex.Message}");
        }

        if (_voices.Count == 0)
        {
            cbVoices.Items.Add("（系统默认）");
            cbVoices.SelectedIndex = 0;
            cbVoices.Enabled = false;
            return;
        }

        cbVoices.DataSource = _voices.Select(e => $"{e.Name}[{e.Culture}]").ToList();

        var index = _voices.FindIndex(e => e.Name.EqualIgnoreCase(voiceName));
        if (index < 0) index = _voices.FindIndex(e => e.Culture.Name.StartsWith("zh"));
        if (index < 0) index = 0;
        cbVoices.SelectedIndex = index;
    }

    private String GetSelectedVoiceName()
    {
        if (cbVoices.SelectedItem is not String name) return "";

        var index = name.IndexOf('[');
        return index > 0 ? name[..index] : name;
    }

    private SpeechSynthesizer CreateSpeech()
    {
        try
        {
            var speech = new SpeechSynthesizer();
            speech.SetOutputToDefaultAudioDevice();
            return speech;
        }
        catch (Exception ex)
        {
            XTrace.WriteLine($"[网络语音播报] 初始化 TTS 失败：{ex.Message}");
            SetStatus("TTS 不可用", Color.Firebrick);
            return null;
        }
    }

    private void Speak(String text)
    {
        text = (text ?? "").Replace('\r', ' ').Replace('\n', ' ').Trim();
        if (text.Length == 0) return;
        if (text.Length > 1000) text = text[..1000] + "……";

        try
        {
            _speech ??= CreateSpeech();
            if (_speech == null) return;

            // 播放过程中切换声音会抛异常，留到下一句再生效
            var voice = GetSelectedVoiceName();
            if (!voice.IsNullOrEmpty() && _speech.State != SynthesizerState.Speaking)
            {
                try { _speech.SelectVoice(voice); } catch { }
            }

            _speech.Volume = (Int32)numVolume.Value;
            _speech.Rate = (Int32)numRate.Value;
            _speech.SpeakAsync(text);

            if (_listener != null) SetStatus($"监听中 :{_listener.Port}", Color.SeaGreen);
        }
        catch (Exception ex)
        {
            XTrace.WriteLine($"[网络语音播报] TTS 播放失败：{ex.Message}");
            if (_listener != null) SetStatus("监听中（TTS异常）", Color.OrangeRed);
        }
    }

    private void btnTest_Click(Object sender, EventArgs e)
    {
        var text = txtTest.Text.Trim();
        if (text.Length == 0) return;

        AddMessage(DateTime.Now, "本机", text);
        Speak(text);
    }
    #endregion

    #region 辅助
    private void AddMessage(DateTime time, String source, String text)
    {
        var item = new ListViewItem(time.ToString("HH:mm:ss"));
        item.SubItems.Add(source ?? "");
        item.SubItems.Add(text ?? "");
        lvMessages.Items.Insert(0, item);

        while (lvMessages.Items.Count > 500)
        {
            lvMessages.Items.RemoveAt(lvMessages.Items.Count - 1);
        }

        _count++;
        lblCount.Text = $"共 {_count} 条";
    }

    private void btnClear_Click(Object sender, EventArgs e)
    {
        lvMessages.Items.Clear();
        _count = 0;
        lblCount.Text = "共 0 条";
    }

    private void SetStatus(String text, Color color)
    {
        lblStatus.Text = text;
        lblStatus.ForeColor = color;
    }

    private void SaveConfig()
    {
        try
        {
            var cfg = XConfig.Current;
            cfg.VoiceNotifyPort = (Int32)numPort.Value;
            cfg.VoiceNotifyAutoStart = cbAutoStart.Checked;
            cfg.VoiceNotifyVoice = GetSelectedVoiceName();
            cfg.VoiceNotifyVolume = (Int32)numVolume.Value;
            cfg.VoiceNotifyRate = (Int32)numRate.Value;
            cfg.Save();
        }
        catch (Exception ex)
        {
            XTrace.WriteLine($"[网络语音播报] 保存配置失败：{ex.Message}");
        }
    }
    #endregion
}
