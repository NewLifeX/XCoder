using ALib.Audio;
using ALib.WinForm;
using NAudio.Wave;
using NewLife.Log;
using System.Drawing;
using NewLife;

namespace ALib.Audio
{
    public partial class AudioRecordForm : Form
    {
        public AudioRecordForm()
        {
            InitializeComponent();
        }

        private AudioRecord _recorder;

        private float _leftVolume;
        private float _rightVolume;

        /// <summary>左声道梅尔特征提取器</summary>
        private AudioFeatureExtractor _afeLeft;
        /// <summary>右声道梅尔特征提取器</summary>
        private AudioFeatureExtractor _afeRight;

        /// <summary>左声道梅尔流动图</summary>
        private MelStreamImage _melLeft;
        /// <summary>右声道梅尔流动图</summary>
        private MelStreamImage _melRight;

        /// <summary>选中的声卡设备</summary>
        public AudioDevice? SelectedDevice { get; set; }

        private void AudioRecordForm_Load(object sender, EventArgs e)
        {
            // 显示设备信息
            if (SelectedDevice != null)
            {
                Text = $"录音测试 - {SelectedDevice.Name}";
                rtbInfo.Text = SelectedDevice.ToString();
            }

            // 初始化音量显示
            UpdateVolumeDisplay();

            // 初始化梅尔图
            InitMelDisplay();
        }

        /// <summary>初始化梅尔图显示</summary>
        private void InitMelDisplay()
        {
            _melLeft = new MelStreamImage(pbMelL.Width) { ColorMode = 0 };
            _melRight = new MelStreamImage(pbMelR.Width) { ColorMode = 0 };

            pbMelL.Image = null;
            pbMelR.Image = null;
        }

