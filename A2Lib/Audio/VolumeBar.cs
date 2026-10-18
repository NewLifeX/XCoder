using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ALib.Audio
{
    /// <summary>
    /// 纵向音量条控件，用于显示音量级别
    /// </summary>
    public class VolumeBar : ProgressBar
    {
        private Color _volumeColor = Color.LimeGreen;
        private Color _volumeColorHigh = Color.Yellow;
        private Color _volumeColorMax = Color.Red;
        private int _highThreshold = 70;
        private int _maxThreshold = 90;

        public VolumeBar()
        {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// 音量颜色（低音量）
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("低音量时的颜色")]
        public Color VolumeColor
        {
            get { return _volumeColor; }
            set { _volumeColor = value; this.Invalidate(); }
        }

        /// <summary>
        /// 高音量颜色
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("高音量时的颜色")]
        public Color VolumeColorHigh
        {
            get { return _volumeColorHigh; }
            set { _volumeColorHigh = value; this.Invalidate(); }
        }

        /// <summary>
        /// 最大音量颜色
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("最大音量时的颜色")]
        public Color VolumeColorMax
        {
            get { return _volumeColorMax; }
            set { _volumeColorMax = value; this.Invalidate(); }
        }

        /// <summary>
        /// 高音量阈值（百分比）
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("高音量阈值百分比")]
        public int HighThreshold
        {
            get { return _highThreshold; }
            set { _highThreshold = value; this.Invalidate(); }
        }

        /// <summary>
        /// 最大音量阈值（百分比）
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("最大音量阈值百分比")]
        public int MaxThreshold
        {
            get { return _maxThreshold; }
            set { _maxThreshold = value; this.Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = this.ClientRectangle;

            // 绘制背景
            using (SolidBrush bgBrush = new SolidBrush(this.BackColor))
            {
                g.FillRectangle(bgBrush, rect);
            }

            // 绘制边框
            ControlPaint.DrawBorder(g, rect, Color.Gray, ButtonBorderStyle.Solid);

            // 计算进度值百分比
            double percent = (double)this.Value / this.Maximum;

            // 计算进度条高度（从底部向上填充）
            int progressHeight = (int)((rect.Height - 2) * percent);
            int progressY = rect.Bottom - progressHeight - 1;

            if (progressHeight > 0)
            {
                // 根据音量级别选择颜色
                Color fillColor = GetVolumeColor(this.Value);

                // 绘制进度条（纵向，从底部向上）
                Rectangle progressRect = new Rectangle(rect.X + 1, progressY, rect.Width - 2, progressHeight);

                // 使用渐变填充
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    progressRect,
                    ControlPaint.Light(fillColor),
                    fillColor,
                    LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(brush, progressRect);
                }
            }

            // 绘制分段线（可选，增加视觉效果）
            DrawScaleLines(g, rect);
        }

        /// <summary>
        /// 根据音量值获取对应颜色
        /// </summary>
        private Color GetVolumeColor(int value)
        {
            int percent = value * 100 / this.Maximum;

            if (percent >= _maxThreshold)
                return _volumeColorMax;
            else if (percent >= _highThreshold)
                return _volumeColorHigh;
            else
                return _volumeColor;
        }

        /// <summary>
        /// 绘制刻度线
        /// </summary>
        private void DrawScaleLines(Graphics g, Rectangle rect)
        {
            int segments = 10;
            int segmentHeight = (rect.Height - 2) / segments;

            using (Pen pen = new Pen(Color.FromArgb(60, Color.Gray), 1))
            {
                for (int i = 1; i < segments; i++)
                {
                    int y = rect.Bottom - 1 - (segmentHeight * i);
                    g.DrawLine(pen, rect.X + 1, y, rect.Right - 1, y);
                }
            }
        }

        /// <summary>
        /// 设置音量值（0-100）
        /// </summary>
        public void SetVolume(int volume)
        {
            if (volume < 0) volume = 0;
            if (volume > 100) volume = 100;

            if (this.InvokeRequired)
                this.Invoke(() => this.Value = volume);
            else
                this.Value = volume;
        }

        private float _minDb = -60f;
        private float _maxDb = 0f;

        /// <summary>
        /// 最小dB值（默认-60dB）
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("最小dB值，对应音量0%")]
        public float MinDb
        {
            get { return _minDb; }
            set { _minDb = value; }
        }

        /// <summary>
        /// 最大dB值（默认0dB）
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("最大dB值，对应音量100%")]
        public float MaxDb
        {
            get { return _maxDb; }
            set { _maxDb = value; }
        }

        /// <summary>
        /// 设置音量值（dB）
        /// </summary>
        /// <param name="db">dB值，范围由MinDb和MaxDb决定</param>
        /// <param name="logarithmic">是否使用对数映射（更符合人耳感知），默认true</param>
        public void SetVolumeDb(float db, bool logarithmic = true)
        {
            int volume;

            if (logarithmic)
            {
                // 对数映射：linear = 10^(dB/20)
                // 将dB转换为线性值后映射到百分比
                if (db <= _minDb)
                {
                    volume = 0;
                }
                else if (db >= _maxDb)
                {
                    volume = 100;
                }
                else
                {
                    // 计算相对于最大值的线性比例
                    float linear = (float)Math.Pow(10, db / 20);
                    float maxLinear = (float)Math.Pow(10, _maxDb / 20);
                    float minLinear = (float)Math.Pow(10, _minDb / 20);
                    
                    // 映射到0-100
                    volume = (int)((linear - minLinear) / (maxLinear - minLinear) * 100);
                    if (volume < 0) volume = 0;
                    if (volume > 100) volume = 100;
                }
            }
            else
            {
                // 线性映射
                if (db <= _minDb)
                {
                    volume = 0;
                }
                else if (db >= _maxDb)
                {
                    volume = 100;
                }
                else
                {
                    volume = (int)((db - _minDb) / (_maxDb - _minDb) * 100);
                }
            }

            SetVolume(volume);
        }
    }
}
