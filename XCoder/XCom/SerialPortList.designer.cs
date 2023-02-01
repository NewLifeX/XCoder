namespace NewLife.Windows
{
    partial class SerialPortList
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cbBaudrate = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbName = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mi数据位 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi停止位 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi校验 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi高级 = new System.Windows.Forms.ToolStripMenuItem();
            this.miDTR = new System.Windows.Forms.ToolStripMenuItem();
            this.miRTS = new System.Windows.Forms.ToolStripMenuItem();
            this.miBreak = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mi字符串编码 = new System.Windows.Forms.ToolStripMenuItem();
            this.miHEX编码接收 = new System.Windows.Forms.ToolStripMenuItem();
            this.miHex不换行 = new System.Windows.Forms.ToolStripMenuItem();
            this.miHex自动换行 = new System.Windows.Forms.ToolStripMenuItem();
            this.miHEX编码发送 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbBaudrate
            // 
            this.cbBaudrate.FormattingEnabled = true;
            this.cbBaudrate.Location = new System.Drawing.Point(488, 8);
            this.cbBaudrate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbBaudrate.Name = "cbBaudrate";
            this.cbBaudrate.Size = new System.Drawing.Size(106, 28);
            this.cbBaudrate.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(416, 12);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "波特率：";
            // 
            // cbName
            // 
            this.cbName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbName.FormattingEnabled = true;
            this.cbName.Location = new System.Drawing.Point(62, 8);
            this.cbName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbName.Name = "cbName";
            this.cbName.Size = new System.Drawing.Size(343, 28);
            this.cbName.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 12);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "端口：";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi数据位,
            this.mi停止位,
            this.mi校验,
            this.mi高级,
            this.toolStripMenuItem1,
            this.mi字符串编码,
            this.miHEX编码接收,
            this.miHEX编码发送,
            this.toolStripMenuItem2});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(169, 184);
            // 
            // mi数据位
            // 
            this.mi数据位.Name = "mi数据位";
            this.mi数据位.Size = new System.Drawing.Size(168, 24);
            this.mi数据位.Text = "数据位";
            // 
            // mi停止位
            // 
            this.mi停止位.Name = "mi停止位";
            this.mi停止位.Size = new System.Drawing.Size(168, 24);
            this.mi停止位.Text = "停止位";
            // 
            // mi校验
            // 
            this.mi校验.Name = "mi校验";
            this.mi校验.Size = new System.Drawing.Size(168, 24);
            this.mi校验.Text = "校验";
            // 
            // mi高级
            // 
            this.mi高级.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miDTR,
            this.miRTS,
            this.miBreak});
            this.mi高级.Name = "mi高级";
            this.mi高级.Size = new System.Drawing.Size(168, 24);
            this.mi高级.Text = "高级";
            // 
            // miDTR
            // 
            this.miDTR.Name = "miDTR";
            this.miDTR.Size = new System.Drawing.Size(132, 26);
            this.miDTR.Text = "DTR";
            this.miDTR.Click += new System.EventHandler(this.miDTR_Click);
            // 
            // miRTS
            // 
            this.miRTS.Name = "miRTS";
            this.miRTS.Size = new System.Drawing.Size(132, 26);
            this.miRTS.Text = "RTS";
            this.miRTS.Click += new System.EventHandler(this.miDTR_Click);
            // 
            // miBreak
            // 
            this.miBreak.Name = "miBreak";
            this.miBreak.Size = new System.Drawing.Size(132, 26);
            this.miBreak.Text = "Break";
            this.miBreak.Click += new System.EventHandler(this.miDTR_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(165, 6);
            // 
            // mi字符串编码
            // 
            this.mi字符串编码.Name = "mi字符串编码";
            this.mi字符串编码.Size = new System.Drawing.Size(168, 24);
            this.mi字符串编码.Text = "字符串编码";
            this.mi字符串编码.Click += new System.EventHandler(this.mi字符串编码_Click);
            // 
            // miHEX编码接收
            // 
            this.miHEX编码接收.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miHex不换行,
            this.miHex自动换行});
            this.miHEX编码接收.Name = "miHEX编码接收";
            this.miHEX编码接收.Size = new System.Drawing.Size(168, 24);
            this.miHEX编码接收.Text = "HEX编码接收";
            this.miHEX编码接收.Click += new System.EventHandler(this.mi字符串编码_Click);
            // 
            // miHex不换行
            // 
            this.miHex不换行.Name = "miHex不换行";
            this.miHex不换行.Size = new System.Drawing.Size(152, 26);
            this.miHex不换行.Tag = "false";
            this.miHex不换行.Text = "不换行";
            this.miHex不换行.Click += new System.EventHandler(this.miHex自动换行_Click);
            // 
            // miHex自动换行
            // 
            this.miHex自动换行.Name = "miHex自动换行";
            this.miHex自动换行.Size = new System.Drawing.Size(152, 26);
            this.miHex自动换行.Tag = "true";
            this.miHex自动换行.Text = "自动换行";
            this.miHex自动换行.Click += new System.EventHandler(this.miHex自动换行_Click);
            // 
            // miHEX编码发送
            // 
            this.miHEX编码发送.Name = "miHEX编码发送";
            this.miHEX编码发送.Size = new System.Drawing.Size(168, 24);
            this.miHEX编码发送.Text = "HEX编码发送";
            this.miHEX编码发送.Click += new System.EventHandler(this.miHEX编码发送_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(165, 6);
            // 
            // SerialPortList
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.cbBaudrate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbName);
            this.Controls.Add(this.label3);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "SerialPortList";
            this.Size = new System.Drawing.Size(600, 48);
            this.Load += new System.EventHandler(this.SerialPortList_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbBaudrate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mi数据位;
        private System.Windows.Forms.ToolStripMenuItem mi停止位;
        private System.Windows.Forms.ToolStripMenuItem mi校验;
        private System.Windows.Forms.ToolStripMenuItem mi字符串编码;
        private System.Windows.Forms.ToolStripMenuItem miHEX编码接收;
        private System.Windows.Forms.ToolStripMenuItem miHex不换行;
        private System.Windows.Forms.ToolStripMenuItem miHex自动换行;
        private System.Windows.Forms.ToolStripMenuItem mi高级;
        private System.Windows.Forms.ToolStripMenuItem miDTR;
        private System.Windows.Forms.ToolStripMenuItem miRTS;
        private System.Windows.Forms.ToolStripMenuItem miBreak;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem miHEX编码发送;

    }
}
