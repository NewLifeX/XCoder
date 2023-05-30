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

    SpeechSynthesizer GetSynthesizer()
    {
        var sync = new SpeechSynthesizer
        {
            Volume = (Int32)numVolume.Value,
            Rate = (Int32)numRate.Value
        };

        var name = cbVoices.SelectedItem as String;
        var p = name.IndexOf('[');
        if (p > 0) name = name[..p];
        sync.SelectVoice(name);

        return sync;
    }

    private void btnSpeak_Click(Object sender, EventArgs e)
    {
        var txt = richTextBox1.Text;
        if (txt.IsNullOrEmpty()) return;

        //txt.SpeakAsync();

        var sync = GetSynthesizer();

        sync.SpeakAsync(txt);
    }

    private void btnSave_Click(Object sender, EventArgs e)
    {
        var txt = richTextBox1.Text;
        if (txt.IsNullOrEmpty()) return;

        var sync = GetSynthesizer();

        var flg = new SaveFileDialog();
        flg.Filter = "音频文件(*.wav)|*.wav";
        if (flg.ShowDialog() == DialogResult.OK)
        {
            sync.SetOutputToWaveFile(flg.FileName);

            sync.SpeakAsync(txt);

            MessageBox.Show("保存成功！");
        }
    }
}
