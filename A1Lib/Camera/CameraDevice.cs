using ALib.UsbHub;
using AForge.Video;
using AForge.Video.DirectShow;
using NewLife;
using NewLife.Log;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace ALib.Camera
{
    /// <summary>AForge 相机</summary>
    public class CameraDevice : DisposeBase
    {
        /// <summary>摄像头编号（与 Windows 视频设备枚举序一致，仅作兜底/日志用）</summary>
        // public int CameraIndex = 0;

        /// <summary>目标相机的 PNPDeviceID，用于与 DirectShow 设备做精确匹配，避免索引顺序不一致导致选错相机</summary>
        public string TargetPnp { get; set; }

        /// <summary>像素尺寸（Bitmap 像素尺寸）</summary>
        public Size Size { get; set; } = new Size(640, 480);

        // public CameraDevice(int idx = 0) { CameraIndex = idx; }
        public CameraDevice() { }

        // ---- AForge(DirectShow) 采集相关 ----
        private FilterInfoCollection _Devices;
        private VideoCaptureDevice _Source;
        private readonly object _lockBmp = new object();
        private System.Drawing.Bitmap _Latest;

        public bool IsOpened() => _Source != null && _Source.IsRunning;

        private bool _Error = false;
        public bool Error() => _Error;


        public void Open(bool setPix = false, bool tryMax = true)
        {
            _Error = false;
            if (_Source != null && _Source.IsRunning) return;

            if (_Devices == null)
                _Devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (_Devices.Count == 0)
                throw new Exception("未找到任何摄像头设备");

            // 优先按 PnPId 精确匹配 DirectShow 设备，避免 PnP 枚举序与 DSHOW 枚举序不一致选错相机
            _Source = null;
            if (!string.IsNullOrEmpty(TargetPnp))
            {
                string norm = NormalizePnpId(TargetPnp);
                foreach (FilterInfo fi in _Devices)
                {
                    if (fi.MonikerString.IndexOf(norm, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        XTrace.WriteLine($"按 PnPId 匹配到相机: {fi.Name} (Moniker={fi.MonikerString})");
                        _Source = new VideoCaptureDevice(fi.MonikerString);
                        break;
                    }
                }
            }

            _Source.NewFrame += OnNewFrame;
            _Source.VideoSourceError += _Source_VideoSourceError; ;

            // 选择分辨率（必须在 Start 之前设置）
            if (_Source.VideoCapabilities != null && _Source.VideoCapabilities.Length > 0)
            {
                VideoCapabilities chosen = null;
                if (tryMax)
                    chosen = _Source.VideoCapabilities
                        .OrderByDescending(c => (long)c.FrameSize.Width * c.FrameSize.Height).FirstOrDefault();
                else if (setPix)
                    chosen = _Source.VideoCapabilities.OrderBy(
                        c => Math.Abs(c.FrameSize.Width - Size.Width) + Math.Abs(c.FrameSize.Height - Size.Height)
                        ).FirstOrDefault();

                if (chosen != null)
                {
                    _Source.VideoResolution = chosen;
                    Size = new Size(chosen.FrameSize.Width, chosen.FrameSize.Height);
                }
            }

            _Source.Start();
        }

        private void _Source_VideoSourceError(object sender, VideoSourceErrorEventArgs eventArgs) { _Error = true; }

        /// <summary>把任意 Bitmap 深层拷贝成一张完全独立的新位图（不共享底层像素）</summary>
        private static System.Drawing.Bitmap DeepCloneBitmap(System.Drawing.Bitmap src)
        {
            if (src == null) return null;
            try
            {
                var dst = new System.Drawing.Bitmap(src.Width, src.Height, src.PixelFormat);
                dst.SetResolution(src.HorizontalResolution, src.VerticalResolution);
                using (var g = System.Drawing.Graphics.FromImage(dst)) { g.DrawImageUnscaled(src, 0, 0); }
                return dst;
            }
            catch (Exception ex)
            {
                NewLife.Log.XTrace.WriteException(ex);
                return null;
            }
        }

        /// <summary>新帧事件</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNewFrame(object sender, NewFrameEventArgs e)
        {
            // AForge 的 e.Frame 是它拥有的共享帧。
            // 这里必须做"真正深层"拷贝（复制像素），不能只做共享底层内存的 Clone()，
            // 否则异步保存/识别时可能拿到被 AForge 复用的半写帧数据。
            var clone = DeepCloneBitmap(e.Frame);
            lock (_lockBmp)
            {
                var old = _Latest;
                _Latest = clone;
                old?.Dispose();
            }
        }

        /// <summary>拍一张照片，返回 Bitmap</summary>
        public System.Drawing.Bitmap TakePhoto_AsBitmap()
        {
            if (!IsOpened()) return null;

            lock (_lockBmp)
            {
                return DeepCloneBitmap(_Latest);
            }
        }

        #region 十字准星 照片

        /// <summary>获取一张照片，画上十字准星</summary>
        /// <returns></returns>
        public async Task<Bitmap> TakeCrosshairImage(int w = 0, int h = 0)
        {
            var img = TakePhoto_AsBitmap();
            if (img == null) return null;

            if (w > 0 && h > 0)
            {
                // 剪裁一下，太大没有用
                if ((img.Width > w) || (img.Height > h))
                {
                    var size = new System.Drawing.Size(w, h);
                    var point = new System.Drawing.Point((img.Width - w) / 2, (img.Height - h) / 2);
                    var croRect = new Rectangle(point, size);
                    var cropped = img.Clone(croRect, img.PixelFormat);
                    img.Dispose(); // 剪裁后释放原整图
                    img = cropped;
                }
            }

            // 中心不划线长度
            int crossSize = 5;
            // 增加显示效果并显示
            using (var g = Graphics.FromImage(img))
            {
                // 红色十字，线宽2
                Pen pen = new Pen(Color.Red, 1);
                // 绘制十字准星（不画中心点圆点）
                int centerX = img.Width / 2;
                int centerY = img.Height / 2;
                // 水平线（不画中心点）
                g.DrawLine(pen, 0, centerY, centerX - crossSize, centerY);
                g.DrawLine(pen, centerX + crossSize, centerY, img.Width - 1, centerY);
                // 垂直线（不画中心点）
                g.DrawLine(pen, centerX, 0, centerX, centerY - crossSize);
                g.DrawLine(pen, centerX, centerY + crossSize, centerX, img.Height - 1);
                // // 图片上写RGB值
                // var Font = new Font("Arial", centerY / 8, FontStyle.Bold);
                // var brush = new SolidBrush(Color.Black);
                // g.DrawString($"A{p.A} R{p.R} G{p.G} B{p.B}", Font, brush, new Point(0, 0));
                // g.DrawString($"Bri p {pbnss:F2} agv {pAvgbnss:F2}", Font, brush,
                //    new Point(0, img.Height - (centerY / 8) - 5));
                pen.Dispose();
            }

            return img;
        }

        /// <summary>获取一张照片，画上十字准星</summary>
        /// <param name="size">照片大小</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<Bitmap> TakeCrosshairImage(System.Drawing.Size size) => await TakeCrosshairImage(size.Width, size.Height);

        #endregion

        /// <summary>获取当前摄像头参数值（仅在摄像头打开状态下有效）</summary>
        public Dictionary<string, double> GetCameraParameters()
        {
            if (!IsOpened()) return null;

            var parameters = new Dictionary<string, double>();
            try
            {
                parameters["FrameWidth"] = Size.Width;
                parameters["FrameHeight"] = Size.Height;
            }
            catch (Exception ex)
            {
                XTrace.WriteLine($"读取摄像头参数时出错: {ex.Message}");
            }
            return parameters;
        }

        public void Close()
        {
            if (_Source != null)
            {
                try { _Source.SignalToStop(); } catch { }
                try { _Source.WaitForStop(); } catch { }
                _Source.NewFrame -= OnNewFrame;
                _Source.VideoSourceError -= _Source_VideoSourceError;
                _Source = null;
            }

            lock (_lockBmp)
            {
                _Latest?.Dispose();
                _Latest = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Close();
        }

        public static CameraDevice Create(UsbDevice dev)
        {
            var pnp = dev.GetCameraPnp();
            if(pnp.IsNullOrEmpty()) return null;

            var cam = new CameraDevice();
            cam.TargetPnp = pnp;

            return cam;
        }

        /// <summary>创建相机</summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static CameraDevice Create(UsbHubPort port)
        {
            if (!port.Exists()) return null;
            var pnp = port.GetUsbDevice().GetCameraPnp();
            if (pnp.IsNullOrEmpty()) return null;

            var cam = new CameraDevice();
            cam.TargetPnp = pnp;

            return cam;
        }

        /// <summary>将 PNPDeviceID (如 USB\VID_046D&PID_08C9\...) 规范化为可与 DirectShow MonikerString 比较的形式</summary>
        private static string NormalizePnpId(string pnpId)
        {
            // MonikerString 形如 @device:pnp:\\?\usb#vid_046d&pid_08c9&mi_00#... ，其中路径片段与 PNPDeviceID 一致（\ 替换为 #）
            return pnpId.Replace('\\', '#').ToLowerInvariant();
        }
    }
}
