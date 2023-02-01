namespace XNet
{
    partial class FrmMqtt
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
            this.menuReceive = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi日志着色 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSend = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mi清空2 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnConnect = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.txtPass = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbRemote = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.gbSend = new System.Windows.Forms.GroupBox();
            this.cbRetain = new System.Windows.Forms.CheckBox();
            this.cbQos2 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cbTopic2 = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtSend = new System.Windows.Forms.RichTextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtClientId = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cbClearSession = new System.Windows.Forms.CheckBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.gbSubscribe = new System.Windows.Forms.GroupBox();
            this.cbQos = new System.Windows.Forms.ComboBox();
            this.btnSubscribe = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.cbTopic = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.gbSetting = new System.Windows.Forms.GroupBox();
            this.gbReceive.SuspendLayout();
            this.menuReceive.SuspendLayout();
            this.menuSend.SuspendLayout();
            this.gbSend.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.gbSubscribe.SuspendLayout();
            this.gbSetting.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbReceive
            // 
            this.gbReceive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbReceive.Controls.Add(this.txtReceive);
            this.gbReceive.Location = new System.Drawing.Point(14, 224);
            this.gbReceive.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbReceive.Name = "gbReceive";
            this.gbReceive.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbReceive.Size = new System.Drawing.Size(1088, 406);
            this.gbReceive.TabIndex = 4;
            this.gbReceive.TabStop = false;
            this.gbReceive.Text = "消息";
            // 
            // txtReceive
            // 
            this.txtReceive.ContextMenuStrip = this.menuReceive;
            this.txtReceive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtReceive.HideSelection = false;
            this.txtReceive.Location = new System.Drawing.Point(4, 23);
            this.txtReceive.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtReceive.Name = "txtReceive";
            this.txtReceive.Size = new System.Drawing.Size(1080, 380);
            this.txtReceive.TabIndex = 1;
            this.txtReceive.Text = "";
            // 
            // menuReceive
            // 
            this.menuReceive.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuReceive.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.mi日志着色,
            this.toolStripMenuItem3});
            this.menuReceive.Name = "menuSend";
            this.menuReceive.Size = new System.Drawing.Size(139, 58);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(138, 24);
            this.toolStripMenuItem1.Text = "清空";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.mi清空_Click);
            // 
            // mi日志着色
            // 
            this.mi日志着色.Name = "mi日志着色";
            this.mi日志着色.Size = new System.Drawing.Size(138, 24);
            this.mi日志着色.Text = "日志着色";
            this.mi日志着色.Click += new System.EventHandler(this.mi日志着色_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(135, 6);
            // 
            // menuSend
            // 
            this.menuSend.ImageScalingSize = new System.Drawing.Size(24, 24);
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
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnConnect.Location = new System.Drawing.Point(978, 47);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(111, 49);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "连接";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // txtPass
            // 
            this.txtPass.Location = new System.Drawing.Point(704, 33);
            this.txtPass.Name = "txtPass";
            this.txtPass.Size = new System.Drawing.Size(204, 27);
            this.txtPass.TabIndex = 21;
            this.txtPass.Text = "public";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(641, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 17);
            this.label3.TabIndex = 20;
            this.label3.Text = "密码：";
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(479, 33);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(155, 27);
            this.txtUser.TabIndex = 19;
            this.txtUser.Text = "admin";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(386, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 17);
            this.label1.TabIndex = 18;
            this.label1.Text = "用户名：";
            // 
            // cbRemote
            // 
            this.cbRemote.FormattingEnabled = true;
            this.cbRemote.Location = new System.Drawing.Point(74, 34);
            this.cbRemote.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbRemote.Name = "cbRemote";
            this.cbRemote.Size = new System.Drawing.Size(280, 25);
            this.cbRemote.TabIndex = 17;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 39);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 17);
            this.label4.TabIndex = 16;
            this.label4.Text = "主机：";
            // 
            // gbSend
            // 
            this.gbSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSend.Controls.Add(this.cbRetain);
            this.gbSend.Controls.Add(this.cbQos2);
            this.gbSend.Controls.Add(this.label7);
            this.gbSend.Controls.Add(this.cbTopic2);
            this.gbSend.Controls.Add(this.label8);
            this.gbSend.Controls.Add(this.txtSend);
            this.gbSend.Controls.Add(this.btnSend);
            this.gbSend.Enabled = false;
            this.gbSend.Location = new System.Drawing.Point(14, 639);
            this.gbSend.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbSend.Name = "gbSend";
            this.gbSend.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbSend.Size = new System.Drawing.Size(1088, 210);
            this.gbSend.TabIndex = 15;
            this.gbSend.TabStop = false;
            this.gbSend.Text = "发布";
            // 
            // cbRetain
            // 
            this.cbRetain.AutoSize = true;
            this.cbRetain.Location = new System.Drawing.Point(781, 26);
            this.cbRetain.Name = "cbRetain";
            this.cbRetain.Size = new System.Drawing.Size(64, 21);
            this.cbRetain.TabIndex = 29;
            this.cbRetain.Text = "保留";
            this.cbRetain.UseVisualStyleBackColor = true;
            // 
            // cbQos2
            // 
            this.cbQos2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbQos2.FormattingEnabled = true;
            this.cbQos2.Items.AddRange(new object[] {
            "0",
            "1",
            "2"});
            this.cbQos2.Location = new System.Drawing.Point(667, 24);
            this.cbQos2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbQos2.Name = "cbQos2";
            this.cbQos2.Size = new System.Drawing.Size(81, 25);
            this.cbQos2.TabIndex = 31;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(550, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 17);
            this.label7.TabIndex = 30;
            this.label7.Text = "服务质量：";
            // 
            // cbTopic2
            // 
            this.cbTopic2.FormattingEnabled = true;
            this.cbTopic2.Location = new System.Drawing.Point(76, 24);
            this.cbTopic2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbTopic2.Name = "cbTopic2";
            this.cbTopic2.Size = new System.Drawing.Size(453, 25);
            this.cbTopic2.TabIndex = 29;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 28);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 17);
            this.label8.TabIndex = 28;
            this.label8.Text = "主题：";
            // 
            // txtSend
            // 
            this.txtSend.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSend.ContextMenuStrip = this.menuSend;
            this.txtSend.HideSelection = false;
            this.txtSend.Location = new System.Drawing.Point(4, 73);
            this.txtSend.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtSend.Name = "txtSend";
            this.txtSend.Size = new System.Drawing.Size(1004, 123);
            this.txtSend.TabIndex = 2;
            this.txtSend.Text = "";
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSend.Location = new System.Drawing.Point(1018, 73);
            this.btnSend.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(57, 128);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "发送";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(74, 77);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(111, 27);
            this.txtPath.TabIndex = 23;
            this.txtPath.Text = "/mqtt";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 17);
            this.label2.TabIndex = 22;
            this.label2.Text = "路径：";
            // 
            // txtClientId
            // 
            this.txtClientId.Location = new System.Drawing.Point(479, 77);
            this.txtClientId.Name = "txtClientId";
            this.txtClientId.Size = new System.Drawing.Size(155, 27);
            this.txtClientId.TabIndex = 25;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(366, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 17);
            this.label5.TabIndex = 24;
            this.label5.Text = "客户端ID：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(641, 81);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 17);
            this.label6.TabIndex = 20;
            this.label6.Text = "心跳：";
            // 
            // cbClearSession
            // 
            this.cbClearSession.AutoSize = true;
            this.cbClearSession.Location = new System.Drawing.Point(211, 79);
            this.cbClearSession.Name = "cbClearSession";
            this.cbClearSession.Size = new System.Drawing.Size(98, 21);
            this.cbClearSession.TabIndex = 27;
            this.cbClearSession.Text = "清除会话";
            this.cbClearSession.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(704, 77);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(84, 27);
            this.numericUpDown1.TabIndex = 28;
            this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDown1.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // gbSubscribe
            // 
            this.gbSubscribe.Controls.Add(this.cbQos);
            this.gbSubscribe.Controls.Add(this.btnSubscribe);
            this.gbSubscribe.Controls.Add(this.label11);
            this.gbSubscribe.Controls.Add(this.cbTopic);
            this.gbSubscribe.Controls.Add(this.label12);
            this.gbSubscribe.Enabled = false;
            this.gbSubscribe.Location = new System.Drawing.Point(16, 136);
            this.gbSubscribe.Name = "gbSubscribe";
            this.gbSubscribe.Size = new System.Drawing.Size(924, 87);
            this.gbSubscribe.TabIndex = 2;
            this.gbSubscribe.TabStop = false;
            this.gbSubscribe.Text = "订阅";
            // 
            // cbQos
            // 
            this.cbQos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbQos.FormattingEnabled = true;
            this.cbQos.Items.AddRange(new object[] {
            "0",
            "1",
            "2"});
            this.cbQos.Location = new System.Drawing.Point(666, 41);
            this.cbQos.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbQos.Name = "cbQos";
            this.cbQos.Size = new System.Drawing.Size(81, 25);
            this.cbQos.TabIndex = 27;
            // 
            // btnSubscribe
            // 
            this.btnSubscribe.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSubscribe.Location = new System.Drawing.Point(780, 31);
            this.btnSubscribe.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSubscribe.Name = "btnSubscribe";
            this.btnSubscribe.Size = new System.Drawing.Size(111, 49);
            this.btnSubscribe.TabIndex = 26;
            this.btnSubscribe.Text = "订阅";
            this.btnSubscribe.UseVisualStyleBackColor = true;
            this.btnSubscribe.Click += new System.EventHandler(this.btnSubscribe_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(549, 46);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(93, 17);
            this.label11.TabIndex = 25;
            this.label11.Text = "服务质量：";
            // 
            // cbTopic
            // 
            this.cbTopic.FormattingEnabled = true;
            this.cbTopic.Location = new System.Drawing.Point(74, 41);
            this.cbTopic.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbTopic.Name = "cbTopic";
            this.cbTopic.Size = new System.Drawing.Size(453, 25);
            this.cbTopic.TabIndex = 24;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(14, 46);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(59, 17);
            this.label12.TabIndex = 23;
            this.label12.Text = "主题：";
            // 
            // gbSetting
            // 
            this.gbSetting.Controls.Add(this.txtPass);
            this.gbSetting.Controls.Add(this.cbRemote);
            this.gbSetting.Controls.Add(this.numericUpDown1);
            this.gbSetting.Controls.Add(this.cbClearSession);
            this.gbSetting.Controls.Add(this.label4);
            this.gbSetting.Controls.Add(this.label6);
            this.gbSetting.Controls.Add(this.txtClientId);
            this.gbSetting.Controls.Add(this.label1);
            this.gbSetting.Controls.Add(this.label5);
            this.gbSetting.Controls.Add(this.txtUser);
            this.gbSetting.Controls.Add(this.txtPath);
            this.gbSetting.Controls.Add(this.label3);
            this.gbSetting.Controls.Add(this.label2);
            this.gbSetting.Location = new System.Drawing.Point(14, 10);
            this.gbSetting.Name = "gbSetting";
            this.gbSetting.Size = new System.Drawing.Size(923, 117);
            this.gbSetting.TabIndex = 3;
            this.gbSetting.TabStop = false;
            this.gbSetting.Text = "连接";
            // 
            // FrmMqtt
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1111, 864);
            this.Controls.Add(this.gbSubscribe);
            this.Controls.Add(this.gbSetting);
            this.Controls.Add(this.gbSend);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.gbReceive);
            this.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "FrmMqtt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MQTT客户端";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.gbReceive.ResumeLayout(false);
            this.menuReceive.ResumeLayout(false);
            this.menuSend.ResumeLayout(false);
            this.gbSend.ResumeLayout(false);
            this.gbSend.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.gbSubscribe.ResumeLayout(false);
            this.gbSubscribe.PerformLayout();
            this.gbSetting.ResumeLayout(false);
            this.gbSetting.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbReceive;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip menuSend;
        private System.Windows.Forms.ToolStripMenuItem mi清空2;
        private System.Windows.Forms.RichTextBox txtReceive;
        private System.Windows.Forms.ContextMenuStrip menuReceive;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.GroupBox gbSend;
        private System.Windows.Forms.RichTextBox txtSend;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem mi日志着色;
        private System.Windows.Forms.ComboBox cbRemote;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtClientId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbClearSession;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.GroupBox gbSubscribe;
        private System.Windows.Forms.ComboBox cbQos;
        private System.Windows.Forms.Button btnSubscribe;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cbTopic;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox gbSetting;
        private System.Windows.Forms.CheckBox cbRetain;
        private System.Windows.Forms.ComboBox cbQos2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbTopic2;
        private System.Windows.Forms.Label label8;
    }
}

