namespace XCoder.Tools
{
    partial class FrmSymmetric
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
            gbFunc = new GroupBox();
            btn3DES2 = new Button();
            btn3DES = new Button();
            btnRijndael2 = new Button();
            btnRijndael = new Button();
            btnRC22 = new Button();
            btnRC2 = new Button();
            cmbPadding = new ComboBox();
            label2 = new Label();
            cmbCipher = new ComboBox();
            label1 = new Label();
            btnRC42 = new Button();
            btnRC4 = new Button();
            btnAES2 = new Button();
            btnAES = new Button();
            btnDES2 = new Button();
            btnDES = new Button();
            gbSource = new GroupBox();
            btnExchange = new Button();
            rbBase64 = new RadioButton();
            rbHex = new RadioButton();
            rbString = new RadioButton();
            rtSource = new RichTextBox();
            gbResult = new GroupBox();
            cbBase64 = new CheckBox();
            cbHex = new CheckBox();
            cbString = new CheckBox();
            rtResult = new RichTextBox();
            gbPass = new GroupBox();
            rbBase642 = new RadioButton();
            rbHex2 = new RadioButton();
            rbString2 = new RadioButton();
            rtPass = new RichTextBox();
            btnSM42 = new Button();
            btnSM4 = new Button();
            gbFunc.SuspendLayout();
            gbSource.SuspendLayout();
            gbResult.SuspendLayout();
            gbPass.SuspendLayout();
            SuspendLayout();
            // 
            // gbFunc
            // 
            gbFunc.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            gbFunc.Controls.Add(btnSM42);
            gbFunc.Controls.Add(btnSM4);
            gbFunc.Controls.Add(btn3DES2);
            gbFunc.Controls.Add(btn3DES);
            gbFunc.Controls.Add(btnRijndael2);
            gbFunc.Controls.Add(btnRijndael);
            gbFunc.Controls.Add(btnRC22);
            gbFunc.Controls.Add(btnRC2);
            gbFunc.Controls.Add(cmbPadding);
            gbFunc.Controls.Add(label2);
            gbFunc.Controls.Add(cmbCipher);
            gbFunc.Controls.Add(label1);
            gbFunc.Controls.Add(btnRC42);
            gbFunc.Controls.Add(btnRC4);
            gbFunc.Controls.Add(btnAES2);
            gbFunc.Controls.Add(btnAES);
            gbFunc.Controls.Add(btnDES2);
            gbFunc.Controls.Add(btnDES);
            gbFunc.Location = new Point(18, 20);
            gbFunc.Margin = new Padding(4);
            gbFunc.Name = "gbFunc";
            gbFunc.Padding = new Padding(4);
            gbFunc.Size = new Size(258, 1009);
            gbFunc.TabIndex = 0;
            gbFunc.TabStop = false;
            gbFunc.Text = "加密解密";
            // 
            // btn3DES2
            // 
            btn3DES2.Location = new Point(138, 446);
            btn3DES2.Margin = new Padding(4);
            btn3DES2.Name = "btn3DES2";
            btn3DES2.Size = new Size(112, 50);
            btn3DES2.TabIndex = 23;
            btn3DES2.Text = "3DES解密";
            btn3DES2.UseVisualStyleBackColor = true;
            btn3DES2.Click += btn3DES2_Click;
            // 
            // btn3DES
            // 
            btn3DES.Location = new Point(17, 446);
            btn3DES.Margin = new Padding(4);
            btn3DES.Name = "btn3DES";
            btn3DES.Size = new Size(112, 50);
            btn3DES.TabIndex = 22;
            btn3DES.Text = "3DES加密";
            btn3DES.UseVisualStyleBackColor = true;
            btn3DES.Click += btn3DES_Click;
            // 
            // btnRijndael2
            // 
            btnRijndael2.Location = new Point(138, 387);
            btnRijndael2.Margin = new Padding(4);
            btnRijndael2.Name = "btnRijndael2";
            btnRijndael2.Size = new Size(112, 50);
            btnRijndael2.TabIndex = 21;
            btnRijndael2.Text = "Rijndael解";
            btnRijndael2.UseVisualStyleBackColor = true;
            btnRijndael2.Click += btnRijndael2_Click;
            // 
            // btnRijndael
            // 
            btnRijndael.Location = new Point(17, 387);
            btnRijndael.Margin = new Padding(4);
            btnRijndael.Name = "btnRijndael";
            btnRijndael.Size = new Size(112, 50);
            btnRijndael.TabIndex = 20;
            btnRijndael.Text = "Rijndael加";
            btnRijndael.UseVisualStyleBackColor = true;
            btnRijndael.Click += btnRijndael_Click;
            // 
            // btnRC22
            // 
            btnRC22.Location = new Point(138, 328);
            btnRC22.Margin = new Padding(4);
            btnRC22.Name = "btnRC22";
            btnRC22.Size = new Size(112, 50);
            btnRC22.TabIndex = 19;
            btnRC22.Text = "RC2解密";
            btnRC22.UseVisualStyleBackColor = true;
            btnRC22.Click += btnRC22_Click;
            // 
            // btnRC2
            // 
            btnRC2.Location = new Point(17, 328);
            btnRC2.Margin = new Padding(4);
            btnRC2.Name = "btnRC2";
            btnRC2.Size = new Size(112, 50);
            btnRC2.TabIndex = 18;
            btnRC2.Text = "RC2加密";
            btnRC2.UseVisualStyleBackColor = true;
            btnRC2.Click += btnRC2_Click;
            // 
            // cmbPadding
            // 
            cmbPadding.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPadding.FormattingEnabled = true;
            cmbPadding.Location = new Point(129, 101);
            cmbPadding.Name = "cmbPadding";
            cmbPadding.Size = new Size(121, 32);
            cmbPadding.TabIndex = 17;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(25, 106);
            label2.Name = "label2";
            label2.Size = new Size(100, 24);
            label2.TabIndex = 16;
            label2.Text = "填充类型：";
            // 
            // cmbCipher
            // 
            cmbCipher.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCipher.FormattingEnabled = true;
            cmbCipher.Location = new Point(129, 47);
            cmbCipher.Name = "cmbCipher";
            cmbCipher.Size = new Size(121, 32);
            cmbCipher.TabIndex = 15;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(7, 51);
            label1.Name = "label1";
            label1.Size = new Size(118, 24);
            label1.TabIndex = 14;
            label1.Text = "块密码模式：";
            // 
            // btnRC42
            // 
            btnRC42.Location = new Point(139, 269);
            btnRC42.Margin = new Padding(4);
            btnRC42.Name = "btnRC42";
            btnRC42.Size = new Size(112, 50);
            btnRC42.TabIndex = 13;
            btnRC42.Text = "RC4解密";
            btnRC42.UseVisualStyleBackColor = true;
            btnRC42.Click += btnRC42_Click;
            // 
            // btnRC4
            // 
            btnRC4.Location = new Point(18, 269);
            btnRC4.Margin = new Padding(4);
            btnRC4.Name = "btnRC4";
            btnRC4.Size = new Size(112, 50);
            btnRC4.TabIndex = 12;
            btnRC4.Text = "RC4加密";
            btnRC4.UseVisualStyleBackColor = true;
            btnRC4.Click += btnRC4_Click;
            // 
            // btnAES2
            // 
            btnAES2.Location = new Point(139, 211);
            btnAES2.Margin = new Padding(4);
            btnAES2.Name = "btnAES2";
            btnAES2.Size = new Size(112, 50);
            btnAES2.TabIndex = 11;
            btnAES2.Text = "AES解密";
            btnAES2.UseVisualStyleBackColor = true;
            btnAES2.Click += btnAES2_Click;
            // 
            // btnAES
            // 
            btnAES.Location = new Point(18, 211);
            btnAES.Margin = new Padding(4);
            btnAES.Name = "btnAES";
            btnAES.Size = new Size(112, 50);
            btnAES.TabIndex = 10;
            btnAES.Text = "AES加密";
            btnAES.UseVisualStyleBackColor = true;
            btnAES.Click += btnAES_Click;
            // 
            // btnDES2
            // 
            btnDES2.Location = new Point(139, 152);
            btnDES2.Margin = new Padding(4);
            btnDES2.Name = "btnDES2";
            btnDES2.Size = new Size(112, 50);
            btnDES2.TabIndex = 9;
            btnDES2.Text = "DES解密";
            btnDES2.UseVisualStyleBackColor = true;
            btnDES2.Click += btnDES2_Click;
            // 
            // btnDES
            // 
            btnDES.Location = new Point(18, 152);
            btnDES.Margin = new Padding(4);
            btnDES.Name = "btnDES";
            btnDES.Size = new Size(112, 50);
            btnDES.TabIndex = 8;
            btnDES.Text = "DES加密";
            btnDES.UseVisualStyleBackColor = true;
            btnDES.Click += btnDES_Click;
            // 
            // gbSource
            // 
            gbSource.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbSource.Controls.Add(btnExchange);
            gbSource.Controls.Add(rbBase64);
            gbSource.Controls.Add(rbHex);
            gbSource.Controls.Add(rbString);
            gbSource.Controls.Add(rtSource);
            gbSource.Location = new Point(285, 20);
            gbSource.Margin = new Padding(4);
            gbSource.Name = "gbSource";
            gbSource.Padding = new Padding(4);
            gbSource.Size = new Size(1006, 417);
            gbSource.TabIndex = 3;
            gbSource.TabStop = false;
            gbSource.Text = "原文";
            // 
            // btnExchange
            // 
            btnExchange.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExchange.Location = new Point(894, 14);
            btnExchange.Margin = new Padding(4);
            btnExchange.Name = "btnExchange";
            btnExchange.Size = new Size(104, 57);
            btnExchange.TabIndex = 6;
            btnExchange.Text = "上下互换";
            btnExchange.UseVisualStyleBackColor = true;
            btnExchange.Click += btnExchange_Click;
            // 
            // rbBase64
            // 
            rbBase64.AutoSize = true;
            rbBase64.Location = new Point(299, 31);
            rbBase64.Margin = new Padding(2);
            rbBase64.Name = "rbBase64";
            rbBase64.Size = new Size(137, 28);
            rbBase64.TabIndex = 5;
            rbBase64.Text = "BASE64编码";
            rbBase64.UseVisualStyleBackColor = true;
            // 
            // rbHex
            // 
            rbHex.AutoSize = true;
            rbHex.Location = new Point(154, 31);
            rbHex.Margin = new Padding(2);
            rbHex.Name = "rbHex";
            rbHex.Size = new Size(107, 28);
            rbHex.TabIndex = 4;
            rbHex.Text = "HEX编码";
            rbHex.UseVisualStyleBackColor = true;
            // 
            // rbString
            // 
            rbString.AutoSize = true;
            rbString.Checked = true;
            rbString.Location = new Point(19, 31);
            rbString.Margin = new Padding(2);
            rbString.Name = "rbString";
            rbString.Size = new Size(89, 28);
            rbString.TabIndex = 3;
            rbString.TabStop = true;
            rbString.Text = "字符串";
            rbString.UseVisualStyleBackColor = true;
            // 
            // rtSource
            // 
            rtSource.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtSource.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            rtSource.Location = new Point(4, 72);
            rtSource.Margin = new Padding(4);
            rtSource.Name = "rtSource";
            rtSource.Size = new Size(998, 340);
            rtSource.TabIndex = 2;
            rtSource.Text = "学无先后达者为师";
            rtSource.TextChanged += rtSource_TextChanged;
            // 
            // gbResult
            // 
            gbResult.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gbResult.Controls.Add(cbBase64);
            gbResult.Controls.Add(cbHex);
            gbResult.Controls.Add(cbString);
            gbResult.Controls.Add(rtResult);
            gbResult.Location = new Point(285, 613);
            gbResult.Margin = new Padding(4);
            gbResult.Name = "gbResult";
            gbResult.Padding = new Padding(4);
            gbResult.Size = new Size(1006, 417);
            gbResult.TabIndex = 4;
            gbResult.TabStop = false;
            gbResult.Text = "结果";
            // 
            // cbBase64
            // 
            cbBase64.AutoSize = true;
            cbBase64.Checked = true;
            cbBase64.CheckState = CheckState.Checked;
            cbBase64.Location = new Point(299, 31);
            cbBase64.Margin = new Padding(2);
            cbBase64.Name = "cbBase64";
            cbBase64.Size = new Size(138, 28);
            cbBase64.TabIndex = 8;
            cbBase64.Text = "BASE64编码";
            cbBase64.UseVisualStyleBackColor = true;
            // 
            // cbHex
            // 
            cbHex.AutoSize = true;
            cbHex.Checked = true;
            cbHex.CheckState = CheckState.Checked;
            cbHex.Location = new Point(154, 31);
            cbHex.Margin = new Padding(2);
            cbHex.Name = "cbHex";
            cbHex.Size = new Size(108, 28);
            cbHex.TabIndex = 7;
            cbHex.Text = "HEX编码";
            cbHex.UseVisualStyleBackColor = true;
            // 
            // cbString
            // 
            cbString.AutoSize = true;
            cbString.Checked = true;
            cbString.CheckState = CheckState.Checked;
            cbString.Location = new Point(19, 31);
            cbString.Margin = new Padding(2);
            cbString.Name = "cbString";
            cbString.Size = new Size(90, 28);
            cbString.TabIndex = 6;
            cbString.Text = "字符串";
            cbString.UseVisualStyleBackColor = true;
            // 
            // rtResult
            // 
            rtResult.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtResult.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            rtResult.Location = new Point(4, 63);
            rtResult.Margin = new Padding(4);
            rtResult.Name = "rtResult";
            rtResult.Size = new Size(998, 348);
            rtResult.TabIndex = 2;
            rtResult.Text = "";
            // 
            // gbPass
            // 
            gbPass.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbPass.Controls.Add(rbBase642);
            gbPass.Controls.Add(rbHex2);
            gbPass.Controls.Add(rbString2);
            gbPass.Controls.Add(rtPass);
            gbPass.Location = new Point(285, 442);
            gbPass.Margin = new Padding(4);
            gbPass.Name = "gbPass";
            gbPass.Padding = new Padding(4);
            gbPass.Size = new Size(1006, 167);
            gbPass.TabIndex = 5;
            gbPass.TabStop = false;
            gbPass.Text = "密码";
            // 
            // rbBase642
            // 
            rbBase642.AutoSize = true;
            rbBase642.Location = new Point(299, 33);
            rbBase642.Margin = new Padding(2);
            rbBase642.Name = "rbBase642";
            rbBase642.Size = new Size(137, 28);
            rbBase642.TabIndex = 8;
            rbBase642.Text = "BASE64编码";
            rbBase642.UseVisualStyleBackColor = true;
            // 
            // rbHex2
            // 
            rbHex2.AutoSize = true;
            rbHex2.Location = new Point(155, 33);
            rbHex2.Margin = new Padding(2);
            rbHex2.Name = "rbHex2";
            rbHex2.Size = new Size(107, 28);
            rbHex2.TabIndex = 7;
            rbHex2.Text = "HEX编码";
            rbHex2.UseVisualStyleBackColor = true;
            // 
            // rbString2
            // 
            rbString2.AutoSize = true;
            rbString2.Checked = true;
            rbString2.Location = new Point(19, 33);
            rbString2.Margin = new Padding(2);
            rbString2.Name = "rbString2";
            rbString2.Size = new Size(89, 28);
            rbString2.TabIndex = 6;
            rbString2.TabStop = true;
            rbString2.Text = "字符串";
            rbString2.UseVisualStyleBackColor = true;
            // 
            // rtPass
            // 
            rtPass.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtPass.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            rtPass.Location = new Point(4, 71);
            rtPass.Margin = new Padding(4);
            rtPass.Name = "rtPass";
            rtPass.Size = new Size(998, 91);
            rtPass.TabIndex = 2;
            rtPass.Text = "NewLife";
            // 
            // btnSM42
            // 
            btnSM42.Location = new Point(138, 504);
            btnSM42.Margin = new Padding(4);
            btnSM42.Name = "btnSM42";
            btnSM42.Size = new Size(112, 50);
            btnSM42.TabIndex = 25;
            btnSM42.Text = "SM4解密";
            btnSM42.UseVisualStyleBackColor = true;
            btnSM42.Click += btnSM42_Click;
            // 
            // btnSM4
            // 
            btnSM4.Location = new Point(17, 504);
            btnSM4.Margin = new Padding(4);
            btnSM4.Name = "btnSM4";
            btnSM4.Size = new Size(112, 50);
            btnSM4.TabIndex = 24;
            btnSM4.Text = "SM4加密";
            btnSM4.UseVisualStyleBackColor = true;
            btnSM4.Click += btnSM4_Click;
            // 
            // FrmSymmetric
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1304, 1049);
            Controls.Add(gbPass);
            Controls.Add(gbResult);
            Controls.Add(gbSource);
            Controls.Add(gbFunc);
            Margin = new Padding(4);
            Name = "FrmSymmetric";
            Text = "加密解密";
            Load += FrmSecurity_Load;
            gbFunc.ResumeLayout(false);
            gbFunc.PerformLayout();
            gbSource.ResumeLayout(false);
            gbSource.PerformLayout();
            gbResult.ResumeLayout(false);
            gbResult.PerformLayout();
            gbPass.ResumeLayout(false);
            gbPass.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox gbFunc;
        private GroupBox gbSource;
        private RichTextBox rtSource;
        private GroupBox gbResult;
        private RichTextBox rtResult;
        private Button btnRC42;
        private Button btnRC4;
        private Button btnAES2;
        private Button btnAES;
        private Button btnDES2;
        private Button btnDES;
        private GroupBox gbPass;
        private RichTextBox rtPass;
        private Button btnExchange;
        private CheckBox cbBase64;
        private CheckBox cbHex;
        private CheckBox cbString;
        private RadioButton rbBase64;
        private RadioButton rbHex;
        private RadioButton rbString;
        private RadioButton rbBase642;
        private RadioButton rbHex2;
        private RadioButton rbString2;
        private ComboBox cmbPadding;
        private Label label2;
        private ComboBox cmbCipher;
        private Label label1;
        private Button btn3DES2;
        private Button btn3DES;
        private Button btnRijndael2;
        private Button btnRijndael;
        private Button btnRC22;
        private Button btnRC2;
        private Button btnSM42;
        private Button btnSM4;
    }
}