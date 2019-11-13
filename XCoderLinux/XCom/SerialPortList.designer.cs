using Gtk;

namespace NewLife.Windows
{
    partial class SerialPortList
    {
        #region 组件设计器生成的代码

        /// <summary> 
        /// </summary>
        private void InitializeComponent()
        {
            this.cbBaudrate = new ComboBox(new string[] { });
            this.label2 = new Label();
            this.cbName = new ComboBox(new string[] { });
            this.label3 = new Label();
            //this.contextMenuStrip1 = new ContextMenuStrip(this.components);
            //this.mi数据位 = new ToolStripMenuItem();
            //this.mi停止位 = new ToolStripMenuItem();
            //this.mi校验 = new ToolStripMenuItem();
            //this.mi高级 = new ToolStripMenuItem();
            //this.miDTR = new ToolStripMenuItem();
            //this.miRTS = new ToolStripMenuItem();
            //this.miBreak = new ToolStripMenuItem();
            //this.toolStripMenuItem1 = new ToolStripSeparator();
            //this.mi字符串编码 = new ToolStripMenuItem();
            //this.miHEX编码接收 = new ToolStripMenuItem();
            //this.miHex不换行 = new ToolStripMenuItem();
            //this.miHex自动换行 = new ToolStripMenuItem();
            //this.miHEX编码发送 = new ToolStripMenuItem();
            //this.toolStripMenuItem2 = new ToolStripSeparator();
            //this.contextMenuStrip1.SuspendLayout();
            // 
            // cbBaudrate
            // 
            this.cbBaudrate.Name = "cbBaudrate";
            // 
            // label2
            // 
            this.label2.Name = "label2";
            this.label2.Text = "波特率：";
            // 
            // cbName
            // 
            this.cbName.Name = "cbName";
            // 
            // label3
            // 
            this.label3.Name = "label3";
            this.label3.Text = "端口：";
            //// 
            //// contextMenuStrip1
            //// 
            ////this.contextMenuStrip1.Items.AddRange(new ToolStripItem[] {
            ////this.mi数据位,
            ////this.mi停止位,
            ////this.mi校验,
            ////this.mi高级,
            ////this.toolStripMenuItem1,
            ////this.mi字符串编码,
            ////this.miHEX编码接收,
            ////this.miHEX编码发送,
            ////this.toolStripMenuItem2});
            ////this.contextMenuStrip1.Name = "contextMenuStrip1";
            ////this.contextMenuStrip1.Size = new System.Drawing.Size(149, 170);
            //// 
            //// mi数据位
            //// 
            //this.mi数据位.Name = "mi数据位";
            //this.mi数据位.Size = new System.Drawing.Size(148, 22);
            //this.mi数据位.Text = "数据位";
            //// 
            //// mi停止位
            //// 
            //this.mi停止位.Name = "mi停止位";
            //this.mi停止位.Size = new System.Drawing.Size(148, 22);
            //this.mi停止位.Text = "停止位";
            //// 
            //// mi校验
            //// 
            //this.mi校验.Name = "mi校验";
            //this.mi校验.Size = new System.Drawing.Size(148, 22);
            //this.mi校验.Text = "校验";
            //// 
            //// mi高级
            //// 
            //this.mi高级.DropDownItems.AddRange(new ToolStripItem[] {
            //this.miDTR,
            //this.miRTS,
            //this.miBreak});
            //this.mi高级.Name = "mi高级";
            //this.mi高级.Size = new System.Drawing.Size(148, 22);
            //this.mi高级.Text = "高级";
            //// 
            //// miDTR
            //// 
            //this.miDTR.Name = "miDTR";
            //this.miDTR.Size = new System.Drawing.Size(110, 22);
            //this.miDTR.Text = "DTR";
            //this.miDTR.Click += new System.EventHandler(this.miDTR_Click);
            //// 
            //// miRTS
            //// 
            //this.miRTS.Name = "miRTS";
            //this.miRTS.Size = new System.Drawing.Size(110, 22);
            //this.miRTS.Text = "RTS";
            //this.miRTS.Click += new System.EventHandler(this.miDTR_Click);
            //// 
            //// miBreak
            //// 
            //this.miBreak.Name = "miBreak";
            //this.miBreak.Size = new System.Drawing.Size(110, 22);
            //this.miBreak.Text = "Break";
            //this.miBreak.Click += new System.EventHandler(this.miDTR_Click);
            //// 
            //// toolStripMenuItem1
            //// 
            //this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            //this.toolStripMenuItem1.Size = new System.Drawing.Size(145, 6);
            //// 
            //// mi字符串编码
            //// 
            //this.mi字符串编码.Name = "mi字符串编码";
            //this.mi字符串编码.Size = new System.Drawing.Size(148, 22);
            //this.mi字符串编码.Text = "字符串编码";
            //this.mi字符串编码.Click += new System.EventHandler(this.mi字符串编码_Click);
            //// 
            //// miHEX编码接收
            //// 
            //this.miHEX编码接收.DropDownItems.AddRange(new ToolStripItem[] {
            //this.miHex不换行,
            //this.miHex自动换行});
            //this.miHEX编码接收.Name = "miHEX编码接收";
            //this.miHEX编码接收.Size = new System.Drawing.Size(148, 22);
            //this.miHEX编码接收.Text = "HEX编码接收";
            //this.miHEX编码接收.Click += new System.EventHandler(this.mi字符串编码_Click);
            //// 
            //// miHex不换行
            //// 
            //this.miHex不换行.Name = "miHex不换行";
            //this.miHex不换行.Size = new System.Drawing.Size(124, 22);
            //this.miHex不换行.Tag = "false";
            //this.miHex不换行.Text = "不换行";
            //this.miHex不换行.Click += new System.EventHandler(this.miHex自动换行_Click);
            //// 
            //// miHex自动换行
            //// 
            //this.miHex自动换行.Name = "miHex自动换行";
            //this.miHex自动换行.Size = new System.Drawing.Size(124, 22);
            //this.miHex自动换行.Tag = "true";
            //this.miHex自动换行.Text = "自动换行";
            //this.miHex自动换行.Click += new System.EventHandler(this.miHex自动换行_Click);
            //// 
            //// miHEX编码发送
            //// 
            //this.miHEX编码发送.Name = "miHEX编码发送";
            //this.miHEX编码发送.Size = new System.Drawing.Size(148, 22);
            //this.miHEX编码发送.Text = "HEX编码发送";
            //this.miHEX编码发送.Click += new System.EventHandler(this.miHEX编码发送_Click);
            //// 
            //// toolStripMenuItem2
            //// 
            //this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            //this.toolStripMenuItem2.Size = new System.Drawing.Size(145, 6);
            // 
            // SerialPortList
            // 
            //this.ContextMenuStrip = this.contextMenuStrip1;
            this.PackStart(this.label3, false, false, 1);
            this.PackStart(this.cbName, false, false, 1);
            this.PackStart(this.label2, false, false, 1);
            this.PackStart(this.cbBaudrate, false, false, 1);
            this.Name = "SerialPortList";
            this.Shown += new System.EventHandler(this.SerialPortList_Load);
            //this.contextMenuStrip1.ResumeLayout(false);
            //this.ResumeLayout(false);
            //this.PerformLayout();
        }

        #endregion

        private ComboBox cbBaudrate;
        private Label label2;
        private ComboBox cbName;
        private Label label3;
        //private ContextMenuStrip contextMenuStrip1;
        //private ToolStripMenuItem mi数据位;
        //private ToolStripMenuItem mi停止位;
        //private ToolStripMenuItem mi校验;
        //private ToolStripMenuItem mi字符串编码;
        //private ToolStripMenuItem miHEX编码接收;
        //private ToolStripMenuItem miHex不换行;
        //private ToolStripMenuItem miHex自动换行;
        //private ToolStripMenuItem mi高级;
        //private ToolStripMenuItem miDTR;
        //private ToolStripMenuItem miRTS;
        //private ToolStripMenuItem miBreak;
        //private ToolStripSeparator toolStripMenuItem1;
        //private ToolStripSeparator toolStripMenuItem2;
        //private ToolStripMenuItem miHEX编码发送;

    }
}
