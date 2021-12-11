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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.pnlSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtReceive
            // 
            this.txtReceive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReceive.HideSelection = false;
            this.txtReceive.Location = new System.Drawing.Point(383, 66);
            this.txtReceive.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtReceive.Name = "txtReceive";
            this.txtReceive.Size = new System.Drawing.Size(598, 488);
            this.txtReceive.TabIndex = 1;
            this.txtReceive.Text = "";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(881, 13);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(100, 43);
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
            this.label1.Location = new System.Drawing.Point(8, 13);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 18);
            this.label1.TabIndex = 6;
            this.label1.Text = "模式";
            // 
            // lbLocal
            // 
            this.lbLocal.AutoSize = true;
            this.lbLocal.Location = new System.Drawing.Point(525, 13);
            this.lbLocal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbLocal.Name = "lbLocal";
            this.lbLocal.Size = new System.Drawing.Size(62, 18);
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
            this.cbMode.Location = new System.Drawing.Point(62, 8);
            this.cbMode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbMode.Name = "cbMode";
            this.cbMode.Size = new System.Drawing.Size(138, 26);
            this.cbMode.TabIndex = 9;
            // 
            // numPort
            // 
            this.numPort.Location = new System.Drawing.Point(277, 7);
            this.numPort.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.numPort.Size = new System.Drawing.Size(76, 28);
            this.numPort.TabIndex = 11;
            this.numPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.numPort, "端口");
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
            this.pnlSetting.Location = new System.Drawing.Point(14, 12);
            this.pnlSetting.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlSetting.Name = "pnlSetting";
            this.pnlSetting.Size = new System.Drawing.Size(858, 47);
            this.pnlSetting.TabIndex = 13;
            // 
            // numHost
            // 
            this.numHost.Location = new System.Drawing.Point(429, 7);
            this.numHost.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.numHost.Size = new System.Drawing.Size(76, 28);
            this.numHost.TabIndex = 21;
            this.numHost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.numHost, "端口");
            this.numHost.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(372, 13);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 18);
            this.label4.TabIndex = 22;
            this.label4.Text = "站号：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(693, 13);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 18);
            this.label3.TabIndex = 20;
            this.label3.Text = "个数：";
            // 
            // numCount
            // 
            this.numCount.Location = new System.Drawing.Point(760, 7);
            this.numCount.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.numCount.Size = new System.Drawing.Size(76, 28);
            this.numCount.TabIndex = 18;
            this.numCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.numCount, "端口");
            this.numCount.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            // 
            // numAddress
            // 
            this.numAddress.Location = new System.Drawing.Point(585, 7);
            this.numAddress.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.numAddress.Size = new System.Drawing.Size(96, 28);
            this.numAddress.TabIndex = 17;
            this.numAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.numAddress, "端口");
            this.numAddress.Value = new decimal(new int[] {
            40000,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(224, 13);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 18);
            this.label2.TabIndex = 19;
            this.label2.Text = "端口：";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(14, 67);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 27;
            this.dataGridView1.Size = new System.Drawing.Size(362, 487);
            this.dataGridView1.TabIndex = 14;
            // 
            // FrmModbusSlave
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 569);
            this.Controls.Add(this.txtReceive);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.pnlSetting);
            this.Controls.Add(this.btnConnect);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FrmModbusSlave";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ModbusSlave";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.pnlSetting.ResumeLayout(false);
            this.pnlSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
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
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.NumericUpDown numCount;
        private System.Windows.Forms.NumericUpDown numAddress;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numHost;
    }
}

