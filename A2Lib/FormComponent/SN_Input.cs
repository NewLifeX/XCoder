using NewLife.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ALib.Forms
{
    [DefaultEvent("Key_Click")]
    public partial class SN_Input : UserControl
    {
        public SN_Input()
        {
            InitializeComponent();
        }

        #region 属性

        /// <summary>名称</summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get { return tbSN.Text; }
            set
            {
                var str = value ?? "";
                tbSN.Text = str;
            }
        }

        public event KeyPressEventHandler SN_KeyPress;

        public event KeyEventHandler SN_KeyDown;

        public event KeyEventHandler SN_KeyUp;

        public String KeyName
        {
            get { return btKey.Text; }
            set
            {
                var str = value ?? "";
                btKey.Text = str;
            }
        }

        public event EventHandler Key_Click;

        public void Clear() => tbSN.Clear();

        #endregion

        private void tbSN_KeyDown(object sender, KeyEventArgs e)
        {
            if (SN_KeyDown != null)
            {
                SN_KeyDown.Invoke(this, e);
                return;
            }
        }

        private void tbSN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (SN_KeyPress != null)
            {
                SN_KeyPress.Invoke(this, e);
                return;
            }
        }

        private void tbSN_KeyUp(object sender, KeyEventArgs e)
        {
            if (SN_KeyUp != null)
            {
                SN_KeyUp.Invoke(this, e);
                return;
            }

            // 外部没有注册，使用默认逻辑
            var key = e.KeyCode;
            if (key == Keys.Enter)
            {
                Key_Click?.Invoke(this, e);
                e.Handled = true;
            }
            else if (key == Keys.Escape)
            {
                tbSN.Clear();
                e.Handled = true;
            }
        }

        private void btKey_Click(object sender, EventArgs e)
        {
            if (Key_Click != null)
            {
                Key_Click.Invoke(this, e);
                return;
            }
        }
    }
}
