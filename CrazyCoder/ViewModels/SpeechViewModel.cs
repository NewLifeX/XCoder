#nullable enable
using System.Collections.ObjectModel;
using System.Speech.Synthesis;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using NewLife;

namespace CrazyCoder.ViewModels;

/// <summary>语音助手 (TTS) ViewModel</summary>
public partial class SpeechViewModel : ObservableObject
{
    #region 属性
    private SpeechSynthesizer? _synth;

    /// <summary>可用语音列表</summary>
    public ObservableCollection<String> Voices { get; } = [];

    /// <summary>语音数据列表</summary>
    private List<VoiceInfo> _voiceInfos = [];

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
    #endregion

    #region 构造
    /// <summary>实例化语音助手 ViewModel</summary>
    public SpeechViewModel()
    {
        LoadVoices();
    }

    private void LoadVoices()
    {
        try
        {
            using var sync = new SpeechSynthesizer();
            var voices = sync.GetInstalledVoices();
            _voiceInfos = voices.Select(e => e.VoiceInfo).ToList();

            Voices.Clear();
            foreach (var vi in _voiceInfos)
            {
                Voices.Add($"{vi.Name}[{vi.Culture}]");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"加载语音失败：{ex.Message}", "提示");
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

        try
        {
            _synth = CreateSynthesizer();
            _synth.SpeakCompleted += OnSpeakCompleted;
            IsSpeaking = true;
            _synth.SpeakAsync(txt);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"朗读失败：{ex.Message}", "提示");
            IsSpeaking = false;
        }
    }

    /// <summary>停止朗读</summary>
    [RelayCommand]
    private void Stop()
    {
        if (_synth != null)
        {
            try
            {
                _synth.SpeakAsyncCancelAll();
            }
            catch
            {
            }
            _synth.Dispose();
            _synth = null;
        }
        IsSpeaking = false;
    }

    /// <summary>保存为 WAV 文件</summary>
    [RelayCommand]
    private void SaveWav()
    {
        var txt = SpeakText;
        if (txt.IsNullOrEmpty())
        {
            MessageBox.Show("请输入要转换的文本！", "提示");
            return;
        }

        var dlg = new SaveFileDialog
        {
            Filter = "音频文件 (*.wav)|*.wav",
            Title = "保存为 WAV 文件",
            DefaultExt = "wav"
        };
        if (dlg.ShowDialog() != true) return;

        try
        {
            using var sync = CreateSynthesizer();
            sync.SetOutputToWaveFile(dlg.FileName);
            sync.Speak(txt);

            MessageBox.Show("保存成功！", "提示");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"保存失败：{ex.Message}", "提示");
        }
    }
    #endregion

    #region 辅助
    private SpeechSynthesizer CreateSynthesizer()
    {
        var sync = new SpeechSynthesizer
        {
            Volume = Volume,
            Rate = Rate
        };

        if (SelectedVoiceIndex >= 0 && SelectedVoiceIndex < _voiceInfos.Count)
        {
            sync.SelectVoice(_voiceInfos[SelectedVoiceIndex].Name);
        }

        return sync;
    }

    private void OnSpeakCompleted(Object? sender, SpeakCompletedEventArgs e)
    {
        IsSpeaking = false;
        if (_synth != null)
        {
            _synth.SpeakCompleted -= OnSpeakCompleted;
            _synth.Dispose();
            _synth = null;
        }
    }
    #endregion
}
