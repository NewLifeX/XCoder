using Gtk;
namespace XApi
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gbReceive = new Frame();
            this.txtReceive = new TextView();
            //this.menuReceive = new ContextMenuStrip(this.components);
            //this.toolStripMenuItem1 = new ToolStripMenuItem();
            //this.mi日志着色 = new ToolStripMenuItem();
            //this.toolStripMenuItem3 = new ToolStripSeparator();
            //this.mi显示应用日志 = new ToolStripMenuItem();
            //this.mi显示编码日志 = new ToolStripMenuItem();
            //this.mi显示发送数据 = new ToolStripMenuItem();
            //this.mi显示接收数据 = new ToolStripMenuItem();
            //this.mi显示统计信息 = new ToolStripMenuItem();
            //this.menuSend = new ContextMenuStrip(this.components);
            //this.mi清空2 = new ToolStripMenuItem();
            this.btnConnect = new Button();
            //this.timer1 = new Timer(this.components);
            //this.fontDialog1 = new FontDialog();
            //this.colorDialog1 = new ColorDialog();
            this.label1 = new Label();
            this.lbAddr = new Label();
            this.cbMode = new ComboBox(new[] { "服务端", "客户端" });
            this.cbAddr = ComboBoxText.NewWithEntry();
            this.pnlSetting = new HBox();
            this.numPort = new SpinButton(1, 63353, 1);
            this.label5 = new Label();
            this.cbAction = new ComboBoxText();
            this.gbSend = new Frame();
            this.boxSend = new VBox();
            this.boxSendSetting = new HBox();
            this.numThreads = new SpinButton(1, 100000, 1);
            this.numSleep = new SpinButton(1000, 1000000, 1);
            this.txtSend = new TextView();
            this.btnSend = new Button();
            this.numMutilSend = new SpinButton(1, 1000000, 1);
            this.label2 = new Label();
            this.label7 = new Label();
            this.label8 = new Label();
            //this.toolTip1 = new ToolTip(this.components);
            this.label3 = new Label();

            //this.menuReceive.SuspendLayout();
            //this.menuSend.SuspendLayout();
            //this.SuspendLayout();
            //
            // gbReceive
            //
            this.gbReceive.Add(this.txtReceive);
            this.gbReceive.Margin = 6;
            this.gbReceive.Name = "gbReceive";
            this.gbReceive.SetSizeRequest(878, 298);
            // gbReceive.Margin = 10;
            gbReceive.Label = "接收区：已接收0字节";
            gbReceive.Halign = Align.Fill;
            gbReceive.Valign = Align.Fill;
            gbReceive.ShadowType = ShadowType.Out;
            //
            // txtReceive
            //
            // this.txtReceive.ContextMenuStrip = this.menuReceive;
            // this.txtReceive.Dock = DockStyle.Fill;
            // this.txtReceive.HideSelection = false;
            // this.txtReceive.Location = new System.Drawing.Point(4, 25);
            this.txtReceive.Margin = 4;
            this.txtReceive.Name = "txtReceive";
            // this.txtReceive.Size = new System.Drawing.Size(970, 269);
            var buffer = this.txtReceive.Buffer;
            buffer.Text = "各就各位！";
            this.txtReceive.Editable = false;
            ////
            //// menuReceive
            ////
            //this.menuReceive.ImageScalingSize = new System.Drawing.Size(32, 32);
            //this.menuReceive.Items.AddRange(new ToolStripItem[] {
            //this.toolStripMenuItem1,
            //this.mi日志着色,
            //this.toolStripMenuItem3,
            //this.mi显示应用日志,
            //this.mi显示编码日志,
            //this.mi显示发送数据,
            //this.mi显示接收数据,
            //this.mi显示统计信息});
            //this.menuReceive.Name = "menuSend";
            //this.menuReceive.Size = new System.Drawing.Size(189, 206);
            ////
            //// toolStripMenuItem1
            ////
            //this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            //this.toolStripMenuItem1.Size = new System.Drawing.Size(188, 28);
            //this.toolStripMenuItem1.Text = "清空";
            //this.toolStripMenuItem1.Click += new System.EventHandler(this.mi清空_Click);
            ////
            //// mi日志着色
            ////
            //this.mi日志着色.Name = "mi日志着色";
            //this.mi日志着色.Size = new System.Drawing.Size(188, 28);
            //this.mi日志着色.Text = "日志着色";
            //this.mi日志着色.Click += new System.EventHandler(this.miCheck_Click);
            ////
            //// toolStripMenuItem3
            ////
            //this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            //this.toolStripMenuItem3.Size = new System.Drawing.Size(185, 6);
            ////
            //// mi显示应用日志
            ////
            //this.mi显示应用日志.Name = "mi显示应用日志";
            //this.mi显示应用日志.Size = new System.Drawing.Size(188, 28);
            //this.mi显示应用日志.Text = "显示应用日志";
            //this.mi显示应用日志.Click += new System.EventHandler(this.miCheck_Click);
            ////
            //// mi显示编码日志
            ////
            //this.mi显示编码日志.Name = "mi显示编码日志";
            //this.mi显示编码日志.Size = new System.Drawing.Size(188, 28);
            //this.mi显示编码日志.Text = "显示编码日志";
            //this.mi显示编码日志.Click += new System.EventHandler(this.miCheck_Click);
            ////
            //// mi显示发送数据
            ////
            //this.mi显示发送数据.Name = "mi显示发送数据";
            //this.mi显示发送数据.Size = new System.Drawing.Size(188, 28);
            //this.mi显示发送数据.Text = "显示发送数据";
            //this.mi显示发送数据.Click += new System.EventHandler(this.miCheck_Click);
            ////
            //// mi显示接收数据
            ////
            //this.mi显示接收数据.Name = "mi显示接收数据";
            //this.mi显示接收数据.Size = new System.Drawing.Size(188, 28);
            //this.mi显示接收数据.Text = "显示接收数据";
            //this.mi显示接收数据.Click += new System.EventHandler(this.miCheck_Click);
            ////
            //// mi显示统计信息
            ////
            //this.mi显示统计信息.Name = "mi显示统计信息";
            //this.mi显示统计信息.Size = new System.Drawing.Size(188, 28);
            //this.mi显示统计信息.Text = "显示统计信息";
            //this.mi显示统计信息.Click += new System.EventHandler(this.miCheck_Click);
            ////
            //// menuSend
            ////
            //this.menuSend.ImageScalingSize = new System.Drawing.Size(32, 32);
            //this.menuSend.Items.AddRange(new ToolStripItem[] {
            //this.mi清空2});
            //this.menuSend.Name = "menuSend";
            //this.menuSend.Size = new System.Drawing.Size(117, 32);
            ////
            //// mi清空2
            ////
            //this.mi清空2.Name = "mi清空2";
            //this.mi清空2.Size = new System.Drawing.Size(116, 28);
            //this.mi清空2.Text = "清空";
            //this.mi清空2.Click += new System.EventHandler(this.mi清空2_Click);
            //
            // btnConnect
            //
            // this.btnConnect.Location = new System.Drawing.Point(748, 12);
            this.btnConnect.Margin = 4;
            this.btnConnect.Name = "btnConnect";
            // this.btnConnect.Size = new System.Drawing.Size(100, 44);
            // this.btnConnect.TabIndex = 3;
            this.btnConnect.Label = "打开";
            this.btnConnect.Clicked += new System.EventHandler(this.btnConnect_Click);
            ////
            //// timer1
            ////
            //this.timer1.Enabled = true;
            //this.timer1.Interval = 300;
            //this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            //
            // label1
            //
            //this.label1.AutoSize = true;
            this.label1.Xalign = 0.8F;
            this.label1.Yalign = 0.5F;
            this.label1.MarginTop = 4;
            this.label1.MarginBottom = 4;
            this.label1.Name = "label1";
            this.label1.WidthRequest = 62;
            this.label1.HeightRequest = 18;
            this.label1.Text = "模式：";
            //
            // lbAddr
            //
            // this.lbAddr.AutoSize = true;
            // this.lbAddr.Location = new System.Drawing.Point(374, 14);
            this.lbAddr.Xalign = 0.8F;
            this.lbAddr.Yalign = 0.5F;
            this.lbAddr.MarginTop = 4;
            this.lbAddr.MarginBottom = 4;
            this.lbAddr.Name = "lbAddr";
            this.lbAddr.WidthRequest = 62;
            this.lbAddr.HeightRequest = 18;
            this.lbAddr.Text = "地址：";
            //
            // cbMode
            //
            //this.cbMode.DropDownStyle = ComboBoxStyle.DropDownList;
            //this.cbMode.FormattingEnabled = true;
            //this.cbMode.Location = new System.Drawing.Point(72, 11);
            //this.cbMode.Margin = 4;
            this.cbMode.Active = 1;
            this.cbMode.Name = "cbMode";
            //this.cbMode.SetSizeRequest(1, 1);
            this.cbMode.Changed += new System.EventHandler(this.cbMode_SelectedIndexChanged);
            //
            // cbAddr
            //
            //this.cbAddr.Margin = 4;
            this.cbAddr.Name = "cbAddr";
            this.cbAddr.SetSizeRequest(200, 18);
            // this.cbAddr.Size = new System.Drawing.Size(247, 26);
            this.cbAddr.Sensitive = true;
            //this.cbAddr.
            //
            // pnlSetting
            //
            this.pnlSetting.PackStart(this.label1, false, false, 2);
            this.pnlSetting.PackStart(this.cbMode, false, false, 2);
            this.pnlSetting.PackStart(this.label5, false, false, 2);
            this.pnlSetting.PackStart(this.numPort, false, false, 2);
            this.pnlSetting.PackStart(this.lbAddr, false, false, 2);
            this.pnlSetting.PackStart(this.cbAddr, false, false, 2);
            this.pnlSetting.PackEnd(this.btnConnect, false, false, 2);

            //this.pnlSetting.Add();
            //this.pnlSetting.Position = 5;// .Location = new System.Drawing.Point(14, 12);
            this.pnlSetting.Margin = 4;
            this.pnlSetting.Name = "pnlSetting";
            // this.pnlSetting.Valign = Align.Fill;
            // this.pnlSetting.Halign = Align.Fill;
            // this.pnlSetting.SetSizeRequest(708, 20);
            // pnlSetting.Orientation = Orientation.Horizontal;
            //this.pnlSetting.Size = new System.Drawing.Size(708, 46);
            //this.pnlSetting.TabIndex = 13;
            //
            // numPort
            //
            // this.numPort.Location = new System.Drawing.Point(270, 10);
            this.numPort.Margin = 4;
            this.numPort.Name = "numPort";
            this.numPort.Value = 777;
            this.numPort.SetSizeRequest(94, 28);
            //
            // label5
            //
            //this.label5.AutoSize = true;
            //this.label5.Location = new System.Drawing.Point(218, 14);
            this.label5.Xalign = 0.8F;
            this.label5.Yalign = 0.5F;
            this.label5.MarginTop = 4;
            this.label5.MarginBottom = 4;
            this.label5.Name = "label5";
            this.label5.SetSizeRequest(62, 18);
            this.label5.Text = "端口：";
            //
            // cbAction
            //
            this.cbAction.Margin = 4;
            this.cbAction.Name = "cbAction";
            //this.cbAction.Size = new System.Drawing.Size(470, 26);
            this.cbAction.Visible = false;
            this.cbAction.Changed += new System.EventHandler(this.cbAction_SelectedIndexChanged);
            //
            // gbSend
            //
            this.gbSend.Add(this.boxSend);
            // this.gbSend.Controls.Add(this.numThreads);
            // this.gbSend.Controls.Add(this.numSleep);
            // this.gbSend.Controls.Add(this.btnSend);
            // this.gbSend.Controls.Add(this.numMutilSend);
            // this.gbSend.Controls.Add(this.label2);
            // this.gbSend.Controls.Add(this.label7);
            // this.gbSend.Location = new System.Drawing.Point(14, 424);
            this.gbSend.Margin = 4;
            this.gbSend.Name = "gbSend";
            this.gbSend.SetSizeRequest(778, 126);
            this.gbSend.Label = "发送区：已发送0字节";
            //
            // boxSend
            //
            this.boxSend.PackStart(this.txtSend, false, false, 2);
            this.boxSend.PackStart(this.boxSendSetting, false, false, 2);
            // this.boxSend.Orientation = Orientation.Vertical;
            this.boxSend.Margin = 4;
            this.boxSend.Name = "boxSend";
            //
            // boxSendSetting
            //
            this.boxSendSetting.PackStart(this.label7, false, false, 2);
            this.boxSendSetting.PackStart(this.numMutilSend, false, false, 2);
            this.boxSendSetting.PackStart(this.label8, false, false, 2);
            this.boxSendSetting.PackStart(this.numThreads, false, false, 2);
            this.boxSendSetting.PackStart(this.label2, false, false, 2);
            this.boxSendSetting.PackStart(this.numSleep, false, false, 2);
            this.boxSendSetting.PackStart(this.btnSend, false, false, 2);
            this.boxSendSetting.Margin = 4;
            this.boxSendSetting.Name = "sendSetting";
            //
            // numThreads
            //
            this.numThreads.Margin = 4;
            this.numThreads.Name = "numThreads";
            this.numThreads.SetSizeRequest(78, 28);
            // this.toolTip1.SetToolTip(this.numThreads, "模拟多客户端发送，用于压力测试！");
            //
            // numSleep
            //
            this.numSleep.Margin = 4;
            this.numSleep.Name = "numSleep";
            this.numSleep.SetSizeRequest(109, 28);
            //
            // txtSend
            //
            // this.txtSend.ContextMenuStrip = this.menuSend;
            // this.txtSend.Location = new System.Drawing.Point(0, 28);
            this.txtSend.Margin = 4;
            this.txtSend.Name = "txtSend";
            this.txtSend.SetSizeRequest(621, 86);
            this.txtSend.WrapMode = WrapMode.Word;
            //this.txtSend.Editable = false;
            //
            // btnSend
            //
            this.btnSend.Margin = 4;
            this.btnSend.Name = "btnSend";
            this.btnSend.SetSizeRequest(75, 45);
            this.btnSend.Label = "发送";
            this.btnSend.Clicked += new System.EventHandler(this.btnSend_Click);
            //
            // numMutilSend
            //
            this.numMutilSend.Margin = 4;
            this.numMutilSend.Name = "numMutilSend";
            this.numMutilSend.SetSizeRequest(109, 28);
            //
            // label2
            //
            // this.label2.Location = new System.Drawing.Point(729, 87);
            this.label2.Margin = 4;
            this.label2.Name = "label2";
            this.label2.SetSizeRequest(62, 18);
            this.label2.Text = "间隔：";
            //
            // label7
            //
            // this.label7.Location = new System.Drawing.Point(729, 39);
            this.label7.Margin = 4;
            this.label7.Name = "label7";
            this.label7.SetSizeRequest(62, 18);
            this.label7.Text = "次数：";
            //
            // label8
            //
            this.label8.Margin = 4;
            this.label8.Name = "label8";
            this.label8.SetSizeRequest(62, 18);
            this.label8.Text = "线程：";
            //
            // label3
            //
            // this.label3.AutoSize = true;
            // this.label3.Location = new System.Drawing.Point(15, 74);
            this.label3.Xalign = 0.04F;
            // this.label3.MarginTop = 4;
            // this.label3.MarginBottom = 4;
            this.label3.Name = "label3";
            this.label3.SetSizeRequest(62, 18);
            this.label3.Text = "服务：";
            ////
            //// FrmMain
            ////
            //this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            //this.AutoScaleMode = AutoScaleMode.Font;
            //this.ClientSize = new System.Drawing.Size(1000, 568);
            this.PackStart(this.pnlSetting, false, false, 2);
            this.PackStart(this.label3, false, false, 2);
            this.PackStart(this.cbAction, false, false, 2);
            this.PackStart(this.gbReceive, false, false, 2);
            this.PackStart(this.gbSend, false, false, 2);
            //this.Margin = new Padding(4);
            //this.Name = "FrmMain";
            //this.StartPosition = FormStartPosition.CenterScreen;
            //this.Text = "Api调试";
            this.Shown += new System.EventHandler(this.FrmMain_Load);
            //this.menuReceive.ResumeLayout(false);
            //this.menuSend.ResumeLayout(false);
            //this.pnlSetting.ResumeLayout(false);
            //this.pnlSetting.PerformLayout();
            //((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            //((System.ComponentModel.ISupportInitialize)(this.numThreads)).EndInit();
            //((System.ComponentModel.ISupportInitialize)(this.numSleep)).EndInit();
            //((System.ComponentModel.ISupportInitialize)(this.numMutilSend)).EndInit();
            //this.ResumeLayout(false);
            //this.PerformLayout();
            this.Fill = true;
            this.Halign = Align.Fill;
            this.Valign = Align.Fill;
            this.Orientation = Orientation.Vertical;
        }

        #endregion

        private Frame gbReceive;
        private Button btnConnect;
        //private Timer timer1;
        //private ContextMenuStrip menuSend;
        //private ToolStripMenuItem mi清空2;
        private TextView txtReceive;
        //private FontDialog fontDialog1;
        //private ColorDialog colorDialog1;
        private Label label1;
        private Label lbAddr;
        private ComboBox cbMode;
        private ComboBoxText cbAddr;
        private HBox pnlSetting;
        //private ContextMenuStrip menuReceive;
        //private ToolStripMenuItem toolStripMenuItem1;
        //private ToolStripSeparator toolStripMenuItem3;
        private Frame gbSend;
        private VBox boxSend;
        private HBox boxSendSetting;
        private SpinButton numSleep;
        private TextView txtSend;
        private Button btnSend;
        private SpinButton numMutilSend;
        private Label label2;
        private Label label7;
        private Label label8;
        //private ToolStripMenuItem mi显示统计信息;
        private SpinButton numThreads;
        //private ToolStripMenuItem mi显示应用日志;
        //private ToolStripMenuItem mi显示编码日志;
        //private ToolTip toolTip1;
        private ComboBoxText cbAction;
        //private ToolStripMenuItem mi日志着色;
        private SpinButton numPort;
        private Label label5;
        private Label label3;
        //private ToolStripMenuItem mi显示发送数据;
        //private ToolStripMenuItem mi显示接收数据;
    }
}

