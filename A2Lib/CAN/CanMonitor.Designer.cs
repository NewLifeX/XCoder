namespace ALib.CAN
{
    partial class CanMonitor
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
            lvMessage = new ListView();
            lvCanId = new ColumnHeader();
            lvData = new ColumnHeader();
            lvTime = new ColumnHeader();
            lvInv = new ColumnHeader();
            cmsMessage = new ContextMenuStrip(components);
            tsmiClearMsg = new ToolStripMenuItem();
            cbSeCanId = new ComboBox();
            label1 = new Label();
            canSelect = new CanSelect();
            lvtime1 = new System.Windows.Forms.Timer(components);
            cbStop = new CheckBox();
            btPause = new Button();
            label5 = new Label();
            cmsMessage.SuspendLayout();
            SuspendLayout();
            // 
            // lvMessage
            // 
            lvMessage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvMessage.Columns.AddRange(new ColumnHeader[] { lvCanId, lvData, lvTime, lvInv });
            lvMessage.ContextMenuStrip = cmsMessage;
            lvMessage.Font = new Font("Cascadia Mono", 10.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lvMessage.FullRowSelect = true;
            lvMessage.GridLines = true;
            lvMessage.Location = new Point(192, 12);
            lvMessage.MultiSelect = false;
            lvMessage.Name = "lvMessage";
            lvMessage.Size = new Size(566, 746);
            lvMessage.TabIndex = 16;
            lvMessage.UseCompatibleStateImageBehavior = false;
            lvMessage.View = View.Details;
            lvMessage.ItemSelectionChanged += lvMessage_ItemSelectionChanged;
            // 
            // lvCanId
            // 
            lvCanId.Text = "CAN ID";
            lvCanId.Width = 120;
            // 
            // lvData
            // 
            lvData.Text = "数据";
            lvData.Width = 180;
            // 
            // lvTime
            // 
            lvTime.Text = "时间戳";
            lvTime.Width = 160;
            // 
            // lvInv
            // 
            lvInv.Text = "间隔";
            lvInv.Width = 100;
            // 
            // cmsMessage
            // 
            cmsMessage.Items.AddRange(new ToolStripItem[] { tsmiClearMsg });
            cmsMessage.Name = "cmsMessage";
            cmsMessage.Size = new Size(101, 26);
            // 
            // tsmiClearMsg
            // 
            tsmiClearMsg.Name = "tsmiClearMsg";
            tsmiClearMsg.Size = new Size(100, 22);
            tsmiClearMsg.Text = "清空";
            tsmiClearMsg.Click += tsmiClearMsg_Click;
            // 
            // cbSeCanId
            // 
            cbSeCanId.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSeCanId.Font = new Font("Cascadia Mono", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cbSeCanId.FormattingEnabled = true;
            cbSeCanId.Location = new Point(12, 373);
            cbSeCanId.Name = "cbSeCanId";
            cbSeCanId.Size = new Size(174, 24);
            cbSeCanId.TabIndex = 17;
            cbSeCanId.DropDown += cbSeCanId_DropDown;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 353);
            label1.Name = "label1";
            label1.Size = new Size(56, 17);
            label1.TabIndex = 18;
            label1.Text = "消息筛选";
            // 
            // canSelect
            // 
            canSelect.Location = new Point(12, 0);
            canSelect.Name = "canSelect";
            canSelect.Size = new Size(174, 164);
            canSelect.TabIndex = 19;
            canSelect.PortOpened += canSelect_PortOpened;
            canSelect.Received += canSelect_Received;
            // 
            // lvtime1
            // 
            lvtime1.Enabled = true;
            lvtime1.Interval = 50;
            lvtime1.Tick += lvtime1_Tick;
            // 
            // cbStop
            // 
            cbStop.AutoSize = true;
            cbStop.Enabled = false;
            cbStop.Location = new Point(59, 445);
            cbStop.Name = "cbStop";
            cbStop.Size = new Size(75, 21);
            cbStop.TabIndex = 32;
            cbStop.Text = "暂停接收";
            cbStop.UseVisualStyleBackColor = true;
            // 
            // btPause
            // 
            btPause.Location = new Point(12, 472);
            btPause.Name = "btPause";
            btPause.Size = new Size(174, 108);
            btPause.TabIndex = 33;
            btPause.Text = "暂停接收";
            btPause.UseVisualStyleBackColor = true;
            btPause.Click += btPause_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 583);
            label5.Name = "label5";
            label5.Size = new Size(101, 17);
            label5.TabIndex = 34;
            label5.Text = "空格键 暂停/恢复";
            // 
            // CanMonitor
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(770, 770);
            Controls.Add(canSelect);
            Controls.Add(label5);
            Controls.Add(btPause);
            Controls.Add(cbStop);
            Controls.Add(label1);
            Controls.Add(cbSeCanId);
            Controls.Add(lvMessage);
            Name = "CanMonitor";
            Text = "CanMonitor";
            Load += CanMonitor_Load;
            cmsMessage.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView lvMessage;
        private ContextMenuStrip cmsMessage;
        private ToolStripMenuItem tsmiClearMsg;
        private ColumnHeader lvTime;
        private ColumnHeader lvCanId;
        private ColumnHeader lvData;
        private ComboBox cbSeCanId;
        private Label label1;
        private CanSelect canSelect;
        private System.Windows.Forms.Timer lvtime1;
        private ColumnHeader lvInv;
        private CheckBox cbStop;
        private Button btPause;
        private Label label5;
    }
}