        private void btnSelectDevice_Click(object sender, EventArgs e)
        {
            var form = new AudioSelect();
            form.ShowDialog();

            SelectedDevice = form.Devices[form.SelectedDevice];
            rtbInfo.Text = SelectedDevice.ToString();
            Text = $"录音测试 - {SelectedDevice.Name}";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (SelectedDevice?.RecordingDevice == null)
            {
                MessageBox.Show("请先选择声卡设备", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                WriteLog($"使用录音设备: {SelectedDevice.RecordingDevice.DeviceFriendlyName}");

                // 使用 AudioRecord 创建录音实例
                _recorder = new AudioRecord(SelectedDevice);

                var recordFormat = _recorder.RecordFormat;
                WriteLog($"设备实际格式: {recordFormat.Encoding}, {recordFormat.SampleRate}Hz, {recordFormat.Channels}声道, {recordFormat.BitsPerSample}位");
                WriteLog($"保存文件: {(cbSaveFile.Checked ? "是" : "否")}");

                // 初始化梅尔特征提取器（按设备实际格式与声道数）
                CreateMelExtractors(recordFormat);

                // 订阅音量事件
                _recorder.VolumeUpdated += OnVolumeUpdated;

                // 订阅音频数据事件（用于实时梅尔图）
                _recorder.DataUpdated += OnDataUpdated;

                // 订阅完成事件
                _recorder.Stoped += OnRecorderStoped;

                // 开始录音（-1 表示一直录，直到手动停止）
                if (!_recorder.Start(-1))
                {
                    throw new Exception(L.T("Failed to start recording."));
                }

                // 更新UI
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                btnSelectDevice.Enabled = false;
                cbSaveFile.Enabled = false;
                lbRecordStatus.Text = "正在录音...";
                lbRecordStatus.ForeColor = Color.Red;

                WriteLog("开始录音");
            }
            catch (Exception ex)
            {
                WriteLog($"启动录音失败: {ex.Message}");
                MessageBox.Show($"启动录音失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>音量更新回调</summary>
        private void OnVolumeUpdated(Object sender, byte[] vols)
        {
            _leftVolume = vols[0];
            _rightVolume = vols[1];

            this.UpdateUIControl(UpdateVolumeDisplay);
        }

        /// <summary>创建梅尔特征提取器</summary>
        /// <param name="format">设备录音格式</param>
        private void CreateMelExtractors(WaveFormat format)
        {
            if (format == null) return;

            _afeLeft = new AudioFeatureExtractor(format, 30);
            _melLeft = new MelStreamImage(pbMelL.Width) { ColorMode = 0 };

            // 单声道时右声道复用左声道数据
            _afeRight = new AudioFeatureExtractor(format, 30);
            _melRight = new MelStreamImage(pbMelR.Width) { ColorMode = 0 };
        }

        /// <summary>音频数据更新回调，实时刷新梅尔图</summary>
        private void OnDataUpdated(Object sender, float[][] audioData)
        {
            if (audioData == null || audioData.Length == 0) return;
            if (_afeLeft == null || _melLeft == null) return;

            try
            {
                // 左声道
                var dbsLeft = _afeLeft.ProcessAsDecibels(audioData[0]);
                if (dbsLeft != null && dbsLeft.Length > 0)
                {
                    _melLeft.PushDbs(dbsLeft);
                    var imgL = _melLeft.GetImage("L");
                    this.UpdateUIControl(() =>
                    {
                        var old = pbMelL.Image;
                        pbMelL.Image = imgL;
                        old?.Dispose();
                    });
                }

                // 右声道：多声道取第二声道，单声道复用第一声道
                var rightIndex = audioData.Length > 1 ? 1 : 0;
                var dbsRight = _afeRight.ProcessAsDecibels(audioData[rightIndex]);
                if (dbsRight != null && dbsRight.Length > 0)
                {
                    _melRight.PushDbs(dbsRight);
                    var imgR = _melRight.GetImage("R");
                    this.UpdateUIControl(() =>
                    {
                        var old = pbMelR.Image;
                        pbMelR.Image = imgR;
                        old?.Dispose();
                    });
                }
            }
            catch (Exception ex)
            {
                WriteLog($"梅尔图计算失败: {ex.Message}");
            }
        }

        /// <summary>录音停止回调</summary>
        private void OnRecorderStoped(object sender, EventArgs e)
        {
            // 清理梅尔图数据
            _melLeft?.Clear();
            _melRight?.Clear();

            // 更新UI
            this.UpdateUIControl(() =>
            {
                btnStart.Enabled = true;
                btnStop.Enabled = false;
                btnSelectDevice.Enabled = true;
                cbSaveFile.Enabled = true;
                lbRecordStatus.Text = "录音已停止";
                lbRecordStatus.ForeColor = Color.Black;

                // 重置音量显示
                _leftVolume = 0;
                _rightVolume = 0;
                UpdateVolumeDisplay();

                // 清理梅尔图显示
                if (pbMelL.Image != null) { pbMelL.Image.Dispose(); pbMelL.Image = null; }
                if (pbMelR.Image != null) { pbMelR.Image.Dispose(); pbMelR.Image = null; }

                // 如果勾选了保存文件，转存到磁盘
                if (cbSaveFile.Checked && _recorder != null && _recorder.AudioData.Length > 0)
                {
                    try
                    {
                        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"record_{timestamp}.wav");

                        if (_recorder.SaveTo(path))
                        {
                            WriteLog($"录音完成: {path}");
                        }
                        else
                        {
                            WriteLog("录音完成但保存文件失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog($"保存文件失败: {ex.Message}");
                    }
                }
                else
                {
                    WriteLog("录音完成（未保存）");
                }
            });
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                _recorder?.Stop();
            }
            catch (Exception ex)
            {
                WriteLog($"停止录音失败: {ex.Message}");
            }
        }

        private void UpdateVolumeDisplay()
        {
            progressLeft.Value = (int)Math.Min(_leftVolume, 100);
            progressRight.Value = (int)Math.Min(_rightVolume, 100);
            lbLeft.Text = $"L: {_leftVolume:F1}%";
            lbRight.Text = $"R: {_rightVolume:F1}%";
        }

        private void AudioRecordForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_recorder != null)
            {
                _recorder.DataUpdated -= OnDataUpdated;
                if (_recorder.IsRecording) _recorder.Stop();
            }
        }

        #region 日志

        public ILog Log { get; set; } = Logger.Null;

        private void WriteLog(String format, params Object[] args)
        {
            var msg = String.Format(format, args);
            rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\r\n");
            Log?.Info(format, args);
        }

        #endregion
    }
}
