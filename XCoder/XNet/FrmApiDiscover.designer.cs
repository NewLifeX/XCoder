namespace XNet
{
    partial class FrmApiDiscover
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
            this.dgv = new System.Windows.Forms.DataGridView();
            this.btnDiscover = new System.Windows.Forms.Button();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.pnlSetting = new System.Windows.Forms.Panel();
            this.chkUdp = new System.Windows.Forms.CheckBox();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.gbReceive.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.pnlSetting.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbReceive
            // 
            this.gbReceive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbReceive.Controls.Add(this.dgv);
            this.gbReceive.Location = new System.Drawing.Point(17, 86);
            this.gbReceive.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.gbReceive.Name = "gbReceive";
            this.gbReceive.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.gbReceive.Size = new System.Drawing.Size(1195, 655);
            this.gbReceive.TabIndex = 4;
            this.gbReceive.TabStop = false;
            this.gbReceive.Text = "信息";
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgv.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.Location = new System.Drawing.Point(5, 29);
            this.dgv.Margin = new System.Windows.Forms.Padding(4);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersWidth = 51;
            this.dgv.RowTemplate.Height = 29;
            this.dgv.Size = new System.Drawing.Size(1185, 620);
            this.dgv.TabIndex = 0;
            // 
            // btnDiscover
            // 
            this.btnDiscover.Location = new System.Drawing.Point(1076, 18);
            this.btnDiscover.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnDiscover.Name = "btnDiscover";
            this.btnDiscover.Size = new System.Drawing.Size(122, 58);
            this.btnDiscover.TabIndex = 3;
            this.btnDiscover.Text = "搜索";
            this.btnDiscover.UseVisualStyleBackColor = true;
            this.btnDiscover.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // numPort
            // 
            this.numPort.Location = new System.Drawing.Point(361, 16);
            this.numPort.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
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
            this.toolTip1.SetToolTip(this.numPort, "端口");
            this.numPort.Value = new decimal(new int[] {
            5500,
            0,
            0,
            0});
            // 
            // pnlSetting
            // 
            this.pnlSetting.Controls.Add(this.chkUdp);
            this.pnlSetting.Controls.Add(this.txtIP);
            this.pnlSetting.Controls.Add(this.label4);
            this.pnlSetting.Controls.Add(this.numPort);
            this.pnlSetting.Location = new System.Drawing.Point(17, 16);
            this.pnlSetting.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.pnlSetting.Name = "pnlSetting";
            this.pnlSetting.Size = new System.Drawing.Size(1049, 62);
            this.pnlSetting.TabIndex = 13;
            // 
            // chkUdp
            // 
            this.chkUdp.AutoSize = true;
            this.chkUdp.Checked = true;
            this.chkUdp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUdp.Location = new System.Drawing.Point(541, 17);
            this.chkUdp.Margin = new System.Windows.Forms.Padding(4);
            this.chkUdp.Name = "chkUdp";
            this.chkUdp.Size = new System.Drawing.Size(146, 28);
            this.chkUdp.TabIndex = 20;
            this.chkUdp.Text = "广播UDP探测";
            this.chkUdp.UseVisualStyleBackColor = true;
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(90, 16);
            this.txtIP.Margin = new System.Windows.Forms.Padding(4);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(261, 30);
            this.txtIP.TabIndex = 18;
            this.txtIP.Text = "192.168.1.1/24";
            this.toolTip1.SetToolTip(this.txtIP, "指定要搜索的远程IP段，将搜索ApiServer");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 19);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 24);
            this.label4.TabIndex = 16;
            this.label4.Text = "远程：";
            // 
            // FrmApiDiscover
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1222, 758);
            this.Controls.Add(this.pnlSetting);
            this.Controls.Add(this.btnDiscover);
            this.Controls.Add(this.gbReceive);
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "FrmApiDiscover";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ApiServer搜索";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.gbReceive.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.pnlSetting.ResumeLayout(false);
            this.pnlSetting.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbReceive;
        private System.Windows.Forms.Button btnDiscover;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.Panel pnlSetting;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label4;
        private TextBox txtIP;
        private CheckBox chkUdp;
        private DataGridView dgv;
    }
}

