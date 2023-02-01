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
            this.gbFunc = new System.Windows.Forms.GroupBox();
            this.btn3DES2 = new System.Windows.Forms.Button();
            this.btn3DES = new System.Windows.Forms.Button();
            this.btnRijndael2 = new System.Windows.Forms.Button();
            this.btnRijndael = new System.Windows.Forms.Button();
            this.btnRC22 = new System.Windows.Forms.Button();
            this.btnRC2 = new System.Windows.Forms.Button();
            this.cmbPadding = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbCipher = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRC42 = new System.Windows.Forms.Button();
            this.btnRC4 = new System.Windows.Forms.Button();
            this.btnAES2 = new System.Windows.Forms.Button();
            this.btnAES = new System.Windows.Forms.Button();
            this.btnDES2 = new System.Windows.Forms.Button();
            this.btnDES = new System.Windows.Forms.Button();
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
            this.gbSource.SuspendLayout();
            this.gbResult.SuspendLayout();
            this.gbPass.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbFunc
            // 
            this.gbFunc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbFunc.Controls.Add(this.btn3DES2);
            this.gbFunc.Controls.Add(this.btn3DES);
            this.gbFunc.Controls.Add(this.btnRijndael2);
            this.gbFunc.Controls.Add(this.btnRijndael);
            this.gbFunc.Controls.Add(this.btnRC22);
            this.gbFunc.Controls.Add(this.btnRC2);
            this.gbFunc.Controls.Add(this.cmbPadding);
            this.gbFunc.Controls.Add(this.label2);
            this.gbFunc.Controls.Add(this.cmbCipher);
            this.gbFunc.Controls.Add(this.label1);
            this.gbFunc.Controls.Add(this.btnRC42);
            this.gbFunc.Controls.Add(this.btnRC4);
            this.gbFunc.Controls.Add(this.btnAES2);
            this.gbFunc.Controls.Add(this.btnAES);
            this.gbFunc.Controls.Add(this.btnDES2);
            this.gbFunc.Controls.Add(this.btnDES);
            this.gbFunc.Location = new System.Drawing.Point(18, 20);
            this.gbFunc.Margin = new System.Windows.Forms.Padding(4);
            this.gbFunc.Name = "gbFunc";
            this.gbFunc.Padding = new System.Windows.Forms.Padding(4);
            this.gbFunc.Size = new System.Drawing.Size(258, 1009);
            this.gbFunc.TabIndex = 0;
            this.gbFunc.TabStop = false;
            this.gbFunc.Text = "加密解密";
            // 
            // btn3DES2
            // 
            this.btn3DES2.Location = new System.Drawing.Point(138, 446);
            this.btn3DES2.Margin = new System.Windows.Forms.Padding(4);
            this.btn3DES2.Name = "btn3DES2";
            this.btn3DES2.Size = new System.Drawing.Size(112, 50);
            this.btn3DES2.TabIndex = 23;
            this.btn3DES2.Text = "3DES解密";
            this.btn3DES2.UseVisualStyleBackColor = true;
            this.btn3DES2.Click += new System.EventHandler(this.btn3DES2_Click);
            // 
            // btn3DES
            // 
            this.btn3DES.Location = new System.Drawing.Point(17, 446);
            this.btn3DES.Margin = new System.Windows.Forms.Padding(4);
            this.btn3DES.Name = "btn3DES";
            this.btn3DES.Size = new System.Drawing.Size(112, 50);
            this.btn3DES.TabIndex = 22;
            this.btn3DES.Text = "3DES加密";
            this.btn3DES.UseVisualStyleBackColor = true;
            this.btn3DES.Click += new System.EventHandler(this.btn3DES_Click);
            // 
            // btnRijndael2
            // 
            this.btnRijndael2.Location = new System.Drawing.Point(138, 387);
            this.btnRijndael2.Margin = new System.Windows.Forms.Padding(4);
            this.btnRijndael2.Name = "btnRijndael2";
            this.btnRijndael2.Size = new System.Drawing.Size(112, 50);
            this.btnRijndael2.TabIndex = 21;
            this.btnRijndael2.Text = "Rijndael解";
            this.btnRijndael2.UseVisualStyleBackColor = true;
            this.btnRijndael2.Click += new System.EventHandler(this.btnRijndael2_Click);
            // 
            // btnRijndael
            // 
            this.btnRijndael.Location = new System.Drawing.Point(17, 387);
            this.btnRijndael.Margin = new System.Windows.Forms.Padding(4);
            this.btnRijndael.Name = "btnRijndael";
            this.btnRijndael.Size = new System.Drawing.Size(112, 50);
            this.btnRijndael.TabIndex = 20;
            this.btnRijndael.Text = "Rijndael加";
            this.btnRijndael.UseVisualStyleBackColor = true;
            this.btnRijndael.Click += new System.EventHandler(this.btnRijndael_Click);
            // 
            // btnRC22
            // 
            this.btnRC22.Location = new System.Drawing.Point(138, 328);
            this.btnRC22.Margin = new System.Windows.Forms.Padding(4);
            this.btnRC22.Name = "btnRC22";
            this.btnRC22.Size = new System.Drawing.Size(112, 50);
            this.btnRC22.TabIndex = 19;
            this.btnRC22.Text = "RC2解密";
            this.btnRC22.UseVisualStyleBackColor = true;
            this.btnRC22.Click += new System.EventHandler(this.btnRC22_Click);
            // 
            // btnRC2
            // 
            this.btnRC2.Location = new System.Drawing.Point(17, 328);
            this.btnRC2.Margin = new System.Windows.Forms.Padding(4);
            this.btnRC2.Name = "btnRC2";
            this.btnRC2.Size = new System.Drawing.Size(112, 50);
            this.btnRC2.TabIndex = 18;
            this.btnRC2.Text = "RC2加密";
            this.btnRC2.UseVisualStyleBackColor = true;
            this.btnRC2.Click += new System.EventHandler(this.btnRC2_Click);
            // 
            // cmbPadding
            // 
            this.cmbPadding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPadding.FormattingEnabled = true;
            this.cmbPadding.Location = new System.Drawing.Point(129, 101);
            this.cmbPadding.Name = "cmbPadding";
            this.cmbPadding.Size = new System.Drawing.Size(121, 28);
            this.cmbPadding.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 20);
            this.label2.TabIndex = 16;
            this.label2.Text = "填充类型：";
            // 
            // cmbCipher
            // 
            this.cmbCipher.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCipher.FormattingEnabled = true;
            this.cmbCipher.Location = new System.Drawing.Point(129, 47);
            this.cmbCipher.Name = "cmbCipher";
            this.cmbCipher.Size = new System.Drawing.Size(121, 28);
            this.cmbCipher.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 20);
            this.label1.TabIndex = 14;
            this.label1.Text = "块密码模式：";
            // 
            // btnRC42
            // 
            this.btnRC42.Location = new System.Drawing.Point(139, 269);
            this.btnRC42.Margin = new System.Windows.Forms.Padding(4);
            this.btnRC42.Name = "btnRC42";
            this.btnRC42.Size = new System.Drawing.Size(112, 50);
            this.btnRC42.TabIndex = 13;
            this.btnRC42.Text = "RC4解密";
            this.btnRC42.UseVisualStyleBackColor = true;
            this.btnRC42.Click += new System.EventHandler(this.btnRC42_Click);
            // 
            // btnRC4
            // 
            this.btnRC4.Location = new System.Drawing.Point(18, 269);
            this.btnRC4.Margin = new System.Windows.Forms.Padding(4);
            this.btnRC4.Name = "btnRC4";
            this.btnRC4.Size = new System.Drawing.Size(112, 50);
            this.btnRC4.TabIndex = 12;
            this.btnRC4.Text = "RC4加密";
            this.btnRC4.UseVisualStyleBackColor = true;
            this.btnRC4.Click += new System.EventHandler(this.btnRC4_Click);
            // 
            // btnAES2
            // 
            this.btnAES2.Location = new System.Drawing.Point(139, 211);
            this.btnAES2.Margin = new System.Windows.Forms.Padding(4);
            this.btnAES2.Name = "btnAES2";
            this.btnAES2.Size = new System.Drawing.Size(112, 50);
            this.btnAES2.TabIndex = 11;
            this.btnAES2.Text = "AES解密";
            this.btnAES2.UseVisualStyleBackColor = true;
            this.btnAES2.Click += new System.EventHandler(this.btnAES2_Click);
            // 
            // btnAES
            // 
            this.btnAES.Location = new System.Drawing.Point(18, 211);
            this.btnAES.Margin = new System.Windows.Forms.Padding(4);
            this.btnAES.Name = "btnAES";
            this.btnAES.Size = new System.Drawing.Size(112, 50);
            this.btnAES.TabIndex = 10;
            this.btnAES.Text = "AES加密";
            this.btnAES.UseVisualStyleBackColor = true;
            this.btnAES.Click += new System.EventHandler(this.btnAES_Click);
            // 
            // btnDES2
            // 
            this.btnDES2.Location = new System.Drawing.Point(139, 152);
            this.btnDES2.Margin = new System.Windows.Forms.Padding(4);
            this.btnDES2.Name = "btnDES2";
            this.btnDES2.Size = new System.Drawing.Size(112, 50);
            this.btnDES2.TabIndex = 9;
            this.btnDES2.Text = "DES解密";
            this.btnDES2.UseVisualStyleBackColor = true;
            this.btnDES2.Click += new System.EventHandler(this.btnDES2_Click);
            // 
            // btnDES
            // 
            this.btnDES.Location = new System.Drawing.Point(18, 152);
            this.btnDES.Margin = new System.Windows.Forms.Padding(4);
            this.btnDES.Name = "btnDES";
            this.btnDES.Size = new System.Drawing.Size(112, 50);
            this.btnDES.TabIndex = 8;
            this.btnDES.Text = "DES加密";
            this.btnDES.UseVisualStyleBackColor = true;
            this.btnDES.Click += new System.EventHandler(this.btnDES_Click);
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
            this.gbSource.Location = new System.Drawing.Point(285, 20);
            this.gbSource.Margin = new System.Windows.Forms.Padding(4);
            this.gbSource.Name = "gbSource";
            this.gbSource.Padding = new System.Windows.Forms.Padding(4);
            this.gbSource.Size = new System.Drawing.Size(1006, 417);
            this.gbSource.TabIndex = 3;
            this.gbSource.TabStop = false;
            this.gbSource.Text = "原文";
            // 
            // btnExchange
            // 
            this.btnExchange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExchange.Location = new System.Drawing.Point(894, 14);
            this.btnExchange.Margin = new System.Windows.Forms.Padding(4);
            this.btnExchange.Name = "btnExchange";
            this.btnExchange.Size = new System.Drawing.Size(104, 57);
            this.btnExchange.TabIndex = 6;
            this.btnExchange.Text = "上下互换";
            this.btnExchange.UseVisualStyleBackColor = true;
            this.btnExchange.Click += new System.EventHandler(this.btnExchange_Click);
            // 
            // rbBase64
            // 
            this.rbBase64.AutoSize = true;
            this.rbBase64.Location = new System.Drawing.Point(299, 31);
            this.rbBase64.Margin = new System.Windows.Forms.Padding(2);
            this.rbBase64.Name = "rbBase64";
            this.rbBase64.Size = new System.Drawing.Size(115, 24);
            this.rbBase64.TabIndex = 5;
            this.rbBase64.Text = "BASE64编码";
            this.rbBase64.UseVisualStyleBackColor = true;
            // 
            // rbHex
            // 
            this.rbHex.AutoSize = true;
            this.rbHex.Location = new System.Drawing.Point(154, 31);
            this.rbHex.Margin = new System.Windows.Forms.Padding(2);
            this.rbHex.Name = "rbHex";
            this.rbHex.Size = new System.Drawing.Size(90, 24);
            this.rbHex.TabIndex = 4;
            this.rbHex.Text = "HEX编码";
            this.rbHex.UseVisualStyleBackColor = true;
            // 
            // rbString
            // 
            this.rbString.AutoSize = true;
            this.rbString.Checked = true;
            this.rbString.Location = new System.Drawing.Point(19, 31);
            this.rbString.Margin = new System.Windows.Forms.Padding(2);
            this.rbString.Name = "rbString";
            this.rbString.Size = new System.Drawing.Size(75, 24);
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
            this.rtSource.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.rtSource.Location = new System.Drawing.Point(4, 72);
            this.rtSource.Margin = new System.Windows.Forms.Padding(4);
            this.rtSource.Name = "rtSource";
            this.rtSource.Size = new System.Drawing.Size(998, 340);
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
            this.gbResult.Location = new System.Drawing.Point(285, 613);
            this.gbResult.Margin = new System.Windows.Forms.Padding(4);
            this.gbResult.Name = "gbResult";
            this.gbResult.Padding = new System.Windows.Forms.Padding(4);
            this.gbResult.Size = new System.Drawing.Size(1006, 417);
            this.gbResult.TabIndex = 4;
            this.gbResult.TabStop = false;
            this.gbResult.Text = "结果";
            // 
            // cbBase64
            // 
            this.cbBase64.AutoSize = true;
            this.cbBase64.Checked = true;
            this.cbBase64.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBase64.Location = new System.Drawing.Point(299, 31);
            this.cbBase64.Margin = new System.Windows.Forms.Padding(2);
            this.cbBase64.Name = "cbBase64";
            this.cbBase64.Size = new System.Drawing.Size(116, 24);
            this.cbBase64.TabIndex = 8;
            this.cbBase64.Text = "BASE64编码";
            this.cbBase64.UseVisualStyleBackColor = true;
            // 
            // cbHex
            // 
            this.cbHex.AutoSize = true;
            this.cbHex.Checked = true;
            this.cbHex.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbHex.Location = new System.Drawing.Point(154, 31);
            this.cbHex.Margin = new System.Windows.Forms.Padding(2);
            this.cbHex.Name = "cbHex";
            this.cbHex.Size = new System.Drawing.Size(91, 24);
            this.cbHex.TabIndex = 7;
            this.cbHex.Text = "HEX编码";
            this.cbHex.UseVisualStyleBackColor = true;
            // 
            // cbString
            // 
            this.cbString.AutoSize = true;
            this.cbString.Checked = true;
            this.cbString.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbString.Location = new System.Drawing.Point(19, 31);
            this.cbString.Margin = new System.Windows.Forms.Padding(2);
            this.cbString.Name = "cbString";
            this.cbString.Size = new System.Drawing.Size(76, 24);
            this.cbString.TabIndex = 6;
            this.cbString.Text = "字符串";
            this.cbString.UseVisualStyleBackColor = true;
            // 
            // rtResult
            // 
            this.rtResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtResult.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.rtResult.Location = new System.Drawing.Point(4, 63);
            this.rtResult.Margin = new System.Windows.Forms.Padding(4);
            this.rtResult.Name = "rtResult";
            this.rtResult.Size = new System.Drawing.Size(998, 348);
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
            this.gbPass.Location = new System.Drawing.Point(285, 442);
            this.gbPass.Margin = new System.Windows.Forms.Padding(4);
            this.gbPass.Name = "gbPass";
            this.gbPass.Padding = new System.Windows.Forms.Padding(4);
            this.gbPass.Size = new System.Drawing.Size(1006, 167);
            this.gbPass.TabIndex = 5;
            this.gbPass.TabStop = false;
            this.gbPass.Text = "密码";
            // 
            // rbBase642
            // 
            this.rbBase642.AutoSize = true;
            this.rbBase642.Location = new System.Drawing.Point(299, 33);
            this.rbBase642.Margin = new System.Windows.Forms.Padding(2);
            this.rbBase642.Name = "rbBase642";
            this.rbBase642.Size = new System.Drawing.Size(115, 24);
            this.rbBase642.TabIndex = 8;
            this.rbBase642.Text = "BASE64编码";
            this.rbBase642.UseVisualStyleBackColor = true;
            // 
            // rbHex2
            // 
            this.rbHex2.AutoSize = true;
            this.rbHex2.Location = new System.Drawing.Point(155, 33);
            this.rbHex2.Margin = new System.Windows.Forms.Padding(2);
            this.rbHex2.Name = "rbHex2";
            this.rbHex2.Size = new System.Drawing.Size(90, 24);
            this.rbHex2.TabIndex = 7;
            this.rbHex2.Text = "HEX编码";
            this.rbHex2.UseVisualStyleBackColor = true;
            // 
            // rbString2
            // 
            this.rbString2.AutoSize = true;
            this.rbString2.Checked = true;
            this.rbString2.Location = new System.Drawing.Point(19, 33);
            this.rbString2.Margin = new System.Windows.Forms.Padding(2);
            this.rbString2.Name = "rbString2";
            this.rbString2.Size = new System.Drawing.Size(75, 24);
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
            this.rtPass.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.rtPass.Location = new System.Drawing.Point(4, 71);
            this.rtPass.Margin = new System.Windows.Forms.Padding(4);
            this.rtPass.Name = "rtPass";
            this.rtPass.Size = new System.Drawing.Size(998, 91);
            this.rtPass.TabIndex = 2;
            this.rtPass.Text = "NewLife";
            // 
            // FrmSymmetric
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1304, 1049);
            this.Controls.Add(this.gbPass);
            this.Controls.Add(this.gbResult);
            this.Controls.Add(this.gbSource);
            this.Controls.Add(this.gbFunc);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmSymmetric";
            this.Text = "加密解密";
            this.Load += new System.EventHandler(this.FrmSecurity_Load);
            this.gbFunc.ResumeLayout(false);
            this.gbFunc.PerformLayout();
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
        private System.Windows.Forms.Button btnRC42;
        private System.Windows.Forms.Button btnRC4;
        private System.Windows.Forms.Button btnAES2;
        private System.Windows.Forms.Button btnAES;
        private System.Windows.Forms.Button btnDES2;
        private System.Windows.Forms.Button btnDES;
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
        private System.Windows.Forms.ComboBox cmbPadding;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbCipher;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn3DES2;
        private System.Windows.Forms.Button btn3DES;
        private System.Windows.Forms.Button btnRijndael2;
        private System.Windows.Forms.Button btnRijndael;
        private System.Windows.Forms.Button btnRC22;
        private System.Windows.Forms.Button btnRC2;
    }
}