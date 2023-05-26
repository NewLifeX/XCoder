namespace NewLife.XRegex
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
            groupBox1 = new GroupBox();
            txtPattern = new RichTextBox();
            ptMenu = new ContextMenuStrip(components);
            正则ToolStripMenuItem = new ToolStripMenuItem();
            txtOption = new TextBox();
            chkSingleline = new CheckBox();
            chkIgnorePatternWhitespace = new CheckBox();
            chkMultiline = new CheckBox();
            chkIgnoreCase = new CheckBox();
            groupBox2 = new GroupBox();
            txtContent = new RichTextBox();
            txtMenu = new ContextMenuStrip(components);
            例子ToolStripMenuItem = new ToolStripMenuItem();
            radioButton2 = new RadioButton();
            radioButton1 = new RadioButton();
            panel1 = new Panel();
            textBox2 = new TextBox();
            button4 = new Button();
            label1 = new Label();
            button5 = new Button();
            button2 = new Button();
            groupBox3 = new GroupBox();
            splitContainer3 = new SplitContainer();
            lvMatch = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader8 = new ColumnHeader();
            splitContainer4 = new SplitContainer();
            lvGroup = new ListView();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            columnHeader7 = new ColumnHeader();
            columnHeader9 = new ColumnHeader();
            lvCapture = new ListView();
            columnHeader5 = new ColumnHeader();
            columnHeader6 = new ColumnHeader();
            columnHeader10 = new ColumnHeader();
            rtReplace = new RichTextBox();
            statusStrip1 = new StatusStrip();
            lbStatus = new ToolStripStatusLabel();
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            toolTip1 = new ToolTip(components);
            folderBrowserDialog1 = new FolderBrowserDialog();
            groupBox1.SuspendLayout();
            ptMenu.SuspendLayout();
            groupBox2.SuspendLayout();
            txtMenu.SuspendLayout();
            panel1.SuspendLayout();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer4).BeginInit();
            splitContainer4.Panel1.SuspendLayout();
            splitContainer4.Panel2.SuspendLayout();
            splitContainer4.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtPattern);
            groupBox1.Controls.Add(txtOption);
            groupBox1.Controls.Add(chkSingleline);
            groupBox1.Controls.Add(chkIgnorePatternWhitespace);
            groupBox1.Controls.Add(chkMultiline);
            groupBox1.Controls.Add(chkIgnoreCase);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Margin = new Padding(4, 5, 4, 5);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 5, 4, 5);
            groupBox1.Size = new Size(1150, 255);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "正则表达式";
            // 
            // txtPattern
            // 
            txtPattern.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtPattern.BackColor = Color.FromArgb(255, 224, 192);
            txtPattern.ContextMenuStrip = ptMenu;
            txtPattern.DetectUrls = false;
            txtPattern.Font = new Font("微软雅黑", 16F, FontStyle.Regular, GraphicsUnit.Point);
            txtPattern.HideSelection = false;
            txtPattern.Location = new Point(9, 75);
            txtPattern.Margin = new Padding(4, 5, 4, 5);
            txtPattern.Name = "txtPattern";
            txtPattern.Size = new Size(1117, 160);
            txtPattern.TabIndex = 15;
            txtPattern.Text = "";
            // 
            // ptMenu
            // 
            ptMenu.ImageScalingSize = new Size(20, 20);
            ptMenu.Items.AddRange(new ToolStripItem[] { 正则ToolStripMenuItem });
            ptMenu.Name = "ptMenu";
            ptMenu.Size = new Size(117, 34);
            ptMenu.Opening += ptMenu_Opening;
            // 
            // 正则ToolStripMenuItem
            // 
            正则ToolStripMenuItem.Name = "正则ToolStripMenuItem";
            正则ToolStripMenuItem.Size = new Size(116, 30);
            正则ToolStripMenuItem.Text = "正则";
            正则ToolStripMenuItem.Click += MenuItem_Click;
            // 
            // txtOption
            // 
            txtOption.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtOption.Location = new Point(631, 29);
            txtOption.Margin = new Padding(4, 5, 4, 5);
            txtOption.Name = "txtOption";
            txtOption.ReadOnly = true;
            txtOption.Size = new Size(495, 30);
            txtOption.TabIndex = 14;
            // 
            // chkSingleline
            // 
            chkSingleline.AutoSize = true;
            chkSingleline.Checked = true;
            chkSingleline.CheckState = CheckState.Checked;
            chkSingleline.Location = new Point(159, 33);
            chkSingleline.Margin = new Padding(4, 5, 4, 5);
            chkSingleline.Name = "chkSingleline";
            chkSingleline.Size = new Size(108, 28);
            chkSingleline.TabIndex = 6;
            chkSingleline.Text = "单行模式";
            toolTip1.SetToolTip(chkSingleline, "使用单行模式，其中句点 (.)匹配每个字符（而不是与除 \\n 之外的每个字符匹配）。 ");
            chkSingleline.UseVisualStyleBackColor = true;
            chkSingleline.Click += checkBox1_CheckedChanged;
            // 
            // chkIgnorePatternWhitespace
            // 
            chkIgnorePatternWhitespace.AutoSize = true;
            chkIgnorePatternWhitespace.Checked = true;
            chkIgnorePatternWhitespace.CheckState = CheckState.Checked;
            chkIgnorePatternWhitespace.Location = new Point(420, 33);
            chkIgnorePatternWhitespace.Margin = new Padding(4, 5, 4, 5);
            chkIgnorePatternWhitespace.Name = "chkIgnorePatternWhitespace";
            chkIgnorePatternWhitespace.Size = new Size(180, 28);
            chkIgnorePatternWhitespace.TabIndex = 4;
            chkIgnorePatternWhitespace.Text = "忽略正则中的空白";
            toolTip1.SetToolTip(chkIgnorePatternWhitespace, "从模式中排除保留的空白并启用数字符号 ( #) 后的注释。");
            chkIgnorePatternWhitespace.UseVisualStyleBackColor = true;
            chkIgnorePatternWhitespace.Click += checkBox1_CheckedChanged;
            // 
            // chkMultiline
            // 
            chkMultiline.AutoSize = true;
            chkMultiline.Checked = true;
            chkMultiline.CheckState = CheckState.Checked;
            chkMultiline.Location = new Point(289, 33);
            chkMultiline.Margin = new Padding(4, 5, 4, 5);
            chkMultiline.Name = "chkMultiline";
            chkMultiline.Size = new Size(108, 28);
            chkMultiline.TabIndex = 3;
            chkMultiline.Text = "多线模式";
            toolTip1.SetToolTip(chkMultiline, "使用多线模式，其中 ^ 和 $ 匹配每行的开头和末尾（不是输入字符串的开头和末尾）。 ");
            chkMultiline.UseVisualStyleBackColor = true;
            chkMultiline.Click += checkBox1_CheckedChanged;
            // 
            // chkIgnoreCase
            // 
            chkIgnoreCase.AutoSize = true;
            chkIgnoreCase.Checked = true;
            chkIgnoreCase.CheckState = CheckState.Checked;
            chkIgnoreCase.Location = new Point(10, 33);
            chkIgnoreCase.Margin = new Padding(4, 5, 4, 5);
            chkIgnoreCase.Name = "chkIgnoreCase";
            chkIgnoreCase.Size = new Size(126, 28);
            chkIgnoreCase.TabIndex = 2;
            chkIgnoreCase.Text = "忽略大小写";
            toolTip1.SetToolTip(chkIgnoreCase, "使用不区分大小写的匹配");
            chkIgnoreCase.UseVisualStyleBackColor = true;
            chkIgnoreCase.CheckedChanged += checkBox1_CheckedChanged;
            chkIgnoreCase.Click += checkBox1_CheckedChanged;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(txtContent);
            groupBox2.Controls.Add(radioButton2);
            groupBox2.Controls.Add(radioButton1);
            groupBox2.Controls.Add(panel1);
            groupBox2.Controls.Add(button2);
            groupBox2.Dock = DockStyle.Fill;
            groupBox2.Location = new Point(0, 0);
            groupBox2.Margin = new Padding(4, 5, 4, 5);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4, 5, 4, 5);
            groupBox2.Size = new Size(1150, 324);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "数据源";
            // 
            // txtContent
            // 
            txtContent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtContent.BackColor = Color.FromArgb(255, 255, 192);
            txtContent.ContextMenuStrip = txtMenu;
            txtContent.Font = new Font("微软雅黑", 10F, FontStyle.Regular, GraphicsUnit.Point);
            txtContent.HideSelection = false;
            txtContent.Location = new Point(9, 72);
            txtContent.Margin = new Padding(4, 5, 4, 5);
            txtContent.Name = "txtContent";
            txtContent.Size = new Size(1117, 246);
            txtContent.TabIndex = 16;
            txtContent.Text = "";
            // 
            // txtMenu
            // 
            txtMenu.ImageScalingSize = new Size(20, 20);
            txtMenu.Items.AddRange(new ToolStripItem[] { 例子ToolStripMenuItem });
            txtMenu.Name = "txtMenu";
            txtMenu.Size = new Size(117, 34);
            txtMenu.Opening += txtMenu_Opening;
            // 
            // 例子ToolStripMenuItem
            // 
            例子ToolStripMenuItem.Name = "例子ToolStripMenuItem";
            例子ToolStripMenuItem.Size = new Size(116, 30);
            例子ToolStripMenuItem.Text = "例子";
            例子ToolStripMenuItem.Click += MenuItem_Click;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(94, 27);
            radioButton2.Margin = new Padding(4, 5, 4, 5);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(71, 28);
            radioButton2.TabIndex = 12;
            radioButton2.TabStop = true;
            radioButton2.Text = "替换";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Checked = true;
            radioButton1.Location = new Point(14, 27);
            radioButton1.Margin = new Padding(4, 5, 4, 5);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(71, 28);
            radioButton1.TabIndex = 11;
            radioButton1.TabStop = true;
            radioButton1.Text = "匹配";
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;
            // 
            // panel1
            // 
            panel1.Controls.Add(textBox2);
            panel1.Controls.Add(button4);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(button5);
            panel1.Location = new Point(532, 21);
            panel1.Margin = new Padding(4, 5, 4, 5);
            panel1.Name = "panel1";
            panel1.Size = new Size(426, 47);
            panel1.TabIndex = 10;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(208, 5);
            textBox2.Margin = new Padding(4, 5, 4, 5);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(82, 30);
            textBox2.TabIndex = 5;
            textBox2.Text = "*.*";
            // 
            // button4
            // 
            button4.Location = new Point(10, 3);
            button4.Margin = new Padding(4, 5, 4, 5);
            button4.Name = "button4";
            button4.Size = new Size(112, 39);
            button4.TabIndex = 4;
            button4.Text = "打开目录";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(147, 12);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(50, 24);
            label1.TabIndex = 6;
            label1.Text = "过滤:";
            // 
            // button5
            // 
            button5.Location = new Point(305, 3);
            button5.Margin = new Padding(4, 5, 4, 5);
            button5.Name = "button5";
            button5.Size = new Size(112, 39);
            button5.TabIndex = 7;
            button5.Text = "全部替换";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button2
            // 
            button2.Location = new Point(198, 21);
            button2.Margin = new Padding(4, 5, 4, 5);
            button2.Name = "button2";
            button2.Size = new Size(112, 39);
            button2.TabIndex = 1;
            button2.Text = "正则匹配";
            toolTip1.SetToolTip(button2, "如果正则中选中部分文本，则以选中部分文本作为匹配用正则。");
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(splitContainer3);
            groupBox3.Controls.Add(rtReplace);
            groupBox3.Dock = DockStyle.Fill;
            groupBox3.Location = new Point(0, 0);
            groupBox3.Margin = new Padding(4, 5, 4, 5);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(4, 5, 4, 5);
            groupBox3.Size = new Size(1150, 298);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "匹配结果(Match|Group|Capture)";
            // 
            // splitContainer3
            // 
            splitContainer3.Dock = DockStyle.Fill;
            splitContainer3.Location = new Point(4, 28);
            splitContainer3.Margin = new Padding(4, 5, 4, 5);
            splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(lvMatch);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(splitContainer4);
            splitContainer3.Size = new Size(1142, 265);
            splitContainer3.SplitterDistance = 333;
            splitContainer3.SplitterWidth = 6;
            splitContainer3.TabIndex = 0;
            // 
            // lvMatch
            // 
            lvMatch.BackColor = Color.FromArgb(255, 192, 255);
            lvMatch.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader8 });
            lvMatch.Dock = DockStyle.Fill;
            lvMatch.FullRowSelect = true;
            lvMatch.GridLines = true;
            lvMatch.Location = new Point(0, 0);
            lvMatch.Margin = new Padding(4, 5, 4, 5);
            lvMatch.MultiSelect = false;
            lvMatch.Name = "lvMatch";
            lvMatch.Size = new Size(333, 265);
            lvMatch.TabIndex = 0;
            lvMatch.UseCompatibleStateImageBehavior = false;
            lvMatch.View = View.Details;
            lvMatch.SelectedIndexChanged += lvMatch_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "顺序";
            columnHeader1.Width = 36;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "匹配项";
            columnHeader2.Width = 118;
            // 
            // columnHeader8
            // 
            columnHeader8.Text = "位置";
            // 
            // splitContainer4
            // 
            splitContainer4.Dock = DockStyle.Fill;
            splitContainer4.Location = new Point(0, 0);
            splitContainer4.Margin = new Padding(4, 5, 4, 5);
            splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            splitContainer4.Panel1.Controls.Add(lvGroup);
            // 
            // splitContainer4.Panel2
            // 
            splitContainer4.Panel2.Controls.Add(lvCapture);
            splitContainer4.Size = new Size(803, 265);
            splitContainer4.SplitterDistance = 475;
            splitContainer4.SplitterWidth = 6;
            splitContainer4.TabIndex = 0;
            // 
            // lvGroup
            // 
            lvGroup.BackColor = Color.FromArgb(255, 192, 255);
            lvGroup.Columns.AddRange(new ColumnHeader[] { columnHeader3, columnHeader4, columnHeader7, columnHeader9 });
            lvGroup.Dock = DockStyle.Fill;
            lvGroup.FullRowSelect = true;
            lvGroup.GridLines = true;
            lvGroup.Location = new Point(0, 0);
            lvGroup.Margin = new Padding(4, 5, 4, 5);
            lvGroup.MultiSelect = false;
            lvGroup.Name = "lvGroup";
            lvGroup.Size = new Size(475, 265);
            lvGroup.TabIndex = 0;
            lvGroup.UseCompatibleStateImageBehavior = false;
            lvGroup.View = View.Details;
            lvGroup.SelectedIndexChanged += lvGroup_SelectedIndexChanged;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "顺序";
            columnHeader3.Width = 36;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "名称";
            // 
            // columnHeader7
            // 
            columnHeader7.Text = "匹配项";
            columnHeader7.Width = 124;
            // 
            // columnHeader9
            // 
            columnHeader9.Text = "位置";
            // 
            // lvCapture
            // 
            lvCapture.BackColor = Color.FromArgb(255, 192, 255);
            lvCapture.Columns.AddRange(new ColumnHeader[] { columnHeader5, columnHeader6, columnHeader10 });
            lvCapture.Dock = DockStyle.Fill;
            lvCapture.FullRowSelect = true;
            lvCapture.GridLines = true;
            lvCapture.Location = new Point(0, 0);
            lvCapture.Margin = new Padding(4, 5, 4, 5);
            lvCapture.MultiSelect = false;
            lvCapture.Name = "lvCapture";
            lvCapture.Size = new Size(322, 265);
            lvCapture.TabIndex = 0;
            lvCapture.UseCompatibleStateImageBehavior = false;
            lvCapture.View = View.Details;
            lvCapture.SelectedIndexChanged += lvCapture_SelectedIndexChanged;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "顺序";
            columnHeader5.Width = 36;
            // 
            // columnHeader6
            // 
            columnHeader6.Text = "匹配项";
            columnHeader6.Width = 109;
            // 
            // columnHeader10
            // 
            columnHeader10.Text = "位置";
            // 
            // rtReplace
            // 
            rtReplace.BackColor = Color.FromArgb(192, 255, 192);
            rtReplace.DetectUrls = false;
            rtReplace.Dock = DockStyle.Fill;
            rtReplace.Font = new Font("新宋体", 10F, FontStyle.Regular, GraphicsUnit.Point);
            rtReplace.HideSelection = false;
            rtReplace.Location = new Point(4, 28);
            rtReplace.Margin = new Padding(4, 5, 4, 5);
            rtReplace.Name = "rtReplace";
            rtReplace.Size = new Size(1142, 265);
            rtReplace.TabIndex = 1;
            rtReplace.Text = "";
            rtReplace.Visible = false;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { lbStatus });
            statusStrip1.Location = new Point(0, 905);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 21, 0);
            statusStrip1.Size = new Size(1176, 31);
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // lbStatus
            // 
            lbStatus.Name = "lbStatus";
            lbStatus.Size = new Size(64, 24);
            lbStatus.Text = "已就绪";
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(13, 14);
            splitContainer1.Margin = new Padding(4, 5, 4, 5);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(1150, 891);
            splitContainer1.SplitterDistance = 255;
            splitContainer1.SplitterWidth = 7;
            splitContainer1.TabIndex = 4;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Margin = new Padding(4, 5, 4, 5);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(groupBox2);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(groupBox3);
            splitContainer2.Size = new Size(1150, 629);
            splitContainer2.SplitterDistance = 324;
            splitContainer2.SplitterWidth = 7;
            splitContainer2.TabIndex = 0;
            // 
            // folderBrowserDialog1
            // 
            folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // FrmMain
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1176, 936);
            Controls.Add(splitContainer1);
            Controls.Add(statusStrip1);
            Margin = new Padding(4, 5, 4, 5);
            Name = "FrmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "正则表达式";
            FormClosing += FrmMain_FormClosing;
            Shown += FrmMain_Shown;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ptMenu.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            txtMenu.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            groupBox3.ResumeLayout(false);
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
            splitContainer4.Panel1.ResumeLayout(false);
            splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer4).EndInit();
            splitContainer4.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lbStatus;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private CheckBox chkIgnorePatternWhitespace;
        private ToolTip toolTip1;
        private CheckBox chkMultiline;
        private CheckBox chkIgnoreCase;
        private SplitContainer splitContainer3;
        private ListView lvMatch;
        private SplitContainer splitContainer4;
        private ListView lvGroup;
        private ListView lvCapture;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private ColumnHeader columnHeader7;
        private Label label1;
        private TextBox textBox2;
        private Button button4;
        private Button button2;
        private Button button5;
        private ContextMenuStrip ptMenu;
        private ContextMenuStrip txtMenu;
        private ToolStripMenuItem 正则ToolStripMenuItem;
        private ToolStripMenuItem 例子ToolStripMenuItem;
        private CheckBox chkSingleline;
        private ColumnHeader columnHeader8;
        private ColumnHeader columnHeader9;
        private ColumnHeader columnHeader10;
        private FolderBrowserDialog folderBrowserDialog1;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private Panel panel1;
        private RichTextBox rtReplace;
        private TextBox txtOption;
        private RichTextBox txtPattern;
        private RichTextBox txtContent;
    }
}

