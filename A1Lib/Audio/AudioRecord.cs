using NAudio.CoreAudioApi;
using NAudio.Dmo;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALib.Audio
{
    /// <summary>
    /// 录音，只录wav格式
    /// 数据默认存内存，不存磁盘，提供 SaveTo 转存功能
    /// </summary>
    public class AudioRecord : IDisposable
    {
        #region 属性

        /// <summary>存文件的wav格式，如果与设备录音格式不同，会自动转换为此格式</summary>
        public WaveFormat Format { get; set; }

        /// <summary>设备实际录音格式</summary>
        public WaveFormat RecordFormat => _waveIn?.WaveFormat;

        /// <summary>是否正在录音</summary>
        public bool IsRecording { get; private set; }

        /// <summary>录音数据（内存）</summary>
        public MemoryStream AudioData => _memoryStream;

        /// <summary>开始录音时，会有一段空白数据，此长度为忽略的帧数</summary>
        public Int32 IgnoreLength { get; set; } = 16000;

        /// <summary>还需要忽略的剩余长度,启动录音时重置</summary>
        private Int32 _IgnoreRemain;

        #endregion

        #region 字段

        /// <summary>最大内存容量 (512MB)</summary>
        public const long MaxMemorySize = 512L * 1024 * 1024;

        /// <summary>录音采集器</summary>
        private WasapiCapture _waveIn;

        /// <summary>内存流，用于存储录音原始数据</summary>
        private MemoryStream _memoryStream;

        /// <summary>定时停止用的 CancellationTokenSource</summary>
        private System.Threading.CancellationTokenSource _cts;

        /// <summary>是否因内存满而停止</summary>
        private bool _stoppedByMemoryLimit;

        #endregion

        #region 构造

        /// <summary>
        /// 创建录音实例
        /// </summary>
        /// <param name="dev">录音设备</param>
        public AudioRecord(AudioDevice dev)
        {
            if (dev == null) throw new ArgumentNullException(nameof(dev));
            if (dev.RecordingDevice == null) throw new ArgumentException("录音设备的 RecordingDevice 不能为 null", nameof(dev));
            if (dev.RecordingDevice.AudioClient.MixFormat.Channels < 1)
                throw new ArgumentException("录音设备声道数无效", nameof(dev));

            // 使用 WasapiCapture 直接通过 MMDevice 录音
            _waveIn = new WasapiCapture(dev.RecordingDevice) { ShareMode = AudioClientShareMode.Shared };

            // 初始化内存流
            _memoryStream = new MemoryStream();

            // 注册事件
            _waveIn.DataAvailable += OnDataAvailable;
            _waveIn.RecordingStopped += OnRecordingStopped;

            Format = new WaveFormat(16000, 16, RecordFormat.Channels);
        }

        #endregion

        #region 异步

        /// <summary>启动录音</summary>
        /// <param name="ms">录音时长(毫秒)，-1：一直录直到手动停止</param>
        /// <returns></returns>
        public bool Start(int ms = -1)
        {
            if (IsRecording) return false;

            try
            {
                // 重置内存流，默认32M容量
                _memoryStream?.Dispose();
                _memoryStream = new MemoryStream(32 * 1024 * 1024);

                // 重置内存满标志
                _stoppedByMemoryLimit = false;
                _IgnoreRemain = IgnoreLength;

                _waveIn.StartRecording();
                IsRecording = true;

                // 如果指定了时长，设置定时停止
                if (ms > 0)
                {
                    _cts = new System.Threading.CancellationTokenSource();
                    Task.Delay(ms, _cts.Token).ContinueWith(_ =>
                    {
                        if (!_cts.Token.IsCancellationRequested)
                            Stop();
                    });
                }

                return true;
            }
            catch (Exception)
            {
                IsRecording = false;
                return false;
            }
        }

        /// <summary>停止录音</summary>
        public void Stop()
        {
            if (!IsRecording) return;

            // 取消定时器
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            try
            {
                _waveIn.StopRecording();
            }
            catch (Exception) { }
        }

        /// <summary>录音完成事件</summary>
        public event EventHandler Stoped;

        #endregion

        #region 同步

        /// <summary>录音，录完返回</summary>
        /// <param name="ms">录音时长(毫秒)</param>
        /// <returns>是否成功</returns>
        public async Task<bool> Record(int ms)
        {
            var tcs = new TaskCompletionSource<bool>();

            EventHandler handler = null;
            handler = (s, e) =>
            {
                Stoped -= handler;
                tcs.TrySetResult(true);
            };
            Stoped += handler;

            if (!Start(ms)) return false;

            return await tcs.Task;
        }

        #endregion

        #region 数据输出

        /// <summary>
        /// 将内存中的录音数据转存为 WAV 文件
        /// </summary>
        /// <param name="path">保存路径（如 "C:/output.wav"）</param>
        /// <returns>是否成功</returns>
        public bool SaveTo(string path)
        {
            if (_memoryStream == null || _memoryStream.Length == 0)
                return false;

            try
            {
                // 确定使用的格式：优先使用用户设定的 Format，否则使用实际录音格式
                var targetFormat = Format ?? _waveIn.WaveFormat;

                string dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                using var writer = new WaveFileWriter(fs, targetFormat);

                // 将内存数据写入文件
                var buffer = _memoryStream.ToArray();

                // 如果目标格式与录音格式一致，直接写入
                if (targetFormat.Encoding == _waveIn.WaveFormat.Encoding &&
                    targetFormat.SampleRate == _waveIn.WaveFormat.SampleRate &&
                    targetFormat.Channels == _waveIn.WaveFormat.Channels &&
                    targetFormat.BitsPerSample == _waveIn.WaveFormat.BitsPerSample)
                {
                    writer.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    // 需要转换格式：从内存流读取原始数据并重采样
                    using var msCopy = new MemoryStream(buffer);
                    using var reader = new RawWaveStream(msCopy, _waveIn.WaveFormat);
                    using var resampler = new MediaFoundationResampler(reader, targetFormat);

                    byte[] resampleBuffer = new byte[4096];
                    int read;
                    while ((read = resampler.Read(resampleBuffer, 0, resampleBuffer.Length)) > 0)
                    {
                        writer.Write(resampleBuffer, 0, read);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>获取数据</summary>
        /// <remarks>长度不足返回静音内容，在后面填充静音</remarks>
        /// <param name="length">数据个数，每声道都有。-1表示取所有数据</param>
        /// <returns>按照声道获取的音频数据，格式为 AudioFileReader 按照声道整理</returns>
        public float[][] ReadAudioData(int length = -1)
        {
            int channelCount = _waveIn?.WaveFormat?.Channels ?? 1;

            // 无数据时返回静音
            if (_memoryStream == null || _memoryStream.Length == 0)
            {
                // -1 时返回空数组
                if (length < 0) return new float[0][];

                float[][] silence = new float[channelCount][];
                for (int i = 0; i < channelCount; i++)
                {
                    silence[i] = new float[length];
                }
                return silence;
            }

            var format = _waveIn.WaveFormat;
            int bytesPerSample = format.BitsPerSample / 8;

            // 获取原始数据
            byte[] rawData = _memoryStream.ToArray();
            int totalSamples = rawData.Length / (bytesPerSample * channelCount);

            // -1 时取所有数据
            if (length < 0) length = totalSamples;
            if (length <= 0) return new float[0][];

            // 计算实际读取的样本数
            int samplesToRead = Math.Min(length, totalSamples);

            // 为每个声道创建缓冲区
            float[][] channelBuffers = new float[channelCount][];
            for (int i = 0; i < channelCount; i++)
            {
                channelBuffers[i] = new float[length];
            }

            // 解交错音频数据并转换为float
            for (int i = 0; i < samplesToRead; i++)
            {
                for (int ch = 0; ch < channelCount; ch++)
                {
                    int byteIndex = i * bytesPerSample * channelCount + ch * bytesPerSample;
                    if (format.Encoding == WaveFormatEncoding.IeeeFloat)
                    {
                        channelBuffers[ch][i] = BitConverter.ToSingle(rawData, byteIndex);
                    }
                    else if (format.BitsPerSample == 16)
                    {
                        channelBuffers[ch][i] = BitConverter.ToInt16(rawData, byteIndex) / 32768f;
                    }
                    else if (format.BitsPerSample == 32)
                    {
                        channelBuffers[ch][i] = BitConverter.ToInt32(rawData, byteIndex) / (float)int.MaxValue;
                    }
                }
            }

            // 长度不足时，后面已经是静音（默认为0），无需额外处理
            return channelBuffers;
        }

        /// <summary>获取数据</summary>
        /// <remarks>长度不足返回静音内容，在前面填充静音</remarks>
        /// <param name="length">数据个数，每声道都有。-1表示取所有数据</param>
        /// <returns>按照声道获取的音频数据，格式为 AudioFileReader 按照声道整理</returns>
        public float[][] ReadAudioLastData(int length = -1)
        {
            int channelCount = _waveIn?.WaveFormat?.Channels ?? 1;

            // 无数据时返回静音
            if (_memoryStream == null || _memoryStream.Length == 0)
            {
                // -1 时返回空数组
                if (length < 0) return new float[0][];

                float[][] silence = new float[channelCount][];
                for (int i = 0; i < channelCount; i++)
                {
                    silence[i] = new float[length];
                }
                return silence;
            }

            var format = _waveIn.WaveFormat;
            int bytesPerSample = format.BitsPerSample / 8;

            // 获取原始数据
            byte[] rawData = _memoryStream.ToArray();
            int totalSamples = rawData.Length / (bytesPerSample * channelCount);

            // -1 时取所有数据
            if (length < 0) length = totalSamples;
            if (length <= 0) return new float[0][];

            // 为每个声道创建缓冲区
            float[][] channelBuffers = new float[channelCount][];
            for (int i = 0; i < channelCount; i++)
            {
                channelBuffers[i] = new float[length];
            }

            // 计算需要跳过的样本数（从末尾取 length 个，不足则前面填充静音）
            int skipSamples = Math.Max(0, totalSamples - length);
            int samplesToRead = Math.Min(length, totalSamples);
            int destOffset = length - samplesToRead; // 目标偏移，用于前面填充静音

            // 解交错音频数据并转换为float
            for (int i = 0; i < samplesToRead; i++)
            {
                for (int ch = 0; ch < channelCount; ch++)
                {
                    int srcByteIndex = (skipSamples + i) * bytesPerSample * channelCount + ch * bytesPerSample;
                    if (format.Encoding == WaveFormatEncoding.IeeeFloat)
                    {
                        channelBuffers[ch][destOffset + i] = BitConverter.ToSingle(rawData, srcByteIndex);
                    }
                    else if (format.BitsPerSample == 16)
                    {
                        channelBuffers[ch][destOffset + i] = BitConverter.ToInt16(rawData, srcByteIndex) / 32768f;
                    }
                    else if (format.BitsPerSample == 32)
                    {
                        channelBuffers[ch][destOffset + i] = BitConverter.ToInt32(rawData, srcByteIndex) / (float)int.MaxValue;
                    }
                }
            }

            // 前面已经是静音（默认为0），无需额外处理
            return channelBuffers;
        }

        /// <summary>获取最后录音的平均音量</summary>
        /// <returns>各声道音量数组(0~100)，长度等于声道数</returns>
        public byte[] ReadAudioLastDataVolumes()
        {
            var datas = ReadAudioLastData();
            var vols = new byte[datas.Length];
            for (int i = 0; i < datas.Length; i++)
            {
                var sum = 0f;
                for (int j = 0; j < datas[i].Length; j++)
                {
                    sum += Math.Abs(datas[i][j]);
                }
                vols[i] = (byte)(sum / datas[i].Length * 100);
            }

            return vols;
        }

        /// <summary>给定时间算帧数</summary>
        /// <param name="ms">毫秒</param>
        /// <returns>音频帧数</returns>
        public int FramsFormTime(int ms) { return (int)(ms * _waveIn.WaveFormat.SampleRate / 1000); }

        #endregion

        #region 音量

        /// <summary>音量变化回调，参数为各声道音量字节数组 (0~100)，数组长度等于声道数</summary>
        // public delegate void VolumeUpdate(byte[] vols);

        /// <summary>音量变化事件</summary>
        public event EventHandler<byte[]> VolumeUpdated;

        /// <summary>内存满事件</summary>
        public event EventHandler MemoryLimitReached;

        /// <summary>是否因内存满而停止</summary>
        public bool StoppedByMemoryLimit => _stoppedByMemoryLimit;

        #endregion

        #region 音频数据

        /// <summary>音频数据回调，参数为各声道音频数据 float[][]，第一维为声道索引</summary>
        // public delegate void AudioDataReceived(float[][] audioData);

        /// <summary>音频数据接收事件，实时返回按声道组织的音频数据。可以用于AudioFeatureExtractor->MelStreamImage生成实时mel图</summary>
        public event EventHandler<float[][]> DataUpdated;

        #endregion

        #region 内部事件处理

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            if (e.BytesRecorded > 0)
            {
                if (_IgnoreRemain > 0)
                {
                    var blen = e.BytesRecorded;
                    var format = RecordFormat;
                    var fmlen = format.Channels;
                    if (format.Encoding == WaveFormatEncoding.IeeeFloat) { fmlen *= 4; }
                    else if (format.BitsPerSample == 16) { fmlen *= 2; }
                    else if (format.BitsPerSample == 32) { fmlen *= 2; }
                    // 本次帧个数
                    var fmsize = blen / fmlen;
                    // 忽略掉的，可以多忽略一些，音频，无所谓。
                    _IgnoreRemain -= fmsize;
                    return;
                }

                // 检查内存限制
                if (_memoryStream.Length + e.BytesRecorded > MaxMemorySize)
                {
                    // 标记为内存满停止
                    _stoppedByMemoryLimit = true;

                    // 触发内存满事件
                    MemoryLimitReached?.Invoke(this, EventArgs.Empty);

                    // 自动停止录音
                    Stop();
                    return;
                }

                // 写入内存
                _memoryStream?.Write(e.Buffer, 0, e.BytesRecorded);

                // 计算并报告音量
                if (VolumeUpdated != null)
                {
                    var vols = CalculateVolume(e.Buffer, e.BytesRecorded, _waveIn.WaveFormat);
                    VolumeUpdated?.Invoke(this, vols);
                }

                // 触发音频数据事件
                if (DataUpdated != null)
                {
                    var audioData = ConvertToAudioData(e.Buffer, e.BytesRecorded, _waveIn.WaveFormat);
                    DataUpdated?.Invoke(this, audioData);
                }
            }
        }

        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            IsRecording = false;

            // 触发完成事件
            Stoped?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 将原始音频缓冲区转换为按声道组织的 float[][] 格式
        /// </summary>
        /// <param name="buffer">原始音频数据</param>
        /// <param name="bytesRecorded">有效数据长度</param>
        /// <param name="format">音频格式</param>
        /// <returns>按声道组织的音频数据，第一维为声道索引</returns>
        private static float[][] ConvertToAudioData(byte[] buffer, int bytesRecorded, WaveFormat format)
        {
            if (format.Channels < 1 || buffer == null || bytesRecorded <= 0)
            {
                return new float[0][];
            }

            int channels = format.Channels;
            int bytesPerSample = format.BitsPerSample / 8;
            int frameSize = bytesPerSample * channels;
            int sampleCount = bytesRecorded / frameSize;

            // 为每个声道创建缓冲区
            float[][] channelBuffers = new float[channels][];
            for (int i = 0; i < channels; i++)
            {
                channelBuffers[i] = new float[sampleCount];
            }

            // 解交错音频数据并转换为float
            for (int i = 0; i < sampleCount; i++)
            {
                for (int ch = 0; ch < channels; ch++)
                {
                    int byteIndex = i * frameSize + ch * bytesPerSample;
                    if (format.Encoding == WaveFormatEncoding.IeeeFloat)
                    {
                        channelBuffers[ch][i] = BitConverter.ToSingle(buffer, byteIndex);
                    }
                    else if (format.BitsPerSample == 16)
                    {
                        channelBuffers[ch][i] = BitConverter.ToInt16(buffer, byteIndex) / 32768f;
                    }
                    else if (format.BitsPerSample == 32)
                    {
                        channelBuffers[ch][i] = BitConverter.ToInt32(buffer, byteIndex) / (float)int.MaxValue;
                    }
                }
            }

            return channelBuffers;
        }

        /// <summary>
        /// 计算各声道音量
        /// </summary>
        /// <returns>各声道音量数组(0~100)，长度等于声道数</returns>
        private static byte[] CalculateVolume(byte[] buffer, int bytesRecorded, WaveFormat format)
        {
            if (format.Channels < 1 || buffer == null || bytesRecorded <= 0)
            {
                return format.Channels > 0 ? new byte[format.Channels] : new byte[0];
            }

            int channels = format.Channels;
            var bytesPerSample = format.BitsPerSample / 8;
            int frameSize = bytesPerSample * channels;

            // 各声道累加和
            float[] sums = new float[channels];
            int count = 0;

            for (int i = 0; i + frameSize <= bytesRecorded; i += frameSize)
            {
                for (int ch = 0; ch < channels; ch++)
                {
                    int byteIndex = i + ch * bytesPerSample;
                    float sampleValue = 0;

                    if (format.Encoding == WaveFormatEncoding.IeeeFloat)
                    {
                        sampleValue = Math.Abs(BitConverter.ToSingle(buffer, byteIndex));
                    }
                    else if (format.BitsPerSample == 16)
                    {
                        short sample = BitConverter.ToInt16(buffer, byteIndex);
                        sampleValue = sample == short.MinValue ? short.MaxValue : Math.Abs(sample);
                        sampleValue /= short.MaxValue;
                    }
                    else if (format.BitsPerSample == 32)
                    {
                        int sample = BitConverter.ToInt32(buffer, byteIndex);
                        sampleValue = Math.Abs((float)sample / int.MaxValue);
                    }

                    sums[ch] += sampleValue;
                }

                count++;
            }

            if (count == 0) return new byte[channels];

            byte[] volumes = new byte[channels];
            for (int ch = 0; ch < channels; ch++)
            {
                // 对于 float 格式，sums[ch] 已经是 0~1 范围
                // 对于 16/32 位整型，已在上面归一化到 0~1 范围
                float vol = (sums[ch] / count) * 100;
                volumes[ch] = (byte)Math.Min((int)vol, 100);
            }

            return volumes;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Stop();
            _waveIn.DataAvailable -= OnDataAvailable;
            _waveIn.RecordingStopped -= OnRecordingStopped;
            _waveIn?.Dispose();
            _waveIn = null;
            _memoryStream?.Dispose();
            _memoryStream = null;
            _cts?.Dispose();
            _cts = null;
        }

        #endregion
    }

    #region 辅助类 - RawWaveStream

    /// <summary>
    /// 将原始 PCM 数据包装为 IWaveProvider，用于格式转换时的输入源
    /// </summary>
    internal class RawWaveStream : Stream, IWaveProvider
    {
        private readonly Stream _source;
        private readonly WaveFormat _waveFormat;

        public RawWaveStream(Stream source, WaveFormat format)
        {
            _source = source;
            _waveFormat = format;
        }

        public WaveFormat WaveFormat => _waveFormat;

        public override bool CanRead => true;
        public override bool CanSeek => _source.CanSeek;
        public override bool CanWrite => false;
        public override long Length => _source.Length;
        public override long Position
        {
            get => _source.Position;
            set => _source.Position = value;
        }

        public override void Flush() => _source.Flush();
        public override long Seek(long offset, SeekOrigin origin) => _source.Seek(offset, origin);
        public override void SetLength(long value) => _source.SetLength(value);

        public override int Read(byte[] buffer, int offset, int count) => _source.Read(buffer, offset, count);
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing) _source?.Dispose();
            base.Dispose(disposing);
        }
    }

    #endregion
}
