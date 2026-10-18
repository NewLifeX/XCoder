namespace ALib.ComPort
{
    partial class GrblTool
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
            components = new System.ComponentModel.Container();
            btYInc = new Button();
            btYDec = new Button();
            btXDec = new Button();
            btXInc = new Button();
            btZDec = new Button();
            btZInc = new Button();
            lbXYZ = new Label();
            lbCurrXYZ = new Label();
            cbMoveValue = new ComboBox();
            label3 = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            label1 = new Label();
            label2 = new Label();
            label4 = new Label();
            label6 = new Label();
            btUnlock = new Button();
            label7 = new Label();
            SuspendLayout();
            // 
            // btYInc
            // 
            btYInc.Location = new Point(136, 145);
            btYInc.Name = "btYInc";
            btYInc.Size = new Size(60, 60);
            btYInc.TabIndex = 0;
            btYInc.TabStop = false;
            btYInc.Text = "Y+";
            btYInc.UseVisualStyleBackColor = true;
            btYInc.Click += btYInc_Click;
            btYInc.KeyDown += GrblTool_KeyDown;
            // 
            // btYDec
            // 
            btYDec.Location = new Point(136, 211);
            btYDec.Name = "btYDec";
            btYDec.Size = new Size(60, 60);
            btYDec.TabIndex = 1;
            btYDec.TabStop = false;
            btYDec.Text = "Y-";
            btYDec.UseVisualStyleBackColor = true;
            btYDec.Click += btYDec_Click;
            btYDec.KeyDown += GrblTool_KeyDown;
            // 
            // btXDec
            // 
            btXDec.Location = new Point(70, 211);
            btXDec.Name = "btXDec";
            btXDec.Size = new Size(60, 60);
            btXDec.TabIndex = 2;
            btXDec.TabStop = false;
            btXDec.Text = "X-";
            btXDec.UseVisualStyleBackColor = true;
            btXDec.Click += btXDec_Click;
            btXDec.KeyDown += GrblTool_KeyDown;
            // 
            // btXInc
            // 
            btXInc.Location = new Point(202, 211);
            btXInc.Name = "btXInc";
            btXInc.Size = new Size(60, 60);
            btXInc.TabIndex = 3;
            btXInc.TabStop = false;
            btXInc.Text = "X+";
            btXInc.UseVisualStyleBackColor = true;
            btXInc.Click += btXInc_Click;
            btXInc.KeyDown += GrblTool_KeyDown;
            // 
            // btZDec
            // 
            btZDec.Location = new Point(306, 211);
            btZDec.Name = "btZDec";
            btZDec.Size = new Size(60, 60);
            btZDec.TabIndex = 5;
            btZDec.TabStop = false;
            btZDec.Text = "Z-";
            btZDec.UseVisualStyleBackColor = true;
            btZDec.Click += btZDec_Click;
            btZDec.KeyDown += GrblTool_KeyDown;
            // 
            // btZInc
            // 
            btZInc.Location = new Point(306, 145);
            btZInc.Name = "btZInc";
            btZInc.Size = new Size(60, 60);
            btZInc.TabIndex = 4;
            btZInc.TabStop = false;
            btZInc.Text = "Z+";
            btZInc.UseVisualStyleBackColor = true;
            btZInc.Click += btZInc_Click;
            btZInc.KeyDown += GrblTool_KeyDown;
            // 
            // lbXYZ
            // 
            lbXYZ.AutoSize = true;
            lbXYZ.Location = new Point(72, 55);
            lbXYZ.Name = "lbXYZ";
            lbXYZ.Size = new Size(57, 17);
            lbXYZ.TabIndex = 0;
            lbXYZ.Text = "X0,Y0,Z0";
            // 
            // lbCurrXYZ
            // 
            lbCurrXYZ.AutoSize = true;
            lbCurrXYZ.Location = new Point(72, 84);
            lbCurrXYZ.Name = "lbCurrXYZ";
            lbCurrXYZ.Size = new Size(68, 17);
            lbCurrXYZ.TabIndex = 7;
            lbCurrXYZ.Text = "CURR_XYZ";
            // 
            // cbMoveValue
            // 
            cbMoveValue.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMoveValue.FormattingEnabled = true;
            cbMoveValue.Items.AddRange(new object[] { "0.1", "1", "5", "10" });
            cbMoveValue.Location = new Point(120, 304);
            cbMoveValue.Name = "cbMoveValue";
            cbMoveValue.Size = new Size(76, 25);
            cbMoveValue.TabIndex = 9;
            cbMoveValue.TabStop = false;
            cbMoveValue.KeyDown += GrblTool_KeyDown;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(70, 307);
            label3.Name = "label3";
            label3.Size = new Size(44, 17);
            label3.TabIndex = 10;
            label3.Text = "单步值";
            // 
            // timer1
            // 
            timer1.Enabled = false;
            timer1.Tick += timer1_Tick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(372, 167);
            label1.Name = "label1";
            label1.Size = new Size(40, 17);
            label1.TabIndex = 11;
            label1.Text = "PgUp";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(372, 233);
            label2.Name = "label2";
            label2.Size = new Size(39, 17);
            label2.TabIndex = 12;
            label2.Text = "PgDn";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(202, 307);
            label4.Name = "label4";
            label4.Size = new Size(39, 17);
            label4.TabIndex = 13;
            label4.Text = "F1-F4";
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Location = new Point(278, 361);
            label6.Name = "label6";
            label6.Size = new Size(164, 17);
            label6.TabIndex = 15;
            label6.Text = "方向键移动，F1-F4切换单步值。";
            // 
            // btUnlock
            // 
            btUnlock.Location = new Point(306, 41);
            btUnlock.Name = "btUnlock";
            btUnlock.Size = new Size(60, 60);
            btUnlock.TabIndex = 17;
            btUnlock.TabStop = false;
            btUnlock.Text = "unlock";
            btUnlock.UseVisualStyleBackColor = true;
            btUnlock.Click += btUnlock_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(372, 63);
            label7.Name = "label7";
            label7.Size = new Size(16, 17);
            label7.TabIndex = 18;
            label7.Text = "X";
            // 
            // GrblTool
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(454, 387);
            Controls.Add(label7);
            Controls.Add(btUnlock);
            Controls.Add(label6);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(label3);
            Controls.Add(cbMoveValue);
            Controls.Add(lbCurrXYZ);
            Controls.Add(lbXYZ);
            Controls.Add(btZDec);
            Controls.Add(btZInc);
            Controls.Add(btXInc);
            Controls.Add(btXDec);
            Controls.Add(btYDec);
            Controls.Add(btYInc);
            ImeMode = ImeMode.Disable;
            KeyPreview = true;
            Name = "GrblTool";
            Text = "GrblTool";
            Load += GrblTool_Load;
            KeyDown += GrblTool_KeyDown;
            KeyPress += GrblTool_KeyPress;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btYInc;
        private Button btYDec;
        private Button btXDec;
        private Button btXInc;
        private Button btZDec;
        private Button btZInc;
        private Label lbXYZ;
        private Label lbCurrXYZ;
        private ComboBox cbMoveValue;
        private Label label3;
        private System.Windows.Forms.Timer timer1;
        private Label label1;
        private Label label2;
        private Label label4;
        private Label label6;
        private Button btUnlock;
        private Label label7;
    }
}