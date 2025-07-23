namespace XCoder
{
    partial class UsbHubSelect
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
            cb_Hub = new ComboBox();
            rtb_Show = new RichTextBox();
            bt_Comf = new Button();
            bt_Canel = new Button();
            label1 = new Label();
            SuspendLayout();
            // 
            // cb_Hub
            // 
            cb_Hub.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_Hub.FormattingEnabled = true;
            cb_Hub.Location = new Point(52, 12);
            cb_Hub.Name = "cb_Hub";
            cb_Hub.Size = new Size(113, 25);
            cb_Hub.TabIndex = 0;
            // 
            // rtb_Show
            // 
            rtb_Show.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtb_Show.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            rtb_Show.Location = new Point(12, 43);
            rtb_Show.Name = "rtb_Show";
            rtb_Show.Size = new Size(889, 327);
            rtb_Show.TabIndex = 1;
            rtb_Show.Text = "";
            // 
            // bt_Comf
            // 
            bt_Comf.Location = new Point(217, 12);
            bt_Comf.Name = "bt_Comf";
            bt_Comf.Size = new Size(75, 25);
            bt_Comf.TabIndex = 2;
            bt_Comf.Text = "确认";
            bt_Comf.UseVisualStyleBackColor = true;
            bt_Comf.Click += bt_Comf_Click;
            // 
            // bt_Canel
            // 
            bt_Canel.Location = new Point(310, 11);
            bt_Canel.Name = "bt_Canel";
            bt_Canel.Size = new Size(75, 25);
            bt_Canel.TabIndex = 3;
            bt_Canel.Text = "取消";
            bt_Canel.UseVisualStyleBackColor = true;
            bt_Canel.Click += bt_Canel_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(34, 17);
            label1.TabIndex = 4;
            label1.Text = "HUB";
            // 
            // UsbHubSelect
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(913, 382);
            Controls.Add(label1);
            Controls.Add(bt_Canel);
            Controls.Add(bt_Comf);
            Controls.Add(rtb_Show);
            Controls.Add(cb_Hub);
            Name = "UsbHubSelect";
            Text = "UsbHubSelect";
            Load += UsbHubSelect_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cb_Hub;
        private RichTextBox rtb_Show;
        private Button bt_Comf;
        private Button bt_Canel;
        private Label label1;
    }
}