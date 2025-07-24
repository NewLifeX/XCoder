
using NAudio.CoreAudioApi;
using NAudio.Utils;
using NAudio.Wave;
using NewLife;
using NewLife.Log;
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
    [DisplayName("声卡选择器")]
    public partial class AudioSelect : Form,IXForm
    {
        public AudioSelect()
        {
            InitializeComponent();
        }

        /// <summary>选中的声卡</summary>
        public String SelectedDevice { get; set; } = String.Empty;

        /// <summary>音量</summary>
        public float Volume { get; set; } = 0.5f;

        /// <summary>声卡列表</summary>
        public Dictionary<String, AudioDevice> Devices { get; set; }

        private void AudioSelect_Load(object sender, EventArgs e)
        {
            rtb_Info.MouseWheel += Rtb_Show_MouseWheel;

            Devices = AudioHelper.GetAudioDevices();
            // 如果没有声卡，直接退出
            if (Devices == null || Devices.Count < 1)
            {
                WriteLog("没有找到任何声卡");
                MessageBox.Show("没有找到任何声卡", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            var cids = Devices.Select(d => d.Key).ToList();
            cb_Dev.DataSource = cids;
        }

        private void cb_Dev_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 清空信息
            rtb_Info.Clear();
            btn_ok.Enabled = false;
            // 默认 50%
            tb_Volume.Value = tb_Volume.Maximum / 2;

            // 选择的设备ID不能为空
            var cid = cb_Dev.SelectedItem as String;
            if (cid.IsNullOrEmpty()) return;

            // 打印设备信息
            var dev = Devices[cid];
            rtb_Info.Text = dev.ToString();

            /*
            // 需要有播放和录音设备，且至少是立体声，才能启用确定按钮
            bool en = true;
            if (dev.PlaybackDevice == null) en = false;
            if (dev.RecordingDevice == null) en = false;
            if (dev.PlaybackDevice?.AudioClient.MixFormat.Channels < 2) en = false;
            if (dev.RecordingDevice?.AudioClient.MixFormat.Channels < 2) en = false;
            btn_ok.Enabled = en;
            */

            // 如果有播放设备，启用测试按钮
            btn_Spk.Enabled = dev.PlaybackDevice != null;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            var cid = cb_Dev.SelectedItem as String;

            SelectedDevice = cid;
            Volume = tb_Volume.Value * 0.02f;

            this.Close();
        }

        private void btn_Spk_Click(object sender, EventArgs e)
        {
            // 选择的设备ID不能为空
            var cid = cb_Dev.SelectedItem as String;
            if (cid.IsNullOrEmpty()) return;
            var dev = Devices[cid];

            // 设置音量
            dev.PlaybackDevice.AudioEndpointVolume.MasterVolumeLevelScalar = tb_Volume.Value * 0.02f;
            // 确保未静音
            dev.PlaybackDevice.AudioEndpointVolume.Mute = false;

            // 指定声卡播放测试音频
            // var af = new AudioFileReader(@"playTest.wav");
            var af = new WaveFileReader(new MemoryStream(CrazyCoder.Properties.Resources.playTest));
            // af.TotalTime;
            var waveOut = new WasapiOut(dev.PlaybackDevice, AudioClientShareMode.Shared, false, 100);
            waveOut.Init(af);
            waveOut.PlaybackStopped += (s, a) =>
            {
                af.Dispose();
                waveOut.Dispose();
                this.Invoke(() => { btn_Spk.Enabled = true; });
            };
            btn_Spk.Enabled = false;
            waveOut.Play();
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

                // int delta = (short)(m.WParam.ToInt32() >> 16);
                // Console.WriteLine("Mouse wheel moved: " + delta);

                if (delta > 0)
                {
                    // 向上滚动
                    if (cb_Dev.SelectedIndex > 0)
                    {
                        cb_Dev.SelectedIndex--;
                    }
                }
                else if (delta < 0)
                {
                    // 向下滚动
                    if (cb_Dev.SelectedIndex < cb_Dev.Items.Count - 1)
                    {
                        cb_Dev.SelectedIndex++;
                    }
                }

                // 如果你想阻止事件传递给其他控件，可以在这里返回
                return;
            }

            base.WndProc(ref m);
        }

        private void Rtb_Show_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                // 向上滚动
                if (cb_Dev.SelectedIndex > 0)
                {
                    cb_Dev.SelectedIndex--;
                }
            }
            else if (e.Delta < 0)
            {
                // 向下滚动
                if (cb_Dev.SelectedIndex < cb_Dev.Items.Count - 1)
                {
                    cb_Dev.SelectedIndex++;
                }
            }
        }

        #region 日志

        public ILog Log { get; set; } = Logger.Null;

        public void WriteLog(String format, params Object[] args) => Log?.Info(format, args);

        #endregion
    }
}
