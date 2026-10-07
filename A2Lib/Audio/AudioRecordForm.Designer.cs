namespace ALib.Audio
{
    partial class AudioRecordForm
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
            rtbInfo = new RichTextBox();
            grpVolume = new GroupBox();
            pbMelL = new PictureBox();
            pbMelR = new PictureBox();
            progressLeft = new ProgressBar();
            progressRight = new ProgressBar();
            lbLeft = new Label();
            lbRight = new Label();
            grpRecord = new GroupBox();
            btnSelectDevice = new Button();
            btnStart = new Button();
            btnStop = new Button();
            lbRecordStatus = new Label();
            cbSaveFile = new CheckBox();
            rtbLog = new RichTextBox();
            grpVolume.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbMelL).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbMelR).BeginInit();
            grpRecord.SuspendLayout();
            SuspendLayout();
            // 
            // rtbInfo
            // 
            rtbInfo.Location = new Point(12, 12);
            rtbInfo.Name = "rtbInfo";
            rtbInfo.ReadOnly = true;
            rtbInfo.Size = new Size(560, 134);
            rtbInfo.TabIndex = 0;
            rtbInfo.Text = "";
            // 
            // grpVolume
            // 
            grpVolume.Controls.Add(pbMelL);
            grpVolume.Controls.Add(pbMelR);
            grpVolume.Controls.Add(progressLeft);
            grpVolume.Controls.Add(progressRight);
            grpVolume.Controls.Add(lbLeft);
            grpVolume.Controls.Add(lbRight);
            grpVolume.Location = new Point(12, 152);
            grpVolume.Name = "grpVolume";
            grpVolume.Size = new Size(560, 210);
            grpVolume.TabIndex = 1;
            grpVolume.TabStop = false;
            grpVolume.Text = "音量显示";
            // 
            // pbMelL
            // 
            pbMelL.Location = new Point(15, 62);
            pbMelL.Name = "pbMelL";
            pbMelL.Size = new Size(250, 140);
            pbMelL.TabIndex = 4;
            pbMelL.TabStop = false;
            // 
            // pbMelR
            // 
            pbMelR.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbMelR.Location = new Point(295, 62);
            pbMelR.Name = "pbMelR";
            pbMelR.Size = new Size(250, 140);
            pbMelR.TabIndex = 5;
            pbMelR.TabStop = false;
            // 
            // progressLeft
            // 
            progressLeft.Location = new Point(15, 39);
            progressLeft.Name = "progressLeft";
            progressLeft.Size = new Size(250, 17);
            progressLeft.TabIndex = 0;
            // 
            // progressRight
            // 
            progressRight.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            progressRight.Location = new Point(295, 39);
            progressRight.Name = "progressRight";
            progressRight.Size = new Size(250, 17);
            progressRight.TabIndex = 1;
            // 
            // lbLeft
            // 
            lbLeft.AutoSize = true;
            lbLeft.Location = new Point(15, 19);
            lbLeft.Name = "lbLeft";
            lbLeft.Size = new Size(49, 17);
            lbLeft.TabIndex = 2;
            lbLeft.Text = "L: 0.0%";
            // 
            // lbRight
            // 
            lbRight.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbRight.AutoSize = true;
            lbRight.Location = new Point(295, 19);
            lbRight.Name = "lbRight";
            lbRight.Size = new Size(51, 17);
            lbRight.TabIndex = 3;
            lbRight.Text = "R: 0.0%";
            // 
            // grpRecord
            // 
            grpRecord.Controls.Add(btnSelectDevice);
            grpRecord.Controls.Add(btnStart);
            grpRecord.Controls.Add(btnStop);
            grpRecord.Controls.Add(lbRecordStatus);
            grpRecord.Controls.Add(cbSaveFile);
            grpRecord.Location = new Point(12, 368);
            grpRecord.Name = "grpRecord";
            grpRecord.Size = new Size(560, 80);
            grpRecord.TabIndex = 2;
            grpRecord.TabStop = false;
            grpRecord.Text = "录音控制";
            // 
            // btnSelectDevice
            // 
            btnSelectDevice.Location = new Point(15, 30);
            btnSelectDevice.Name = "btnSelectDevice";
            btnSelectDevice.Size = new Size(100, 30);
            btnSelectDevice.TabIndex = 0;
            btnSelectDevice.Text = "选择声卡";
            btnSelectDevice.UseVisualStyleBackColor = true;
            btnSelectDevice.Click += btnSelectDevice_Click;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(130, 30);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(100, 30);
            btnStart.TabIndex = 1;
            btnStart.Text = "开始录音";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(245, 30);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(100, 30);
            btnStop.TabIndex = 2;
            btnStop.Text = "停止录音";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // lbRecordStatus
            // 
            lbRecordStatus.AutoSize = true;
            lbRecordStatus.Location = new Point(360, 38);
            lbRecordStatus.Name = "lbRecordStatus";
            lbRecordStatus.Size = new Size(68, 17);
            lbRecordStatus.TabIndex = 3;
            lbRecordStatus.Text = "未开始录音";
            // 
            // cbSaveFile
            // 
            cbSaveFile.AutoSize = true;
            cbSaveFile.Checked = true;
            cbSaveFile.CheckState = CheckState.Checked;
            cbSaveFile.Location = new Point(130, 58);
            cbSaveFile.Name = "cbSaveFile";
            cbSaveFile.Size = new Size(75, 21);
            cbSaveFile.TabIndex = 4;
            cbSaveFile.Text = "保存文件";
            cbSaveFile.UseVisualStyleBackColor = true;
            // 
            // rtbLog
            // 
            rtbLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            rtbLog.Location = new Point(12, 454);
            rtbLog.Name = "rtbLog";
            rtbLog.Size = new Size(560, 168);
            rtbLog.TabIndex = 3;
            rtbLog.Text = "";
            // 
            // AudioRecordForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 634);
            Controls.Add(rtbLog);
            Controls.Add(grpRecord);
            Controls.Add(grpVolume);
            Controls.Add(rtbInfo);
            Name = "AudioRecordForm";
            Text = "录音测试";
            FormClosing += AudioRecordForm_FormClosing;
            Load += AudioRecordForm_Load;
            grpVolume.ResumeLayout(false);
            grpVolume.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbMelL).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbMelR).EndInit();
            grpRecord.ResumeLayout(false);
            grpRecord.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox rtbInfo;
        private GroupBox grpVolume;
        private ProgressBar progressLeft;
        private ProgressBar progressRight;
        private Label lbLeft;
        private Label lbRight;
        private GroupBox grpRecord;
        private Button btnSelectDevice;
        private Button btnStart;
        private Button btnStop;
        private Label lbRecordStatus;
        private CheckBox cbSaveFile;
        private RichTextBox rtbLog;
        private PictureBox pbMelR;
        private PictureBox pbMelL;
    }
}
