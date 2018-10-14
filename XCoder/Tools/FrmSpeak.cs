using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XCoder.Tools
{
    [DisplayName("语音助手")]
    public partial class FrmSpeak : Form, IXForm
    {
        public FrmSpeak()
        {
            InitializeComponent();
        }

        private void btnSpeak_Click(Object sender, EventArgs e)
        {
            var txt = richTextBox1.Text;
            if (txt.IsNullOrEmpty()) return;

            txt.SpeakAsync();
        }
    }
}
