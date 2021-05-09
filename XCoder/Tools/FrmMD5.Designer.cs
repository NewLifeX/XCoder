namespace XCoder.Tools
{
    partial class FrmMD5
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
            this.components = new System.ComponentModel.Container();
            this.gbFunc = new System.Windows.Forms.GroupBox();
            this.lbProgress = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lbPosition = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lbTotal = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbSymbol = new System.Windows.Forms.CheckBox();
            this.cbCharUpper = new System.Windows.Forms.CheckBox();
            this.cbCharLower = new System.Windows.Forms.CheckBox();
            this.cbNumber = new System.Windows.Forms.CheckBox();
            this.numLength = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btnMD5 = new System.Windows.Forms.Button();
            this.gbSource = new System.Windows.Forms.GroupBox();
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.rbBase64 = new System.Windows.Forms.RadioButton();
            this.rbHex = new System.Windows.Forms.RadioButton();
            this.rbString = new System.Windows.Forms.RadioButton();
            this.lbCost = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbMaxCost = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbSpeed = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.gbFunc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLength)).BeginInit();
            this.gbSource.SuspendLayout();
            this.gbResult.SuspendLayout();
            this.gbPass.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbFunc
            // 
            this.gbFunc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbFunc.Controls.Add(this.lbMaxCost);
            this.gbFunc.Controls.Add(this.lbSpeed);
            this.gbFunc.Controls.Add(this.label8);
            this.gbFunc.Controls.Add(this.label6);
            this.gbFunc.Controls.Add(this.lbCost);
            this.gbFunc.Controls.Add(this.label4);
            this.gbFunc.Controls.Add(this.lbProgress);
            this.gbFunc.Controls.Add(this.label7);
            this.gbFunc.Controls.Add(this.lbPosition);
            this.gbFunc.Controls.Add(this.label5);
            this.gbFunc.Controls.Add(this.lbTotal);
            this.gbFunc.Controls.Add(this.label2);
            this.gbFunc.Controls.Add(this.cbSymbol);
            this.gbFunc.Controls.Add(this.cbCharUpper);
            this.gbFunc.Controls.Add(this.cbCharLower);
            this.gbFunc.Controls.Add(this.cbNumber);
            this.gbFunc.Controls.Add(this.numLength);
            this.gbFunc.Controls.Add(this.label1);
            this.gbFunc.Controls.Add(this.btnMD5);
            this.gbFunc.Location = new System.Drawing.Point(18, 18);
            this.gbFunc.Margin = new System.Windows.Forms.Padding(4);
            this.gbFunc.Name = "gbFunc";
            this.gbFunc.Padding = new System.Windows.Forms.Padding(4);
            this.gbFunc.Size = new System.Drawing.Size(258, 908);
            this.gbFunc.TabIndex = 0;
            this.gbFunc.TabStop = false;
            this.gbFunc.Text = "加密解密";
            // 
            // lbProgress
            // 
            this.lbProgress.AutoSize = true;
            this.lbProgress.ForeColor = System.Drawing.Color.Red;
            this.lbProgress.Location = new System.Drawing.Point(100, 268);
            this.lbProgress.Name = "lbProgress";
            this.lbProgress.Size = new System.Drawing.Size(53, 18);
            this.lbProgress.TabIndex = 16;
            this.lbProgress.Text = "0.00%";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 268);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(98, 18);
            this.label7.TabIndex = 15;
            this.label7.Text = "计算进度：";
            // 
            // lbPosition
            // 
            this.lbPosition.AutoSize = true;
            this.lbPosition.ForeColor = System.Drawing.Color.Red;
            this.lbPosition.Location = new System.Drawing.Point(100, 235);
            this.lbPosition.Name = "lbPosition";
            this.lbPosition.Size = new System.Drawing.Size(17, 18);
            this.lbPosition.TabIndex = 14;
            this.lbPosition.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 235);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 18);
            this.label5.TabIndex = 13;
            this.label5.Text = "当前位置：";
            // 
            // lbTotal
            // 
            this.lbTotal.AutoSize = true;
            this.lbTotal.ForeColor = System.Drawing.Color.Red;
            this.lbTotal.Location = new System.Drawing.Point(100, 202);
            this.lbTotal.Name = "lbTotal";
            this.lbTotal.Size = new System.Drawing.Size(89, 18);
            this.lbTotal.TabIndex = 12;
            this.lbTotal.Text = "1,000,000";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 202);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 18);
            this.label2.TabIndex = 11;
            this.label2.Text = "总计算量：";
            // 
            // cbSymbol
            // 
            this.cbSymbol.AutoSize = true;
            this.cbSymbol.Location = new System.Drawing.Point(100, 154);
            this.cbSymbol.Name = "cbSymbol";
            this.cbSymbol.Size = new System.Drawing.Size(106, 22);
            this.cbSymbol.TabIndex = 10;
            this.cbSymbol.Text = "特殊符号";
            this.cbSymbol.UseVisualStyleBackColor = true;
            this.cbSymbol.CheckedChanged += new System.EventHandler(this.numLength_ValueChanged);
            // 
            // cbCharUpper
            // 
            this.cbCharUpper.AutoSize = true;
            this.cbCharUpper.Location = new System.Drawing.Point(100, 126);
            this.cbCharUpper.Name = "cbCharUpper";
            this.cbCharUpper.Size = new System.Drawing.Size(106, 22);
            this.cbCharUpper.TabIndex = 9;
            this.cbCharUpper.Text = "大写字母";
            this.cbCharUpper.UseVisualStyleBackColor = true;
            this.cbCharUpper.CheckedChanged += new System.EventHandler(this.numLength_ValueChanged);
            // 
            // cbCharLower
            // 
            this.cbCharLower.AutoSize = true;
            this.cbCharLower.Location = new System.Drawing.Point(100, 98);
            this.cbCharLower.Name = "cbCharLower";
            this.cbCharLower.Size = new System.Drawing.Size(106, 22);
            this.cbCharLower.TabIndex = 8;
            this.cbCharLower.Text = "小写字母";
            this.cbCharLower.UseVisualStyleBackColor = true;
            this.cbCharLower.CheckedChanged += new System.EventHandler(this.numLength_ValueChanged);
            // 
            // cbNumber
            // 
            this.cbNumber.AutoSize = true;
            this.cbNumber.Checked = true;
            this.cbNumber.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbNumber.Location = new System.Drawing.Point(100, 70);
            this.cbNumber.Name = "cbNumber";
            this.cbNumber.Size = new System.Drawing.Size(70, 22);
            this.cbNumber.TabIndex = 7;
            this.cbNumber.Text = "数字";
            this.cbNumber.UseVisualStyleBackColor = true;
            this.cbNumber.CheckedChanged += new System.EventHandler(this.numLength_ValueChanged);
            // 
            // numLength
            // 
            this.numLength.Location = new System.Drawing.Point(100, 26);
            this.numLength.Name = "numLength";
            this.numLength.Size = new System.Drawing.Size(94, 28);
            this.numLength.TabIndex = 6;
            this.numLength.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.numLength.ValueChanged += new System.EventHandler(this.numLength_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 18);
            this.label1.TabIndex = 5;
            this.label1.Text = "密码位数：";
            // 
            // btnMD5
            // 
            this.btnMD5.Location = new System.Drawing.Point(58, 444);
            this.btnMD5.Margin = new System.Windows.Forms.Padding(4);
            this.btnMD5.Name = "btnMD5";
            this.btnMD5.Size = new System.Drawing.Size(112, 45);
            this.btnMD5.TabIndex = 4;
            this.btnMD5.Text = "MD5爆破";
            this.btnMD5.UseVisualStyleBackColor = true;
            this.btnMD5.Click += new System.EventHandler(this.btnMD5_Click);
            // 
            // gbSource
            // 
            this.gbSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSource.Controls.Add(this.rbBase64);
            this.gbSource.Controls.Add(this.rbHex);
            this.gbSource.Controls.Add(this.rbString);
            this.gbSource.Controls.Add(this.rtSource);
            this.gbSource.Location = new System.Drawing.Point(285, 18);
            this.gbSource.Margin = new System.Windows.Forms.Padding(4);
            this.gbSource.Name = "gbSource";
            this.gbSource.Padding = new System.Windows.Forms.Padding(4);
            this.gbSource.Size = new System.Drawing.Size(1006, 375);
            this.gbSource.TabIndex = 3;
            this.gbSource.TabStop = false;
            this.gbSource.Text = "原文";
            // 
            // rtSource
            // 
            this.rtSource.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtSource.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtSource.Location = new System.Drawing.Point(4, 65);
            this.rtSource.Margin = new System.Windows.Forms.Padding(4);
            this.rtSource.Name = "rtSource";
            this.rtSource.Size = new System.Drawing.Size(998, 306);
            this.rtSource.TabIndex = 2;
            this.rtSource.Text = "4c5fca3ed14e45d865d31b780a7bd40c";
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
            this.gbResult.Location = new System.Drawing.Point(285, 552);
            this.gbResult.Margin = new System.Windows.Forms.Padding(4);
            this.gbResult.Name = "gbResult";
            this.gbResult.Padding = new System.Windows.Forms.Padding(4);
            this.gbResult.Size = new System.Drawing.Size(1006, 375);
            this.gbResult.TabIndex = 4;
            this.gbResult.TabStop = false;
            this.gbResult.Text = "结果";
            // 
            // cbBase64
            // 
            this.cbBase64.AutoSize = true;
            this.cbBase64.Checked = true;
            this.cbBase64.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBase64.Location = new System.Drawing.Point(299, 28);
            this.cbBase64.Margin = new System.Windows.Forms.Padding(2);
            this.cbBase64.Name = "cbBase64";
            this.cbBase64.Size = new System.Drawing.Size(124, 22);
            this.cbBase64.TabIndex = 8;
            this.cbBase64.Text = "BASE64编码";
            this.cbBase64.UseVisualStyleBackColor = true;
            // 
            // cbHex
            // 
            this.cbHex.AutoSize = true;
            this.cbHex.Checked = true;
            this.cbHex.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbHex.Location = new System.Drawing.Point(154, 28);
            this.cbHex.Margin = new System.Windows.Forms.Padding(2);
            this.cbHex.Name = "cbHex";
            this.cbHex.Size = new System.Drawing.Size(97, 22);
            this.cbHex.TabIndex = 7;
            this.cbHex.Text = "HEX编码";
            this.cbHex.UseVisualStyleBackColor = true;
            // 
            // cbString
            // 
            this.cbString.AutoSize = true;
            this.cbString.Checked = true;
            this.cbString.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbString.Location = new System.Drawing.Point(19, 28);
            this.cbString.Margin = new System.Windows.Forms.Padding(2);
            this.cbString.Name = "cbString";
            this.cbString.Size = new System.Drawing.Size(88, 22);
            this.cbString.TabIndex = 6;
            this.cbString.Text = "字符串";
            this.cbString.UseVisualStyleBackColor = true;
            // 
            // rtResult
            // 
            this.rtResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtResult.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtResult.Location = new System.Drawing.Point(4, 57);
            this.rtResult.Margin = new System.Windows.Forms.Padding(4);
            this.rtResult.Name = "rtResult";
            this.rtResult.Size = new System.Drawing.Size(998, 314);
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
            this.gbPass.Location = new System.Drawing.Point(285, 398);
            this.gbPass.Margin = new System.Windows.Forms.Padding(4);
            this.gbPass.Name = "gbPass";
            this.gbPass.Padding = new System.Windows.Forms.Padding(4);
            this.gbPass.Size = new System.Drawing.Size(1006, 150);
            this.gbPass.TabIndex = 5;
            this.gbPass.TabStop = false;
            this.gbPass.Text = "密码";
            // 
            // rbBase642
            // 
            this.rbBase642.AutoSize = true;
            this.rbBase642.Location = new System.Drawing.Point(299, 30);
            this.rbBase642.Margin = new System.Windows.Forms.Padding(2);
            this.rbBase642.Name = "rbBase642";
            this.rbBase642.Size = new System.Drawing.Size(123, 22);
            this.rbBase642.TabIndex = 8;
            this.rbBase642.Text = "BASE64编码";
            this.rbBase642.UseVisualStyleBackColor = true;
            // 
            // rbHex2
            // 
            this.rbHex2.AutoSize = true;
            this.rbHex2.Location = new System.Drawing.Point(155, 30);
            this.rbHex2.Margin = new System.Windows.Forms.Padding(2);
            this.rbHex2.Name = "rbHex2";
            this.rbHex2.Size = new System.Drawing.Size(96, 22);
            this.rbHex2.TabIndex = 7;
            this.rbHex2.Text = "HEX编码";
            this.rbHex2.UseVisualStyleBackColor = true;
            // 
            // rbString2
            // 
            this.rbString2.AutoSize = true;
            this.rbString2.Checked = true;
            this.rbString2.Location = new System.Drawing.Point(19, 30);
            this.rbString2.Margin = new System.Windows.Forms.Padding(2);
            this.rbString2.Name = "rbString2";
            this.rbString2.Size = new System.Drawing.Size(87, 22);
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
            this.rtPass.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtPass.Location = new System.Drawing.Point(4, 64);
            this.rtPass.Margin = new System.Windows.Forms.Padding(4);
            this.rtPass.Name = "rtPass";
            this.rtPass.Size = new System.Drawing.Size(998, 82);
            this.rtPass.TabIndex = 2;
            this.rtPass.Text = "NewLife";
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // rbBase64
            // 
            this.rbBase64.AutoSize = true;
            this.rbBase64.Location = new System.Drawing.Point(299, 28);
            this.rbBase64.Margin = new System.Windows.Forms.Padding(2);
            this.rbBase64.Name = "rbBase64";
            this.rbBase64.Size = new System.Drawing.Size(123, 22);
            this.rbBase64.TabIndex = 5;
            this.rbBase64.Text = "BASE64编码";
            this.rbBase64.UseVisualStyleBackColor = true;
            // 
            // rbHex
            // 
            this.rbHex.AutoSize = true;
            this.rbHex.Location = new System.Drawing.Point(154, 28);
            this.rbHex.Margin = new System.Windows.Forms.Padding(2);
            this.rbHex.Name = "rbHex";
            this.rbHex.Size = new System.Drawing.Size(96, 22);
            this.rbHex.TabIndex = 4;
            this.rbHex.Text = "HEX编码";
            this.rbHex.UseVisualStyleBackColor = true;
            // 
            // rbString
            // 
            this.rbString.AutoSize = true;
            this.rbString.Checked = true;
            this.rbString.Location = new System.Drawing.Point(19, 28);
            this.rbString.Margin = new System.Windows.Forms.Padding(2);
            this.rbString.Name = "rbString";
            this.rbString.Size = new System.Drawing.Size(87, 22);
            this.rbString.TabIndex = 3;
            this.rbString.TabStop = true;
            this.rbString.Text = "字符串";
            this.rbString.UseVisualStyleBackColor = true;
            // 
            // lbCost
            // 
            this.lbCost.AutoSize = true;
            this.lbCost.ForeColor = System.Drawing.Color.Red;
            this.lbCost.Location = new System.Drawing.Point(100, 301);
            this.lbCost.Name = "lbCost";
            this.lbCost.Size = new System.Drawing.Size(17, 18);
            this.lbCost.TabIndex = 18;
            this.lbCost.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 301);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 18);
            this.label4.TabIndex = 17;
            this.label4.Text = "耗时：";
            // 
            // lbMaxCost
            // 
            this.lbMaxCost.AutoSize = true;
            this.lbMaxCost.ForeColor = System.Drawing.Color.Red;
            this.lbMaxCost.Location = new System.Drawing.Point(100, 368);
            this.lbMaxCost.Name = "lbMaxCost";
            this.lbMaxCost.Size = new System.Drawing.Size(17, 18);
            this.lbMaxCost.TabIndex = 20;
            this.lbMaxCost.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 334);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 18);
            this.label6.TabIndex = 19;
            this.label6.Text = "速度：";
            // 
            // lbSpeed
            // 
            this.lbSpeed.AutoSize = true;
            this.lbSpeed.ForeColor = System.Drawing.Color.Red;
            this.lbSpeed.Location = new System.Drawing.Point(100, 334);
            this.lbSpeed.Name = "lbSpeed";
            this.lbSpeed.Size = new System.Drawing.Size(17, 18);
            this.lbSpeed.TabIndex = 22;
            this.lbSpeed.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 368);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(98, 18);
            this.label8.TabIndex = 21;
            this.label8.Text = "估算耗时：";
            // 
            // FrmMD5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1304, 944);
            this.Controls.Add(this.gbPass);
            this.Controls.Add(this.gbResult);
            this.Controls.Add(this.gbSource);
            this.Controls.Add(this.gbFunc);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmMD5";
            this.Text = "MD5解密";
            this.Load += new System.EventHandler(this.FrmSecurity_Load);
            this.gbFunc.ResumeLayout(false);
            this.gbFunc.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLength)).EndInit();
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
        private System.Windows.Forms.Button btnMD5;
        private System.Windows.Forms.GroupBox gbPass;
        private System.Windows.Forms.RichTextBox rtPass;
        private System.Windows.Forms.CheckBox cbBase64;
        private System.Windows.Forms.CheckBox cbHex;
        private System.Windows.Forms.CheckBox cbString;
        private System.Windows.Forms.RadioButton rbBase642;
        private System.Windows.Forms.RadioButton rbHex2;
        private System.Windows.Forms.RadioButton rbString2;
        private System.Windows.Forms.Label lbTotal;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbSymbol;
        private System.Windows.Forms.CheckBox cbCharUpper;
        private System.Windows.Forms.CheckBox cbCharLower;
        private System.Windows.Forms.CheckBox cbNumber;
        private System.Windows.Forms.NumericUpDown numLength;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbProgress;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lbPosition;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.RadioButton rbBase64;
        private System.Windows.Forms.RadioButton rbHex;
        private System.Windows.Forms.RadioButton rbString;
        private System.Windows.Forms.Label lbCost;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbMaxCost;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbSpeed;
        private System.Windows.Forms.Label label8;
    }
}