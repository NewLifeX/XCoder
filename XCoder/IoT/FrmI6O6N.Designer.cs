namespace WinModbus
{
    partial class FrmI6O6N
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
            groupBox2 = new GroupBox();
            label6 = new Label();
            button1 = new Button();
            numBaudrate = new NumericUpDown();
            btnReadInfo = new Button();
            label5 = new Label();
            txtUUID = new TextBox();
            label4 = new Label();
            txtVersion = new TextBox();
            label3 = new Label();
            txtProduct = new TextBox();
            label2 = new Label();
            btnWriteAddr = new Button();
            numHost = new NumericUpDown();
            btnReadIn = new Button();
            btnReadAll = new Button();
            label1 = new Label();
            numDelay = new NumericUpDown();
            btnCloseAll = new Button();
            btnOpenAll = new Button();
            groupBox1 = new GroupBox();
            outputPort1 = new OutputPort();
            groupBox3 = new GroupBox();
            inputPort1 = new InputPort();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numBaudrate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numHost).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDelay).BeginInit();
            groupBox1.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(button1);
            groupBox2.Controls.Add(numBaudrate);
            groupBox2.Controls.Add(btnReadInfo);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(txtUUID);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(txtVersion);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(txtProduct);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(btnWriteAddr);
            groupBox2.Controls.Add(numHost);
            groupBox2.Location = new Point(8, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(901, 125);
            groupBox2.TabIndex = 12;
            groupBox2.TabStop = false;
            groupBox2.Text = "基础功能";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(383, 82);
            label6.Name = "label6";
            label6.Size = new Size(69, 20);
            label6.TabIndex = 29;
            label6.Text = "波特率：";
            // 
            // button1
            // 
            button1.Location = new Point(569, 71);
            button1.Name = "button1";
            button1.Size = new Size(82, 42);
            button1.TabIndex = 28;
            button1.Tag = "1";
            button1.Text = "修改";
            button1.UseVisualStyleBackColor = true;
            // 
            // numBaudrate
            // 
            numBaudrate.Location = new Point(457, 80);
            numBaudrate.Margin = new Padding(2);
            numBaudrate.Maximum = new decimal(new int[] { 2048000, 0, 0, 0 });
            numBaudrate.Name = "numBaudrate";
            numBaudrate.Size = new Size(107, 27);
            numBaudrate.TabIndex = 27;
            numBaudrate.TextAlign = HorizontalAlignment.Right;
            // 
            // btnReadInfo
            // 
            btnReadInfo.Location = new Point(800, 18);
            btnReadInfo.Name = "btnReadInfo";
            btnReadInfo.Size = new Size(82, 42);
            btnReadInfo.TabIndex = 26;
            btnReadInfo.Tag = "1";
            btnReadInfo.Text = "读取信息";
            btnReadInfo.UseVisualStyleBackColor = true;
            btnReadInfo.Click += btnReadInfo_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(19, 82);
            label5.Name = "label5";
            label5.Size = new Size(84, 20);
            label5.TabIndex = 25;
            label5.Text = "站号地址：";
            // 
            // txtUUID
            // 
            txtUUID.Location = new Point(496, 26);
            txtUUID.Name = "txtUUID";
            txtUUID.Size = new Size(300, 27);
            txtUUID.TabIndex = 24;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(410, 29);
            label4.Name = "label4";
            label4.Size = new Size(84, 20);
            label4.TabIndex = 23;
            label4.Text = "唯一标识：";
            // 
            // txtVersion
            // 
            txtVersion.Location = new Point(291, 26);
            txtVersion.Name = "txtVersion";
            txtVersion.Size = new Size(100, 27);
            txtVersion.TabIndex = 22;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(231, 29);
            label3.Name = "label3";
            label3.Size = new Size(54, 20);
            label3.TabIndex = 21;
            label3.Text = "版本：";
            // 
            // txtProduct
            // 
            txtProduct.Location = new Point(105, 26);
            txtProduct.Name = "txtProduct";
            txtProduct.Size = new Size(100, 27);
            txtProduct.TabIndex = 20;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(19, 29);
            label2.Name = "label2";
            label2.Size = new Size(84, 20);
            label2.TabIndex = 19;
            label2.Text = "产品类型：";
            // 
            // btnWriteAddr
            // 
            btnWriteAddr.Location = new Point(210, 71);
            btnWriteAddr.Name = "btnWriteAddr";
            btnWriteAddr.Size = new Size(82, 42);
            btnWriteAddr.TabIndex = 18;
            btnWriteAddr.Tag = "1";
            btnWriteAddr.Text = "修改";
            btnWriteAddr.UseVisualStyleBackColor = true;
            btnWriteAddr.Click += btnWriteAddr_Click;
            // 
            // numHost
            // 
            numHost.Location = new Point(105, 80);
            numHost.Margin = new Padding(2);
            numHost.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numHost.Name = "numHost";
            numHost.Size = new Size(64, 27);
            numHost.TabIndex = 17;
            numHost.TextAlign = HorizontalAlignment.Right;
            // 
            // btnReadIn
            // 
            btnReadIn.Location = new Point(813, 137);
            btnReadIn.Name = "btnReadIn";
            btnReadIn.Size = new Size(82, 42);
            btnReadIn.TabIndex = 19;
            btnReadIn.Tag = "1";
            btnReadIn.Text = "读光耦";
            btnReadIn.UseVisualStyleBackColor = true;
            btnReadIn.Click += btnReadIn_Click;
            // 
            // btnReadAll
            // 
            btnReadAll.Location = new Point(291, 172);
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
            label1.Location = new Point(695, 177);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(99, 20);
            label1.TabIndex = 14;
            label1.Text = "延时（毫秒）";
            // 
            // numDelay
            // 
            numDelay.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            numDelay.Location = new Point(798, 172);
            numDelay.Margin = new Padding(2);
            numDelay.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            numDelay.Name = "numDelay";
            numDelay.Size = new Size(84, 27);
            numDelay.TabIndex = 13;
            numDelay.TextAlign = HorizontalAlignment.Right;
            // 
            // btnCloseAll
            // 
            btnCloseAll.Location = new Point(109, 172);
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
            btnOpenAll.Location = new Point(21, 172);
            btnOpenAll.Name = "btnOpenAll";
            btnOpenAll.Size = new Size(82, 42);
            btnOpenAll.TabIndex = 8;
            btnOpenAll.Tag = "1";
            btnOpenAll.Text = "全部打开";
            btnOpenAll.UseVisualStyleBackColor = true;
            btnOpenAll.Click += btnOpenAll_Click;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(outputPort1);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(numDelay);
            groupBox1.Controls.Add(btnReadAll);
            groupBox1.Controls.Add(btnCloseAll);
            groupBox1.Controls.Add(btnOpenAll);
            groupBox1.Location = new Point(8, 143);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(901, 226);
            groupBox1.TabIndex = 13;
            groupBox1.TabStop = false;
            groupBox1.Text = "开关控制";
            // 
            // outputPort1
            // 
            outputPort1.Address = 0;
            outputPort1.Host = 0;
            outputPort1.Location = new Point(6, 26);
            outputPort1.Modbus = null;
            outputPort1.Name = "outputPort1";
            outputPort1.Size = new Size(120, 120);
            outputPort1.TabIndex = 16;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(inputPort1);
            groupBox3.Controls.Add(btnReadIn);
            groupBox3.Location = new Point(8, 375);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(901, 197);
            groupBox3.TabIndex = 14;
            groupBox3.TabStop = false;
            groupBox3.Text = "输入口";
            // 
            // inputPort1
            // 
            inputPort1.Address = 0;
            inputPort1.Host = 0;
            inputPort1.Location = new Point(6, 26);
            inputPort1.Modbus = null;
            inputPort1.Name = "inputPort1";
            inputPort1.Size = new Size(120, 120);
            inputPort1.TabIndex = 20;
            // 
            // FrmI6O6N
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(921, 584);
            Controls.Add(groupBox3);
            Controls.Add(groupBox1);
            Controls.Add(groupBox2);
            Name = "FrmI6O6N";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "开关控制器";
            Load += Form1_Load;
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numBaudrate).EndInit();
            ((System.ComponentModel.ISupportInitialize)numHost).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDelay).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private GroupBox groupBox2;
        private Button btnCloseAll;
        private Button btnOpenAll;
        private NumericUpDown numDelay;
        private Label label1;
        private Button btnReadAll;
        private Button btnWriteAddr;
        private NumericUpDown numHost;
        private Button btnReadIn;
        private GroupBox groupBox1;
        private GroupBox groupBox3;
        private TextBox txtProduct;
        private Label label2;
        private TextBox txtUUID;
        private Label label4;
        private TextBox txtVersion;
        private Label label3;
        private Label label5;
        private Button btnReadInfo;
        private Label label6;
        private Button button1;
        private NumericUpDown numBaudrate;
        private OutputPort outputPort1;
        private InputPort inputPort1;
    }
}