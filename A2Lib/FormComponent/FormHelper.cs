using NewLife.Log;
using NewLife.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ALib.FormComponent
{
    public static class FormHelper
    {
        /// <summary>非UI线程/UI线程，执行UI动作</summary>
        /// <param name="fm"></param>
        /// <param name="action"></param>
        public static void UpdateUIControl(this Form fm, Action action)
        {
            if (fm.InvokeRequired)
            {
                try
                {
                    fm.Invoke(() =>
                    {
                        if (fm.IsDisposed || fm.Disposing || !fm.IsHandleCreated) return;
                        action();
                    });
                } catch { }
            }
            else
            {
                if (fm.IsDisposed || fm.Disposing || !fm.IsHandleCreated) return;
                action();
            }
        }

        /// <summary>给 PictureBox 设置图片，自动释放上一张（必须在 UI 线程调用；img 可为 null）</summary>
        /// <param name="pic"></param>
        /// <param name="img"></param>
        public static void SetImage(this PictureBox pic, System.Drawing.Image img)
        {
            if (pic == null) { img?.Dispose(); return; }

            var old = pic.Image;
            pic.Image = img;
            old?.Dispose();
        }

        /// <summary>开启控件双缓冲，减少频繁刷新时的闪烁</summary>
        /// <param name="ctrl"></param>
        public static void EnableDoubleBuffer(this Control ctrl)
        {
            if (ctrl == null) return;

            // DoubleBuffered 是 protected 属性，反射设置
            var pi = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi?.SetValue(ctrl, true, null);
        }

        /// <summary>非UI线程/UI线程，执行UI DialogShow函数</summary>
        /// <param name="fm">主UI</param>
        /// <param name="dialog">需要DialogShow的form</param>
        /// <returns></returns>
        public static DialogResult ShowModalDialogOnUIThread(this Form fm, Form dialog)
        {
            if (fm.InvokeRequired)
            {
                return fm.Invoke(() =>
                {
                    // 外部设置吧
                    // dialog.StartPosition = FormStartPosition.CenterParent;
                    return dialog.ShowDialog(fm);
                });
            }
            else
            {
                return dialog.ShowDialog(fm);
            }
        }

        /// <summary>绑定输出到 RichTextBox</summary>
        /// <param name="rtbLog"></param>
        public static void RootLog2Rtb<T>(this _StructuralLog<T> slog, RichTextBox rtbLog) where T : _StructuralLog<T>
        {
            rtbLog.Clear();

            var tcl = new TextControlLog() { Control = rtbLog, MaxLines = 1000 };
            slog.RootLog.Add(tcl);

            tcl.Write(LogLevel.Info, $"Name:{slog.Name} 日志");
        }
    }
}
