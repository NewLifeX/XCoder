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
            this.gbReceive = new System.Windows.Forms.GroupBox();
            this.txtReceive = new System.Windows.Forms.RichTextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnlSetting = new System.Windows.Forms.Panel();
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
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.gbReceive.SuspendLayout();
            this.pnlSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHost)).BeginInit();
            this.gbSend.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // gbReceive
            // 
            this.gbReceive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbReceive.Controls.Add(this.txtReceive);
            this.gbReceive.Location = new System.Drawing.Point(12, 54);
            this.gbReceive.Margin = new System.Windows.Forms.Padding(4);
            this.gbReceive.Name = "gbReceive";
            this.gbReceive.Padding = new System.Windows.Forms.Padding(4);
            this.gbReceive.Size = new System.Drawing.Size(869, 292);
            this.gbReceive.TabIndex = 4;
            this.gbReceive.TabStop = false;
            this.gbReceive.Text = "接收区：";
            // 
            // txtReceive
            // 
            this.txtReceive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtReceive.HideSelection = false;
            this.txtReceive.Location = new System.Drawing.Point(4, 22);
            this.txtReceive.Margin = new System.Windows.Forms.Padding(4);
            this.txtReceive.Name = "txtReceive";
            this.txtReceive.Size = new System.Drawing.Size(861, 266);
            this.txtReceive.TabIndex = 1;
            this.txtReceive.Text = "";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(783, 11);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(89, 36);
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
            this.pnlSetting.Location = new System.Drawing.Point(12, 10);
            this.pnlSetting.Margin = new System.Windows.Forms.Padding(4);
            this.pnlSetting.Name = "pnlSetting";
            this.pnlSetting.Size = new System.Drawing.Size(763, 39);
            this.pnlSetting.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(306, 12);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 18;
            this.label3.Text = "站号：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 12);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 15);
            this.label4.TabIndex = 16;
            this.label4.Text = "远程";
            // 
            // numHost
            // 
            this.numHost.Location = new System.Drawing.Point(362, 7);
            this.numHost.Margin = new System.Windows.Forms.Padding(4);
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
            this.numHost.Size = new System.Drawing.Size(68, 25);
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
            this.gbSend.Location = new System.Drawing.Point(12, 354);
            this.gbSend.Margin = new System.Windows.Forms.Padding(4);
            this.gbSend.Name = "gbSend";
            this.gbSend.Padding = new System.Windows.Forms.Padding(4);
            this.gbSend.Size = new System.Drawing.Size(869, 105);
            this.gbSend.TabIndex = 15;
            this.gbSend.TabStop = false;
            this.gbSend.Text = "发送区：";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(340, 41);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 15);
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
            this.cbFunctionCode.Location = new System.Drawing.Point(72, 37);
            this.cbFunctionCode.Name = "cbFunctionCode";
            this.cbFunctionCode.Size = new System.Drawing.Size(100, 23);
            this.cbFunctionCode.TabIndex = 19;
            // 
            // numCount
            // 
            this.numCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numCount.Location = new System.Drawing.Point(400, 36);
            this.numCount.Margin = new System.Windows.Forms.Padding(4);
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
            this.numCount.Size = new System.Drawing.Size(69, 25);
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
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Location = new System.Drawing.Point(509, 29);
            this.btnSend.Margin = new System.Windows.Forms.Padding(4);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(67, 38);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "发送";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // numAddress
            // 
            this.numAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numAddress.Location = new System.Drawing.Point(235, 36);
            this.numAddress.Margin = new System.Windows.Forms.Padding(4);
            this.numAddress.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numAddress.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numAddress.Name = "numAddress";
            this.numAddress.Size = new System.Drawing.Size(77, 25);
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
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 41);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 17;
            this.label2.Text = "读取：";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(189, 41);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 15);
            this.label7.TabIndex = 15;
            this.label7.Text = "地址：";
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(72, 7);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(208, 25);
            this.txtAddress.TabIndex = 19;
            this.txtAddress.Text = "tcp://127.0.0.1:502";
            // 
            // FrmModbusMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 474);
            this.Controls.Add(this.gbSend);
            this.Controls.Add(this.pnlSetting);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.gbReceive);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmModbusMaster";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "网络调试";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.gbReceive.ResumeLayout(false);
            this.pnlSetting.ResumeLayout(false);
            this.pnlSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHost)).EndInit();
            this.gbSend.ResumeLayout(false);
            this.gbSend.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAddress)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbReceive;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.RichTextBox txtReceive;
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
    }
}

