namespace WinModbus
{
    partial class FrmSwitch
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnOpen1 = new Button();
            btnClose1 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            button7 = new Button();
            button8 = new Button();
            groupBox2 = new GroupBox();
            btnReadIn = new Button();
            btnWriteAddr = new Button();
            numAddr = new NumericUpDown();
            btnReadAddr = new Button();
            btnReadAll = new Button();
            label1 = new Label();
            numDelay = new NumericUpDown();
            btnCloseAll = new Button();
            btnOpenAll = new Button();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numAddr).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDelay).BeginInit();
            SuspendLayout();
            // 
            // btnOpen1
            // 
            btnOpen1.Location = new Point(21, 29);
            btnOpen1.Name = "btnOpen1";
            btnOpen1.Size = new Size(82, 42);
            btnOpen1.TabIndex = 0;
            btnOpen1.Tag = "1";
            btnOpen1.Text = "打开1号";
            btnOpen1.UseVisualStyleBackColor = true;
            btnOpen1.Click += btnOpen1_Click;
            // 
            // btnClose1
            // 
            btnClose1.Location = new Point(110, 29);
            btnClose1.Name = "btnClose1";
            btnClose1.Size = new Size(82, 42);
            btnClose1.TabIndex = 1;
            btnClose1.Tag = "1";
            btnClose1.Text = "关闭1号";
            btnClose1.UseVisualStyleBackColor = true;
            btnClose1.Click += btnClose1_Click;
            // 
            // button3
            // 
            button3.Location = new Point(110, 78);
            button3.Name = "button3";
            button3.Size = new Size(82, 42);
            button3.TabIndex = 3;
            button3.Tag = "2";
            button3.Text = "关闭2号";
            button3.UseVisualStyleBackColor = true;
            button3.Click += btnClose1_Click;
            // 
            // button4
            // 
            button4.Location = new Point(21, 78);
            button4.Name = "button4";
            button4.Size = new Size(82, 42);
            button4.TabIndex = 2;
            button4.Tag = "2";
            button4.Text = "打开2号";
            button4.UseVisualStyleBackColor = true;
            button4.Click += btnOpen1_Click;
            // 
            // button5
            // 
            button5.Location = new Point(110, 126);
            button5.Name = "button5";
            button5.Size = new Size(82, 42);
            button5.TabIndex = 5;
            button5.Tag = "3";
            button5.Text = "关闭3号";
            button5.UseVisualStyleBackColor = true;
            button5.Click += btnClose1_Click;
            // 
            // button6
            // 
            button6.Location = new Point(21, 126);
            button6.Name = "button6";
            button6.Size = new Size(82, 42);
            button6.TabIndex = 4;
            button6.Tag = "3";
            button6.Text = "打开3号";
            button6.UseVisualStyleBackColor = true;
            button6.Click += btnOpen1_Click;
            // 
            // button7
            // 
            button7.Location = new Point(110, 174);
            button7.Name = "button7";
            button7.Size = new Size(82, 42);
            button7.TabIndex = 7;
            button7.Tag = "4";
            button7.Text = "关闭4号";
            button7.UseVisualStyleBackColor = true;
            button7.Click += btnClose1_Click;
            // 
            // button8
            // 
            button8.Location = new Point(21, 174);
            button8.Name = "button8";
            button8.Size = new Size(82, 42);
            button8.TabIndex = 6;
            button8.Tag = "4";
            button8.Text = "打开4号";
            button8.UseVisualStyleBackColor = true;
            button8.Click += btnOpen1_Click;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(btnReadIn);
            groupBox2.Controls.Add(btnWriteAddr);
            groupBox2.Controls.Add(numAddr);
            groupBox2.Controls.Add(btnReadAddr);
            groupBox2.Controls.Add(btnReadAll);
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(numDelay);
            groupBox2.Controls.Add(btnCloseAll);
            groupBox2.Controls.Add(btnOpenAll);
            groupBox2.Controls.Add(button3);
            groupBox2.Controls.Add(btnOpen1);
            groupBox2.Controls.Add(btnClose1);
            groupBox2.Controls.Add(button7);
            groupBox2.Controls.Add(button4);
            groupBox2.Controls.Add(button8);
            groupBox2.Controls.Add(button6);
            groupBox2.Controls.Add(button5);
            groupBox2.Location = new Point(12, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(495, 226);
            groupBox2.TabIndex = 12;
            groupBox2.TabStop = false;
            groupBox2.Text = "功能区";
            // 
            // btnReadIn
            // 
            btnReadIn.Location = new Point(331, 174);
            btnReadIn.Name = "btnReadIn";
            btnReadIn.Size = new Size(82, 42);
            btnReadIn.TabIndex = 19;
            btnReadIn.Tag = "1";
            btnReadIn.Text = "读光耦";
            btnReadIn.UseVisualStyleBackColor = true;
            btnReadIn.Click += btnReadIn_Click;
            // 
            // btnWriteAddr
            // 
            btnWriteAddr.Location = new Point(331, 78);
            btnWriteAddr.Name = "btnWriteAddr";
            btnWriteAddr.Size = new Size(82, 42);
            btnWriteAddr.TabIndex = 18;
            btnWriteAddr.Tag = "1";
            btnWriteAddr.Text = "写地址";
            btnWriteAddr.UseVisualStyleBackColor = true;
            btnWriteAddr.Click += btnWriteAddr_Click;
            // 
            // numAddr
            // 
            numAddr.Location = new Point(430, 38);
            numAddr.Margin = new Padding(2);
            numAddr.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numAddr.Name = "numAddr";
            numAddr.Size = new Size(64, 27);
            numAddr.TabIndex = 17;
            numAddr.TextAlign = HorizontalAlignment.Right;
            // 
            // btnReadAddr
            // 
            btnReadAddr.Location = new Point(331, 29);
            btnReadAddr.Name = "btnReadAddr";
            btnReadAddr.Size = new Size(82, 42);
            btnReadAddr.TabIndex = 16;
            btnReadAddr.Tag = "1";
            btnReadAddr.Text = "读地址";
            btnReadAddr.UseVisualStyleBackColor = true;
            btnReadAddr.Click += btnReadAddr_Click;
            // 
            // btnReadAll
            // 
            btnReadAll.Location = new Point(219, 174);
            btnReadAll.Name = "btnReadAll";
            btnReadAll.Size = new Size(82, 42);
            btnReadAll.TabIndex = 15;
            btnReadAll.Tag = "1";
            btnReadAll.Text = "读继电器";
            btnReadAll.UseVisualStyleBackColor = true;
            btnReadAll.Click += btnReadAll_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(219, 16);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(99, 20);
            label1.TabIndex = 14;
            label1.Text = "延时（毫秒）";
            // 
            // numDelay
            // 
            numDelay.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            numDelay.Location = new Point(219, 38);
            numDelay.Margin = new Padding(2);
            numDelay.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            numDelay.Name = "numDelay";
            numDelay.Size = new Size(84, 27);
            numDelay.TabIndex = 13;
            numDelay.TextAlign = HorizontalAlignment.Right;
            // 
            // btnCloseAll
            // 
            btnCloseAll.Location = new Point(219, 126);
            btnCloseAll.Name = "btnCloseAll";
            btnCloseAll.Size = new Size(82, 42);
            btnCloseAll.TabIndex = 9;
            btnCloseAll.Tag = "1";
            btnCloseAll.Text = "全部关闭";
            btnCloseAll.UseVisualStyleBackColor = true;
            btnCloseAll.Click += btnCloseAll_Click;
            // 
            // btnOpenAll
            // 
            btnOpenAll.Location = new Point(219, 78);
            btnOpenAll.Name = "btnOpenAll";
            btnOpenAll.Size = new Size(82, 42);
            btnOpenAll.TabIndex = 8;
            btnOpenAll.Tag = "1";
            btnOpenAll.Text = "全部打开";
            btnOpenAll.UseVisualStyleBackColor = true;
            btnOpenAll.Click += btnOpenAll_Click;
            // 
            // FrmSwitch
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(515, 250);
            Controls.Add(groupBox2);
            Name = "FrmSwitch";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "开关控制器";
            Load += Form1_Load;
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numAddr).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDelay).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnOpen1;
        private Button btnClose1;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private Button button7;
        private Button button8;
        private GroupBox groupBox2;
        private Button btnCloseAll;
        private Button btnOpenAll;
        private NumericUpDown numDelay;
        private Label label1;
        private Button btnReadAll;
        private Button btnWriteAddr;
        private NumericUpDown numAddr;
        private Button btnReadAddr;
        private Button btnReadIn;
    }
}