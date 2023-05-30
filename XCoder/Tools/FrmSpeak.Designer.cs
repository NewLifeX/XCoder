namespace XCoder.Tools
{
    partial class FrmSpeak
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
            richTextBox1 = new RichTextBox();
            btnSpeak = new Button();
            groupBox1 = new GroupBox();
            cbVoices = new ComboBox();
            label3 = new Label();
            numRate = new NumericUpDown();
            label2 = new Label();
            numVolume = new NumericUpDown();
            label1 = new Label();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numRate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numVolume).BeginInit();
            SuspendLayout();
            // 
            // richTextBox1
            // 
            richTextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            richTextBox1.Font = new Font("Microsoft YaHei UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            richTextBox1.Location = new Point(12, 86);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(997, 485);
            richTextBox1.TabIndex = 0;
            richTextBox1.Text = "学无先后达者为师！";
            // 
            // btnSpeak
            // 
            btnSpeak.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSpeak.Location = new Point(911, 21);
            btnSpeak.Name = "btnSpeak";
            btnSpeak.Size = new Size(98, 51);
            btnSpeak.TabIndex = 1;
            btnSpeak.Text = "文字转语音";
            btnSpeak.UseVisualStyleBackColor = true;
            btnSpeak.Click += btnSpeak_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(cbVoices);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(numRate);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(numVolume);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(794, 68);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "TTS";
            // 
            // cbVoices
            // 
            cbVoices.DropDownStyle = ComboBoxStyle.DropDownList;
            cbVoices.FormattingEnabled = true;
            cbVoices.Location = new Point(66, 26);
            cbVoices.Name = "cbVoices";
            cbVoices.Size = new Size(396, 28);
            cbVoices.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 30);
            label3.Name = "label3";
            label3.Size = new Size(54, 20);
            label3.TabIndex = 4;
            label3.Text = "声音：";
            // 
            // numRate
            // 
            numRate.Location = new Point(711, 27);
            numRate.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numRate.Minimum = new decimal(new int[] { 10, 0, 0, Int32.MinValue });
            numRate.Name = "numRate";
            numRate.Size = new Size(66, 27);
            numRate.TabIndex = 3;
            numRate.TextAlign = HorizontalAlignment.Right;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(651, 30);
            label2.Name = "label2";
            label2.Size = new Size(54, 20);
            label2.TabIndex = 2;
            label2.Text = "速度：";
            // 
            // numVolume
            // 
            numVolume.Location = new Point(549, 27);
            numVolume.Name = "numVolume";
            numVolume.Size = new Size(66, 27);
            numVolume.TabIndex = 1;
            numVolume.TextAlign = HorizontalAlignment.Right;
            numVolume.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(489, 30);
            label1.Name = "label1";
            label1.Size = new Size(54, 20);
            label1.TabIndex = 0;
            label1.Text = "音量：";
            // 
            // FrmSpeak
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1021, 583);
            Controls.Add(groupBox1);
            Controls.Add(btnSpeak);
            Controls.Add(richTextBox1);
            Name = "FrmSpeak";
            Text = "FrmSpeech";
            Load += FrmSpeak_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numRate).EndInit();
            ((System.ComponentModel.ISupportInitialize)numVolume).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox richTextBox1;
        private Button btnSpeak;
        private GroupBox groupBox1;
        private NumericUpDown numVolume;
        private Label label1;
        private NumericUpDown numRate;
        private Label label2;
        private ComboBox cbVoices;
        private Label label3;
    }
}