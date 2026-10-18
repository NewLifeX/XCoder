namespace ALib.CAN
{
    partial class DbcViewer
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
            tvMain = new TreeView();
            lvDetail = new ListView();
            lvSearch = new ListView();
            splitMain = new SplitContainer();
            splitRight = new SplitContainer();
            panelTop = new Panel();
            btExport = new Button();
            btReload = new Button();
            btOpen = new Button();
            lblPath = new Label();
            lblWarn = new Label();
            panelSearch = new Panel();
            btSearch = new Button();
            txtSearch = new TextBox();
            lblSearchTip = new Label();
            lblSearch = new Label();
            lblDetail = new Label();
            panelDetail = new Panel();
            tipWarn = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitRight).BeginInit();
            splitRight.Panel1.SuspendLayout();
            splitRight.Panel2.SuspendLayout();
            splitRight.SuspendLayout();
            panelTop.SuspendLayout();
            panelSearch.SuspendLayout();
            panelDetail.SuspendLayout();
            SuspendLayout();
            // 
            // tvMain
            // 
            tvMain.Dock = DockStyle.Fill;
            tvMain.Font = new Font("Cascadia Mono", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tvMain.HideSelection = false;
            tvMain.Location = new Point(0, 0);
            tvMain.Name = "tvMain";
            tvMain.Size = new Size(380, 800);
            tvMain.TabIndex = 0;
            tvMain.AfterSelect += tvMain_AfterSelect;
            // 
            // splitMain
            // 
            splitMain.Dock = DockStyle.Fill;
            splitMain.Location = new Point(0, 56);
            splitMain.Name = "splitMain";
            splitMain.Panel1.Controls.Add(tvMain);
            splitMain.Panel1MinSize = 260;
            splitMain.Panel2.Controls.Add(splitRight);
            splitMain.Size = new Size(1180, 800);
            splitMain.SplitterDistance = 380;
            splitMain.TabIndex = 1;
            // 
            // splitRight
            // 
            splitRight.Dock = DockStyle.Fill;
            splitRight.Location = new Point(0, 0);
            splitRight.Name = "splitRight";
            splitRight.Orientation = Orientation.Horizontal;
            splitRight.Panel1.Controls.Add(lvDetail);
            splitRight.Panel1.Controls.Add(panelDetail);
            splitRight.Panel1MinSize = 200;
            splitRight.Panel2.Controls.Add(lvSearch);
            splitRight.Panel2.Controls.Add(panelSearch);
            splitRight.Panel2MinSize = 160;
            splitRight.Size = new Size(796, 800);
            splitRight.SplitterDistance = 480;
            splitRight.TabIndex = 0;
            // 
            // lvDetail
            // 
            lvDetail.Dock = DockStyle.Fill;
            lvDetail.Font = new Font("Cascadia Mono", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lvDetail.FullRowSelect = true;
            lvDetail.GridLines = true;
            lvDetail.Location = new Point(0, 44);
            lvDetail.MultiSelect = false;
            lvDetail.Name = "lvDetail";
            lvDetail.Size = new Size(796, 436);
            lvDetail.TabIndex = 1;
            lvDetail.UseCompatibleStateImageBehavior = false;
            lvDetail.View = View.Details;
            // 
            // panelDetail
            // 
            panelDetail.Controls.Add(lblDetail);
            panelDetail.Dock = DockStyle.Top;
            panelDetail.Location = new Point(0, 0);
            panelDetail.Name = "panelDetail";
            panelDetail.Padding = new Padding(6, 4, 6, 4);
            panelDetail.Size = new Size(796, 44);
            panelDetail.TabIndex = 0;
            // 
            // lblDetail
            // 
            lblDetail.Dock = DockStyle.Fill;
            lblDetail.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            lblDetail.Location = new Point(6, 4);
            lblDetail.Name = "lblDetail";
            lblDetail.Size = new Size(784, 36);
            lblDetail.TabIndex = 0;
            lblDetail.Text = "请选择左侧内容";
            // 
            // lvSearch
            // 
            lvSearch.Dock = DockStyle.Fill;
            lvSearch.Font = new Font("Cascadia Mono", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lvSearch.FullRowSelect = true;
            lvSearch.GridLines = true;
            lvSearch.Location = new Point(0, 34);
            lvSearch.MultiSelect = false;
            lvSearch.Name = "lvSearch";
            lvSearch.Size = new Size(796, 282);
            lvSearch.TabIndex = 1;
            lvSearch.UseCompatibleStateImageBehavior = false;
            lvSearch.View = View.Details;
            lvSearch.MouseDoubleClick += lvSearch_MouseDoubleClick;
            // 
            // panelSearch
            // 
            panelSearch.Controls.Add(lblSearch);
            panelSearch.Controls.Add(lblSearchTip);
            panelSearch.Controls.Add(txtSearch);
            panelSearch.Controls.Add(btSearch);
            panelSearch.Dock = DockStyle.Top;
            panelSearch.Location = new Point(0, 0);
            panelSearch.Name = "panelSearch";
            panelSearch.Padding = new Padding(6, 4, 6, 4);
            panelSearch.Size = new Size(796, 34);
            panelSearch.TabIndex = 0;
            // 
            // btSearch
            // 
            btSearch.Location = new Point(6, 3);
            btSearch.Name = "btSearch";
            btSearch.Size = new Size(72, 27);
            btSearch.TabIndex = 0;
            btSearch.Text = "搜索";
            btSearch.UseVisualStyleBackColor = true;
            btSearch.Click += btSearch_Click;
            // 
            // txtSearch
            // 
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSearch.Font = new Font("Cascadia Mono", 10.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtSearch.Location = new Point(84, 4);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(430, 25);
            txtSearch.TabIndex = 1;
            txtSearch.KeyDown += txtSearch_KeyDown;
            // 
            // lblSearchTip
            // 
            lblSearchTip.AutoSize = true;
            lblSearchTip.Location = new Point(522, 9);
            lblSearchTip.Name = "lblSearchTip";
            lblSearchTip.Size = new Size(244, 17);
            lblSearchTip.TabIndex = 2;
            lblSearchTip.Text = "输入报文名/信号名/SPN/PGN，回车搜索";
            // 
            // lblSearch
            // 
            lblSearch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblSearch.AutoSize = true;
            lblSearch.Location = new Point(766, 9);
            lblSearch.Name = "lblSearch";
            lblSearch.Size = new Size(0, 17);
            lblSearch.TabIndex = 3;
            // 
            // panelTop
            // 
            panelTop.Controls.Add(lblWarn);
            panelTop.Controls.Add(lblPath);
            panelTop.Controls.Add(btOpen);
            panelTop.Controls.Add(btReload);
            panelTop.Controls.Add(btExport);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(0, 0);
            panelTop.Name = "panelTop";
            panelTop.Padding = new Padding(6, 4, 6, 4);
            panelTop.Size = new Size(1180, 56);
            panelTop.TabIndex = 0;
            // 
            // btOpen
            // 
            btOpen.Location = new Point(6, 5);
            btOpen.Name = "btOpen";
            btOpen.Size = new Size(96, 29);
            btOpen.TabIndex = 0;
            btOpen.Text = "打开 DBC";
            btOpen.UseVisualStyleBackColor = true;
            btOpen.Click += btOpen_Click;
            // 
            // btReload
            // 
            btReload.Enabled = false;
            btReload.Location = new Point(108, 5);
            btReload.Name = "btReload";
            btReload.Size = new Size(96, 29);
            btReload.TabIndex = 1;
            btReload.Text = "重新加载";
            btReload.UseVisualStyleBackColor = true;
            btReload.Click += btReload_Click;
            // 
            // btExport
            // 
            btExport.Location = new Point(210, 5);
            btExport.Name = "btExport";
            btExport.Size = new Size(110, 29);
            btExport.TabIndex = 2;
            btExport.Text = "导出 CSV";
            btExport.UseVisualStyleBackColor = true;
            btExport.Click += btExport_Click;
            // 
            // lblPath
            // 
            lblPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblPath.AutoEllipsis = true;
            lblPath.Location = new Point(330, 12);
            lblPath.Name = "lblPath";
            lblPath.Size = new Size(720, 17);
            lblPath.TabIndex = 3;
            lblPath.Text = "（未加载）";
            // 
            // lblWarn
            // 
            lblWarn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblWarn.AutoSize = true;
            lblWarn.ForeColor = Color.Firebrick;
            lblWarn.Location = new Point(1056, 12);
            lblWarn.Name = "lblWarn";
            lblWarn.Size = new Size(0, 17);
            lblWarn.TabIndex = 4;
            // 
            // DbcViewer
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1180, 856);
            Controls.Add(splitMain);
            Controls.Add(panelTop);
            MinimumSize = new Size(900, 600);
            Name = "DbcViewer";
            Text = "DBC 查看器";
            Load += DbcViewer_Load;
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            splitRight.Panel1.ResumeLayout(false);
            splitRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitRight).EndInit();
            splitRight.ResumeLayout(false);
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            panelSearch.ResumeLayout(false);
            panelSearch.PerformLayout();
            panelDetail.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TreeView tvMain;
        private ListView lvDetail;
        private ListView lvSearch;
        private SplitContainer splitMain;
        private SplitContainer splitRight;
        private Panel panelTop;
        private Panel panelSearch;
        private Panel panelDetail;
        private Button btOpen;
        private Button btReload;
        private Button btExport;
        private Button btSearch;
        private TextBox txtSearch;
        private Label lblPath;
        private Label lblWarn;
        private Label lblSearchTip;
        private Label lblSearch;
        private Label lblDetail;
        private ToolTip tipWarn;
    }
}
