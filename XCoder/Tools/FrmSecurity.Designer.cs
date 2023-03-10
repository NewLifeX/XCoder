namespace XCoder.Tools
{
    partial class FrmSecurity
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
            btnJWT = new Button();
            btnSnowflake = new Button();
            btnComputerInfo = new Button();
            btnTime = new Button();
            btnSHA512 = new Button();
            btnSHA384 = new Button();
            btnSHA256 = new Button();
            btnSHA1 = new Button();
            btnHtml2 = new Button();
            btnHtml = new Button();
            btnUrl2 = new Button();
            btnUrl = new Button();
            btnDSA2 = new Button();
            btnDSA = new Button();
            btnRSA2 = new Button();
            btnRSA = new Button();
            btnCRC2 = new Button();
            btnCRC = new Button();
            btnMD52 = new Button();
            btnMD5 = new Button();
            btnB642 = new Button();
            btnB64 = new Button();
            btnHex2 = new Button();
            btnHex = new Button();
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
            btnVersion = new Button();
            gbFunc.SuspendLayout();
            gbSource.SuspendLayout();
            gbResult.SuspendLayout();
            gbPass.SuspendLayout();
            SuspendLayout();
            // 
            // gbFunc
            // 
            gbFunc.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            gbFunc.Controls.Add(btnVersion);
            gbFunc.Controls.Add(btnJWT);
            gbFunc.Controls.Add(btnSnowflake);
            gbFunc.Controls.Add(btnComputerInfo);
            gbFunc.Controls.Add(btnTime);
            gbFunc.Controls.Add(btnSHA512);
            gbFunc.Controls.Add(btnSHA384);
            gbFunc.Controls.Add(btnSHA256);
            gbFunc.Controls.Add(btnSHA1);
            gbFunc.Controls.Add(btnHtml2);
            gbFunc.Controls.Add(btnHtml);
            gbFunc.Controls.Add(btnUrl2);
            gbFunc.Controls.Add(btnUrl);
            gbFunc.Controls.Add(btnDSA2);
            gbFunc.Controls.Add(btnDSA);
            gbFunc.Controls.Add(btnRSA2);
            gbFunc.Controls.Add(btnRSA);
            gbFunc.Controls.Add(btnCRC2);
            gbFunc.Controls.Add(btnCRC);
            gbFunc.Controls.Add(btnMD52);
            gbFunc.Controls.Add(btnMD5);
            gbFunc.Controls.Add(btnB642);
            gbFunc.Controls.Add(btnB64);
            gbFunc.Controls.Add(btnHex2);
            gbFunc.Controls.Add(btnHex);
            gbFunc.Location = new Point(18, 20);
            gbFunc.Margin = new Padding(4);
            gbFunc.Name = "gbFunc";
            gbFunc.Padding = new Padding(4);
            gbFunc.Size = new Size(258, 839);
            gbFunc.TabIndex = 0;
            gbFunc.TabStop = false;
            gbFunc.Text = "加密解密";
            // 
            // btnJWT
            // 
            btnJWT.Location = new Point(129, 678);
            btnJWT.Margin = new Padding(4);
            btnJWT.Name = "btnJWT";
            btnJWT.Size = new Size(112, 50);
            btnJWT.TabIndex = 29;
            btnJWT.Text = "JWT令牌";
            btnJWT.UseVisualStyleBackColor = true;
            btnJWT.Click += btnJWT_Click;
            // 
            // btnSnowflake
            // 
            btnSnowflake.Location = new Point(9, 678);
            btnSnowflake.Margin = new Padding(4);
            btnSnowflake.Name = "btnSnowflake";
            btnSnowflake.Size = new Size(112, 50);
            btnSnowflake.TabIndex = 28;
            btnSnowflake.Text = "雪花Id";
            btnSnowflake.UseVisualStyleBackColor = true;
            btnSnowflake.Click += btnSnowflake_Click;
            // 
            // btnComputerInfo
            // 
            btnComputerInfo.Location = new Point(130, 619);
            btnComputerInfo.Margin = new Padding(4);
            btnComputerInfo.Name = "btnComputerInfo";
            btnComputerInfo.Size = new Size(112, 50);
            btnComputerInfo.TabIndex = 27;
            btnComputerInfo.Text = "机器信息";
            btnComputerInfo.UseVisualStyleBackColor = true;
            btnComputerInfo.Click += BtnComputerInfo_Click;
            // 
            // btnTime
            // 
            btnTime.Location = new Point(9, 619);
            btnTime.Margin = new Padding(4);
            btnTime.Name = "btnTime";
            btnTime.Size = new Size(112, 50);
            btnTime.TabIndex = 26;
            btnTime.Text = "时间戳";
            btnTime.UseVisualStyleBackColor = true;
            btnTime.Click += btnTime_Click;
            // 
            // btnSHA512
            // 
            btnSHA512.Location = new Point(130, 267);
            btnSHA512.Margin = new Padding(4);
            btnSHA512.Name = "btnSHA512";
            btnSHA512.Size = new Size(112, 50);
            btnSHA512.TabIndex = 25;
            btnSHA512.Text = "SHA512";
            btnSHA512.UseVisualStyleBackColor = true;
            btnSHA512.Click += btnSHA512_Click;
            // 
            // btnSHA384
            // 
            btnSHA384.Location = new Point(9, 267);
            btnSHA384.Margin = new Padding(4);
            btnSHA384.Name = "btnSHA384";
            btnSHA384.Size = new Size(112, 50);
            btnSHA384.TabIndex = 24;
            btnSHA384.Text = "SHA384";
            btnSHA384.UseVisualStyleBackColor = true;
            btnSHA384.Click += btnSHA384_Click;
            // 
            // btnSHA256
            // 
            btnSHA256.Location = new Point(130, 209);
            btnSHA256.Margin = new Padding(4);
            btnSHA256.Name = "btnSHA256";
            btnSHA256.Size = new Size(112, 50);
            btnSHA256.TabIndex = 23;
            btnSHA256.Text = "SHA256";
            btnSHA256.UseVisualStyleBackColor = true;
            btnSHA256.Click += btnSHA256_Click;
            // 
            // btnSHA1
            // 
            btnSHA1.Location = new Point(9, 209);
            btnSHA1.Margin = new Padding(4);
            btnSHA1.Name = "btnSHA1";
            btnSHA1.Size = new Size(112, 50);
            btnSHA1.TabIndex = 22;
            btnSHA1.Text = "SHA1";
            btnSHA1.UseVisualStyleBackColor = true;
            btnSHA1.Click += btnSHA1_Click;
            // 
            // btnHtml2
            // 
            btnHtml2.Location = new Point(130, 559);
            btnHtml2.Margin = new Padding(4);
            btnHtml2.Name = "btnHtml2";
            btnHtml2.Size = new Size(112, 50);
            btnHtml2.TabIndex = 21;
            btnHtml2.Text = "Html解码";
            btnHtml2.UseVisualStyleBackColor = true;
            btnHtml2.Click += btnHtml2_Click;
            // 
            // btnHtml
            // 
            btnHtml.Location = new Point(9, 559);
            btnHtml.Margin = new Padding(4);
            btnHtml.Name = "btnHtml";
            btnHtml.Size = new Size(112, 50);
            btnHtml.TabIndex = 20;
            btnHtml.Text = "Html编码";
            btnHtml.UseVisualStyleBackColor = true;
            btnHtml.Click += btnHtml_Click;
            // 
            // btnUrl2
            // 
            btnUrl2.Location = new Point(130, 501);
            btnUrl2.Margin = new Padding(4);
            btnUrl2.Name = "btnUrl2";
            btnUrl2.Size = new Size(112, 50);
            btnUrl2.TabIndex = 19;
            btnUrl2.Text = "Url解码";
            btnUrl2.UseVisualStyleBackColor = true;
            btnUrl2.Click += btnUrl2_Click;
            // 
            // btnUrl
            // 
            btnUrl.Location = new Point(9, 501);
            btnUrl.Margin = new Padding(4);
            btnUrl.Name = "btnUrl";
            btnUrl.Size = new Size(112, 50);
            btnUrl.TabIndex = 18;
            btnUrl.Text = "Url编码";
            btnUrl.UseVisualStyleBackColor = true;
            btnUrl.Click += btnUrl_Click;
            // 
            // btnDSA2
            // 
            btnDSA2.Location = new Point(130, 442);
            btnDSA2.Margin = new Padding(4);
            btnDSA2.Name = "btnDSA2";
            btnDSA2.Size = new Size(112, 50);
            btnDSA2.TabIndex = 17;
            btnDSA2.Text = "DSA验证";
            btnDSA2.UseVisualStyleBackColor = true;
            btnDSA2.Click += btnDSA2_Click;
            // 
            // btnDSA
            // 
            btnDSA.Location = new Point(9, 442);
            btnDSA.Margin = new Padding(4);
            btnDSA.Name = "btnDSA";
            btnDSA.Size = new Size(112, 50);
            btnDSA.TabIndex = 16;
            btnDSA.Text = "DSA签名";
            btnDSA.UseVisualStyleBackColor = true;
            btnDSA.Click += btnDSA_Click;
            // 
            // btnRSA2
            // 
            btnRSA2.Location = new Point(130, 383);
            btnRSA2.Margin = new Padding(4);
            btnRSA2.Name = "btnRSA2";
            btnRSA2.Size = new Size(112, 50);
            btnRSA2.TabIndex = 15;
            btnRSA2.Text = "RSA解密";
            btnRSA2.UseVisualStyleBackColor = true;
            btnRSA2.Click += btnRSA2_Click;
            // 
            // btnRSA
            // 
            btnRSA.Location = new Point(9, 383);
            btnRSA.Margin = new Padding(4);
            btnRSA.Name = "btnRSA";
            btnRSA.Size = new Size(112, 50);
            btnRSA.TabIndex = 14;
            btnRSA.Text = "RSA加密";
            btnRSA.UseVisualStyleBackColor = true;
            btnRSA.Click += btnRSA_Click;
            // 
            // btnCRC2
            // 
            btnCRC2.Location = new Point(130, 324);
            btnCRC2.Margin = new Padding(4);
            btnCRC2.Name = "btnCRC2";
            btnCRC2.Size = new Size(112, 50);
            btnCRC2.TabIndex = 7;
            btnCRC2.Text = "CRC_16";
            btnCRC2.UseVisualStyleBackColor = true;
            btnCRC2.Click += btnCRC2_Click;
            // 
            // btnCRC
            // 
            btnCRC.Location = new Point(9, 324);
            btnCRC.Margin = new Padding(4);
            btnCRC.Name = "btnCRC";
            btnCRC.Size = new Size(112, 50);
            btnCRC.TabIndex = 6;
            btnCRC.Text = "CRC_32";
            btnCRC.UseVisualStyleBackColor = true;
            btnCRC.Click += btnCRC_Click;
            // 
            // btnMD52
            // 
            btnMD52.Location = new Point(130, 150);
            btnMD52.Margin = new Padding(4);
            btnMD52.Name = "btnMD52";
            btnMD52.Size = new Size(112, 50);
            btnMD52.TabIndex = 5;
            btnMD52.Text = "MD5_16";
            btnMD52.UseVisualStyleBackColor = true;
            btnMD52.Click += btnMD52_Click;
            // 
            // btnMD5
            // 
            btnMD5.Location = new Point(9, 150);
            btnMD5.Margin = new Padding(4);
            btnMD5.Name = "btnMD5";
            btnMD5.Size = new Size(112, 50);
            btnMD5.TabIndex = 4;
            btnMD5.Text = "MD5_32";
            btnMD5.UseVisualStyleBackColor = true;
            btnMD5.Click += btnMD5_Click;
            // 
            // btnB642
            // 
            btnB642.Location = new Point(130, 91);
            btnB642.Margin = new Padding(4);
            btnB642.Name = "btnB642";
            btnB642.Size = new Size(112, 50);
            btnB642.TabIndex = 3;
            btnB642.Text = "Base64解码";
            btnB642.UseVisualStyleBackColor = true;
            btnB642.Click += btnB642_Click;
            // 
            // btnB64
            // 
            btnB64.Location = new Point(9, 91);
            btnB64.Margin = new Padding(4);
            btnB64.Name = "btnB64";
            btnB64.Size = new Size(112, 50);
            btnB64.TabIndex = 2;
            btnB64.Text = "Base64编码";
            btnB64.UseVisualStyleBackColor = true;
            btnB64.Click += btnB64_Click;
            // 
            // btnHex2
            // 
            btnHex2.Location = new Point(130, 33);
            btnHex2.Margin = new Padding(4);
            btnHex2.Name = "btnHex2";
            btnHex2.Size = new Size(112, 50);
            btnHex2.TabIndex = 1;
            btnHex2.Text = "HEX解码";
            btnHex2.UseVisualStyleBackColor = true;
            btnHex2.Click += btnHex2_Click;
            // 
            // btnHex
            // 
            btnHex.Location = new Point(9, 33);
            btnHex.Margin = new Padding(4);
            btnHex.Name = "btnHex";
            btnHex.Size = new Size(112, 50);
            btnHex.TabIndex = 0;
            btnHex.Text = "HEX编码";
            btnHex.UseVisualStyleBackColor = true;
            btnHex.Click += btnHex_Click;
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
            gbResult.Size = new Size(1006, 247);
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
            rtResult.Size = new Size(998, 178);
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
            gbPass.Text = "密码 / 时间基准";
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
            // btnVersion
            // 
            btnVersion.Location = new Point(9, 736);
            btnVersion.Margin = new Padding(4);
            btnVersion.Name = "btnVersion";
            btnVersion.Size = new Size(112, 50);
            btnVersion.TabIndex = 30;
            btnVersion.Text = "版本号";
            btnVersion.UseVisualStyleBackColor = true;
            btnVersion.Click += btnVersion_Click;
            // 
            // FrmSecurity
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1304, 879);
            Controls.Add(gbPass);
            Controls.Add(gbResult);
            Controls.Add(gbSource);
            Controls.Add(gbFunc);
            Margin = new Padding(4);
            Name = "FrmSecurity";
            Text = "加密解密";
            Load += FrmSecurity_Load;
            gbFunc.ResumeLayout(false);
            gbSource.ResumeLayout(false);
            gbSource.PerformLayout();
            gbResult.ResumeLayout(false);
            gbResult.PerformLayout();
            gbPass.ResumeLayout(false);
            gbPass.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox gbFunc;
        private System.Windows.Forms.GroupBox gbSource;
        private System.Windows.Forms.RichTextBox rtSource;
        private System.Windows.Forms.GroupBox gbResult;
        private System.Windows.Forms.RichTextBox rtResult;
        private System.Windows.Forms.Button btnHex2;
        private System.Windows.Forms.Button btnHex;
        private System.Windows.Forms.Button btnB642;
        private System.Windows.Forms.Button btnB64;
        private System.Windows.Forms.Button btnMD52;
        private System.Windows.Forms.Button btnMD5;
        private System.Windows.Forms.Button btnCRC2;
        private System.Windows.Forms.Button btnCRC;
        private System.Windows.Forms.Button btnHtml2;
        private System.Windows.Forms.Button btnHtml;
        private System.Windows.Forms.Button btnUrl2;
        private System.Windows.Forms.Button btnUrl;
        private System.Windows.Forms.Button btnDSA2;
        private System.Windows.Forms.Button btnDSA;
        private System.Windows.Forms.Button btnRSA2;
        private System.Windows.Forms.Button btnRSA;
        private System.Windows.Forms.GroupBox gbPass;
        private System.Windows.Forms.RichTextBox rtPass;
        private System.Windows.Forms.Button btnExchange;
        private System.Windows.Forms.Button btnSHA512;
        private System.Windows.Forms.Button btnSHA384;
        private System.Windows.Forms.Button btnSHA256;
        private System.Windows.Forms.Button btnSHA1;
        private System.Windows.Forms.Button btnTime;
        private System.Windows.Forms.Button btnComputerInfo;
        private System.Windows.Forms.CheckBox cbBase64;
        private System.Windows.Forms.CheckBox cbHex;
        private System.Windows.Forms.CheckBox cbString;
        private System.Windows.Forms.RadioButton rbBase64;
        private System.Windows.Forms.RadioButton rbHex;
        private System.Windows.Forms.RadioButton rbString;
        private System.Windows.Forms.RadioButton rbBase642;
        private System.Windows.Forms.RadioButton rbHex2;
        private System.Windows.Forms.RadioButton rbString2;
        private System.Windows.Forms.Button btnJWT;
        private System.Windows.Forms.Button btnSnowflake;
        private Button btnVersion;
    }
}