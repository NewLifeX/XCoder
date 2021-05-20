namespace XCoder.Data
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
            this.gbSource = new System.Windows.Forms.GroupBox();
            this.btnExchange = new System.Windows.Forms.Button();
            this.rbBase64 = new System.Windows.Forms.RadioButton();
            this.rbHex = new System.Windows.Forms.RadioButton();
            this.rbString = new System.Windows.Forms.RadioButton();
            this.rtSource = new System.Windows.Forms.RichTextBox();
            this.gbResult = new System.Windows.Forms.GroupBox();
            this.cbBase64 = new System.Windows.Forms.CheckBox();
            this.cbHex = new System.Windows.Forms.CheckBox();
            this.cbString = new System.Windows.Forms.CheckBox();
            this.rtResult = new System.Windows.Forms.RichTextBox();
            this.gbPass = new System.Windows.Forms.GroupBox();
            this.rbBase642 = new System.Windows.Forms.RadioButton();
            this.rbHex2 = new System.Windows.Forms.RadioButton();
            this.rbString2 = new System.Windows.Forms.RadioButton();
            this.rtPass = new System.Windows.Forms.RichTextBox();
            this.gbFunc.SuspendLayout();
            this.menuNodes.SuspendLayout();
            this.gbSource.SuspendLayout();
            this.gbResult.SuspendLayout();
            this.gbPass.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbFunc
            // 
            this.gbFunc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbFunc.Controls.Add(this.treeNodes);
            this.gbFunc.Location = new System.Drawing.Point(18, 18);
            this.gbFunc.Margin = new System.Windows.Forms.Padding(4);
            this.gbFunc.Name = "gbFunc";
            this.gbFunc.Padding = new System.Windows.Forms.Padding(4);
            this.gbFunc.Size = new System.Drawing.Size(258, 908);
            this.gbFunc.TabIndex = 0;
            this.gbFunc.TabStop = false;
            // 
            // treeNodes
            // 
            this.treeNodes.ContextMenuStrip = this.menuNodes;
            this.treeNodes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeNodes.Location = new System.Drawing.Point(4, 25);
            this.treeNodes.Name = "treeNodes";
            this.treeNodes.Size = new System.Drawing.Size(250, 879);
            this.treeNodes.TabIndex = 0;
            this.treeNodes.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeNodes_MouseDoubleClick);
            // 
            // menuNodes
            // 
            this.menuNodes.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuNodes.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuEditNode});
            this.menuNodes.Name = "menuNodes";
            this.menuNodes.Size = new System.Drawing.Size(171, 34);
            this.menuNodes.Opening += new System.ComponentModel.CancelEventHandler(this.menuNodes_Opening);
            // 
            // menuEditNode
            // 
            this.menuEditNode.Name = "menuEditNode";
            this.menuEditNode.Size = new System.Drawing.Size(170, 30);
            this.menuEditNode.Text = "添加服务器";
            this.menuEditNode.Click += new System.EventHandler(this.menuEditNode_Click);
            // 
            // gbSource
            // 
            this.gbSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSource.Controls.Add(this.btnExchange);
            this.gbSource.Controls.Add(this.rbBase64);
            this.gbSource.Controls.Add(this.rbHex);
            this.gbSource.Controls.Add(this.rbString);
            this.gbSource.Controls.Add(this.rtSource);
            this.gbSource.Location = new System.Drawing.Point(285, 18);
            this.gbSource.Margin = new System.Windows.Forms.Padding(4);
            this.gbSource.Name = "gbSource";
            this.gbSource.Padding = new System.Windows.Forms.Padding(4);
            this.gbSource.Size = new System.Drawing.Size(1006, 375);
            this.gbSource.TabIndex = 3;
            this.gbSource.TabStop = false;
            this.gbSource.Text = "原文";
            // 
            // btnExchange
            // 
            this.btnExchange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExchange.Location = new System.Drawing.Point(894, 13);
            this.btnExchange.Margin = new System.Windows.Forms.Padding(4);
            this.btnExchange.Name = "btnExchange";
            this.btnExchange.Size = new System.Drawing.Size(104, 51);
            this.btnExchange.TabIndex = 6;
            this.btnExchange.Text = "上下互换";
            this.btnExchange.UseVisualStyleBackColor = true;
            this.btnExchange.Click += new System.EventHandler(this.btnExchange_Click);
            // 
            // rbBase64
            // 
            this.rbBase64.AutoSize = true;
            this.rbBase64.Location = new System.Drawing.Point(299, 28);
            this.rbBase64.Margin = new System.Windows.Forms.Padding(2);
            this.rbBase64.Name = "rbBase64";
            this.rbBase64.Size = new System.Drawing.Size(123, 22);
            this.rbBase64.TabIndex = 5;
            this.rbBase64.Text = "BASE64编码";
            this.rbBase64.UseVisualStyleBackColor = true;
            // 
            // rbHex
            // 
            this.rbHex.AutoSize = true;
            this.rbHex.Location = new System.Drawing.Point(154, 28);
            this.rbHex.Margin = new System.Windows.Forms.Padding(2);
            this.rbHex.Name = "rbHex";
            this.rbHex.Size = new System.Drawing.Size(96, 22);
            this.rbHex.TabIndex = 4;
            this.rbHex.Text = "HEX编码";
            this.rbHex.UseVisualStyleBackColor = true;
            // 
            // rbString
            // 
            this.rbString.AutoSize = true;
            this.rbString.Checked = true;
            this.rbString.Location = new System.Drawing.Point(19, 28);
            this.rbString.Margin = new System.Windows.Forms.Padding(2);
            this.rbString.Name = "rbString";
            this.rbString.Size = new System.Drawing.Size(87, 22);
            this.rbString.TabIndex = 3;
            this.rbString.TabStop = true;
            this.rbString.Text = "字符串";
            this.rbString.UseVisualStyleBackColor = true;
            // 
            // rtSource
            // 
            this.rtSource.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtSource.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtSource.Location = new System.Drawing.Point(4, 65);
            this.rtSource.Margin = new System.Windows.Forms.Padding(4);
            this.rtSource.Name = "rtSource";
            this.rtSource.Size = new System.Drawing.Size(998, 306);
            this.rtSource.TabIndex = 2;
            this.rtSource.Text = "学无先后达者为师";
            this.rtSource.TextChanged += new System.EventHandler(this.rtSource_TextChanged);
            // 
            // gbResult
            // 
            this.gbResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbResult.Controls.Add(this.cbBase64);
            this.gbResult.Controls.Add(this.cbHex);
            this.gbResult.Controls.Add(this.cbString);
            this.gbResult.Controls.Add(this.rtResult);
            this.gbResult.Location = new System.Drawing.Point(285, 552);
            this.gbResult.Margin = new System.Windows.Forms.Padding(4);
            this.gbResult.Name = "gbResult";
            this.gbResult.Padding = new System.Windows.Forms.Padding(4);
            this.gbResult.Size = new System.Drawing.Size(1006, 375);
            this.gbResult.TabIndex = 4;
            this.gbResult.TabStop = false;
            this.gbResult.Text = "结果";
            // 
            // cbBase64
            // 
            this.cbBase64.AutoSize = true;
            this.cbBase64.Checked = true;
            this.cbBase64.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBase64.Location = new System.Drawing.Point(299, 28);
            this.cbBase64.Margin = new System.Windows.Forms.Padding(2);
            this.cbBase64.Name = "cbBase64";
            this.cbBase64.Size = new System.Drawing.Size(124, 22);
            this.cbBase64.TabIndex = 8;
            this.cbBase64.Text = "BASE64编码";
            this.cbBase64.UseVisualStyleBackColor = true;
            // 
            // cbHex
            // 
            this.cbHex.AutoSize = true;
            this.cbHex.Checked = true;
            this.cbHex.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbHex.Location = new System.Drawing.Point(154, 28);
            this.cbHex.Margin = new System.Windows.Forms.Padding(2);
            this.cbHex.Name = "cbHex";
            this.cbHex.Size = new System.Drawing.Size(97, 22);
            this.cbHex.TabIndex = 7;
            this.cbHex.Text = "HEX编码";
            this.cbHex.UseVisualStyleBackColor = true;
            // 
            // cbString
            // 
            this.cbString.AutoSize = true;
            this.cbString.Checked = true;
            this.cbString.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbString.Location = new System.Drawing.Point(19, 28);
            this.cbString.Margin = new System.Windows.Forms.Padding(2);
            this.cbString.Name = "cbString";
            this.cbString.Size = new System.Drawing.Size(88, 22);
            this.cbString.TabIndex = 6;
            this.cbString.Text = "字符串";
            this.cbString.UseVisualStyleBackColor = true;
            // 
            // rtResult
            // 
            this.rtResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtResult.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtResult.Location = new System.Drawing.Point(4, 57);
            this.rtResult.Margin = new System.Windows.Forms.Padding(4);
            this.rtResult.Name = "rtResult";
            this.rtResult.Size = new System.Drawing.Size(998, 314);
            this.rtResult.TabIndex = 2;
            this.rtResult.Text = "";
            // 
            // gbPass
            // 
            this.gbPass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPass.Controls.Add(this.rbBase642);
            this.gbPass.Controls.Add(this.rbHex2);
            this.gbPass.Controls.Add(this.rbString2);
            this.gbPass.Controls.Add(this.rtPass);
            this.gbPass.Location = new System.Drawing.Point(285, 398);
            this.gbPass.Margin = new System.Windows.Forms.Padding(4);
            this.gbPass.Name = "gbPass";
            this.gbPass.Padding = new System.Windows.Forms.Padding(4);
            this.gbPass.Size = new System.Drawing.Size(1006, 150);
            this.gbPass.TabIndex = 5;
            this.gbPass.TabStop = false;
            this.gbPass.Text = "密码";
            // 
            // rbBase642
            // 
            this.rbBase642.AutoSize = true;
            this.rbBase642.Location = new System.Drawing.Point(299, 30);
            this.rbBase642.Margin = new System.Windows.Forms.Padding(2);
            this.rbBase642.Name = "rbBase642";
            this.rbBase642.Size = new System.Drawing.Size(123, 22);
            this.rbBase642.TabIndex = 8;
            this.rbBase642.Text = "BASE64编码";
            this.rbBase642.UseVisualStyleBackColor = true;
            // 
            // rbHex2
            // 
            this.rbHex2.AutoSize = true;
            this.rbHex2.Location = new System.Drawing.Point(155, 30);
            this.rbHex2.Margin = new System.Windows.Forms.Padding(2);
            this.rbHex2.Name = "rbHex2";
            this.rbHex2.Size = new System.Drawing.Size(96, 22);
            this.rbHex2.TabIndex = 7;
            this.rbHex2.Text = "HEX编码";
            this.rbHex2.UseVisualStyleBackColor = true;
            // 
            // rbString2
            // 
            this.rbString2.AutoSize = true;
            this.rbString2.Checked = true;
            this.rbString2.Location = new System.Drawing.Point(19, 30);
            this.rbString2.Margin = new System.Windows.Forms.Padding(2);
            this.rbString2.Name = "rbString2";
            this.rbString2.Size = new System.Drawing.Size(87, 22);
            this.rbString2.TabIndex = 6;
            this.rbString2.TabStop = true;
            this.rbString2.Text = "字符串";
            this.rbString2.UseVisualStyleBackColor = true;
            // 
            // rtPass
            // 
            this.rtPass.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtPass.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtPass.Location = new System.Drawing.Point(4, 64);
            this.rtPass.Margin = new System.Windows.Forms.Padding(4);
            this.rtPass.Name = "rtPass";
            this.rtPass.Size = new System.Drawing.Size(998, 82);
            this.rtPass.TabIndex = 2;
            this.rtPass.Text = "NewLife";
            // 
            // FrmRedis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1304, 944);
            this.Controls.Add(this.gbPass);
            this.Controls.Add(this.gbResult);
            this.Controls.Add(this.gbSource);
            this.Controls.Add(this.gbFunc);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmRedis";
            this.Text = "Redis管理";
            this.Load += new System.EventHandler(this.FrmRedis_Load);
            this.gbFunc.ResumeLayout(false);
            this.menuNodes.ResumeLayout(false);
            this.gbSource.ResumeLayout(false);
            this.gbSource.PerformLayout();
            this.gbResult.ResumeLayout(false);
            this.gbResult.PerformLayout();
            this.gbPass.ResumeLayout(false);
            this.gbPass.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbFunc;
        private System.Windows.Forms.GroupBox gbSource;
        private System.Windows.Forms.RichTextBox rtSource;
        private System.Windows.Forms.GroupBox gbResult;
        private System.Windows.Forms.RichTextBox rtResult;
        private System.Windows.Forms.GroupBox gbPass;
        private System.Windows.Forms.RichTextBox rtPass;
        private System.Windows.Forms.Button btnExchange;
        private System.Windows.Forms.CheckBox cbBase64;
        private System.Windows.Forms.CheckBox cbHex;
        private System.Windows.Forms.CheckBox cbString;
        private System.Windows.Forms.RadioButton rbBase64;
        private System.Windows.Forms.RadioButton rbHex;
        private System.Windows.Forms.RadioButton rbString;
        private System.Windows.Forms.RadioButton rbBase642;
        private System.Windows.Forms.RadioButton rbHex2;
        private System.Windows.Forms.RadioButton rbString2;
        private System.Windows.Forms.TreeView treeNodes;
        private System.Windows.Forms.ContextMenuStrip menuNodes;
        private System.Windows.Forms.ToolStripMenuItem menuEditNode;
    }
}