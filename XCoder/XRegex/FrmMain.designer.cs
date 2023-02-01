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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtPattern = new System.Windows.Forms.RichTextBox();
            this.ptMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.正则ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtOption = new System.Windows.Forms.TextBox();
            this.chkSingleline = new System.Windows.Forms.CheckBox();
            this.chkIgnorePatternWhitespace = new System.Windows.Forms.CheckBox();
            this.chkMultiline = new System.Windows.Forms.CheckBox();
            this.chkIgnoreCase = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtContent = new System.Windows.Forms.RichTextBox();
            this.txtMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.例子ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.lvMatch = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.lvGroup = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
            this.lvCapture = new System.Windows.Forms.ListView();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
            this.rtReplace = new System.Windows.Forms.RichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lbStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1.SuspendLayout();
            this.ptMenu.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.txtMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtPattern);
            this.groupBox1.Controls.Add(this.txtOption);
            this.groupBox1.Controls.Add(this.chkSingleline);
            this.groupBox1.Controls.Add(this.chkIgnorePatternWhitespace);
            this.groupBox1.Controls.Add(this.chkMultiline);
            this.groupBox1.Controls.Add(this.chkIgnoreCase);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(1150, 255);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "正则表达式";
            // 
            // txtPattern
            // 
            this.txtPattern.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPattern.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.txtPattern.ContextMenuStrip = this.ptMenu;
            this.txtPattern.DetectUrls = false;
            this.txtPattern.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtPattern.HideSelection = false;
            this.txtPattern.Location = new System.Drawing.Point(9, 75);
            this.txtPattern.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPattern.Name = "txtPattern";
            this.txtPattern.Size = new System.Drawing.Size(1117, 160);
            this.txtPattern.TabIndex = 15;
            this.txtPattern.Text = "";
            // 
            // ptMenu
            // 
            this.ptMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ptMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.正则ToolStripMenuItem});
            this.ptMenu.Name = "ptMenu";
            this.ptMenu.Size = new System.Drawing.Size(109, 28);
            this.ptMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ptMenu_Opening);
            // 
            // 正则ToolStripMenuItem
            // 
            this.正则ToolStripMenuItem.Name = "正则ToolStripMenuItem";
            this.正则ToolStripMenuItem.Size = new System.Drawing.Size(108, 24);
            this.正则ToolStripMenuItem.Text = "正则";
            this.正则ToolStripMenuItem.Click += new System.EventHandler(this.MenuItem_Click);
            // 
            // txtOption
            // 
            this.txtOption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOption.Location = new System.Drawing.Point(631, 29);
            this.txtOption.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtOption.Name = "txtOption";
            this.txtOption.ReadOnly = true;
            this.txtOption.Size = new System.Drawing.Size(495, 27);
            this.txtOption.TabIndex = 14;
            // 
            // chkSingleline
            // 
            this.chkSingleline.AutoSize = true;
            this.chkSingleline.Checked = true;
            this.chkSingleline.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSingleline.Location = new System.Drawing.Point(159, 33);
            this.chkSingleline.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkSingleline.Name = "chkSingleline";
            this.chkSingleline.Size = new System.Drawing.Size(91, 24);
            this.chkSingleline.TabIndex = 6;
            this.chkSingleline.Text = "单行模式";
            this.toolTip1.SetToolTip(this.chkSingleline, "使用单行模式，其中句点 (.)匹配每个字符（而不是与除 \\n 之外的每个字符匹配）。 ");
            this.chkSingleline.UseVisualStyleBackColor = true;
            this.chkSingleline.Click += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // chkIgnorePatternWhitespace
            // 
            this.chkIgnorePatternWhitespace.AutoSize = true;
            this.chkIgnorePatternWhitespace.Checked = true;
            this.chkIgnorePatternWhitespace.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIgnorePatternWhitespace.Location = new System.Drawing.Point(420, 33);
            this.chkIgnorePatternWhitespace.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkIgnorePatternWhitespace.Name = "chkIgnorePatternWhitespace";
            this.chkIgnorePatternWhitespace.Size = new System.Drawing.Size(151, 24);
            this.chkIgnorePatternWhitespace.TabIndex = 4;
            this.chkIgnorePatternWhitespace.Text = "忽略正则中的空白";
            this.toolTip1.SetToolTip(this.chkIgnorePatternWhitespace, "从模式中排除保留的空白并启用数字符号 ( #) 后的注释。");
            this.chkIgnorePatternWhitespace.UseVisualStyleBackColor = true;
            this.chkIgnorePatternWhitespace.Click += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // chkMultiline
            // 
            this.chkMultiline.AutoSize = true;
            this.chkMultiline.Checked = true;
            this.chkMultiline.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMultiline.Location = new System.Drawing.Point(289, 33);
            this.chkMultiline.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkMultiline.Name = "chkMultiline";
            this.chkMultiline.Size = new System.Drawing.Size(91, 24);
            this.chkMultiline.TabIndex = 3;
            this.chkMultiline.Text = "多线模式";
            this.toolTip1.SetToolTip(this.chkMultiline, "使用多线模式，其中 ^ 和 $ 匹配每行的开头和末尾（不是输入字符串的开头和末尾）。 ");
            this.chkMultiline.UseVisualStyleBackColor = true;
            this.chkMultiline.Click += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // chkIgnoreCase
            // 
            this.chkIgnoreCase.AutoSize = true;
            this.chkIgnoreCase.Checked = true;
            this.chkIgnoreCase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIgnoreCase.Location = new System.Drawing.Point(10, 33);
            this.chkIgnoreCase.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkIgnoreCase.Name = "chkIgnoreCase";
            this.chkIgnoreCase.Size = new System.Drawing.Size(106, 24);
            this.chkIgnoreCase.TabIndex = 2;
            this.chkIgnoreCase.Text = "忽略大小写";
            this.toolTip1.SetToolTip(this.chkIgnoreCase, "使用不区分大小写的匹配");
            this.chkIgnoreCase.UseVisualStyleBackColor = true;
            this.chkIgnoreCase.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            this.chkIgnoreCase.Click += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtContent);
            this.groupBox2.Controls.Add(this.radioButton2);
            this.groupBox2.Controls.Add(this.radioButton1);
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(1150, 324);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "数据源";
            // 
            // txtContent
            // 
            this.txtContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtContent.ContextMenuStrip = this.txtMenu;
            this.txtContent.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtContent.HideSelection = false;
            this.txtContent.Location = new System.Drawing.Point(9, 72);
            this.txtContent.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtContent.Name = "txtContent";
            this.txtContent.Size = new System.Drawing.Size(1117, 246);
            this.txtContent.TabIndex = 16;
            this.txtContent.Text = "";
            // 
            // txtMenu
            // 
            this.txtMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.txtMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.例子ToolStripMenuItem});
            this.txtMenu.Name = "txtMenu";
            this.txtMenu.Size = new System.Drawing.Size(109, 28);
            this.txtMenu.Opening += new System.ComponentModel.CancelEventHandler(this.txtMenu_Opening);
            // 
            // 例子ToolStripMenuItem
            // 
            this.例子ToolStripMenuItem.Name = "例子ToolStripMenuItem";
            this.例子ToolStripMenuItem.Size = new System.Drawing.Size(108, 24);
            this.例子ToolStripMenuItem.Text = "例子";
            this.例子ToolStripMenuItem.Click += new System.EventHandler(this.MenuItem_Click);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(94, 27);
            this.radioButton2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(60, 24);
            this.radioButton2.TabIndex = 12;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "替换";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(14, 27);
            this.radioButton1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(60, 24);
            this.radioButton1.TabIndex = 11;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "匹配";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.button5);
            this.panel1.Location = new System.Drawing.Point(532, 21);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(426, 47);
            this.panel1.TabIndex = 10;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(208, 5);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(82, 27);
            this.textBox2.TabIndex = 5;
            this.textBox2.Text = "*.*";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(10, 3);
            this.button4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(112, 39);
            this.button4.TabIndex = 4;
            this.button4.Text = "打开目录";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(147, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "过滤:";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(305, 3);
            this.button5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(112, 39);
            this.button5.TabIndex = 7;
            this.button5.Text = "全部替换";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(198, 21);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(112, 39);
            this.button2.TabIndex = 1;
            this.button2.Text = "正则匹配";
            this.toolTip1.SetToolTip(this.button2, "如果正则中选中部分文本，则以选中部分文本作为匹配用正则。");
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.splitContainer3);
            this.groupBox3.Controls.Add(this.rtReplace);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Size = new System.Drawing.Size(1150, 298);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "匹配结果(Match|Group|Capture)";
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(4, 25);
            this.splitContainer3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.lvMatch);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.splitContainer4);
            this.splitContainer3.Size = new System.Drawing.Size(1142, 268);
            this.splitContainer3.SplitterDistance = 333;
            this.splitContainer3.SplitterWidth = 6;
            this.splitContainer3.TabIndex = 0;
            // 
            // lvMatch
            // 
            this.lvMatch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lvMatch.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader8});
            this.lvMatch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvMatch.FullRowSelect = true;
            this.lvMatch.GridLines = true;
            this.lvMatch.Location = new System.Drawing.Point(0, 0);
            this.lvMatch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lvMatch.MultiSelect = false;
            this.lvMatch.Name = "lvMatch";
            this.lvMatch.Size = new System.Drawing.Size(333, 268);
            this.lvMatch.TabIndex = 0;
            this.lvMatch.UseCompatibleStateImageBehavior = false;
            this.lvMatch.View = System.Windows.Forms.View.Details;
            this.lvMatch.SelectedIndexChanged += new System.EventHandler(this.lvMatch_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "顺序";
            this.columnHeader1.Width = 36;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "匹配项";
            this.columnHeader2.Width = 118;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "位置";
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.lvGroup);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.lvCapture);
            this.splitContainer4.Size = new System.Drawing.Size(803, 268);
            this.splitContainer4.SplitterDistance = 475;
            this.splitContainer4.SplitterWidth = 6;
            this.splitContainer4.TabIndex = 0;
            // 
            // lvGroup
            // 
            this.lvGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lvGroup.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader7,
            this.columnHeader9});
            this.lvGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvGroup.FullRowSelect = true;
            this.lvGroup.GridLines = true;
            this.lvGroup.Location = new System.Drawing.Point(0, 0);
            this.lvGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lvGroup.MultiSelect = false;
            this.lvGroup.Name = "lvGroup";
            this.lvGroup.Size = new System.Drawing.Size(475, 268);
            this.lvGroup.TabIndex = 0;
            this.lvGroup.UseCompatibleStateImageBehavior = false;
            this.lvGroup.View = System.Windows.Forms.View.Details;
            this.lvGroup.SelectedIndexChanged += new System.EventHandler(this.lvGroup_SelectedIndexChanged);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "顺序";
            this.columnHeader3.Width = 36;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "名称";
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "匹配项";
            this.columnHeader7.Width = 124;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "位置";
            // 
            // lvCapture
            // 
            this.lvCapture.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lvCapture.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader10});
            this.lvCapture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvCapture.FullRowSelect = true;
            this.lvCapture.GridLines = true;
            this.lvCapture.Location = new System.Drawing.Point(0, 0);
            this.lvCapture.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lvCapture.MultiSelect = false;
            this.lvCapture.Name = "lvCapture";
            this.lvCapture.Size = new System.Drawing.Size(322, 268);
            this.lvCapture.TabIndex = 0;
            this.lvCapture.UseCompatibleStateImageBehavior = false;
            this.lvCapture.View = System.Windows.Forms.View.Details;
            this.lvCapture.SelectedIndexChanged += new System.EventHandler(this.lvCapture_SelectedIndexChanged);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "顺序";
            this.columnHeader5.Width = 36;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "匹配项";
            this.columnHeader6.Width = 109;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "位置";
            // 
            // rtReplace
            // 
            this.rtReplace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.rtReplace.DetectUrls = false;
            this.rtReplace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtReplace.Font = new System.Drawing.Font("新宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.rtReplace.HideSelection = false;
            this.rtReplace.Location = new System.Drawing.Point(4, 25);
            this.rtReplace.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rtReplace.Name = "rtReplace";
            this.rtReplace.Size = new System.Drawing.Size(1142, 268);
            this.rtReplace.TabIndex = 1;
            this.rtReplace.Text = "";
            this.rtReplace.Visible = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 910);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1176, 26);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lbStatus
            // 
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(54, 20);
            this.lbStatus.Text = "已就绪";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(13, 14);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1150, 891);
            this.splitContainer1.SplitterDistance = 255;
            this.splitContainer1.SplitterWidth = 7;
            this.splitContainer1.TabIndex = 4;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer2.Size = new System.Drawing.Size(1150, 629);
            this.splitContainer2.SplitterDistance = 324;
            this.splitContainer2.SplitterWidth = 7;
            this.splitContainer2.TabIndex = 0;
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // FrmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1176, 936);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "正则表达式";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Shown += new System.EventHandler(this.FrmMain_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ptMenu.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.txtMenu.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lbStatus;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.CheckBox chkIgnorePatternWhitespace;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkMultiline;
        private System.Windows.Forms.CheckBox chkIgnoreCase;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.ListView lvMatch;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.ListView lvGroup;
        private System.Windows.Forms.ListView lvCapture;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.ContextMenuStrip ptMenu;
        private System.Windows.Forms.ContextMenuStrip txtMenu;
        private System.Windows.Forms.ToolStripMenuItem 正则ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 例子ToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkSingleline;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox rtReplace;
        private System.Windows.Forms.TextBox txtOption;
        private System.Windows.Forms.RichTextBox txtPattern;
        private System.Windows.Forms.RichTextBox txtContent;
    }
}

