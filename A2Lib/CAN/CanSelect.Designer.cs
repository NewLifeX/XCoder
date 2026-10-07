namespace ALib.CAN
{
    partial class CanSelect
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
            if (disposing)
            {
                // 先关硬件再走基类（UserControl 销毁不一定触发 FormClosed）
                ClosePort();

                if (components != null) components.Dispose();
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
            cbCan = new ComboBox();
            label4 = new Label();
            label3 = new Label();
            cbBaudrate = new ComboBox();
            btCanOpen = new Button();
            SuspendLayout();
            // 
            // cbCan
            // 
            cbCan.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbCan.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCan.FormattingEnabled = true;
            cbCan.Location = new Point(0, 23);
            cbCan.Name = "cbCan";
            cbCan.Size = new Size(174, 25);
            cbCan.TabIndex = 33;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(0, 7);
            label4.Name = "label4";
            label4.Size = new Size(34, 17);
            label4.TabIndex = 32;
            label4.Text = "CAN";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(0, 55);
            label3.Name = "label3";
            label3.Size = new Size(61, 17);
            label3.TabIndex = 34;
            label3.Text = "Baudrate";
            // 
            // cbBaudrate
            // 
            cbBaudrate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbBaudrate.DropDownStyle = ComboBoxStyle.DropDownList;
            cbBaudrate.FormattingEnabled = true;
            cbBaudrate.Location = new Point(0, 75);
            cbBaudrate.Name = "cbBaudrate";
            cbBaudrate.Size = new Size(174, 25);
            cbBaudrate.TabIndex = 35;
            // 
            // btCanOpen
            // 
            btCanOpen.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btCanOpen.Location = new Point(0, 122);
            btCanOpen.Name = "btCanOpen";
            btCanOpen.Size = new Size(174, 29);
            btCanOpen.TabIndex = 36;
            btCanOpen.Text = "打开";
            btCanOpen.UseVisualStyleBackColor = true;
            btCanOpen.Click += btCanOpen_Click;
            // 
            // CanSelect
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btCanOpen);
            Controls.Add(cbBaudrate);
            Controls.Add(label3);
            Controls.Add(cbCan);
            Controls.Add(label4);
            Name = "CanSelect";
            Size = new Size(174, 164);
            Load += CanSelect_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cbCan;
        private Label label4;
        private Label label3;
        private ComboBox cbBaudrate;
        private Button btCanOpen;
    }
}
