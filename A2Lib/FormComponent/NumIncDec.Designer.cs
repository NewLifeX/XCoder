namespace ALib.FormComponent
{
    partial class NumIncDec
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
            btDec10 = new Button();
            btDec1 = new Button();
            btInc10 = new Button();
            btInc1 = new Button();
            tbNumber = new TextBox();
            SuspendLayout();
            // 
            // btDec10
            // 
            btDec10.Location = new Point(3, 3);
            btDec10.Name = "btDec10";
            btDec10.Size = new Size(36, 28);
            btDec10.TabIndex = 0;
            btDec10.Tag = "-10";
            btDec10.Text = "<<";
            btDec10.UseVisualStyleBackColor = true;
            // 
            // btDec1
            // 
            btDec1.Location = new Point(45, 3);
            btDec1.Name = "btDec1";
            btDec1.Size = new Size(36, 28);
            btDec1.TabIndex = 1;
            btDec1.Tag = "-1";
            btDec1.Text = "<";
            btDec1.UseVisualStyleBackColor = true;
            // 
            // btInc10
            // 
            btInc10.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btInc10.Location = new Point(238, 3);
            btInc10.Name = "btInc10";
            btInc10.Size = new Size(36, 28);
            btInc10.TabIndex = 3;
            btInc10.Tag = "10";
            btInc10.Text = ">>";
            btInc10.UseVisualStyleBackColor = true;
            // 
            // btInc1
            // 
            btInc1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btInc1.Location = new Point(196, 3);
            btInc1.Name = "btInc1";
            btInc1.Size = new Size(36, 28);
            btInc1.TabIndex = 2;
            btInc1.Tag = "1";
            btInc1.Text = ">";
            btInc1.UseVisualStyleBackColor = true;
            // 
            // tbNumber
            // 
            tbNumber.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbNumber.Location = new Point(87, 6);
            tbNumber.Name = "tbNumber";
            tbNumber.Size = new Size(103, 23);
            tbNumber.TabIndex = 4;
            // 
            // NumIncDec
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tbNumber);
            Controls.Add(btInc10);
            Controls.Add(btInc1);
            Controls.Add(btDec1);
            Controls.Add(btDec10);
            Name = "NumIncDec";
            Size = new Size(277, 35);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btDec10;
        private Button btDec1;
        private Button btInc10;
        private Button btInc1;
        private TextBox tbNumber;
    }
}
