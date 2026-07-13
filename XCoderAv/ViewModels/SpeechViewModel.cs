using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;

namespace XCoderAv.ViewModels;

/// <summary>语音助手 (TTS) ViewModel（跨平台版）</summary>
public partial class SpeechViewModel : ObservableObject
{
    #region 属性
    /// <summary>可用语音列表</summary>
    public ObservableCollection<String> Voices { get; } = [];

    /// <summary>选中语音索引</summary>
    [ObservableProperty]
    private Int32 _selectedVoiceIndex;

    /// <summary>音量 (0-100)</summary>
    [ObservableProperty]
    private Int32 _volume = 100;

    /// <summary>语速 (-10 到 10)</summary>
    [ObservableProperty]
    private Int32 _rate;

    /// <summary>朗读文本</summary>
    [ObservableProperty]
    private String _speakText = "学无先后达者为师";

    /// <summary>是否正在朗读</summary>
    [ObservableProperty]
    private Boolean _isSpeaking;

    /// <summary>状态文本</summary>
    [ObservableProperty]
    private String _status = "就绪";

    /// <summary>TTS 引擎可用</summary>
    [ObservableProperty]
    private Boolean _isTtsAvailable;

    /// <summary>日志文本</summary>
    [ObservableProperty]
    private String _logText = "";
    #endregion

    #region 构造
    /// <summary>实例化语音助手 ViewModel</summary>
    public SpeechViewModel()
    {
        LoadVoices();
    }

    private void LoadVoices()
    {
        // 跨平台 TTS 支持：Windows 使用 System.Speech，其他平台暂不支持
        // 可通过条件编译或运行时检测扩展
        var available = false;
        var voices = new List<String>();

#if NETFRAMEWORK || WINDOWS
        try
        {
            using var synth = new System.Speech.Synthesis.SpeechSynthesizer();
            foreach (var vi in synth.GetInstalledVoices().Select(e => e.VoiceInfo))
            {
                voices.Add($"{vi.Name}[{vi.Culture}]");
            }
            available = voices.Count > 0;
        }
        catch (Exception ex)
        {
            voices.Add($"TTS 不可用：{ex.Message}");
        }
#else
        // 跨平台 TTS 可通过 System.Speech 在 Windows 上工作
        // Linux/macOS 需要额外库
        try
        {
            // 尝试加载 System.Speech (仅 Windows)
            var asm = System.Reflection.Assembly.Load("System.Speech");
            if (asm != null)
            {
                var type = asm.GetType("System.Speech.Synthesis.SpeechSynthesizer");
                if (type != null)
                {
                    using var synth = (IDisposable)Activator.CreateInstance(type)!;
                    var getVoices = type.GetMethod("GetInstalledVoices");
                    if (getVoices != null)
                    {
                        var installedVoices = (System.Collections.IList)getVoices.Invoke(synth, null)!;
                        foreach (var iv in installedVoices)
                        {
                            var viProp = iv.GetType().GetProperty("VoiceInfo");
                            if (viProp?.GetValue(iv) is Object vi)
                            {
                                var name = vi.GetType().GetProperty("Name")?.GetValue(vi)?.ToString() ?? "";
                                var culture = vi.GetType().GetProperty("Culture")?.GetValue(vi)?.ToString() ?? "";
                                voices.Add($"{name}[{culture}]");
                            }
                        }
                        available = voices.Count > 0;
                    }
                }
            }
        }
        catch
        {
            // TTS 不可用
        }
#endif

        IsTtsAvailable = available;

        Voices.Clear();
        if (voices.Count > 0)
        {
            foreach (var v in voices)
                Voices.Add(v);
        }
        else
        {
            Voices.Add("TTS 引擎不可用（仅支持 Windows）");
            Status = "TTS 引擎不可用，当前平台不支持语音合成";
        }
    }
    #endregion

    #region 命令
    /// <summary>朗读</summary>
    [RelayCommand]
    private void Speak()
    {
        var txt = SpeakText;
        if (txt.IsNullOrEmpty()) return;

        Stop();

#if NETFRAMEWORK || WINDOWS
        try
        {
            var synth = new System.Speech.Synthesis.SpeechSynthesizer
            {
                Volume = Volume,
                Rate = Rate
            };

            if (SelectedVoiceIndex >= 0 && SelectedVoiceIndex < Voices.Count)
            {
                // 获取实际语音名称
                var voiceName = Voices[SelectedVoiceIndex];
                if (voiceName.Contains('['))
                    voiceName = voiceName[..voiceName.IndexOf('[')];
                synth.SelectVoice(voiceName);
            }

            IsSpeaking = true;
            Status = "正在朗读...";
            synth.SpeakCompleted += (s, e) =>
            {
                IsSpeaking = false;
                Status = "朗读完成";
                synth.Dispose();
            };
            synth.SpeakAsync(txt);
        }
        catch (Exception ex)
        {
            Status = $"朗读失败：{ex.Message}";
            IsSpeaking = false;
        }
#else
        Status = "当前平台不支持语音合成";
#endif
    }

    /// <summary>停止朗读</summary>
    [RelayCommand]
    private void Stop()
    {
#if NETFRAMEWORK || WINDOWS
        try
        {
            // 通过反射停止所有正在进行的朗读
            var asm = System.Reflection.Assembly.Load("System.Speech");
            var synthType = asm.GetType("System.Speech.Synthesis.SpeechSynthesizer");
            if (synthType != null)
            {
                // 创建新实例来取消
                using var synth = (IDisposable)Activator.CreateInstance(synthType)!;
                var speakAsyncCancelAll = synthType.GetMethod("SpeakAsyncCancelAll");
                speakAsyncCancelAll?.Invoke(synth, null);
            }
        }
        catch
        {
        }
#endif
        IsSpeaking = false;
        Status = "已停止";
    }

    /// <summary>保存为 WAV 文件</summary>
    [RelayCommand]
    private void SaveWav()
    {
        var txt = SpeakText;
        if (txt.IsNullOrEmpty())
        {
            Status = "请输入要转换的文本！";
            return;
        }

        Status = "当前平台不支持保存为 WAV 文件";
    }
    #endregion
}
