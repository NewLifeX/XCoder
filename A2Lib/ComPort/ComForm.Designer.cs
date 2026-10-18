namespace ALib
{
    partial class ComForm
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
            cbName = new ComboBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // cbName
            // 
            cbName.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cbName.FormattingEnabled = true;
            cbName.Location = new Point(56, 12);
            cbName.Name = "cbName";
            cbName.Size = new Size(276, 25);
            cbName.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 15);
            label1.Name = "label1";
            label1.Size = new Size(35, 17);
            label1.TabIndex = 1;
            label1.Text = "串口:";
            // 
            // ComFrom
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(label1);
            Controls.Add(cbName);
            Name = "ComFrom";
            Size = new Size(357, 49);
            Load += ComFrom_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cbName;
        private Label label1;
    }
}
