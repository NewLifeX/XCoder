namespace XCoder.Tools
{
    partial class FrmVoiceNotify
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            gbNet = new GroupBox();
            lnkFirewall = new LinkLabel();
            lblStatus = new Label();
            btnListen = new Button();
            cbAutoStart = new CheckBox();
            numPort = new NumericUpDown();
            lblPort = new Label();
            gbTts = new GroupBox();
            numRate = new NumericUpDown();
            lblRate = new Label();
            numVolume = new NumericUpDown();
            lblVolume = new Label();
            cbVoices = new ComboBox();
            lblVoice = new Label();
            lvMessages = new ListView();
            colTime = new ColumnHeader();
            colSource = new ColumnHeader();
            colText = new ColumnHeader();
            txtTest = new TextBox();
            btnTest = new Button();
            btnClear = new Button();
            lblCount = new Label();
            gbNet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numPort).BeginInit();
            gbTts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numRate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numVolume).BeginInit();
            SuspendLayout();
            // 
            // gbNet
            // 
            gbNet.Controls.Add(lnkFirewall);
            gbNet.Controls.Add(lblStatus);
            gbNet.Controls.Add(btnListen);
            gbNet.Controls.Add(cbAutoStart);
            gbNet.Controls.Add(numPort);
            gbNet.Controls.Add(lblPort);
            gbNet.Location = new Point(12, 12);
            gbNet.Name = "gbNet";
            gbNet.Size = new Size(520, 80);
            gbNet.TabIndex = 0;
            gbNet.TabStop = false;
            gbNet.Text = "UDP 监听（组播 239.255.42.99）";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(400, 32);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(50, 20);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "已停止";
            // 
            // lnkFirewall
            // 
            lnkFirewall.AutoSize = true;
            lnkFirewall.Location = new Point(400, 54);
            lnkFirewall.Name = "lnkFirewall";
            lnkFirewall.Size = new Size(110, 20);
            lnkFirewall.TabIndex = 5;
            lnkFirewall.TabStop = true;
            lnkFirewall.Text = "收不到？放行防火墙";
            lnkFirewall.LinkClicked += lnkFirewall_LinkClicked;
            // 
            // btnListen
            // 
            btnListen.Location = new Point(280, 22);
            btnListen.Name = "btnListen";
            btnListen.Size = new Size(110, 36);
            btnListen.TabIndex = 3;
            btnListen.Text = "开始监听";
            btnListen.UseVisualStyleBackColor = true;
            btnListen.Click += btnListen_Click;
            // 
            // cbAutoStart
            // 
            cbAutoStart.AutoSize = true;
            cbAutoStart.Checked = true;
            cbAutoStart.CheckState = CheckState.Checked;
            cbAutoStart.Location = new Point(152, 30);
            cbAutoStart.Name = "cbAutoStart";
            cbAutoStart.Size = new Size(106, 24);
            cbAutoStart.TabIndex = 2;
            cbAutoStart.Text = "打开即监听";
            cbAutoStart.UseVisualStyleBackColor = true;
            // 
            // numPort
            // 
            numPort.Location = new Point(60, 28);
            numPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numPort.Name = "numPort";
            numPort.Size = new Size(80, 27);
            numPort.TabIndex = 1;
            numPort.TextAlign = HorizontalAlignment.Right;
            numPort.Value = new decimal(new int[] { 5502, 0, 0, 0 });
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Location = new Point(12, 32);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(50, 20);
            lblPort.TabIndex = 0;
            lblPort.Text = "端口：";
            // 
            // gbTts
            // 
            gbTts.Controls.Add(numRate);
            gbTts.Controls.Add(lblRate);
            gbTts.Controls.Add(numVolume);
            gbTts.Controls.Add(lblVolume);
            gbTts.Controls.Add(cbVoices);
            gbTts.Controls.Add(lblVoice);
            gbTts.Location = new Point(540, 12);
            gbTts.Name = "gbTts";
            gbTts.Size = new Size(469, 80);
            gbTts.TabIndex = 1;
            gbTts.TabStop = false;
            gbTts.Text = "TTS";
            // 
            // numRate
            // 
            numRate.Location = new Point(412, 28);
            numRate.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numRate.Minimum = new decimal(new int[] { 10, 0, 0, Int32.MinValue });
            numRate.Name = "numRate";
            numRate.Size = new Size(50, 27);
            numRate.TabIndex = 5;
            numRate.TextAlign = HorizontalAlignment.Right;
            // 
            // lblRate
            // 
            lblRate.AutoSize = true;
            lblRate.Location = new Point(366, 32);
            lblRate.Name = "lblRate";
            lblRate.Size = new Size(50, 20);
            lblRate.TabIndex = 4;
            lblRate.Text = "语速：";
            // 
            // numVolume
            // 
            numVolume.Location = new Point(306, 28);
            numVolume.Name = "numVolume";
            numVolume.Size = new Size(50, 27);
            numVolume.TabIndex = 3;
            numVolume.TextAlign = HorizontalAlignment.Right;
            numVolume.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // lblVolume
            // 
            lblVolume.AutoSize = true;
            lblVolume.Location = new Point(260, 32);
            lblVolume.Name = "lblVolume";
            lblVolume.Size = new Size(50, 20);
            lblVolume.TabIndex = 2;
            lblVolume.Text = "音量：";
            // 
            // cbVoices
            // 
            cbVoices.DropDownStyle = ComboBoxStyle.DropDownList;
            cbVoices.FormattingEnabled = true;
            cbVoices.Location = new Point(56, 28);
            cbVoices.Name = "cbVoices";
            cbVoices.Size = new Size(196, 28);
            cbVoices.TabIndex = 1;
            // 
            // lblVoice
            // 
            lblVoice.AutoSize = true;
            lblVoice.Location = new Point(10, 32);
            lblVoice.Name = "lblVoice";
            lblVoice.Size = new Size(50, 20);
            lblVoice.TabIndex = 0;
            lblVoice.Text = "声音：";
            // 
            // lvMessages
            // 
            lvMessages.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvMessages.Columns.AddRange(new ColumnHeader[] { colTime, colSource, colText });
            lvMessages.FullRowSelect = true;
            lvMessages.GridLines = true;
            lvMessages.Location = new Point(12, 100);
            lvMessages.Name = "lvMessages";
            lvMessages.Size = new Size(997, 420);
            lvMessages.TabIndex = 2;
            lvMessages.UseCompatibleStateImageBehavior = false;
            lvMessages.View = View.Details;
            // 
            // colTime
            // 
            colTime.Text = "时间";
            colTime.Width = 90;
            // 
            // colSource
            // 
            colSource.Text = "来源";
            colSource.Width = 160;
            // 
            // colText
            // 
            colText.Text = "内容";
            colText.Width = 720;
            // 
            // txtTest
            // 
            txtTest.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtTest.Location = new Point(12, 532);
            txtTest.Name = "txtTest";
            txtTest.PlaceholderText = "输入文字，点“试播”在本机朗读";
            txtTest.Size = new Size(720, 27);
            txtTest.TabIndex = 3;
            // 
            // btnTest
            // 
            btnTest.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnTest.Location = new Point(740, 528);
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(90, 36);
            btnTest.TabIndex = 4;
            btnTest.Text = "试播";
            btnTest.UseVisualStyleBackColor = true;
            btnTest.Click += btnTest_Click;
            // 
            // btnClear
            // 
            btnClear.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClear.Location = new Point(838, 528);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(80, 36);
            btnClear.TabIndex = 5;
            btnClear.Text = "清空";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // lblCount
            // 
            lblCount.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblCount.Location = new Point(924, 536);
            lblCount.Name = "lblCount";
            lblCount.Size = new Size(85, 20);
            lblCount.TabIndex = 6;
            lblCount.Text = "共 0 条";
            lblCount.TextAlign = ContentAlignment.MiddleRight;
            // 
            // FrmVoiceNotify
            // 
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1021, 583);
            Controls.Add(lblCount);
            Controls.Add(btnClear);
            Controls.Add(btnTest);
            Controls.Add(txtTest);
            Controls.Add(lvMessages);
            Controls.Add(gbTts);
            Controls.Add(gbNet);
            MinimumSize = new Size(820, 480);
            Name = "FrmVoiceNotify";
            Text = "网络语音播报";
            FormClosing += FrmVoiceNotify_FormClosing;
            Load += FrmVoiceNotify_Load;
            gbNet.ResumeLayout(false);
            gbNet.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numPort).EndInit();
            gbTts.ResumeLayout(false);
            gbTts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numRate).EndInit();
            ((System.ComponentModel.ISupportInitialize)numVolume).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox gbNet;
        private Label lblPort;
        private NumericUpDown numPort;
        private CheckBox cbAutoStart;
        private Button btnListen;
        private Label lblStatus;
        private LinkLabel lnkFirewall;
        private GroupBox gbTts;
        private Label lblVoice;
        private ComboBox cbVoices;
        private Label lblVolume;
        private NumericUpDown numVolume;
        private Label lblRate;
        private NumericUpDown numRate;
        private ListView lvMessages;
        private ColumnHeader colTime;
        private ColumnHeader colSource;
        private ColumnHeader colText;
        private TextBox txtTest;
        private Button btnTest;
        private Button btnClear;
        private Label lblCount;
    }
}
