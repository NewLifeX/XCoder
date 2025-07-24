namespace XCoder
{
    partial class AudioSelect
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
            cb_Dev = new ComboBox();
            label1 = new Label();
            btn_Spk = new Button();
            btn_ok = new Button();
            rtb_Info = new RichTextBox();
            tb_Volume = new TrackBar();
            lb_Volume = new Label();
            ((System.ComponentModel.ISupportInitialize)tb_Volume).BeginInit();
            SuspendLayout();
            // 
            // cb_Dev
            // 
            cb_Dev.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cb_Dev.FormattingEnabled = true;
            cb_Dev.Location = new Point(109, 38);
            cb_Dev.Name = "cb_Dev";
            cb_Dev.Size = new Size(374, 25);
            cb_Dev.TabIndex = 0;
            cb_Dev.SelectedIndexChanged += cb_Dev_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(57, 41);
            label1.Name = "label1";
            label1.Size = new Size(46, 17);
            label1.TabIndex = 1;
            label1.Text = "Device";
            // 
            // btn_Spk
            // 
            btn_Spk.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btn_Spk.Location = new Point(57, 254);
            btn_Spk.Name = "btn_Spk";
            btn_Spk.Size = new Size(91, 34);
            btn_Spk.TabIndex = 2;
            btn_Spk.Text = "TEST";
            btn_Spk.UseVisualStyleBackColor = true;
            btn_Spk.Click += btn_Spk_Click;
            // 
            // btn_ok
            // 
            btn_ok.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btn_ok.Location = new Point(392, 254);
            btn_ok.Name = "btn_ok";
            btn_ok.Size = new Size(91, 34);
            btn_ok.TabIndex = 3;
            btn_ok.Text = "OK";
            btn_ok.UseVisualStyleBackColor = true;
            btn_ok.Click += btn_ok_Click;
            // 
            // rtb_Info
            // 
            rtb_Info.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtb_Info.Location = new Point(57, 72);
            rtb_Info.Name = "rtb_Info";
            rtb_Info.ReadOnly = true;
            rtb_Info.Size = new Size(426, 125);
            rtb_Info.TabIndex = 4;
            rtb_Info.Text = "";
            // 
            // tb_Volume
            // 
            tb_Volume.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tb_Volume.Location = new Point(109, 203);
            tb_Volume.Maximum = 50;
            tb_Volume.Name = "tb_Volume";
            tb_Volume.Size = new Size(374, 45);
            tb_Volume.SmallChange = 5;
            tb_Volume.TabIndex = 5;
            // 
            // lb_Volume
            // 
            lb_Volume.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lb_Volume.AutoSize = true;
            lb_Volume.Location = new Point(57, 212);
            lb_Volume.Name = "lb_Volume";
            lb_Volume.Size = new Size(52, 17);
            lb_Volume.TabIndex = 6;
            lb_Volume.Text = "Volume";
            // 
            // AudioSelect
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(545, 311);
            Controls.Add(lb_Volume);
            Controls.Add(tb_Volume);
            Controls.Add(rtb_Info);
            Controls.Add(btn_ok);
            Controls.Add(btn_Spk);
            Controls.Add(label1);
            Controls.Add(cb_Dev);
            Name = "AudioSelect";
            Text = "AudioSelect";
            Load += AudioSelect_Load;
            ((System.ComponentModel.ISupportInitialize)tb_Volume).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cb_Dev;
        private Label label1;
        private Button btn_Spk;
        private Button btn_ok;
        private RichTextBox rtb_Info;
        private TrackBar tb_Volume;
        private Label lb_Volume;
    }
}