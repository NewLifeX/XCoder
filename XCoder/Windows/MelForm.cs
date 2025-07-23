using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XCoder
{
    [DisplayName("音频梅尔频谱")]
    public partial class MelForm : Form, IXForm
    {
        public MelForm()
        {
            InitializeComponent();
        }

        private void MelForm_Load(object sender, EventArgs e)
        {

        }

        List<Bitmap> _Mel = new List<Bitmap>();
        List<Bitmap> _Vol = new List<Bitmap>();
        Bitmap _MulVol = null;

        private void bt_Open_Click(object sender, EventArgs e)
        {
            cb_ch.Items.Clear();
            // cb_ch.DataSource = null;
            _Mel = null;
            _Vol = null;

            var fm = new OpenFileDialog();
            fm.ShowDialog();

            var fileName = fm.FileName;
            if (!fileName.EndsWith(".wav")) return;

            // 生成多声道梅尔频谱图
            var mulMel = new MultiChannelAudioProcessor(fileName);
            _Mel = mulMel.GenerateMelSpectrograms();
            _Vol = mulMel.GenerateVolumeCurve();
            _MulVol = mulMel.GenerateVolumeCurveOverlay();
            
            for (int i = 0; i < _Mel.Count; i++)
            {
                cb_ch.Items.Add($"声道{i + 1}");
            }
            cb_ch.SelectedIndex = 0;
        }

        private void cb_ch_SelectedIndexChanged(object sender, EventArgs e)
        {
            var idx = cb_ch.SelectedIndex;
            if (_Mel == null) return;
            if (_Vol == null) return;

            pic_mel.Image = _Mel[idx];
            pic_vol.Image = _Vol[idx];
        }


        protected override void WndProc(ref Message m)
        {
            const int WM_MOUSEWHEEL = 0x020A;

            if (m.Msg == WM_MOUSEWHEEL)
            {
                // 在这里处理你的全局滚轮事件
                short delta = 0;
                if (IntPtr.Size == 8)
                {
                    // 64位系统
                    delta = (short)(m.WParam.ToInt64() >> 16);
                }
                else
                {
                    // 32位系统
                    delta = (short)(m.WParam.ToInt32() >> 16);
                }

                if (delta > 0)
                {
                    // 向上滚动
                    if (cb_ch.SelectedIndex > 0)
                    {
                        cb_ch.SelectedIndex--;
                    }
                }
                else if (delta < 0)
                {
                    // 向下滚动
                    if (cb_ch.SelectedIndex < cb_ch.Items.Count - 1)
                    {
                        cb_ch.SelectedIndex++;
                    }
                }

                // 如果你想阻止事件传递给其他控件，可以在这里返回
                return;
            }

            base.WndProc(ref m);
        }

    }
}
