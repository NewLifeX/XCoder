﻿namespace XCom
{
    partial class FrmMain
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
            this.numMutilSend = new System.Windows.Forms.NumericUpDown();
            this.gbReceive = new System.Windows.Forms.GroupBox();
            this.txtReceive = new System.Windows.Forms.RichTextBox();
            this.gbSend = new System.Windows.Forms.GroupBox();
            this.numSleep = new System.Windows.Forms.NumericUpDown();
            this.txtSend = new System.Windows.Forms.RichTextBox();
            this.menuSend = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mi清空2 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSend = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.spList = new NewLife.Windows.SerialPortList();
            ((System.ComponentModel.ISupportInitialize)(this.numMutilSend)).BeginInit();
            this.gbReceive.SuspendLayout();
            this.gbSend.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSleep)).BeginInit();
            this.menuSend.SuspendLayout();
            this.SuspendLayout();
            // 
            // numMutilSend
            // 
            this.numMutilSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numMutilSend.Location = new System.Drawing.Point(654, 51);
            this.numMutilSend.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numMutilSend.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numMutilSend.Name = "numMutilSend";
            this.numMutilSend.Size = new System.Drawing.Size(78, 27);
            this.numMutilSend.TabIndex = 14;
            this.numMutilSend.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numMutilSend.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // gbReceive
            // 
            this.gbReceive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbReceive.Controls.Add(this.txtReceive);
            this.gbReceive.Location = new System.Drawing.Point(13, 71);
            this.gbReceive.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbReceive.Name = "gbReceive";
            this.gbReceive.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbReceive.Size = new System.Drawing.Size(825, 402);
            this.gbReceive.TabIndex = 4;
            this.gbReceive.TabStop = false;
            this.gbReceive.Text = "接收区：已接收0字节";
            // 
            // txtReceive
            // 
            this.txtReceive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtReceive.HideSelection = false;
            this.txtReceive.Location = new System.Drawing.Point(4, 25);
            this.txtReceive.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtReceive.Name = "txtReceive";
            this.txtReceive.Size = new System.Drawing.Size(817, 372);
            this.txtReceive.TabIndex = 1;
            this.txtReceive.Text = "";
            // 
            // gbSend
            // 
            this.gbSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSend.Controls.Add(this.numSleep);
            this.gbSend.Controls.Add(this.txtSend);
            this.gbSend.Controls.Add(this.btnSend);
            this.gbSend.Controls.Add(this.numMutilSend);
            this.gbSend.Controls.Add(this.label1);
            this.gbSend.Controls.Add(this.label7);
            this.gbSend.Location = new System.Drawing.Point(14, 478);
            this.gbSend.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbSend.Name = "gbSend";
            this.gbSend.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbSend.Size = new System.Drawing.Size(825, 151);
            this.gbSend.TabIndex = 5;
            this.gbSend.TabStop = false;
            this.gbSend.Text = "发送区：已发送0字节";
            // 
            // numSleep
            // 
            this.numSleep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numSleep.Location = new System.Drawing.Point(654, 96);
            this.numSleep.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numSleep.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSleep.Name = "numSleep";
            this.numSleep.Size = new System.Drawing.Size(78, 27);
            this.numSleep.TabIndex = 16;
            this.numSleep.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numSleep.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // txtSend
            // 
            this.txtSend.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSend.ContextMenuStrip = this.menuSend;
            this.txtSend.HideSelection = false;
            this.txtSend.Location = new System.Drawing.Point(3, 30);
            this.txtSend.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSend.Name = "txtSend";
            this.txtSend.Size = new System.Drawing.Size(590, 113);
            this.txtSend.TabIndex = 2;
            this.txtSend.Text = "";
            // 
            // menuSend
            // 
            this.menuSend.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuSend.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi清空2});
            this.menuSend.Name = "menuSend";
            this.menuSend.Size = new System.Drawing.Size(109, 28);
            // 
            // mi清空2
            // 
            this.mi清空2.Name = "mi清空2";
            this.mi清空2.Size = new System.Drawing.Size(108, 24);
            this.mi清空2.Text = "清空";
            this.mi清空2.Click += new System.EventHandler(this.mi清空2_Click);
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Location = new System.Drawing.Point(741, 41);
            this.btnSend.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 102);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "发送";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(602, 99);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 20);
            this.label1.TabIndex = 17;
            this.label1.Text = "间隔：";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(602, 54);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 20);
            this.label7.TabIndex = 15;
            this.label7.Text = "次数：";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(739, 14);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(100, 48);
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
            // spList
            // 
            this.spList.BytesOfReceived = 0;
            this.spList.BytesOfSent = 0;
            this.spList.Location = new System.Drawing.Point(18, 13);
            this.spList.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.spList.Name = "spList";
            this.spList.Port = null;
            this.spList.SelectedPort = "";
            this.spList.Size = new System.Drawing.Size(622, 48);
            this.spList.TabIndex = 6;
            this.spList.ReceivedString += new System.EventHandler<NewLife.Windows.StringEventArgs>(this.OnReceived);
            // 
            // FrmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(848, 643);
            this.Controls.Add(this.spList);
            this.Controls.Add(this.gbSend);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.gbReceive);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "串口调试";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numMutilSend)).EndInit();
            this.gbReceive.ResumeLayout(false);
            this.gbSend.ResumeLayout(false);
            this.gbSend.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSleep)).EndInit();
            this.menuSend.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbReceive;
        private System.Windows.Forms.GroupBox gbSend;
        private System.Windows.Forms.NumericUpDown numMutilSend;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ContextMenuStrip menuSend;
        private System.Windows.Forms.ToolStripMenuItem mi清空2;
        private NewLife.Windows.SerialPortList spList;
        private System.Windows.Forms.RichTextBox txtReceive;
        private System.Windows.Forms.RichTextBox txtSend;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numSleep;
    }
}

