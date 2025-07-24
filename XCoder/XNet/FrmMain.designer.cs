namespace XNet
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
            components = new System.ComponentModel.Container();
            gbReceive = new GroupBox();
            txtReceive = new RichTextBox();
            menuReceive = new ContextMenuStrip(components);
            toolStripMenuItem1 = new ToolStripMenuItem();
            mi日志着色 = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripSeparator();
            mi显示应用日志 = new ToolStripMenuItem();
            mi显示网络日志 = new ToolStripMenuItem();
            mi显示接收字符串 = new ToolStripMenuItem();
            mi显示发送数据 = new ToolStripMenuItem();
            mi显示接收数据 = new ToolStripMenuItem();
            mi显示统计信息 = new ToolStripMenuItem();
            toolStripMenuItem4 = new ToolStripSeparator();
            查看Tcp参数ToolStripMenuItem = new ToolStripMenuItem();
            设置最大TcpToolStripMenuItem = new ToolStripMenuItem();
            menuSend = new ContextMenuStrip(components);
            miHexSend = new ToolStripMenuItem();
            mi清空2 = new ToolStripMenuItem();
            btnConnect = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            fontDialog1 = new FontDialog();
            colorDialog1 = new ColorDialog();
            label1 = new Label();
            lbLocal = new Label();
            cbMode = new ComboBox();
            cbLocal = new ComboBox();
            numPort = new NumericUpDown();
            pnlSetting = new Panel();
            cbRemote = new ComboBox();
            label4 = new Label();
            gbSend = new GroupBox();
            numThreads = new NumericUpDown();
            numSleep = new NumericUpDown();
            txtSend = new RichTextBox();
            btnSend = new Button();
            numMutilSend = new NumericUpDown();
            label2 = new Label();
            label7 = new Label();
            toolTip1 = new ToolTip(components);
            分IP记录文本日志ToolStripMenuItem = new ToolStripMenuItem();
            gbReceive.SuspendLayout();
            menuReceive.SuspendLayout();
            menuSend.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numPort).BeginInit();
            pnlSetting.SuspendLayout();
            gbSend.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numThreads).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSleep).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMutilSend).BeginInit();
            SuspendLayout();
            // 
            // gbReceive
            // 
            gbReceive.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gbReceive.Controls.Add(txtReceive);
            gbReceive.Location = new Point(13, 71);
            gbReceive.Margin = new Padding(4, 5, 4, 5);
            gbReceive.Name = "gbReceive";
            gbReceive.Padding = new Padding(4, 5, 4, 5);
            gbReceive.Size = new Size(979, 390);
            gbReceive.TabIndex = 4;
            gbReceive.TabStop = false;
            gbReceive.Text = "接收区：已接收0字节";
            // 
            // txtReceive
            // 
            txtReceive.ContextMenuStrip = menuReceive;
            txtReceive.Dock = DockStyle.Fill;
            txtReceive.HideSelection = false;
            txtReceive.Location = new Point(4, 21);
            txtReceive.Margin = new Padding(4, 5, 4, 5);
            txtReceive.Name = "txtReceive";
            txtReceive.Size = new Size(971, 364);
            txtReceive.TabIndex = 1;
            txtReceive.Text = "";
            // 
            // menuReceive
            // 
            menuReceive.ImageScalingSize = new Size(20, 20);
            menuReceive.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1, mi日志着色, toolStripMenuItem3, mi显示应用日志, mi显示网络日志, mi显示接收字符串, mi显示发送数据, mi显示接收数据, mi显示统计信息, toolStripMenuItem4, 查看Tcp参数ToolStripMenuItem, 设置最大TcpToolStripMenuItem, 分IP记录文本日志ToolStripMenuItem });
            menuReceive.Name = "menuSend";
            menuReceive.Size = new Size(181, 280);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(180, 22);
            toolStripMenuItem1.Text = "清空";
            toolStripMenuItem1.Click += mi清空_Click;
            // 
            // mi日志着色
            // 
            mi日志着色.Name = "mi日志着色";
            mi日志着色.Size = new Size(180, 22);
            mi日志着色.Text = "日志着色";
            mi日志着色.Click += mi日志着色_Click;
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new Size(177, 6);
            // 
            // mi显示应用日志
            // 
            mi显示应用日志.Name = "mi显示应用日志";
            mi显示应用日志.Size = new Size(180, 22);
            mi显示应用日志.Text = "显示应用日志";
            mi显示应用日志.Click += mi显示应用日志_Click;
            // 
            // mi显示网络日志
            // 
            mi显示网络日志.Name = "mi显示网络日志";
            mi显示网络日志.Size = new Size(180, 22);
            mi显示网络日志.Text = "显示网络日志";
            mi显示网络日志.Click += mi显示网络日志_Click;
            // 
            // mi显示接收字符串
            // 
            mi显示接收字符串.Name = "mi显示接收字符串";
            mi显示接收字符串.Size = new Size(180, 22);
            mi显示接收字符串.Text = "显示接收字符串";
            mi显示接收字符串.Click += mi显示接收字符串_Click;
            // 
            // mi显示发送数据
            // 
            mi显示发送数据.Name = "mi显示发送数据";
            mi显示发送数据.Size = new Size(180, 22);
            mi显示发送数据.Text = "显示发送数据";
            mi显示发送数据.Click += mi显示发送数据_Click;
            // 
            // mi显示接收数据
            // 
            mi显示接收数据.Name = "mi显示接收数据";
            mi显示接收数据.Size = new Size(180, 22);
            mi显示接收数据.Text = "显示接收数据";
            mi显示接收数据.Click += mi显示接收数据_Click;
            // 
            // mi显示统计信息
            // 
            mi显示统计信息.Name = "mi显示统计信息";
            mi显示统计信息.Size = new Size(180, 22);
            mi显示统计信息.Text = "显示统计信息";
            mi显示统计信息.Click += mi显示统计信息_Click;
            // 
            // toolStripMenuItem4
            // 
            toolStripMenuItem4.Name = "toolStripMenuItem4";
            toolStripMenuItem4.Size = new Size(177, 6);
            // 
            // 查看Tcp参数ToolStripMenuItem
            // 
            查看Tcp参数ToolStripMenuItem.Name = "查看Tcp参数ToolStripMenuItem";
            查看Tcp参数ToolStripMenuItem.Size = new Size(180, 22);
            查看Tcp参数ToolStripMenuItem.Text = "查看Tcp参数";
            查看Tcp参数ToolStripMenuItem.Click += 查看Tcp参数ToolStripMenuItem_Click;
            // 
            // 设置最大TcpToolStripMenuItem
            // 
            设置最大TcpToolStripMenuItem.Name = "设置最大TcpToolStripMenuItem";
            设置最大TcpToolStripMenuItem.Size = new Size(180, 22);
            设置最大TcpToolStripMenuItem.Text = "设置最大Tcp";
            设置最大TcpToolStripMenuItem.Click += 设置最大TcpToolStripMenuItem_Click;
            // 
            // menuSend
            // 
            menuSend.ImageScalingSize = new Size(20, 20);
            menuSend.Items.AddRange(new ToolStripItem[] { miHexSend, mi清空2 });
            menuSend.Name = "menuSend";
            menuSend.Size = new Size(123, 48);
            // 
            // miHexSend
            // 
            miHexSend.Name = "miHexSend";
            miHexSend.Size = new Size(122, 22);
            miHexSend.Text = "Hex发送";
            miHexSend.Click += miHex发送_Click;
            // 
            // mi清空2
            // 
            mi清空2.Name = "mi清空2";
            mi清空2.Size = new Size(122, 22);
            mi清空2.Text = "清空";
            mi清空2.Click += mi清空2_Click;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(880, 13);
            btnConnect.Margin = new Padding(4, 5, 4, 5);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(100, 48);
            btnConnect.TabIndex = 3;
            btnConnect.Text = "打开";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 300;
            timer1.Tick += timer1_Tick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 15);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(32, 17);
            label1.TabIndex = 6;
            label1.Text = "模式";
            // 
            // lbLocal
            // 
            lbLocal.AutoSize = true;
            lbLocal.Location = new Point(212, 15);
            lbLocal.Margin = new Padding(4, 0, 4, 0);
            lbLocal.Name = "lbLocal";
            lbLocal.Size = new Size(32, 17);
            lbLocal.TabIndex = 7;
            lbLocal.Text = "本地";
            // 
            // cbMode
            // 
            cbMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMode.FormattingEnabled = true;
            cbMode.Location = new Point(62, 8);
            cbMode.Margin = new Padding(4, 5, 4, 5);
            cbMode.Name = "cbMode";
            cbMode.Size = new Size(138, 25);
            cbMode.TabIndex = 9;
            cbMode.SelectedIndexChanged += cbMode_SelectedIndexChanged;
            // 
            // cbLocal
            // 
            cbLocal.DropDownStyle = ComboBoxStyle.DropDownList;
            cbLocal.FormattingEnabled = true;
            cbLocal.Location = new Point(266, 8);
            cbLocal.Margin = new Padding(4, 5, 4, 5);
            cbLocal.Name = "cbLocal";
            cbLocal.Size = new Size(172, 25);
            cbLocal.TabIndex = 10;
            // 
            // numPort
            // 
            numPort.Location = new Point(768, 8);
            numPort.Margin = new Padding(4, 5, 4, 5);
            numPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numPort.Name = "numPort";
            numPort.Size = new Size(76, 23);
            numPort.TabIndex = 11;
            numPort.TextAlign = HorizontalAlignment.Right;
            toolTip1.SetToolTip(numPort, "端口");
            numPort.Value = new decimal(new int[] { 8080, 0, 0, 0 });
            // 
            // pnlSetting
            // 
            pnlSetting.Controls.Add(cbRemote);
            pnlSetting.Controls.Add(label4);
            pnlSetting.Controls.Add(numPort);
            pnlSetting.Controls.Add(cbLocal);
            pnlSetting.Controls.Add(label1);
            pnlSetting.Controls.Add(lbLocal);
            pnlSetting.Controls.Add(cbMode);
            pnlSetting.Location = new Point(13, 11);
            pnlSetting.Margin = new Padding(4, 5, 4, 5);
            pnlSetting.Name = "pnlSetting";
            pnlSetting.Size = new Size(858, 52);
            pnlSetting.TabIndex = 13;
            // 
            // cbRemote
            // 
            cbRemote.FormattingEnabled = true;
            cbRemote.Location = new Point(504, 8);
            cbRemote.Margin = new Padding(4, 5, 4, 5);
            cbRemote.Name = "cbRemote";
            cbRemote.Size = new Size(252, 25);
            cbRemote.TabIndex = 17;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(450, 15);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(32, 17);
            label4.TabIndex = 16;
            label4.Text = "远程";
            // 
            // gbSend
            // 
            gbSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gbSend.Controls.Add(numThreads);
            gbSend.Controls.Add(numSleep);
            gbSend.Controls.Add(txtSend);
            gbSend.Controls.Add(btnSend);
            gbSend.Controls.Add(numMutilSend);
            gbSend.Controls.Add(label2);
            gbSend.Controls.Add(label7);
            gbSend.Location = new Point(13, 471);
            gbSend.Margin = new Padding(4, 5, 4, 5);
            gbSend.Name = "gbSend";
            gbSend.Padding = new Padding(4, 5, 4, 5);
            gbSend.Size = new Size(978, 147);
            gbSend.TabIndex = 15;
            gbSend.TabStop = false;
            gbSend.Text = "发送区：已发送0字节";
            // 
            // numThreads
            // 
            numThreads.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            numThreads.Location = new Point(890, 47);
            numThreads.Margin = new Padding(4, 5, 4, 5);
            numThreads.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numThreads.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numThreads.Name = "numThreads";
            numThreads.Size = new Size(78, 23);
            numThreads.TabIndex = 18;
            numThreads.TextAlign = HorizontalAlignment.Right;
            toolTip1.SetToolTip(numThreads, "模拟多客户端发送，用于压力测试！");
            numThreads.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // numSleep
            // 
            numSleep.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            numSleep.Location = new Point(807, 101);
            numSleep.Margin = new Padding(4, 5, 4, 5);
            numSleep.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            numSleep.Name = "numSleep";
            numSleep.Size = new Size(78, 23);
            numSleep.TabIndex = 16;
            numSleep.TextAlign = HorizontalAlignment.Right;
            numSleep.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            // 
            // txtSend
            // 
            txtSend.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtSend.ContextMenuStrip = menuSend;
            txtSend.HideSelection = false;
            txtSend.Location = new Point(4, 24);
            txtSend.Margin = new Padding(4, 5, 4, 5);
            txtSend.Name = "txtSend";
            txtSend.Size = new Size(744, 113);
            txtSend.TabIndex = 2;
            txtSend.Text = "";
            // 
            // btnSend
            // 
            btnSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSend.Location = new Point(894, 89);
            btnSend.Margin = new Padding(4, 5, 4, 5);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(75, 50);
            btnSend.TabIndex = 1;
            btnSend.Text = "发送";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // numMutilSend
            // 
            numMutilSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            numMutilSend.Location = new Point(807, 47);
            numMutilSend.Margin = new Padding(4, 5, 4, 5);
            numMutilSend.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numMutilSend.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numMutilSend.Name = "numMutilSend";
            numMutilSend.Size = new Size(78, 23);
            numMutilSend.TabIndex = 14;
            numMutilSend.TextAlign = HorizontalAlignment.Right;
            numMutilSend.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(754, 104);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(44, 17);
            label2.TabIndex = 17;
            label2.Text = "间隔：";
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label7.AutoSize = true;
            label7.Location = new Point(754, 50);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(44, 17);
            label7.TabIndex = 15;
            label7.Text = "次数：";
            // 
            // 分IP记录文本日志ToolStripMenuItem
            // 
            分IP记录文本日志ToolStripMenuItem.Name = "分IP记录文本日志ToolStripMenuItem";
            分IP记录文本日志ToolStripMenuItem.Size = new Size(180, 22);
            分IP记录文本日志ToolStripMenuItem.Text = "分IP记录文本日志";
            分IP记录文本日志ToolStripMenuItem.Click += 分IP记录文本日志ToolStripMenuItem_Click;
            // 
            // FrmMain
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1000, 632);
            Controls.Add(gbSend);
            Controls.Add(pnlSetting);
            Controls.Add(btnConnect);
            Controls.Add(gbReceive);
            Margin = new Padding(4, 5, 4, 5);
            Name = "FrmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "网络调试";
            Load += FrmMain_Load;
            gbReceive.ResumeLayout(false);
            menuReceive.ResumeLayout(false);
            menuSend.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numPort).EndInit();
            pnlSetting.ResumeLayout(false);
            pnlSetting.PerformLayout();
            gbSend.ResumeLayout(false);
            gbSend.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numThreads).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSleep).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMutilSend).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbReceive;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip menuSend;
        private System.Windows.Forms.ToolStripMenuItem mi清空2;
        private System.Windows.Forms.RichTextBox txtReceive;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbLocal;
        private System.Windows.Forms.ComboBox cbMode;
        private System.Windows.Forms.ComboBox cbLocal;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.Panel pnlSetting;
        private System.Windows.Forms.ContextMenuStrip menuReceive;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.GroupBox gbSend;
        private System.Windows.Forms.NumericUpDown numSleep;
        private System.Windows.Forms.RichTextBox txtSend;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.NumericUpDown numMutilSend;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ToolStripMenuItem mi显示发送数据;
        private System.Windows.Forms.ToolStripMenuItem mi显示接收数据;
        private System.Windows.Forms.ToolStripMenuItem mi显示统计信息;
        private System.Windows.Forms.NumericUpDown numThreads;
        private System.Windows.Forms.ToolStripMenuItem mi显示接收字符串;
        private System.Windows.Forms.ToolStripMenuItem mi显示应用日志;
        private System.Windows.Forms.ToolStripMenuItem mi显示网络日志;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem miHexSend;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem 查看Tcp参数ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 设置最大TcpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mi日志着色;
        private System.Windows.Forms.ComboBox cbRemote;
        private System.Windows.Forms.Label label4;
        private ToolStripMenuItem 分IP记录文本日志ToolStripMenuItem;
    }
}

