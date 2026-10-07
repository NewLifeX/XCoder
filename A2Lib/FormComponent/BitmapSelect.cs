using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALib.FormComponent
{
    /// <summary>图片选择辅助</summary>
    public class BitmapSelect
    {
        public BitmapSelect(PictureBox pb)
        {
            picBox = pb;
            picBoxSize = new Size(pb.Size.Width, pb.Size.Height);

            pb.MouseDown += pb_MouseDown;
            pb.MouseMove += pb_MouseMove;
            pb.MouseUp += pb_MouseUp;

            var img = new Bitmap(pb.Width, pb.Height);
            using (Graphics g = Graphics.FromImage(img)) { g.Clear(Color.White); }
            pb.Image = img;

            Src = img;
            ShowBm = (bm) => pb.Image = bm;
        }

        /// <summary>显示窗</summary>
        private PictureBox picBox;
        /// <summary>大小</summary>
        private Size picBoxSize;
        /// <summary>原图</summary>
        private Bitmap Src;

        /// <summary>框选开始位置</summary>
        public Point pt_Start { get; private set; } = new Point();
        /// <summary>框选结束位置</summary>
        public Point pt_End { get; private set; } = new Point();
        /// <summary>鼠标当前位置</summary>
        private Point pt_Now = new Point();

        /// <summary>如果图片大于PictureBox是否支持剪裁</summary>
        public bool SupportCutting = false;
        /// <summary>如果在非UI线程执行，请写此函数</summary>
        public Action<Bitmap> ShowBm;

        /// <summary>给新图</summary>
        /// <param name="src"></param>
        public void SetImage(Bitmap src)
        {
            if (src == null) return;

            // 大小有变化
            if (src.Width != picBox.Width || src.Height != picBox.Height)
            {
                // 需要缩处理
                if ((src.Width > picBoxSize.Width) || (src.Height > picBoxSize.Height))
                {
                    if (!SupportCutting)
                    {
                        var str = $"图片过大无法处理 {src.Width},{src.Height}(src) > {picBoxSize.Width},{picBoxSize.Height}(picBox)";
                        throw new Exception(str);
                    }
                    else
                    {
                        // 最终大小
                        var w = src.Width;
                        if (w > picBoxSize.Width) w = picBoxSize.Width;
                        var h = src.Height;
                        if (h > picBoxSize.Height) h = picBoxSize.Height;

                        // 剪裁
                        var point = new System.Drawing.Point((src.Width - w) / 2, (src.Height - h) / 2);
                        var croRect = new Rectangle(point.X, point.Y, w, h);
                        src = src.Clone(croRect, src.PixelFormat);

                        // 剪裁后修改显示框大小
                        picBox.Width = src.Width;
                        picBox.Height = src.Height;
                    }
                }
                else
                {
                    // 直接设置
                    picBox.Width = src.Width;
                    picBox.Height = src.Height;
                }
            }

            // 可能剪裁过
            Src = src;

            // 先刷新一个
            var bm = Src.Clone() as Bitmap;
            pic_Draw(bm);
            ShowBm?.Invoke(bm);
        }

        /// <summary>拿到选取区域</summary>
        /// <returns></returns>
        public Bitmap GetSelect()
        {
            Point pt = pt_Start;
            Size size = new Size(pt_End.X - pt_Start.X, pt_End.Y - pt_Start.Y);
            if (size.Width < 0) return null;
            if (size.Height < 0) return null;

            var croRect = new Rectangle(pt_Start, size);
            return Src.Clone(croRect, Src.PixelFormat);
        }

        #region 干活的

        private void pb_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pt_Start = e.Location;
                pt_End = e.Location;
                pt_Now = e.Location;

                var bm = Src.Clone() as Bitmap;
                pic_Draw(bm);
                ShowBm?.Invoke(bm);
            }
        }

        private void pb_MouseMove(object sender, MouseEventArgs e)
        {
            pt_Now = e.Location;

            if (e.Button == MouseButtons.Left)
            {
                pt_End = e.Location;
            }

            var bm = Src.Clone() as Bitmap;
            pic_Draw(bm);
            ShowBm?.Invoke(bm);
        }

        private void pb_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pt_End = e.Location;
                pt_Now = e.Location;

                var bm = Src.Clone() as Bitmap;
                pic_Draw(bm);
                ShowBm?.Invoke(bm);
            }
        }

        private void pic_Draw(Bitmap bm)
        {
            if (bm == null) return;

            using (var g = Graphics.FromImage(bm))
            {
                Point pt = pt_Start;
                Size size = new Size(pt_End.X - pt_Start.X, pt_End.Y - pt_Start.Y);
                if ((size.Width > 0) && (size.Height > 0))
                {
                    var pen = new Pen(Color.Red, 2);
                    var rac = new Rectangle(pt, size);
                    g.DrawRectangle(pen, rac);
                }

                var pen2 = new Pen(Color.Black, 1);
                g.DrawLine(pen2, 0, pt_Now.Y, bm.Width - 1, pt_Now.Y);
                g.DrawLine(pen2, pt_Now.X, 0, pt_Now.X, bm.Height - 1);
            }
        }

        #endregion
    }
}
