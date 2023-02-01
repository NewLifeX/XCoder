﻿namespace XCoder
{
	partial class FrmMain
	{
		/// <summary>必需的设计器变量。</summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>清理所有正在使用的资源。</summary>
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
            this.bt_Connection = new System.Windows.Forms.Button();
            this.gbConnect = new System.Windows.Forms.GroupBox();
            this.cbConn = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.gbTable = new System.Windows.Forms.GroupBox();
            this.cbIncludeView = new System.Windows.Forms.CheckBox();
            this.btnRefreshTable = new System.Windows.Forms.Button();
            this.btnRenderAll = new System.Windows.Forms.Button();
            this.btnRenderTable = new System.Windows.Forms.Button();
            this.cbTableList = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lb_Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.gbConfig = new System.Windows.Forms.GroupBox();
            this.cbRenderGenEntity = new System.Windows.Forms.CheckBox();
            this.txtBaseClass = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnOpenOutputDir = new System.Windows.Forms.Button();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.txt_ConnName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_NameSpace = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txt_OutPath = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnImport = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.自动格式化设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oracle客户端运行时检查ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.退出XToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.模型ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.模型管理MToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导出模型EToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.架构管理SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sQL查询器QToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.组件手册ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.表名字段名命名规范ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.在线帮助文档ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.qQ群1600800ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.qQ群1600800ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.博客ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.label9 = new System.Windows.Forms.Label();
            this.gbConnect.SuspendLayout();
            this.gbTable.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.gbConfig.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bt_Connection
            // 
            this.bt_Connection.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.bt_Connection.ForeColor = System.Drawing.Color.DeepPink;
            this.bt_Connection.Location = new System.Drawing.Point(442, 43);
            this.bt_Connection.Margin = new System.Windows.Forms.Padding(4);
            this.bt_Connection.Name = "bt_Connection";
            this.bt_Connection.Size = new System.Drawing.Size(78, 38);
            this.bt_Connection.TabIndex = 6;
            this.bt_Connection.Text = "连接";
            this.toolTip1.SetToolTip(this.bt_Connection, "数据库结构带有缓存，如不能获取最新结构，请稍候重试！");
            this.bt_Connection.UseVisualStyleBackColor = true;
            this.bt_Connection.Click += new System.EventHandler(this.bt_Connection_Click);
            // 
            // gbConnect
            // 
            this.gbConnect.Controls.Add(this.cbConn);
            this.gbConnect.Controls.Add(this.label4);
            this.gbConnect.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.gbConnect.ForeColor = System.Drawing.Color.DeepPink;
            this.gbConnect.Location = new System.Drawing.Point(3, 31);
            this.gbConnect.Margin = new System.Windows.Forms.Padding(4);
            this.gbConnect.Name = "gbConnect";
            this.gbConnect.Padding = new System.Windows.Forms.Padding(4);
            this.gbConnect.Size = new System.Drawing.Size(429, 63);
            this.gbConnect.TabIndex = 7;
            this.gbConnect.TabStop = false;
            // 
            // cbConn
            // 
            this.cbConn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbConn.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.cbConn.ForeColor = System.Drawing.Color.DeepPink;
            this.cbConn.FormattingEnabled = true;
            this.cbConn.Location = new System.Drawing.Point(87, 18);
            this.cbConn.Margin = new System.Windows.Forms.Padding(4);
            this.cbConn.Name = "cbConn";
            this.cbConn.Size = new System.Drawing.Size(328, 32);
            this.cbConn.TabIndex = 13;
            this.cbConn.SelectionChangeCommitted += new System.EventHandler(this.cbConn_SelectionChangeCommitted);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 29);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 19);
            this.label4.TabIndex = 12;
            this.label4.Text = "连接：";
            // 
            // gbTable
            // 
            this.gbTable.Controls.Add(this.cbIncludeView);
            this.gbTable.Controls.Add(this.btnRefreshTable);
            this.gbTable.Controls.Add(this.btnRenderAll);
            this.gbTable.Controls.Add(this.btnRenderTable);
            this.gbTable.Controls.Add(this.cbTableList);
            this.gbTable.Controls.Add(this.label5);
            this.gbTable.Enabled = false;
            this.gbTable.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.gbTable.ForeColor = System.Drawing.Color.ForestGreen;
            this.gbTable.Location = new System.Drawing.Point(3, 102);
            this.gbTable.Margin = new System.Windows.Forms.Padding(4);
            this.gbTable.Name = "gbTable";
            this.gbTable.Padding = new System.Windows.Forms.Padding(4);
            this.gbTable.Size = new System.Drawing.Size(1088, 82);
            this.gbTable.TabIndex = 14;
            this.gbTable.TabStop = false;
            // 
            // cbIncludeView
            // 
            this.cbIncludeView.AutoSize = true;
            this.cbIncludeView.Checked = true;
            this.cbIncludeView.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIncludeView.Location = new System.Drawing.Point(915, 38);
            this.cbIncludeView.Margin = new System.Windows.Forms.Padding(4);
            this.cbIncludeView.Name = "cbIncludeView";
            this.cbIncludeView.Size = new System.Drawing.Size(91, 23);
            this.cbIncludeView.TabIndex = 23;
            this.cbIncludeView.Text = "包括视图";
            this.cbIncludeView.UseVisualStyleBackColor = true;
            // 
            // btnRefreshTable
            // 
            this.btnRefreshTable.ForeColor = System.Drawing.Color.ForestGreen;
            this.btnRefreshTable.Location = new System.Drawing.Point(776, 30);
            this.btnRefreshTable.Margin = new System.Windows.Forms.Padding(4);
            this.btnRefreshTable.Name = "btnRefreshTable";
            this.btnRefreshTable.Size = new System.Drawing.Size(120, 38);
            this.btnRefreshTable.TabIndex = 22;
            this.btnRefreshTable.Text = "刷新数据表";
            this.btnRefreshTable.UseVisualStyleBackColor = true;
            this.btnRefreshTable.Click += new System.EventHandler(this.btnRefreshTable_Click);
            // 
            // btnRenderAll
            // 
            this.btnRenderAll.ForeColor = System.Drawing.Color.ForestGreen;
            this.btnRenderAll.Location = new System.Drawing.Point(654, 30);
            this.btnRenderAll.Margin = new System.Windows.Forms.Padding(4);
            this.btnRenderAll.Name = "btnRenderAll";
            this.btnRenderAll.Size = new System.Drawing.Size(112, 38);
            this.btnRenderAll.TabIndex = 21;
            this.btnRenderAll.Text = "生成所有表";
            this.btnRenderAll.UseVisualStyleBackColor = true;
            this.btnRenderAll.Click += new System.EventHandler(this.bt_GenAll_Click);
            // 
            // btnRenderTable
            // 
            this.btnRenderTable.ForeColor = System.Drawing.Color.ForestGreen;
            this.btnRenderTable.Location = new System.Drawing.Point(532, 30);
            this.btnRenderTable.Margin = new System.Windows.Forms.Padding(4);
            this.btnRenderTable.Name = "btnRenderTable";
            this.btnRenderTable.Size = new System.Drawing.Size(112, 38);
            this.btnRenderTable.TabIndex = 19;
            this.btnRenderTable.Text = "生成该表";
            this.btnRenderTable.UseVisualStyleBackColor = true;
            this.btnRenderTable.Click += new System.EventHandler(this.bt_GenTable_Click);
            // 
            // cbTableList
            // 
            this.cbTableList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTableList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTableList.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.cbTableList.ForeColor = System.Drawing.Color.ForestGreen;
            this.cbTableList.Location = new System.Drawing.Point(87, 33);
            this.cbTableList.Margin = new System.Windows.Forms.Padding(4);
            this.cbTableList.Name = "cbTableList";
            this.cbTableList.Size = new System.Drawing.Size(434, 32);
            this.cbTableList.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.ForestGreen;
            this.label5.Location = new System.Drawing.Point(16, 40);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 19);
            this.label5.TabIndex = 16;
            this.label5.Text = "数据表：";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lb_Status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 367);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1095, 26);
            this.statusStrip1.TabIndex = 23;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lb_Status
            // 
            this.lb_Status.Name = "lb_Status";
            this.lb_Status.Size = new System.Drawing.Size(1072, 20);
            this.lb_Status.Spring = true;
            this.lb_Status.Text = "状态";
            this.lb_Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gbConfig
            // 
            this.gbConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbConfig.Controls.Add(this.cbRenderGenEntity);
            this.gbConfig.Controls.Add(this.txtBaseClass);
            this.gbConfig.Controls.Add(this.label10);
            this.gbConfig.Controls.Add(this.btnOpenOutputDir);
            this.gbConfig.Controls.Add(this.checkBox3);
            this.gbConfig.Controls.Add(this.txt_ConnName);
            this.gbConfig.Controls.Add(this.label1);
            this.gbConfig.Controls.Add(this.txt_NameSpace);
            this.gbConfig.Controls.Add(this.label8);
            this.gbConfig.Controls.Add(this.txt_OutPath);
            this.gbConfig.Controls.Add(this.label7);
            this.gbConfig.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.gbConfig.Location = new System.Drawing.Point(3, 192);
            this.gbConfig.Margin = new System.Windows.Forms.Padding(4);
            this.gbConfig.Name = "gbConfig";
            this.gbConfig.Padding = new System.Windows.Forms.Padding(4);
            this.gbConfig.Size = new System.Drawing.Size(1088, 171);
            this.gbConfig.TabIndex = 26;
            this.gbConfig.TabStop = false;
            // 
            // cbRenderGenEntity
            // 
            this.cbRenderGenEntity.AutoSize = true;
            this.cbRenderGenEntity.ForeColor = System.Drawing.Color.BlueViolet;
            this.cbRenderGenEntity.Location = new System.Drawing.Point(260, 77);
            this.cbRenderGenEntity.Margin = new System.Windows.Forms.Padding(4);
            this.cbRenderGenEntity.Name = "cbRenderGenEntity";
            this.cbRenderGenEntity.Size = new System.Drawing.Size(136, 23);
            this.cbRenderGenEntity.TabIndex = 49;
            this.cbRenderGenEntity.Text = "生成泛型实体类";
            this.cbRenderGenEntity.UseVisualStyleBackColor = true;
            this.cbRenderGenEntity.Visible = false;
            // 
            // txtBaseClass
            // 
            this.txtBaseClass.ForeColor = System.Drawing.Color.BlueViolet;
            this.txtBaseClass.Location = new System.Drawing.Point(100, 72);
            this.txtBaseClass.Margin = new System.Windows.Forms.Padding(4);
            this.txtBaseClass.Name = "txtBaseClass";
            this.txtBaseClass.Size = new System.Drawing.Size(144, 27);
            this.txtBaseClass.TabIndex = 47;
            this.txtBaseClass.Text = "Entity";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.BlueViolet;
            this.label10.Location = new System.Drawing.Point(10, 79);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(84, 19);
            this.label10.TabIndex = 46;
            this.label10.Text = "实体基类：";
            // 
            // btnOpenOutputDir
            // 
            this.btnOpenOutputDir.ForeColor = System.Drawing.Color.BlueViolet;
            this.btnOpenOutputDir.Location = new System.Drawing.Point(418, 118);
            this.btnOpenOutputDir.Margin = new System.Windows.Forms.Padding(4);
            this.btnOpenOutputDir.Name = "btnOpenOutputDir";
            this.btnOpenOutputDir.Size = new System.Drawing.Size(112, 38);
            this.btnOpenOutputDir.TabIndex = 45;
            this.btnOpenOutputDir.Text = "打开目录";
            this.btnOpenOutputDir.UseVisualStyleBackColor = true;
            this.btnOpenOutputDir.Click += new System.EventHandler(this.btnOpenOutputDir_Click);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.ForeColor = System.Drawing.Color.BlueViolet;
            this.checkBox3.Location = new System.Drawing.Point(260, 123);
            this.checkBox3.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(106, 23);
            this.checkBox3.TabIndex = 39;
            this.checkBox3.Text = "中文文件名";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.Visible = false;
            // 
            // txt_ConnName
            // 
            this.txt_ConnName.ForeColor = System.Drawing.Color.BlueViolet;
            this.txt_ConnName.Location = new System.Drawing.Point(407, 26);
            this.txt_ConnName.Margin = new System.Windows.Forms.Padding(4);
            this.txt_ConnName.Name = "txt_ConnName";
            this.txt_ConnName.Size = new System.Drawing.Size(144, 27);
            this.txt_ConnName.TabIndex = 34;
            this.txt_ConnName.Text = "default";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.BlueViolet;
            this.label1.Location = new System.Drawing.Point(317, 32);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 19);
            this.label1.TabIndex = 33;
            this.label1.Text = "连接名：";
            // 
            // txt_NameSpace
            // 
            this.txt_NameSpace.ForeColor = System.Drawing.Color.BlueViolet;
            this.txt_NameSpace.Location = new System.Drawing.Point(100, 26);
            this.txt_NameSpace.Margin = new System.Windows.Forms.Padding(4);
            this.txt_NameSpace.Name = "txt_NameSpace";
            this.txt_NameSpace.Size = new System.Drawing.Size(199, 27);
            this.txt_NameSpace.TabIndex = 32;
            this.txt_NameSpace.Text = "XData";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.BlueViolet;
            this.label8.Location = new System.Drawing.Point(10, 32);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(84, 19);
            this.label8.TabIndex = 31;
            this.label8.Text = "命名空间：";
            // 
            // txt_OutPath
            // 
            this.txt_OutPath.ForeColor = System.Drawing.Color.BlueViolet;
            this.txt_OutPath.Location = new System.Drawing.Point(100, 119);
            this.txt_OutPath.Margin = new System.Windows.Forms.Padding(4);
            this.txt_OutPath.Name = "txt_OutPath";
            this.txt_OutPath.Size = new System.Drawing.Size(144, 27);
            this.txt_OutPath.TabIndex = 29;
            this.txt_OutPath.Text = "Dist";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.BlueViolet;
            this.label7.Location = new System.Drawing.Point(10, 126);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 19);
            this.label7.TabIndex = 28;
            this.label7.Text = "输出目录：";
            // 
            // btnImport
            // 
            this.btnImport.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnImport.ForeColor = System.Drawing.Color.DeepPink;
            this.btnImport.Location = new System.Drawing.Point(536, 43);
            this.btnImport.Margin = new System.Windows.Forms.Padding(4);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(120, 38);
            this.btnImport.TabIndex = 30;
            this.btnImport.Text = "导入模型";
            this.toolTip1.SetToolTip(this.btnImport, "把数据库架构信息导出到xml文件，或者从xml文件导入");
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "架构文件|*.xml";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "架构文件|*.xml";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label11.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label11.Location = new System.Drawing.Point(781, 78);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(213, 19);
            this.label11.TabIndex = 33;
            this.label11.Text = "2，导入模型，得到数据表信息";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label12.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label12.Location = new System.Drawing.Point(781, 43);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(228, 19);
            this.label12.TabIndex = 34;
            this.label12.Text = "1，连接数据库，得到数据表信息";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.模型ToolStripMenuItem,
            this.关于ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1095, 27);
            this.menuStrip1.TabIndex = 35;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.自动格式化设置ToolStripMenuItem,
            this.oracle客户端运行时检查ToolStripMenuItem1,
            this.退出XToolStripMenuItem});
            this.文件ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(73, 23);
            this.文件ToolStripMenuItem.Text = "文件(&F)";
            // 
            // 自动格式化设置ToolStripMenuItem
            // 
            this.自动格式化设置ToolStripMenuItem.Name = "自动格式化设置ToolStripMenuItem";
            this.自动格式化设置ToolStripMenuItem.Size = new System.Drawing.Size(260, 26);
            this.自动格式化设置ToolStripMenuItem.Text = "自动格式化设置";
            this.自动格式化设置ToolStripMenuItem.Click += new System.EventHandler(this.自动格式化设置ToolStripMenuItem_Click);
            // 
            // oracle客户端运行时检查ToolStripMenuItem1
            // 
            this.oracle客户端运行时检查ToolStripMenuItem1.Name = "oracle客户端运行时检查ToolStripMenuItem1";
            this.oracle客户端运行时检查ToolStripMenuItem1.Size = new System.Drawing.Size(260, 26);
            this.oracle客户端运行时检查ToolStripMenuItem1.Text = "Oracle客户端运行时检查";
            this.oracle客户端运行时检查ToolStripMenuItem1.Click += new System.EventHandler(this.oracle客户端运行时检查ToolStripMenuItem1_Click);
            // 
            // 退出XToolStripMenuItem
            // 
            this.退出XToolStripMenuItem.Name = "退出XToolStripMenuItem";
            this.退出XToolStripMenuItem.Size = new System.Drawing.Size(260, 26);
            this.退出XToolStripMenuItem.Text = "退出(&X)";
            this.退出XToolStripMenuItem.Click += new System.EventHandler(this.退出XToolStripMenuItem_Click);
            // 
            // 模型ToolStripMenuItem
            // 
            this.模型ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.模型管理MToolStripMenuItem,
            this.导出模型EToolStripMenuItem,
            this.架构管理SToolStripMenuItem,
            this.sQL查询器QToolStripMenuItem});
            this.模型ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.模型ToolStripMenuItem.Name = "模型ToolStripMenuItem";
            this.模型ToolStripMenuItem.Size = new System.Drawing.Size(80, 23);
            this.模型ToolStripMenuItem.Text = "模型(&M)";
            this.模型ToolStripMenuItem.Visible = false;
            // 
            // 模型管理MToolStripMenuItem
            // 
            this.模型管理MToolStripMenuItem.Name = "模型管理MToolStripMenuItem";
            this.模型管理MToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.模型管理MToolStripMenuItem.Text = "模型管理(&M)";
            this.模型管理MToolStripMenuItem.Click += new System.EventHandler(this.模型管理MToolStripMenuItem_Click);
            // 
            // 导出模型EToolStripMenuItem
            // 
            this.导出模型EToolStripMenuItem.Name = "导出模型EToolStripMenuItem";
            this.导出模型EToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.导出模型EToolStripMenuItem.Text = "导出模型(&E)";
            this.导出模型EToolStripMenuItem.Click += new System.EventHandler(this.导出模型EToolStripMenuItem_Click);
            // 
            // 架构管理SToolStripMenuItem
            // 
            this.架构管理SToolStripMenuItem.Name = "架构管理SToolStripMenuItem";
            this.架构管理SToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.架构管理SToolStripMenuItem.Text = "架构管理(&S)";
            this.架构管理SToolStripMenuItem.Visible = false;
            this.架构管理SToolStripMenuItem.Click += new System.EventHandler(this.架构管理SToolStripMenuItem_Click);
            // 
            // sQL查询器QToolStripMenuItem
            // 
            this.sQL查询器QToolStripMenuItem.Name = "sQL查询器QToolStripMenuItem";
            this.sQL查询器QToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.sQL查询器QToolStripMenuItem.Text = "SQL查询器(&Q)";
            this.sQL查询器QToolStripMenuItem.Click += new System.EventHandler(this.sQL查询器QToolStripMenuItem_Click);
            // 
            // 关于ToolStripMenuItem
            // 
            this.关于ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.组件手册ToolStripMenuItem,
            this.表名字段名命名规范ToolStripMenuItem,
            this.在线帮助文档ToolStripMenuItem,
            this.toolStripMenuItem2,
            this.qQ群1600800ToolStripMenuItem,
            this.qQ群1600800ToolStripMenuItem1,
            this.博客ToolStripMenuItem,
            this.关于ToolStripMenuItem1});
            this.关于ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            this.关于ToolStripMenuItem.Size = new System.Drawing.Size(77, 23);
            this.关于ToolStripMenuItem.Text = "帮助(&H)";
            // 
            // 组件手册ToolStripMenuItem
            // 
            this.组件手册ToolStripMenuItem.Name = "组件手册ToolStripMenuItem";
            this.组件手册ToolStripMenuItem.Size = new System.Drawing.Size(275, 26);
            this.组件手册ToolStripMenuItem.Text = "组件手册(&X)";
            this.组件手册ToolStripMenuItem.Click += new System.EventHandler(this.组件手册ToolStripMenuItem_Click);
            // 
            // 表名字段名命名规范ToolStripMenuItem
            // 
            this.表名字段名命名规范ToolStripMenuItem.Name = "表名字段名命名规范ToolStripMenuItem";
            this.表名字段名命名规范ToolStripMenuItem.Size = new System.Drawing.Size(275, 26);
            this.表名字段名命名规范ToolStripMenuItem.Text = "表名字段名命名规范(&N)";
            this.表名字段名命名规范ToolStripMenuItem.Click += new System.EventHandler(this.表名字段名命名规范ToolStripMenuItem_Click);
            // 
            // 在线帮助文档ToolStripMenuItem
            // 
            this.在线帮助文档ToolStripMenuItem.Name = "在线帮助文档ToolStripMenuItem";
            this.在线帮助文档ToolStripMenuItem.Size = new System.Drawing.Size(275, 26);
            this.在线帮助文档ToolStripMenuItem.Text = "在线帮助文档";
            this.在线帮助文档ToolStripMenuItem.Click += new System.EventHandler(this.在线帮助文档ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(272, 6);
            // 
            // qQ群1600800ToolStripMenuItem
            // 
            this.qQ群1600800ToolStripMenuItem.Name = "qQ群1600800ToolStripMenuItem";
            this.qQ群1600800ToolStripMenuItem.Size = new System.Drawing.Size(275, 26);
            this.qQ群1600800ToolStripMenuItem.Text = "论坛www.NewLifeX.com";
            this.qQ群1600800ToolStripMenuItem.Click += new System.EventHandler(this.qQ群1600800ToolStripMenuItem_Click);
            // 
            // qQ群1600800ToolStripMenuItem1
            // 
            this.qQ群1600800ToolStripMenuItem1.Name = "qQ群1600800ToolStripMenuItem1";
            this.qQ群1600800ToolStripMenuItem1.Size = new System.Drawing.Size(275, 26);
            this.qQ群1600800ToolStripMenuItem1.Text = "QQ群1600800";
            // 
            // 博客ToolStripMenuItem
            // 
            this.博客ToolStripMenuItem.Name = "博客ToolStripMenuItem";
            this.博客ToolStripMenuItem.Size = new System.Drawing.Size(275, 26);
            this.博客ToolStripMenuItem.Text = "博客nnhy.cnblogs.com";
            this.博客ToolStripMenuItem.Click += new System.EventHandler(this.博客ToolStripMenuItem_Click);
            // 
            // 关于ToolStripMenuItem1
            // 
            this.关于ToolStripMenuItem1.Name = "关于ToolStripMenuItem1";
            this.关于ToolStripMenuItem1.Size = new System.Drawing.Size(275, 26);
            this.关于ToolStripMenuItem1.Text = "关于(&A)";
            this.关于ToolStripMenuItem1.Click += new System.EventHandler(this.关于ToolStripMenuItem1_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label9.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label9.Location = new System.Drawing.Point(695, 43);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(84, 19);
            this.label9.TabIndex = 32;
            this.label9.Text = "两种用法：";
            // 
            // FrmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1095, 393);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.gbConfig);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.gbTable);
            this.Controls.Add(this.gbConnect);
            this.Controls.Add(this.bt_Connection);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmMain";
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据模型工具";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.Shown += new System.EventHandler(this.FrmMain_Shown);
            this.gbConnect.ResumeLayout(false);
            this.gbConnect.PerformLayout();
            this.gbTable.ResumeLayout(false);
            this.gbTable.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.gbConfig.ResumeLayout(false);
            this.gbConfig.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button bt_Connection;
		private System.Windows.Forms.GroupBox gbConnect;
		private System.Windows.Forms.GroupBox gbTable;
		private System.Windows.Forms.Button btnRenderTable;
		private System.Windows.Forms.ComboBox cbTableList;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnRenderAll;
		private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lb_Status;
        private System.Windows.Forms.GroupBox gbConfig;
        private System.Windows.Forms.TextBox txt_ConnName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_NameSpace;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txt_OutPath;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbConn;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnOpenOutputDir;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TextBox txtBaseClass;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox cbRenderGenEntity;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 模型ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 关于ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出XToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 模型管理MToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导出模型EToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 架构管理SToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sQL查询器QToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 表名字段名命名规范ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem 关于ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 组件手册ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem qQ群1600800ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 博客ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oracle客户端运行时检查ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem qQ群1600800ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 自动格式化设置ToolStripMenuItem;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnRefreshTable;
        private System.Windows.Forms.CheckBox cbIncludeView;
        private System.Windows.Forms.ToolStripMenuItem 在线帮助文档ToolStripMenuItem;
	}
}

