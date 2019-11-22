using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Gtk;

namespace XCoder.Tools
{
    [DisplayName("语音助手")]
    public partial class FrmSpeak : Box, IXForm
    {
        public FrmSpeak(Orientation orientation = Orientation.Horizontal, Int32 spacing = 2) : base(orientation, spacing)
        {
            InitializeComponent();
        }

        private void btnSpeak_Click(Object sender, EventArgs e)
        {
            var txt = richTextBox1.Buffer.Text;
            if (txt.IsNullOrEmpty()) return;

            txt.SpeakAsync();


        }
    }
}
