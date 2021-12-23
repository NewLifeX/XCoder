namespace CrazyCoder.Data
{
    partial class FrmSync
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
            this.gbSource = new System.Windows.Forms.GroupBox();
            this.cbConn = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnConnection = new System.Windows.Forms.Button();
            this.gbTarget = new System.Windows.Forms.GroupBox();
            this.cbTarget = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSync = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.gbSetting = new System.Windows.Forms.GroupBox();
            this.cbIgnoreError = new System.Windows.Forms.CheckBox();
            this.cbSyncSchema = new System.Windows.Forms.CheckBox();
            this.btnSelectOther = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.gbSource.SuspendLayout();
            this.gbTarget.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.gbSetting.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbSource
            // 
            this.gbSource.Controls.Add(this.cbConn);
            this.gbSource.Controls.Add(this.label4);
            this.gbSource.ForeColor = System.Drawing.Color.DeepPink;
            this.gbSource.Location = new System.Drawing.Point(13, 13);
            this.gbSource.Margin = new System.Windows.Forms.Padding(4);
            this.gbSource.Name = "gbSource";
            this.gbSource.Padding = new System.Windows.Forms.Padding(4);
            this.gbSource.Size = new System.Drawing.Size(396, 63);
            this.gbSource.TabIndex = 9;
            this.gbSource.TabStop = false;
            this.gbSource.Text = "来源";
            // 
            // cbConn
            // 
            this.cbConn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbConn.ForeColor = System.Drawing.Color.DeepPink;
            this.cbConn.FormattingEnabled = true;
            this.cbConn.Location = new System.Drawing.Point(87, 18);
            this.cbConn.Margin = new System.Windows.Forms.Padding(4);
            this.cbConn.Name = "cbConn";
            this.cbConn.Size = new System.Drawing.Size(301, 28);
            this.cbConn.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 24);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 20);
            this.label4.TabIndex = 12;
            this.label4.Text = "连接：";
            // 
            // btnConnection
            // 
            this.btnConnection.ForeColor = System.Drawing.Color.DeepPink;
            this.btnConnection.Location = new System.Drawing.Point(417, 27);
            this.btnConnection.Margin = new System.Windows.Forms.Padding(4);
            this.btnConnection.Name = "btnConnection";
            this.btnConnection.Size = new System.Drawing.Size(78, 38);
            this.btnConnection.TabIndex = 8;
            this.btnConnection.Text = "连接";
            this.btnConnection.UseVisualStyleBackColor = true;
            this.btnConnection.Click += new System.EventHandler(this.btnConnection_Click);
            // 
            // gbTarget
            // 
            this.gbTarget.Controls.Add(this.cbTarget);
            this.gbTarget.Controls.Add(this.label1);
            this.gbTarget.Enabled = false;
            this.gbTarget.ForeColor = System.Drawing.Color.DeepPink;
            this.gbTarget.Location = new System.Drawing.Point(558, 13);
            this.gbTarget.Margin = new System.Windows.Forms.Padding(4);
            this.gbTarget.Name = "gbTarget";
            this.gbTarget.Padding = new System.Windows.Forms.Padding(4);
            this.gbTarget.Size = new System.Drawing.Size(531, 63);
            this.gbTarget.TabIndex = 10;
            this.gbTarget.TabStop = false;
            this.gbTarget.Text = "目标";
            // 
            // cbTarget
            // 
            this.cbTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTarget.ForeColor = System.Drawing.Color.DeepPink;
            this.cbTarget.FormattingEnabled = true;
            this.cbTarget.Location = new System.Drawing.Point(88, 18);
            this.cbTarget.Margin = new System.Windows.Forms.Padding(4);
            this.cbTarget.Name = "cbTarget";
            this.cbTarget.Size = new System.Drawing.Size(349, 28);
            this.cbTarget.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 24);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 20);
            this.label1.TabIndex = 12;
            this.label1.Text = "连接：";
            // 
            // btnSync
            // 
            this.btnSync.ForeColor = System.Drawing.Color.DeepPink;
            this.btnSync.Location = new System.Drawing.Point(990, 17);
            this.btnSync.Margin = new System.Windows.Forms.Padding(4);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(78, 38);
            this.btnSync.TabIndex = 14;
            this.btnSync.Text = "同步";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 176);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 29;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1077, 507);
            this.dataGridView1.TabIndex = 11;
            // 
            // gbSetting
            // 
            this.gbSetting.Controls.Add(this.btnSync);
            this.gbSetting.Controls.Add(this.cbIgnoreError);
            this.gbSetting.Controls.Add(this.cbSyncSchema);
            this.gbSetting.Controls.Add(this.btnSelectOther);
            this.gbSetting.Controls.Add(this.btnSelectAll);
            this.gbSetting.Enabled = false;
            this.gbSetting.ForeColor = System.Drawing.Color.DeepPink;
            this.gbSetting.Location = new System.Drawing.Point(13, 84);
            this.gbSetting.Margin = new System.Windows.Forms.Padding(4);
            this.gbSetting.Name = "gbSetting";
            this.gbSetting.Padding = new System.Windows.Forms.Padding(4);
            this.gbSetting.Size = new System.Drawing.Size(1076, 85);
            this.gbSetting.TabIndex = 12;
            this.gbSetting.TabStop = false;
            this.gbSetting.Text = "参数";
            // 
            // cbIgnoreError
            // 
            this.cbIgnoreError.AutoSize = true;
            this.cbIgnoreError.Location = new System.Drawing.Point(346, 32);
            this.cbIgnoreError.Name = "cbIgnoreError";
            this.cbIgnoreError.Size = new System.Drawing.Size(91, 24);
            this.cbIgnoreError.TabIndex = 18;
            this.cbIgnoreError.Text = "忽略错误";
            this.cbIgnoreError.UseVisualStyleBackColor = true;
            // 
            // cbSyncSchema
            // 
            this.cbSyncSchema.AutoSize = true;
            this.cbSyncSchema.Location = new System.Drawing.Point(214, 32);
            this.cbSyncSchema.Name = "cbSyncSchema";
            this.cbSyncSchema.Size = new System.Drawing.Size(91, 24);
            this.cbSyncSchema.TabIndex = 17;
            this.cbSyncSchema.Text = "同步架构";
            this.cbSyncSchema.UseVisualStyleBackColor = true;
            // 
            // btnSelectOther
            // 
            this.btnSelectOther.ForeColor = System.Drawing.Color.DeepPink;
            this.btnSelectOther.Location = new System.Drawing.Point(95, 23);
            this.btnSelectOther.Margin = new System.Windows.Forms.Padding(4);
            this.btnSelectOther.Name = "btnSelectOther";
            this.btnSelectOther.Size = new System.Drawing.Size(78, 38);
            this.btnSelectOther.TabIndex = 16;
            this.btnSelectOther.Text = "反选";
            this.btnSelectOther.UseVisualStyleBackColor = true;
            this.btnSelectOther.Click += new System.EventHandler(this.btnSelectOther_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.ForeColor = System.Drawing.Color.DeepPink;
            this.btnSelectAll.Location = new System.Drawing.Point(9, 23);
            this.btnSelectAll.Margin = new System.Windows.Forms.Padding(4);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(78, 38);
            this.btnSelectAll.TabIndex = 15;
            this.btnSelectAll.Text = "全选";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // FrmSync
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1104, 695);
            this.Controls.Add(this.gbSetting);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.gbTarget);
            this.Controls.Add(this.gbSource);
            this.Controls.Add(this.btnConnection);
            this.Name = "FrmSync";
            this.Text = "数据同步";
            this.Load += new System.EventHandler(this.FrmSync_Load);
            this.gbSource.ResumeLayout(false);
            this.gbSource.PerformLayout();
            this.gbTarget.ResumeLayout(false);
            this.gbTarget.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.gbSetting.ResumeLayout(false);
            this.gbSetting.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox gbSource;
        private ComboBox cbConn;
        private Label label4;
        private Button btnConnection;
        private GroupBox gbTarget;
        private ComboBox cbTarget;
        private Label label1;
        private Button btnSync;
        private DataGridView dataGridView1;
        private GroupBox gbSetting;
        private Button btnSelectOther;
        private Button btnSelectAll;
        private CheckBox cbIgnoreError;
        private CheckBox cbSyncSchema;
    }
}