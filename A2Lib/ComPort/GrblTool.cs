using ALib.WinForm;
using NewLife.Log;
using NewLife.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ALib.ComPort
{
    public partial class GrblTool : Form
    {
        public GrblTool()
        {
            InitializeComponent();
        }

        public double X, Y, Z;

        public Grbl_Com Grbl;

        /// <summary>初始化，从GetStatusReport获取坐标给 XYZ赋值</summary>
        private bool init = false;

        private TimerX _time;

        private int lockValue;

        private void GrblTool_Load(object sender, EventArgs e)
        {
            cbMoveValue.SelectedIndex = 0;

            foreach (Control item in this.Controls)
            {
                item.TabStop = false;
            }
            lbXYZ.TabIndex = 0;
            cbMoveValue.TabStop = true;
            this.ActiveControl = cbMoveValue;

            _time = new TimerX(ReadInfo, null, 100, 1000);
            this.FormClosing += GrblTool_FormClosing;
        }

        private void GrblTool_FormClosing(object sender, FormClosingEventArgs e)
        {
            _time.Dispose();
        }

        private async Task ReadInfo(Object obj)
        {
            if (Grbl == null) return;

            var _ = Grbl.GetStatusReport();
            var info = Grbl.CurrStatus;
            if (info != null)
            {
                var mco = info.MachinePosition;
                if (!init)
                {
                    X = mco[0];
                    Y = mco[1];
                    Z = mco[2];

                    init = true;
                }
                var mstr = $"MCO X{mco[0]:F4},Y{mco[1]:F4},Z{mco[2]:F4} | {info.MachineState}";

                this.UpdateUIControl(() =>
                {
                    lbCurrXYZ.Text = mstr;
                });
            }
        }

        private void GrblTool_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void XYZChange()
        {
            lbXYZ.Text = $"X{X:F4},Y{Y:F4},Z{Z:F4}";

            if (Grbl != null)
            {
                var g0 = $"G0 X{X:F4} Y{Y:F4} Z{Z:F4}";
                var _ = Grbl.SendGcodeLine(g0);
            }
        }

        private void GrblTool_KeyDown(object sender, KeyEventArgs e)
        {
            var move = Convert.ToDouble(cbMoveValue.Text);

            bool xyzChange = false;
            switch (e.KeyCode)
            {
                case Keys.Left: X -= move; xyzChange = true; break;
                case Keys.Right: X += move; xyzChange = true; break;
                case Keys.Up: Y += move; xyzChange = true; break;
                case Keys.Down: Y -= move; xyzChange = true; break;
                case Keys.PageUp: Z += move; xyzChange = true; break;
                case Keys.PageDown: Z -= move; xyzChange = true; break;

                case Keys.Add: if (cbMoveValue.SelectedIndex < cbMoveValue.Items.Count - 1) cbMoveValue.SelectedIndex++; break;
                case Keys.Subtract: if (cbMoveValue.SelectedIndex > 0) cbMoveValue.SelectedIndex--; break;

                case Keys.F1: cbMoveValue.SelectedIndex = 0; break;
                case Keys.F2: cbMoveValue.SelectedIndex = 1; break;
                case Keys.F3: cbMoveValue.SelectedIndex = 2; break;
                case Keys.F4: cbMoveValue.SelectedIndex = 3; break;

                case Keys.F9: btG92_Click(null, null); break;

                // H 键回零
                // case Keys.H: var _ = Grbl.GoHome(); break;
                case Keys.X: var _ = Grbl.UnLock(); break;
                // 清除所有未执行命令
                case Keys.Escape: Grbl.Reset(); break;

                default: break;
            }

            if (xyzChange)
            {
                XYZChange();
            }

            e.Handled = true;
            // e.SuppressKeyPress = true;
        }

        /// <summary>把焦点移回单步值下拉框，避免方向键被按钮的焦点导航吞掉</summary>
        private void FocusMoveValue()
        {
            this.ActiveControl = cbMoveValue;
        }

        private void btXInc_Click(object sender, EventArgs e)
        {
            FocusMoveValue();
            var move = Convert.ToDouble(cbMoveValue.Text);
            X += move;
            XYZChange();
        }
        private void btXDec_Click(object sender, EventArgs e)
        {
            FocusMoveValue();
            var move = Convert.ToDouble(cbMoveValue.Text);
            X -= move;
            XYZChange();
        }

        private void btYInc_Click(object sender, EventArgs e)
        {
            FocusMoveValue();
            var move = Convert.ToDouble(cbMoveValue.Text);
            Y += move;
            XYZChange();
        }

        private void btYDec_Click(object sender, EventArgs e)
        {
            FocusMoveValue();
            var move = Convert.ToDouble(cbMoveValue.Text);
            Y -= move;
            XYZChange();
        }

        private void btZInc_Click(object sender, EventArgs e)
        {
            FocusMoveValue();
            var move = Convert.ToDouble(cbMoveValue.Text);
            Z += move;
            XYZChange();
        }

        private void btZDec_Click(object sender, EventArgs e)
        {
            FocusMoveValue();
            var move = Convert.ToDouble(cbMoveValue.Text);
            Z -= move;
            XYZChange();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }

        private void btG92_Click(object sender, EventArgs e)
        {
            FocusMoveValue();
            var _ = Grbl.SendGcodeLine("G92 X0 Y0 Z0");
        }

        private void btUnlock_Click(object sender, EventArgs e)
        {
            FocusMoveValue();
            var _ = Grbl.UnLock();
        }

        #region 日志

        void WriteLog(String format, params Object[] args) => Grbl?.Log?.Write(LogLevel.Info, format, args);

        #endregion


    }
}
