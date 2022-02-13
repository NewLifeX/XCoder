namespace XNet
{
    partial class FrmModbusMaster
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
            this.btnConnect = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnlSetting = new System.Windows.Forms.Panel();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numHost = new System.Windows.Forms.NumericUpDown();
            this.gbSend = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbFunctionCode = new System.Windows.Forms.ComboBox();
            this.numCount = new System.Windows.Forms.NumericUpDown();
            this.btnSend = new System.Windows.Forms.Button();
            this.numAddress = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
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
            this.dgv = new System.Windows.Forms.DataGridView();
            this.btnAdd = new System.Windows.Forms.Button();
            this.pnlSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHost)).BeginInit();
            this.gbSend.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAddress)).BeginInit();
            this.menuReceive.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(1077, 18);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(122, 58);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "打开";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pnlSetting
            // 
            this.pnlSetting.Controls.Add(this.txtAddress);
            this.pnlSetting.Controls.Add(this.label3);
            this.pnlSetting.Controls.Add(this.label4);
            this.pnlSetting.Controls.Add(this.numHost);
            this.pnlSetting.Location = new System.Drawing.Point(16, 16);
            this.pnlSetting.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.pnlSetting.Name = "pnlSetting";
            this.pnlSetting.Size = new System.Drawing.Size(1049, 62);
            this.pnlSetting.TabIndex = 13;
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(99, 11);
            this.txtAddress.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(284, 30);
            this.txtAddress.TabIndex = 19;
            this.txtAddress.Text = "tcp://127.0.0.1:502";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(421, 19);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 24);
            this.label3.TabIndex = 18;
            this.label3.Text = "站号：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 19);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 24);
            this.label4.TabIndex = 16;
            this.label4.Text = "远程";
            // 
            // numHost
            // 
            this.numHost.Location = new System.Drawing.Point(498, 11);
            this.numHost.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
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
            this.numHost.Size = new System.Drawing.Size(94, 30);
            this.numHost.TabIndex = 11;
            this.numHost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numHost.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // gbSend
            // 
            this.gbSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSend.Controls.Add(this.label1);
            this.gbSend.Controls.Add(this.cbFunctionCode);
            this.gbSend.Controls.Add(this.numCount);
            this.gbSend.Controls.Add(this.btnSend);
            this.gbSend.Controls.Add(this.numAddress);
            this.gbSend.Controls.Add(this.label2);
            this.gbSend.Controls.Add(this.label7);
            this.gbSend.Enabled = false;
            this.gbSend.Location = new System.Drawing.Point(16, 566);
            this.gbSend.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbSend.Name = "gbSend";
            this.gbSend.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbSend.Size = new System.Drawing.Size(1195, 168);
            this.gbSend.TabIndex = 15;
            this.gbSend.TabStop = false;
            this.gbSend.Text = "发送区：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(620, 67);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 24);
            this.label1.TabIndex = 20;
            this.label1.Text = "个数：";
            // 
            // cbFunctionCode
            // 
            this.cbFunctionCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFunctionCode.FormattingEnabled = true;
            this.cbFunctionCode.Items.AddRange(new object[] {
            "01 读线圈状态",
            "02 读离散输入状态",
            "03 读保持寄存器",
            "04 读输入寄存器",
            "05 写单个线圈",
            "06 写单个保存寄存器",
            "15 写多个线圈",
            "16 写多个保持寄存器"});
            this.cbFunctionCode.Location = new System.Drawing.Point(116, 59);
            this.cbFunctionCode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbFunctionCode.Name = "cbFunctionCode";
            this.cbFunctionCode.Size = new System.Drawing.Size(234, 32);
            this.cbFunctionCode.TabIndex = 19;
            this.cbFunctionCode.SelectedIndexChanged += new System.EventHandler(this.cbFunctionCode_SelectedIndexChanged);
            // 
            // numCount
            // 
            this.numCount.Location = new System.Drawing.Point(703, 59);
            this.numCount.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.numCount.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCount.Name = "numCount";
            this.numCount.Size = new System.Drawing.Size(95, 30);
            this.numCount.TabIndex = 18;
            this.numCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numCount.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(852, 48);
            this.btnSend.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(92, 61);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "发送";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // numAddress
            // 
            this.numAddress.Location = new System.Drawing.Point(476, 59);
            this.numAddress.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.numAddress.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numAddress.Name = "numAddress";
            this.numAddress.Size = new System.Drawing.Size(106, 30);
            this.numAddress.TabIndex = 14;
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
            this.label2.Location = new System.Drawing.Point(22, 66);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 24);
            this.label2.TabIndex = 17;
            this.label2.Text = "操作码：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(412, 67);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 24);
            this.label7.TabIndex = 15;
            this.label7.Text = "地址：";
            // 
            // txtReceive
            // 
            this.txtReceive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReceive.ContextMenuStrip = this.menuReceive;
            this.txtReceive.HideSelection = false;
            this.txtReceive.Location = new System.Drawing.Point(490, 91);
            this.txtReceive.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtReceive.Name = "txtReceive";
            this.txtReceive.Size = new System.Drawing.Size(720, 462);
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
            // dgv
            // 
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(16, 90);
            this.dgv.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersWidth = 51;
            this.dgv.RowTemplate.Height = 27;
            this.dgv.Size = new System.Drawing.Size(465, 405);
            this.dgv.TabIndex = 16;
            this.dgv.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(16, 504);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(106, 50);
            this.btnAdd.TabIndex = 17;
            this.btnAdd.Text = "添加";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // FrmModbusMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1222, 758);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.txtReceive);
            this.Controls.Add(this.gbSend);
            this.Controls.Add(this.pnlSetting);
            this.Controls.Add(this.btnConnect);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "FrmModbusMaster";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ModbusMaster";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.pnlSetting.ResumeLayout(false);
            this.pnlSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHost)).EndInit();
            this.gbSend.ResumeLayout(false);
            this.gbSend.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAddress)).EndInit();
            this.menuReceive.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel pnlSetting;
        private System.Windows.Forms.GroupBox gbSend;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.NumericUpDown numAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numHost;
        private System.Windows.Forms.ComboBox cbFunctionCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.RichTextBox txtReceive;
        private System.Windows.Forms.DataGridView dgv;
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

