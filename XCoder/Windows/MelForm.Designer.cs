namespace XCoder
{
    partial class MelForm
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
            bt_Open = new Button();
            cb_ch = new ComboBox();
            pic_mel = new PictureBox();
            pic_vol = new PictureBox();
            label1 = new Label();
            label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)pic_mel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pic_vol).BeginInit();
            SuspendLayout();
            // 
            // bt_Open
            // 
            bt_Open.Location = new Point(12, 12);
            bt_Open.Name = "bt_Open";
            bt_Open.Size = new Size(89, 25);
            bt_Open.TabIndex = 0;
            bt_Open.Text = "打开 wav";
            bt_Open.UseVisualStyleBackColor = true;
            bt_Open.Click += bt_Open_Click;
            // 
            // cb_ch
            // 
            cb_ch.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_ch.FormattingEnabled = true;
            cb_ch.Location = new Point(135, 13);
            cb_ch.Name = "cb_ch";
            cb_ch.Size = new Size(121, 25);
            cb_ch.TabIndex = 1;
            cb_ch.SelectedIndexChanged += cb_ch_SelectedIndexChanged;
            // 
            // pic_mel
            // 
            pic_mel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pic_mel.BackColor = Color.LightGray;
            pic_mel.Location = new Point(12, 71);
            pic_mel.Name = "pic_mel";
            pic_mel.Size = new Size(628, 80);
            pic_mel.SizeMode = PictureBoxSizeMode.Zoom;
            pic_mel.TabIndex = 2;
            pic_mel.TabStop = false;
            // 
            // pic_vol
            // 
            pic_vol.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pic_vol.BackColor = Color.LightGray;
            pic_vol.Location = new Point(12, 186);
            pic_vol.Name = "pic_vol";
            pic_vol.Size = new Size(628, 200);
            pic_vol.SizeMode = PictureBoxSizeMode.Zoom;
            pic_vol.TabIndex = 3;
            pic_vol.TabStop = false;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(12, 51);
            label1.Name = "label1";
            label1.Size = new Size(56, 17);
            label1.TabIndex = 4;
            label1.Text = "梅尔频谱";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(12, 166);
            label2.Name = "label2";
            label2.Size = new Size(32, 17);
            label2.TabIndex = 5;
            label2.Text = "音量";
            // 
            // MelForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(652, 404);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(pic_vol);
            Controls.Add(pic_mel);
            Controls.Add(cb_ch);
            Controls.Add(bt_Open);
            Name = "MelForm";
            Text = "MelForm";
            Load += MelForm_Load;
            ((System.ComponentModel.ISupportInitialize)pic_mel).EndInit();
            ((System.ComponentModel.ISupportInitialize)pic_vol).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button bt_Open;
        private ComboBox cb_ch;
        private PictureBox pic_mel;
        private PictureBox pic_vol;
        private Label label1;
        private Label label2;
    }
}