namespace XNet
{
    partial class FrmModbusSlave
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtReceive = new System.Windows.Forms.RichTextBox();
            this.menuReceive = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi日志着色 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.mi显示应用日志 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi显示网络日志 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi显示接收字符串 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi显示发送数据 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi显示接收数据 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnConnect = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.lbLocal = new System.Windows.Forms.Label();
            this.cbMode = new System.Windows.Forms.ComboBox();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.pnlSetting = new System.Windows.Forms.Panel();
            this.numHost = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numCount = new System.Windows.Forms.NumericUpDown();
            this.numAddress = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.btnAdd = new System.Windows.Forms.Button();
            this.menuReceive.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.pnlSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // txtReceive
            // 
            this.txtReceive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReceive.ContextMenuStrip = this.menuReceive;
            this.txtReceive.HideSelection = false;
            this.txtReceive.Location = new System.Drawing.Point(490, 88);
            this.txtReceive.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.txtReceive.Name = "txtReceive";
            this.txtReceive.Size = new System.Drawing.Size(708, 649);
            this.txtReceive.TabIndex = 1;
            this.txtReceive.Text = "";
            // 
            // menuReceive
            // 
            this.menuReceive.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuReceive.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.mi日志着色,
            this.toolStripMenuItem3,
            this.mi显示应用日志,
            this.mi显示网络日志,
            this.mi显示接收字符串,
            this.mi显示发送数据,
            this.mi显示接收数据});
            this.menuReceive.Name = "menuSend";
            this.menuReceive.Size = new System.Drawing.Size(207, 220);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(206, 30);
            this.toolStripMenuItem1.Text = "清空";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.mi清空_Click);
            // 
            // mi日志着色
            // 
            this.mi日志着色.Name = "mi日志着色";
            this.mi日志着色.Size = new System.Drawing.Size(206, 30);
            this.mi日志着色.Text = "日志着色";
            this.mi日志着色.Click += new System.EventHandler(this.Menu_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(203, 6);
            // 
            // mi显示应用日志
            // 
            this.mi显示应用日志.Name = "mi显示应用日志";
            this.mi显示应用日志.Size = new System.Drawing.Size(206, 30);
            this.mi显示应用日志.Text = "显示应用日志";
            this.mi显示应用日志.Click += new System.EventHandler(this.Menu_Click);
            // 
            // mi显示网络日志
            // 
            this.mi显示网络日志.Name = "mi显示网络日志";
            this.mi显示网络日志.Size = new System.Drawing.Size(206, 30);
            this.mi显示网络日志.Text = "显示网络日志";
            this.mi显示网络日志.Click += new System.EventHandler(this.Menu_Click);
            // 
            // mi显示接收字符串
            // 
            this.mi显示接收字符串.Name = "mi显示接收字符串";
            this.mi显示接收字符串.Size = new System.Drawing.Size(206, 30);
            this.mi显示接收字符串.Text = "显示接收字符串";
            this.mi显示接收字符串.Click += new System.EventHandler(this.Menu_Click);
            // 
            // mi显示发送数据
            // 
            this.mi显示发送数据.Name = "mi显示发送数据";
            this.mi显示发送数据.Size = new System.Drawing.Size(206, 30);
            this.mi显示发送数据.Text = "显示发送数据";
            this.mi显示发送数据.Click += new System.EventHandler(this.Menu_Click);
            // 
            // mi显示接收数据
            // 
            this.mi显示接收数据.Name = "mi显示接收数据";
            this.mi显示接收数据.Size = new System.Drawing.Size(206, 30);
            this.mi显示接收数据.Text = "显示接收数据";
            this.mi显示接收数据.Click += new System.EventHandler(this.Menu_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(1077, 17);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(122, 57);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "开始";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 17);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 24);
            this.label1.TabIndex = 6;
            this.label1.Text = "模式";
            // 
            // lbLocal
            // 
            this.lbLocal.AutoSize = true;
            this.lbLocal.Location = new System.Drawing.Point(642, 17);
            this.lbLocal.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbLocal.Name = "lbLocal";
            this.lbLocal.Size = new System.Drawing.Size(64, 24);
            this.lbLocal.TabIndex = 7;
            this.lbLocal.Text = "地址：";
            // 
            // cbMode
            // 
            this.cbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMode.FormattingEnabled = true;
            this.cbMode.Items.AddRange(new object[] {
            "0x0000",
            "0x7777",
            "0xFFFF",
            "递增",
            "静态随机",
            "动态随机"});
            this.cbMode.Location = new System.Drawing.Point(76, 11);
            this.cbMode.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.cbMode.Name = "cbMode";
            this.cbMode.Size = new System.Drawing.Size(168, 32);
            this.cbMode.TabIndex = 9;
            // 
            // numPort
            // 
            this.numPort.Location = new System.Drawing.Point(339, 9);
            this.numPort.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.numPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPort.Name = "numPort";
            this.numPort.Size = new System.Drawing.Size(93, 30);
            this.numPort.TabIndex = 11;
            this.numPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numPort.Value = new decimal(new int[] {
            502,
            0,
            0,
            0});
            // 
            // pnlSetting
            // 
            this.pnlSetting.Controls.Add(this.numHost);
            this.pnlSetting.Controls.Add(this.label4);
            this.pnlSetting.Controls.Add(this.label3);
            this.pnlSetting.Controls.Add(this.numCount);
            this.pnlSetting.Controls.Add(this.numPort);
            this.pnlSetting.Controls.Add(this.numAddress);
            this.pnlSetting.Controls.Add(this.label2);
            this.pnlSetting.Controls.Add(this.label1);
            this.pnlSetting.Controls.Add(this.lbLocal);
            this.pnlSetting.Controls.Add(this.cbMode);
            this.pnlSetting.Location = new System.Drawing.Point(17, 16);
            this.pnlSetting.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.pnlSetting.Name = "pnlSetting";
            this.pnlSetting.Size = new System.Drawing.Size(1049, 63);
            this.pnlSetting.TabIndex = 13;
            // 
            // numHost
            // 
            this.numHost.Location = new System.Drawing.Point(524, 9);
            this.numHost.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.numHost.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numHost.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numHost.Name = "numHost";
            this.numHost.Size = new System.Drawing.Size(93, 30);
            this.numHost.TabIndex = 21;
            this.numHost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numHost.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(455, 17);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 24);
            this.label4.TabIndex = 22;
            this.label4.Text = "站号：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(847, 17);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 24);
            this.label3.TabIndex = 20;
            this.label3.Text = "个数：";
            // 
            // numCount
            // 
            this.numCount.Location = new System.Drawing.Point(929, 9);
            this.numCount.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.numCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCount.Name = "numCount";
            this.numCount.Size = new System.Drawing.Size(93, 30);
            this.numCount.TabIndex = 18;
            this.numCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numCount.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // numAddress
            // 
            this.numAddress.Location = new System.Drawing.Point(715, 9);
            this.numAddress.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.numAddress.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numAddress.Name = "numAddress";
            this.numAddress.Size = new System.Drawing.Size(117, 30);
            this.numAddress.TabIndex = 17;
            this.numAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numAddress.Value = new decimal(new int[] {
            40000,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(274, 17);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 24);
            this.label2.TabIndex = 19;
            this.label2.Text = "端口：";
            // 
            // dgv
            // 
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(17, 89);
            this.dgv.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersWidth = 51;
            this.dgv.RowTemplate.Height = 27;
            this.dgv.Size = new System.Drawing.Size(464, 595);
            this.dgv.TabIndex = 14;
            this.dgv.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(17, 693);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(106, 49);
            this.btnAdd.TabIndex = 18;
            this.btnAdd.Text = "添加";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // FrmModbusSlave
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1222, 759);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.txtReceive);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.pnlSetting);
            this.Controls.Add(this.btnConnect);
            this.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.Name = "FrmModbusSlave";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ModbusSlave";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.menuReceive.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.pnlSetting.ResumeLayout(false);
            this.pnlSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.RichTextBox txtReceive;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbLocal;
        private System.Windows.Forms.ComboBox cbMode;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.Panel pnlSetting;
        private System.Windows.Forms.NumericUpDown numCount;
        private System.Windows.Forms.NumericUpDown numAddress;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numHost;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ContextMenuStrip menuReceive;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mi日志着色;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem mi显示应用日志;
        private System.Windows.Forms.ToolStripMenuItem mi显示网络日志;
        private System.Windows.Forms.ToolStripMenuItem mi显示接收字符串;
        private System.Windows.Forms.ToolStripMenuItem mi显示发送数据;
        private System.Windows.Forms.ToolStripMenuItem mi显示接收数据;
    }
}

