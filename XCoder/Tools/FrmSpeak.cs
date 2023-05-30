using System.ComponentModel;
using System.Speech.Synthesis;
using NewLife;

namespace XCoder.Tools;

[DisplayName("语音助手")]
public partial class FrmSpeak : Form, IXForm
{
    private IList<VoiceInfo> _voices;

    public FrmSpeak()
    {
        InitializeComponent();
    }

    private void FrmSpeak_Load(Object sender, EventArgs e)
    {
        var sync = new SpeechSynthesizer();
        var voices = sync.GetInstalledVoices();
        //foreach (var vo in voices)
        //{
        //    XTrace.WriteLine(vo.VoiceInfo.Name);
        //}
        //var dic = voices.ToDictionary(e => e.VoiceInfo.Id, e => e.VoiceInfo.Name);
        _voices = voices.Select(e => e.VoiceInfo).ToList();

        cbVoices.DataSource = _voices.Select(e => $"{e.Name}[{e.Culture}]").ToList();
    }

    private void btnSpeak_Click(Object sender, EventArgs e)
    {
        var txt = richTextBox1.Text;
        if (txt.IsNullOrEmpty()) return;

        //txt.SpeakAsync();

        var sync = new SpeechSynthesizer
        {
            Volume = (Int32)numVolume.Value,
            Rate = (Int32)numRate.Value
        };

        var name = cbVoices.SelectedItem as String;
        var p = name.IndexOf('[');
        if (p > 0) name = name[..p];
        sync.SelectVoice(name);
        //sync.Voice = _voices.FirstOrDefault(e => e.Name == name);

        sync.SpeakAsync(txt);
    }
}
