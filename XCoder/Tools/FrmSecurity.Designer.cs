﻿namespace XCoder.Tools
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
            this.gbFunc = new System.Windows.Forms.GroupBox();
            this.btnJWT = new System.Windows.Forms.Button();
            this.btnSnowflake = new System.Windows.Forms.Button();
            this.btnComputerInfo = new System.Windows.Forms.Button();
            this.btnTime = new System.Windows.Forms.Button();
            this.btnSHA512 = new System.Windows.Forms.Button();
            this.btnSHA384 = new System.Windows.Forms.Button();
            this.btnSHA256 = new System.Windows.Forms.Button();
            this.btnSHA1 = new System.Windows.Forms.Button();
            this.btnHtml2 = new System.Windows.Forms.Button();
            this.btnHtml = new System.Windows.Forms.Button();
            this.btnUrl2 = new System.Windows.Forms.Button();
            this.btnUrl = new System.Windows.Forms.Button();
            this.btnDSA2 = new System.Windows.Forms.Button();
            this.btnDSA = new System.Windows.Forms.Button();
            this.btnRSA2 = new System.Windows.Forms.Button();
            this.btnRSA = new System.Windows.Forms.Button();
            this.btnCRC2 = new System.Windows.Forms.Button();
            this.btnCRC = new System.Windows.Forms.Button();
            this.btnMD52 = new System.Windows.Forms.Button();
            this.btnMD5 = new System.Windows.Forms.Button();
            this.btnB642 = new System.Windows.Forms.Button();
            this.btnB64 = new System.Windows.Forms.Button();
            this.btnHex2 = new System.Windows.Forms.Button();
            this.btnHex = new System.Windows.Forms.Button();
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
            this.gbFunc.Controls.Add(this.btnJWT);
            this.gbFunc.Controls.Add(this.btnSnowflake);
            this.gbFunc.Controls.Add(this.btnComputerInfo);
            this.gbFunc.Controls.Add(this.btnTime);
            this.gbFunc.Controls.Add(this.btnSHA512);
            this.gbFunc.Controls.Add(this.btnSHA384);
            this.gbFunc.Controls.Add(this.btnSHA256);
            this.gbFunc.Controls.Add(this.btnSHA1);
            this.gbFunc.Controls.Add(this.btnHtml2);
            this.gbFunc.Controls.Add(this.btnHtml);
            this.gbFunc.Controls.Add(this.btnUrl2);
            this.gbFunc.Controls.Add(this.btnUrl);
            this.gbFunc.Controls.Add(this.btnDSA2);
            this.gbFunc.Controls.Add(this.btnDSA);
            this.gbFunc.Controls.Add(this.btnRSA2);
            this.gbFunc.Controls.Add(this.btnRSA);
            this.gbFunc.Controls.Add(this.btnCRC2);
            this.gbFunc.Controls.Add(this.btnCRC);
            this.gbFunc.Controls.Add(this.btnMD52);
            this.gbFunc.Controls.Add(this.btnMD5);
            this.gbFunc.Controls.Add(this.btnB642);
            this.gbFunc.Controls.Add(this.btnB64);
            this.gbFunc.Controls.Add(this.btnHex2);
            this.gbFunc.Controls.Add(this.btnHex);
            this.gbFunc.Location = new System.Drawing.Point(18, 20);
            this.gbFunc.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbFunc.Name = "gbFunc";
            this.gbFunc.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbFunc.Size = new System.Drawing.Size(258, 839);
            this.gbFunc.TabIndex = 0;
            this.gbFunc.TabStop = false;
            this.gbFunc.Text = "加密解密";
            // 
            // btnJWT
            // 
            this.btnJWT.Location = new System.Drawing.Point(129, 678);
            this.btnJWT.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnJWT.Name = "btnJWT";
            this.btnJWT.Size = new System.Drawing.Size(112, 50);
            this.btnJWT.TabIndex = 29;
            this.btnJWT.Text = "JWT令牌";
            this.btnJWT.UseVisualStyleBackColor = true;
            this.btnJWT.Click += new System.EventHandler(this.btnJWT_Click);
            // 
            // btnSnowflake
            // 
            this.btnSnowflake.Location = new System.Drawing.Point(9, 678);
            this.btnSnowflake.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSnowflake.Name = "btnSnowflake";
            this.btnSnowflake.Size = new System.Drawing.Size(112, 50);
            this.btnSnowflake.TabIndex = 28;
            this.btnSnowflake.Text = "雪花Id";
            this.btnSnowflake.UseVisualStyleBackColor = true;
            this.btnSnowflake.Click += new System.EventHandler(this.btnSnowflake_Click);
            // 
            // btnComputerInfo
            // 
            this.btnComputerInfo.Location = new System.Drawing.Point(130, 619);
            this.btnComputerInfo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnComputerInfo.Name = "btnComputerInfo";
            this.btnComputerInfo.Size = new System.Drawing.Size(112, 50);
            this.btnComputerInfo.TabIndex = 27;
            this.btnComputerInfo.Text = "机器信息";
            this.btnComputerInfo.UseVisualStyleBackColor = true;
            this.btnComputerInfo.Click += new System.EventHandler(this.BtnComputerInfo_Click);
            // 
            // btnTime
            // 
            this.btnTime.Location = new System.Drawing.Point(9, 619);
            this.btnTime.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnTime.Name = "btnTime";
            this.btnTime.Size = new System.Drawing.Size(112, 50);
            this.btnTime.TabIndex = 26;
            this.btnTime.Text = "时间戳";
            this.btnTime.UseVisualStyleBackColor = true;
            this.btnTime.Click += new System.EventHandler(this.btnTime_Click);
            // 
            // btnSHA512
            // 
            this.btnSHA512.Location = new System.Drawing.Point(130, 267);
            this.btnSHA512.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSHA512.Name = "btnSHA512";
            this.btnSHA512.Size = new System.Drawing.Size(112, 50);
            this.btnSHA512.TabIndex = 25;
            this.btnSHA512.Text = "SHA512";
            this.btnSHA512.UseVisualStyleBackColor = true;
            this.btnSHA512.Click += new System.EventHandler(this.btnSHA512_Click);
            // 
            // btnSHA384
            // 
            this.btnSHA384.Location = new System.Drawing.Point(9, 267);
            this.btnSHA384.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSHA384.Name = "btnSHA384";
            this.btnSHA384.Size = new System.Drawing.Size(112, 50);
            this.btnSHA384.TabIndex = 24;
            this.btnSHA384.Text = "SHA384";
            this.btnSHA384.UseVisualStyleBackColor = true;
            this.btnSHA384.Click += new System.EventHandler(this.btnSHA384_Click);
            // 
            // btnSHA256
            // 
            this.btnSHA256.Location = new System.Drawing.Point(130, 209);
            this.btnSHA256.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSHA256.Name = "btnSHA256";
            this.btnSHA256.Size = new System.Drawing.Size(112, 50);
            this.btnSHA256.TabIndex = 23;
            this.btnSHA256.Text = "SHA256";
            this.btnSHA256.UseVisualStyleBackColor = true;
            this.btnSHA256.Click += new System.EventHandler(this.btnSHA256_Click);
            // 
            // btnSHA1
            // 
            this.btnSHA1.Location = new System.Drawing.Point(9, 209);
            this.btnSHA1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSHA1.Name = "btnSHA1";
            this.btnSHA1.Size = new System.Drawing.Size(112, 50);
            this.btnSHA1.TabIndex = 22;
            this.btnSHA1.Text = "SHA1";
            this.btnSHA1.UseVisualStyleBackColor = true;
            this.btnSHA1.Click += new System.EventHandler(this.btnSHA1_Click);
            // 
            // btnHtml2
            // 
            this.btnHtml2.Location = new System.Drawing.Point(130, 559);
            this.btnHtml2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnHtml2.Name = "btnHtml2";
            this.btnHtml2.Size = new System.Drawing.Size(112, 50);
            this.btnHtml2.TabIndex = 21;
            this.btnHtml2.Text = "Html解码";
            this.btnHtml2.UseVisualStyleBackColor = true;
            this.btnHtml2.Click += new System.EventHandler(this.btnHtml2_Click);
            // 
            // btnHtml
            // 
            this.btnHtml.Location = new System.Drawing.Point(9, 559);
            this.btnHtml.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnHtml.Name = "btnHtml";
            this.btnHtml.Size = new System.Drawing.Size(112, 50);
            this.btnHtml.TabIndex = 20;
            this.btnHtml.Text = "Html编码";
            this.btnHtml.UseVisualStyleBackColor = true;
            this.btnHtml.Click += new System.EventHandler(this.btnHtml_Click);
            // 
            // btnUrl2
            // 
            this.btnUrl2.Location = new System.Drawing.Point(130, 501);
            this.btnUrl2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnUrl2.Name = "btnUrl2";
            this.btnUrl2.Size = new System.Drawing.Size(112, 50);
            this.btnUrl2.TabIndex = 19;
            this.btnUrl2.Text = "Url解码";
            this.btnUrl2.UseVisualStyleBackColor = true;
            this.btnUrl2.Click += new System.EventHandler(this.btnUrl2_Click);
            // 
            // btnUrl
            // 
            this.btnUrl.Location = new System.Drawing.Point(9, 501);
            this.btnUrl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnUrl.Name = "btnUrl";
            this.btnUrl.Size = new System.Drawing.Size(112, 50);
            this.btnUrl.TabIndex = 18;
            this.btnUrl.Text = "Url编码";
            this.btnUrl.UseVisualStyleBackColor = true;
            this.btnUrl.Click += new System.EventHandler(this.btnUrl_Click);
            // 
            // btnDSA2
            // 
            this.btnDSA2.Location = new System.Drawing.Point(130, 442);
            this.btnDSA2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDSA2.Name = "btnDSA2";
            this.btnDSA2.Size = new System.Drawing.Size(112, 50);
            this.btnDSA2.TabIndex = 17;
            this.btnDSA2.Text = "DSA验证";
            this.btnDSA2.UseVisualStyleBackColor = true;
            this.btnDSA2.Click += new System.EventHandler(this.btnDSA2_Click);
            // 
            // btnDSA
            // 
            this.btnDSA.Location = new System.Drawing.Point(9, 442);
            this.btnDSA.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDSA.Name = "btnDSA";
            this.btnDSA.Size = new System.Drawing.Size(112, 50);
            this.btnDSA.TabIndex = 16;
            this.btnDSA.Text = "DSA签名";
            this.btnDSA.UseVisualStyleBackColor = true;
            this.btnDSA.Click += new System.EventHandler(this.btnDSA_Click);
            // 
            // btnRSA2
            // 
            this.btnRSA2.Location = new System.Drawing.Point(130, 383);
            this.btnRSA2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRSA2.Name = "btnRSA2";
            this.btnRSA2.Size = new System.Drawing.Size(112, 50);
            this.btnRSA2.TabIndex = 15;
            this.btnRSA2.Text = "RSA解密";
            this.btnRSA2.UseVisualStyleBackColor = true;
            this.btnRSA2.Click += new System.EventHandler(this.btnRSA2_Click);
            // 
            // btnRSA
            // 
            this.btnRSA.Location = new System.Drawing.Point(9, 383);
            this.btnRSA.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRSA.Name = "btnRSA";
            this.btnRSA.Size = new System.Drawing.Size(112, 50);
            this.btnRSA.TabIndex = 14;
            this.btnRSA.Text = "RSA加密";
            this.btnRSA.UseVisualStyleBackColor = true;
            this.btnRSA.Click += new System.EventHandler(this.btnRSA_Click);
            // 
            // btnCRC2
            // 
            this.btnCRC2.Location = new System.Drawing.Point(130, 324);
            this.btnCRC2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCRC2.Name = "btnCRC2";
            this.btnCRC2.Size = new System.Drawing.Size(112, 50);
            this.btnCRC2.TabIndex = 7;
            this.btnCRC2.Text = "CRC_16";
            this.btnCRC2.UseVisualStyleBackColor = true;
            this.btnCRC2.Click += new System.EventHandler(this.btnCRC2_Click);
            // 
            // btnCRC
            // 
            this.btnCRC.Location = new System.Drawing.Point(9, 324);
            this.btnCRC.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCRC.Name = "btnCRC";
            this.btnCRC.Size = new System.Drawing.Size(112, 50);
            this.btnCRC.TabIndex = 6;
            this.btnCRC.Text = "CRC_32";
            this.btnCRC.UseVisualStyleBackColor = true;
            this.btnCRC.Click += new System.EventHandler(this.btnCRC_Click);
            // 
            // btnMD52
            // 
            this.btnMD52.Location = new System.Drawing.Point(130, 150);
            this.btnMD52.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnMD52.Name = "btnMD52";
            this.btnMD52.Size = new System.Drawing.Size(112, 50);
            this.btnMD52.TabIndex = 5;
            this.btnMD52.Text = "MD5_16";
            this.btnMD52.UseVisualStyleBackColor = true;
            this.btnMD52.Click += new System.EventHandler(this.btnMD52_Click);
            // 
            // btnMD5
            // 
            this.btnMD5.Location = new System.Drawing.Point(9, 150);
            this.btnMD5.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnMD5.Name = "btnMD5";
            this.btnMD5.Size = new System.Drawing.Size(112, 50);
            this.btnMD5.TabIndex = 4;
            this.btnMD5.Text = "MD5_32";
            this.btnMD5.UseVisualStyleBackColor = true;
            this.btnMD5.Click += new System.EventHandler(this.btnMD5_Click);
            // 
            // btnB642
            // 
            this.btnB642.Location = new System.Drawing.Point(130, 91);
            this.btnB642.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnB642.Name = "btnB642";
            this.btnB642.Size = new System.Drawing.Size(112, 50);
            this.btnB642.TabIndex = 3;
            this.btnB642.Text = "Base64解码";
            this.btnB642.UseVisualStyleBackColor = true;
            this.btnB642.Click += new System.EventHandler(this.btnB642_Click);
            // 
            // btnB64
            // 
            this.btnB64.Location = new System.Drawing.Point(9, 91);
            this.btnB64.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnB64.Name = "btnB64";
            this.btnB64.Size = new System.Drawing.Size(112, 50);
            this.btnB64.TabIndex = 2;
            this.btnB64.Text = "Base64编码";
            this.btnB64.UseVisualStyleBackColor = true;
            this.btnB64.Click += new System.EventHandler(this.btnB64_Click);
            // 
            // btnHex2
            // 
            this.btnHex2.Location = new System.Drawing.Point(130, 33);
            this.btnHex2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnHex2.Name = "btnHex2";
            this.btnHex2.Size = new System.Drawing.Size(112, 50);
            this.btnHex2.TabIndex = 1;
            this.btnHex2.Text = "HEX解码";
            this.btnHex2.UseVisualStyleBackColor = true;
            this.btnHex2.Click += new System.EventHandler(this.btnHex2_Click);
            // 
            // btnHex
            // 
            this.btnHex.Location = new System.Drawing.Point(9, 33);
            this.btnHex.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnHex.Name = "btnHex";
            this.btnHex.Size = new System.Drawing.Size(112, 50);
            this.btnHex.TabIndex = 0;
            this.btnHex.Text = "HEX编码";
            this.btnHex.UseVisualStyleBackColor = true;
            this.btnHex.Click += new System.EventHandler(this.btnHex_Click);
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
            this.gbSource.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbSource.Name = "gbSource";
            this.gbSource.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbSource.Size = new System.Drawing.Size(1006, 417);
            this.gbSource.TabIndex = 3;
            this.gbSource.TabStop = false;
            this.gbSource.Text = "原文";
            // 
            // btnExchange
            // 
            this.btnExchange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExchange.Location = new System.Drawing.Point(894, 14);
            this.btnExchange.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.rbBase64.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.rbHex.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.rbString.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.rtSource.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.gbResult.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbResult.Name = "gbResult";
            this.gbResult.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbResult.Size = new System.Drawing.Size(1006, 247);
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
            this.cbBase64.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.cbHex.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.cbString.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.rtResult.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rtResult.Name = "rtResult";
            this.rtResult.Size = new System.Drawing.Size(998, 178);
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
            this.gbPass.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbPass.Name = "gbPass";
            this.gbPass.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbPass.Size = new System.Drawing.Size(1006, 167);
            this.gbPass.TabIndex = 5;
            this.gbPass.TabStop = false;
            this.gbPass.Text = "密码 / 时间基准";
            // 
            // rbBase642
            // 
            this.rbBase642.AutoSize = true;
            this.rbBase642.Location = new System.Drawing.Point(299, 33);
            this.rbBase642.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.rbHex2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.rbString2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.rtPass.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rtPass.Name = "rtPass";
            this.rtPass.Size = new System.Drawing.Size(998, 91);
            this.rtPass.TabIndex = 2;
            this.rtPass.Text = "NewLife";
            // 
            // FrmSecurity
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1304, 879);
            this.Controls.Add(this.gbPass);
            this.Controls.Add(this.gbResult);
            this.Controls.Add(this.gbSource);
            this.Controls.Add(this.gbFunc);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FrmSecurity";
            this.Text = "加密解密";
            this.Load += new System.EventHandler(this.FrmSecurity_Load);
            this.gbFunc.ResumeLayout(false);
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
    }
}