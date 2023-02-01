﻿namespace XCoder.Data
{
    partial class FrmRedis
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
            this.components = new System.ComponentModel.Container();
            this.gbFunc = new System.Windows.Forms.GroupBox();
            this.treeNodes = new System.Windows.Forms.TreeView();
            this.menuNodes = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuEditNode = new System.Windows.Forms.ToolStripMenuItem();
            this.gbContent = new System.Windows.Forms.GroupBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.gbFunc.SuspendLayout();
            this.menuNodes.SuspendLayout();
            this.gbContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbFunc
            // 
            this.gbFunc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbFunc.Controls.Add(this.treeNodes);
            this.gbFunc.Location = new System.Drawing.Point(13, 9);
            this.gbFunc.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbFunc.Name = "gbFunc";
            this.gbFunc.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbFunc.Size = new System.Drawing.Size(258, 559);
            this.gbFunc.TabIndex = 0;
            this.gbFunc.TabStop = false;
            // 
            // treeNodes
            // 
            this.treeNodes.ContextMenuStrip = this.menuNodes;
            this.treeNodes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeNodes.Location = new System.Drawing.Point(4, 24);
            this.treeNodes.Name = "treeNodes";
            this.treeNodes.Size = new System.Drawing.Size(250, 531);
            this.treeNodes.TabIndex = 0;
            this.treeNodes.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeNodes_MouseDoubleClick);
            // 
            // menuNodes
            // 
            this.menuNodes.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuNodes.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuEditNode});
            this.menuNodes.Name = "menuNodes";
            this.menuNodes.Size = new System.Drawing.Size(154, 28);
            this.menuNodes.Opening += new System.ComponentModel.CancelEventHandler(this.menuNodes_Opening);
            // 
            // menuEditNode
            // 
            this.menuEditNode.Name = "menuEditNode";
            this.menuEditNode.Size = new System.Drawing.Size(153, 24);
            this.menuEditNode.Text = "添加服务器";
            this.menuEditNode.Click += new System.EventHandler(this.menuEditNode_Click);
            // 
            // gbContent
            // 
            this.gbContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbContent.Controls.Add(this.richTextBox1);
            this.gbContent.Location = new System.Drawing.Point(277, 9);
            this.gbContent.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gbContent.Name = "gbContent";
            this.gbContent.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gbContent.Size = new System.Drawing.Size(741, 555);
            this.gbContent.TabIndex = 1;
            this.gbContent.TabStop = false;
            this.gbContent.Text = "内容";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(5, 23);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(732, 123);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // FrmRedis
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1033, 581);
            this.Controls.Add(this.gbContent);
            this.Controls.Add(this.gbFunc);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FrmRedis";
            this.Text = "Redis管理";
            this.Load += new System.EventHandler(this.FrmRedis_Load);
            this.gbFunc.ResumeLayout(false);
            this.menuNodes.ResumeLayout(false);
            this.gbContent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbFunc;
        private System.Windows.Forms.TreeView treeNodes;
        private System.Windows.Forms.ContextMenuStrip menuNodes;
        private System.Windows.Forms.ToolStripMenuItem menuEditNode;
        private GroupBox gbContent;
        private RichTextBox richTextBox1;
    }
}