namespace XCoder.Tools
{
    partial class FrmBackup
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
            this.gbReceive = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.btnBackup = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lbAddr = new System.Windows.Forms.Label();
            this.pnlSetting = new System.Windows.Forms.Panel();
            this.cbAllowDelete = new System.Windows.Forms.CheckBox();
            this.txtSrc5 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSrc4 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSrc3 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSrc2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSrc1 = new System.Windows.Forms.TextBox();
            this.txtDest = new System.Windows.Forms.TextBox();
            this.btnBrowser = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.gbReceive.SuspendLayout();
            this.pnlSetting.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbReceive
            // 
            this.gbReceive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbReceive.Controls.Add(this.txtLog);
            this.gbReceive.Location = new System.Drawing.Point(13, 257);
            this.gbReceive.Margin = new System.Windows.Forms.Padding(4);
            this.gbReceive.Name = "gbReceive";
            this.gbReceive.Padding = new System.Windows.Forms.Padding(4);
            this.gbReceive.Size = new System.Drawing.Size(978, 298);
            this.gbReceive.TabIndex = 4;
            this.gbReceive.TabStop = false;
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.HideSelection = false;
            this.txtLog.Location = new System.Drawing.Point(4, 25);
            this.txtLog.Margin = new System.Windows.Forms.Padding(4);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(970, 269);
            this.txtLog.TabIndex = 1;
            this.txtLog.Text = "";
            // 
            // btnBackup
            // 
            this.btnBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBackup.Location = new System.Drawing.Point(798, 151);
            this.btnBackup.Margin = new System.Windows.Forms.Padding(4);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(100, 44);
            this.btnBackup.TabIndex = 3;
            this.btnBackup.Text = "开始备份";
            this.btnBackup.UseVisualStyleBackColor = true;
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 16);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 18);
            this.label1.TabIndex = 6;
            this.label1.Text = "目标目录：";
            // 
            // lbAddr
            // 
            this.lbAddr.AutoSize = true;
            this.lbAddr.Location = new System.Drawing.Point(10, 58);
            this.lbAddr.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbAddr.Name = "lbAddr";
            this.lbAddr.Size = new System.Drawing.Size(107, 18);
            this.lbAddr.TabIndex = 7;
            this.lbAddr.Text = "原始目录1：";
            // 
            // pnlSetting
            // 
            this.pnlSetting.Controls.Add(this.button5);
            this.pnlSetting.Controls.Add(this.button4);
            this.pnlSetting.Controls.Add(this.button3);
            this.pnlSetting.Controls.Add(this.button2);
            this.pnlSetting.Controls.Add(this.button1);
            this.pnlSetting.Controls.Add(this.btnBrowser);
            this.pnlSetting.Controls.Add(this.txtSrc5);
            this.pnlSetting.Controls.Add(this.label5);
            this.pnlSetting.Controls.Add(this.txtSrc4);
            this.pnlSetting.Controls.Add(this.label4);
            this.pnlSetting.Controls.Add(this.txtSrc3);
            this.pnlSetting.Controls.Add(this.label3);
            this.pnlSetting.Controls.Add(this.txtSrc2);
            this.pnlSetting.Controls.Add(this.label2);
            this.pnlSetting.Controls.Add(this.txtSrc1);
            this.pnlSetting.Controls.Add(this.txtDest);
            this.pnlSetting.Controls.Add(this.label1);
            this.pnlSetting.Controls.Add(this.lbAddr);
            this.pnlSetting.Location = new System.Drawing.Point(14, 12);
            this.pnlSetting.Margin = new System.Windows.Forms.Padding(4);
            this.pnlSetting.Name = "pnlSetting";
            this.pnlSetting.Size = new System.Drawing.Size(708, 237);
            this.pnlSetting.TabIndex = 13;
            // 
            // cbAllowDelete
            // 
            this.cbAllowDelete.AutoSize = true;
            this.cbAllowDelete.Location = new System.Drawing.Point(792, 85);
            this.cbAllowDelete.Name = "cbAllowDelete";
            this.cbAllowDelete.Size = new System.Drawing.Size(106, 22);
            this.cbAllowDelete.TabIndex = 18;
            this.cbAllowDelete.Text = "允许删除";
            this.cbAllowDelete.UseVisualStyleBackColor = true;
            // 
            // txtSrc5
            // 
            this.txtSrc5.Location = new System.Drawing.Point(115, 189);
            this.txtSrc5.Name = "txtSrc5";
            this.txtSrc5.Size = new System.Drawing.Size(465, 28);
            this.txtSrc5.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 194);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 18);
            this.label5.TabIndex = 16;
            this.label5.Text = "原始目录5：";
            // 
            // txtSrc4
            // 
            this.txtSrc4.Location = new System.Drawing.Point(115, 155);
            this.txtSrc4.Name = "txtSrc4";
            this.txtSrc4.Size = new System.Drawing.Size(465, 28);
            this.txtSrc4.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 160);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 18);
            this.label4.TabIndex = 14;
            this.label4.Text = "原始目录4：";
            // 
            // txtSrc3
            // 
            this.txtSrc3.Location = new System.Drawing.Point(115, 121);
            this.txtSrc3.Name = "txtSrc3";
            this.txtSrc3.Size = new System.Drawing.Size(465, 28);
            this.txtSrc3.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 126);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 18);
            this.label3.TabIndex = 12;
            this.label3.Text = "原始目录3：";
            // 
            // txtSrc2
            // 
            this.txtSrc2.Location = new System.Drawing.Point(115, 87);
            this.txtSrc2.Name = "txtSrc2";
            this.txtSrc2.Size = new System.Drawing.Size(465, 28);
            this.txtSrc2.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 92);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 18);
            this.label2.TabIndex = 10;
            this.label2.Text = "原始目录2：";
            // 
            // txtSrc1
            // 
            this.txtSrc1.Location = new System.Drawing.Point(115, 53);
            this.txtSrc1.Name = "txtSrc1";
            this.txtSrc1.Size = new System.Drawing.Size(465, 28);
            this.txtSrc1.TabIndex = 9;
            // 
            // txtDest
            // 
            this.txtDest.Location = new System.Drawing.Point(115, 11);
            this.txtDest.Name = "txtDest";
            this.txtDest.Size = new System.Drawing.Size(465, 28);
            this.txtDest.TabIndex = 8;
            // 
            // btnBrowser
            // 
            this.btnBrowser.Location = new System.Drawing.Point(601, 12);
            this.btnBrowser.Name = "btnBrowser";
            this.btnBrowser.Size = new System.Drawing.Size(69, 26);
            this.btnBrowser.TabIndex = 19;
            this.btnBrowser.Tag = "txtDest";
            this.btnBrowser.Text = "浏览";
            this.btnBrowser.UseVisualStyleBackColor = true;
            this.btnBrowser.Click += new System.EventHandler(this.btnBrowser_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(601, 54);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(69, 26);
            this.button1.TabIndex = 20;
            this.button1.Tag = "txtSrc1";
            this.button1.Text = "浏览";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnBrowser_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(601, 88);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(69, 26);
            this.button2.TabIndex = 21;
            this.button2.Tag = "txtSrc2";
            this.button2.Text = "浏览";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnBrowser_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(601, 122);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(69, 26);
            this.button3.TabIndex = 22;
            this.button3.Tag = "txtSrc3";
            this.button3.Text = "浏览";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.btnBrowser_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(601, 156);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(69, 26);
            this.button4.TabIndex = 23;
            this.button4.Tag = "txtSrc4";
            this.button4.Text = "浏览";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.btnBrowser_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(601, 190);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(69, 26);
            this.button5.TabIndex = 24;
            this.button5.Tag = "txtSrc5";
            this.button5.Text = "浏览";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.btnBrowser_Click);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // FrmBackup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 568);
            this.Controls.Add(this.pnlSetting);
            this.Controls.Add(this.cbAllowDelete);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.gbReceive);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmBackup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "图片备份";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.gbReceive.ResumeLayout(false);
            this.pnlSetting.ResumeLayout(false);
            this.pnlSetting.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbReceive;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbAddr;
        private System.Windows.Forms.Panel pnlSetting;
        private System.Windows.Forms.TextBox txtDest;
        private System.Windows.Forms.TextBox txtSrc1;
        private System.Windows.Forms.TextBox txtSrc5;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSrc4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSrc3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSrc2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbAllowDelete;
        private System.Windows.Forms.Button btnBrowser;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}

