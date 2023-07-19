namespace WinModbus
{
    partial class InputPort
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
            btnClose = new Button();
            btnOpen = new Button();
            checkBox1 = new CheckBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // btnClose
            // 
            btnClose.Location = new Point(65, 68);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(47, 42);
            btnClose.TabIndex = 18;
            btnClose.Tag = "1";
            btnClose.Text = "关";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // btnOpen
            // 
            btnOpen.Location = new Point(12, 68);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(47, 42);
            btnOpen.TabIndex = 17;
            btnOpen.Tag = "1";
            btnOpen.Text = "开";
            btnOpen.UseVisualStyleBackColor = true;
            btnOpen.Click += btnOpen_Click;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            checkBox1.ForeColor = Color.Green;
            checkBox1.Location = new Point(43, 31);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(54, 31);
            checkBox1.TabIndex = 19;
            checkBox1.Text = "开";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(43, 0);
            label1.Name = "label1";
            label1.Size = new Size(33, 20);
            label1.TabIndex = 20;
            label1.Text = "1号";
            // 
            // Relay
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(label1);
            Controls.Add(checkBox1);
            Controls.Add(btnClose);
            Controls.Add(btnOpen);
            Name = "Relay";
            Size = new Size(120, 120);
            Load += Relay_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnClose;
        private Button btnOpen;
        private CheckBox checkBox1;
        private Label label1;
    }
}
