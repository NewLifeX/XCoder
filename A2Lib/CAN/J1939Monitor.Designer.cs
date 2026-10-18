namespace ALib.CAN
{
    partial class J1939Monitor
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
            components = new System.ComponentModel.Container();
            lvFrame = new ListView();
            colTime = new ColumnHeader();
            colId = new ColumnHeader();
            colPgn = new ColumnHeader();
            colPrio = new ColumnHeader();
            colPf = new ColumnHeader();
            colPs = new ColumnHeader();
            colSa = new ColumnHeader();
            colFormat = new ColumnHeader();
            colData = new ColumnHeader();
            colMsg = new ColumnHeader();
            colSummary = new ColumnHeader();
            colInterval = new ColumnHeader();
            lvSignal = new ListView();
            canSelect = new CanSelect();
            panelLeft = new Panel();
            labelDbc = new Label();
            lblDbc = new Label();
            btOpenDbc = new Button();
            btPause = new Button();
            btClear = new Button();
            cbPause = new CheckBox();
            cbHex = new CheckBox();
            cbUnknownOnly = new CheckBox();
            lblRate = new Label();
            panelTop = new Panel();
            lblFrame = new Label();
            lblDtc = new Label();
            splitMain = new SplitContainer();
            timerRefresh = new System.Windows.Forms.Timer(components);
            panelLeft.SuspendLayout();
            panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            SuspendLayout();
            // 
            // lvFrame
            // 
            lvFrame.Columns.AddRange(new ColumnHeader[] { colTime, colId, colPgn, colPrio, colPf, colPs, colSa, colFormat, colData, colMsg, colSummary, colInterval });
            lvFrame.Dock = DockStyle.Fill;
            lvFrame.Font = new Font("Cascadia Mono", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lvFrame.FullRowSelect = true;
            lvFrame.GridLines = true;
            lvFrame.Location = new Point(0, 0);
            lvFrame.MultiSelect = false;
            lvFrame.Name = "lvFrame";
            lvFrame.Size = new Size(880, 470);
            lvFrame.TabIndex = 0;
            lvFrame.UseCompatibleStateImageBehavior = false;
            lvFrame.View = View.Details;
            lvFrame.ItemSelectionChanged += lvFrame_ItemSelectionChanged;
            // 
            // colTime
            // 
            colTime.Text = "时间";
            colTime.Width = 100;
            // 
            // colId
            // 
            colId.Text = "ID";
            colId.Width = 90;
            // 
            // colPgn
            // 
            colPgn.Text = "PGN";
            colPgn.Width = 80;
            // 
            // colPrio
            // 
            colPrio.Text = "优先级";
            colPrio.Width = 60;
            // 
            // colPf
            // 
            colPf.Text = "PF";
            colPf.Width = 50;
            // 
            // colPs
            // 
            colPs.Text = "PS";
            colPs.Width = 50;
            // 
            // colSa
            // 
            colSa.Text = "SA";
            colSa.Width = 50;
            // 
            // colFormat
            // 
            colFormat.Text = "格式";
            colFormat.Width = 100;
            // 
            // colData
            // 
            colData.Text = "数据";
            colData.Width = 230;
            // 
            // colMsg
            // 
            colMsg.Text = "报文";
            colMsg.Width = 190;
            // 
            // colSummary
            // 
            colSummary.Text = "信号摘要";
            colSummary.Width = 300;
            // 
            // colInterval
            // 
            colInterval.Text = "间隔ms";
            colInterval.Width = 70;
            // 
            // lvSignal
            // 
            lvSignal.Dock = DockStyle.Fill;
            lvSignal.Font = new Font("Cascadia Mono", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lvSignal.FullRowSelect = true;
            lvSignal.GridLines = true;
            lvSignal.Location = new Point(0, 46);
            lvSignal.MultiSelect = false;
            lvSignal.Name = "lvSignal";
            lvSignal.Size = new Size(880, 250);
            lvSignal.TabIndex = 1;
            lvSignal.UseCompatibleStateImageBehavior = false;
            lvSignal.View = View.Details;
            // 
            // panelTop
            // 
            panelTop.Controls.Add(lblDtc);
            panelTop.Controls.Add(lblFrame);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(0, 0);
            panelTop.Name = "panelTop";
            panelTop.Padding = new Padding(6, 4, 6, 4);
            panelTop.Size = new Size(880, 46);
            panelTop.TabIndex = 0;
            // 
            // lblFrame
            // 
            lblFrame.Dock = DockStyle.Top;
            lblFrame.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            lblFrame.Height = 20;
            lblFrame.Location = new Point(6, 4);
            lblFrame.Name = "lblFrame";
            lblFrame.Size = new Size(868, 20);
            lblFrame.TabIndex = 0;
            lblFrame.Text = "选中上方一行查看结合 DBC 解析出的信号值";
            // 
            // lblDtc
            // 
            lblDtc.Dock = DockStyle.Fill;
            lblDtc.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            lblDtc.ForeColor = Color.Firebrick;
            lblDtc.Location = new Point(6, 24);
            lblDtc.Name = "lblDtc";
            lblDtc.Size = new Size(868, 18);
            lblDtc.TabIndex = 1;
            // 
            // panelLeft
            // 
            panelLeft.Controls.Add(lblRate);
            panelLeft.Controls.Add(cbUnknownOnly);
            panelLeft.Controls.Add(cbHex);
            panelLeft.Controls.Add(cbPause);
            panelLeft.Controls.Add(btClear);
            panelLeft.Controls.Add(btPause);
            panelLeft.Controls.Add(btOpenDbc);
            panelLeft.Controls.Add(lblDbc);
            panelLeft.Controls.Add(labelDbc);
            panelLeft.Controls.Add(canSelect);
            panelLeft.Dock = DockStyle.Fill;
            panelLeft.Location = new Point(0, 0);
            panelLeft.Name = "panelLeft";
            panelLeft.Size = new Size(190, 730);
            panelLeft.TabIndex = 0;
            // 
            // canSelect
            // 
            canSelect.AllowSelect = true;
            canSelect.Location = new Point(8, 8);
            canSelect.Name = "canSelect";
            canSelect.Size = new Size(174, 164);
            canSelect.TabIndex = 0;
            // 
            // labelDbc
            // 
            labelDbc.AutoSize = true;
            labelDbc.Location = new Point(8, 180);
            labelDbc.Name = "labelDbc";
            labelDbc.Size = new Size(32, 17);
            labelDbc.TabIndex = 1;
            labelDbc.Text = "DBC";
            // 
            // lblDbc
            // 
            lblDbc.AutoEllipsis = true;
            lblDbc.Location = new Point(8, 200);
            lblDbc.Name = "lblDbc";
            lblDbc.Size = new Size(174, 34);
            lblDbc.TabIndex = 2;
            lblDbc.Text = "（未加载）";
            // 
            // btOpenDbc
            // 
            btOpenDbc.Location = new Point(8, 240);
            btOpenDbc.Name = "btOpenDbc";
            btOpenDbc.Size = new Size(174, 29);
            btOpenDbc.TabIndex = 3;
            btOpenDbc.Text = "打开 DBC";
            btOpenDbc.UseVisualStyleBackColor = true;
            btOpenDbc.Click += btOpenDbc_Click;
            // 
            // btPause
            // 
            btPause.Location = new Point(8, 279);
            btPause.Name = "btPause";
            btPause.Size = new Size(174, 29);
            btPause.TabIndex = 4;
            btPause.Text = "暂停";
            btPause.UseVisualStyleBackColor = true;
            btPause.Click += btPause_Click;
            // 
            // btClear
            // 
            btClear.Location = new Point(8, 314);
            btClear.Name = "btClear";
            btClear.Size = new Size(174, 29);
            btClear.TabIndex = 5;
            btClear.Text = "清空";
            btClear.UseVisualStyleBackColor = true;
            btClear.Click += btClear_Click;
            // 
            // cbPause
            // 
            cbPause.AutoSize = true;
            cbPause.Location = new Point(8, 352);
            cbPause.Name = "cbPause";
            cbPause.Size = new Size(75, 21);
            cbPause.TabIndex = 6;
            cbPause.Text = "暂停刷新";
            cbPause.UseVisualStyleBackColor = true;
            cbPause.Visible = false;
            // 
            // cbHex
            // 
            cbHex.AutoSize = true;
            cbHex.Checked = true;
            cbHex.CheckState = CheckState.Checked;
            cbHex.Location = new Point(8, 352);
            cbHex.Name = "cbHex";
            cbHex.Size = new Size(125, 21);
            cbHex.TabIndex = 0;
            cbHex.Text = "仅显示近 3 秒活跃帧";
            cbHex.UseVisualStyleBackColor = true;
            cbHex.CheckedChanged += cbFilter_CheckedChanged;
            // 
            // cbUnknownOnly
            // 
            cbUnknownOnly.AutoSize = true;
            cbUnknownOnly.Location = new Point(8, 377);
            cbUnknownOnly.Name = "cbUnknownOnly";
            cbUnknownOnly.Size = new Size(147, 21);
            cbUnknownOnly.TabIndex = 1;
            cbUnknownOnly.Text = "仅显示 DBC 未定义";
            cbUnknownOnly.UseVisualStyleBackColor = true;
            cbUnknownOnly.CheckedChanged += cbFilter_CheckedChanged;
            // 
            // lblRate
            // 
            lblRate.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblRate.Location = new Point(8, 690);
            lblRate.Name = "lblRate";
            lblRate.Size = new Size(174, 34);
            lblRate.TabIndex = 2;
            // 
            // splitMain
            // 
            splitMain.Dock = DockStyle.Fill;
            splitMain.Location = new Point(0, 56);
            splitMain.Name = "splitMain";
            splitMain.Orientation = Orientation.Horizontal;
            splitMain.Panel1.Controls.Add(lvFrame);
            splitMain.Panel1MinSize = 120;
            splitMain.Panel2.Controls.Add(lvSignal);
            splitMain.Panel2.Controls.Add(panelTop);
            splitMain.Panel2MinSize = 160;
            splitMain.Size = new Size(1070, 730);
            splitMain.SplitterDistance = 470;
            splitMain.TabIndex = 1;
            // 
            // timerRefresh
            // 
            timerRefresh.Enabled = true;
            timerRefresh.Interval = 200;
            timerRefresh.Tick += timerRefresh_Tick;
            // 
            // J1939Monitor
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1070, 786);
            Controls.Add(splitMain);
            Controls.Add(panelLeft);
            MinimumSize = new Size(900, 600);
            Name = "J1939Monitor";
            Text = "J1939 报文查看器";
            Load += J1939Monitor_Load;
            panelLeft.ResumeLayout(false);
            panelLeft.PerformLayout();
            panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            splitMain.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ListView lvFrame;
        private ListView lvSignal;
        private CanSelect canSelect;
        private Panel panelLeft;
        private Panel panelTop;
        private SplitContainer splitMain;
        private Button btOpenDbc;
        private Button btPause;
        private Button btClear;
        private CheckBox cbPause;
        private CheckBox cbHex;
        private CheckBox cbUnknownOnly;
        private Label lblDbc;
        private Label labelDbc;
        private Label lblRate;
        private Label lblFrame;
        private Label lblDtc;
        private ColumnHeader colTime;
        private ColumnHeader colId;
        private ColumnHeader colPgn;
        private ColumnHeader colPrio;
        private ColumnHeader colPf;
        private ColumnHeader colPs;
        private ColumnHeader colSa;
        private ColumnHeader colFormat;
        private ColumnHeader colData;
        private ColumnHeader colMsg;
        private ColumnHeader colSummary;
        private ColumnHeader colInterval;
        private System.Windows.Forms.Timer timerRefresh;
    }
}
