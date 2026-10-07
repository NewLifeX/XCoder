namespace ALib.Forms
{
    partial class SN_Input
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            tbSN = new TextBox();
            lbSN = new Label();
            btKey = new Button();
            SuspendLayout();
            // 
            // tbSN
            // 
            tbSN.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbSN.Font = new Font("Microsoft YaHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 134);
            tbSN.ImeMode = ImeMode.Disable;
            tbSN.Location = new Point(34, 3);
            tbSN.Name = "tbSN";
            tbSN.Size = new Size(304, 32);
            tbSN.TabIndex = 0;
            tbSN.KeyDown += tbSN_KeyDown;
            tbSN.KeyPress += tbSN_KeyPress;
            tbSN.KeyUp += tbSN_KeyUp;
            // 
            // lbSN
            // 
            lbSN.AutoSize = true;
            lbSN.Location = new Point(3, 10);
            lbSN.Name = "lbSN";
            lbSN.Size = new Size(25, 17);
            lbSN.TabIndex = 1;
            lbSN.Text = "SN";
            // 
            // btKey
            // 
            btKey.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btKey.Location = new Point(344, 3);
            btKey.Name = "btKey";
            btKey.Size = new Size(88, 31);
            btKey.TabIndex = 2;
            btKey.Text = "keyName";
            btKey.UseVisualStyleBackColor = true;
            btKey.Click += btKey_Click;
            // 
            // SN_Input
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btKey);
            Controls.Add(lbSN);
            Controls.Add(tbSN);
            Name = "SN_Input";
            Size = new Size(435, 37);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tbSN;
        private Label lbSN;
        private Button btKey;
    }
}
