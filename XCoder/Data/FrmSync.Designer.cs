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
            this.btnConnect2 = new System.Windows.Forms.Button();
            this.cbTarget = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSync = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.gbSetting = new System.Windows.Forms.GroupBox();
            this.btnDifferent = new System.Windows.Forms.Button();
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
            this.gbSource.Location = new System.Drawing.Point(16, 16);
            this.gbSource.Margin = new System.Windows.Forms.Padding(5);
            this.gbSource.Name = "gbSource";
            this.gbSource.Padding = new System.Windows.Forms.Padding(5);
            this.gbSource.Size = new System.Drawing.Size(484, 76);
            this.gbSource.TabIndex = 9;
            this.gbSource.TabStop = false;
            this.gbSource.Text = "来源";
            // 
            // cbConn
            // 
            this.cbConn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbConn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbConn.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.cbConn.ForeColor = System.Drawing.Color.DeepPink;
            this.cbConn.FormattingEnabled = true;
            this.cbConn.Location = new System.Drawing.Point(86, 22);
            this.cbConn.Margin = new System.Windows.Forms.Padding(5);
            this.cbConn.Name = "cbConn";
            this.cbConn.Size = new System.Drawing.Size(367, 35);
            this.cbConn.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 29);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 24);
            this.label4.TabIndex = 12;
            this.label4.Text = "连接：";
            // 
            // btnConnection
            // 
            this.btnConnection.ForeColor = System.Drawing.Color.DeepPink;
            this.btnConnection.Location = new System.Drawing.Point(510, 32);
            this.btnConnection.Margin = new System.Windows.Forms.Padding(5);
            this.btnConnection.Name = "btnConnection";
            this.btnConnection.Size = new System.Drawing.Size(95, 46);
            this.btnConnection.TabIndex = 8;
            this.btnConnection.Text = "连接";
            this.btnConnection.UseVisualStyleBackColor = true;
            this.btnConnection.Click += new System.EventHandler(this.btnConnection_Click);
            // 
            // gbTarget
            // 
            this.gbTarget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTarget.Controls.Add(this.btnConnect2);
            this.gbTarget.Controls.Add(this.cbTarget);
            this.gbTarget.Controls.Add(this.label1);
            this.gbTarget.Enabled = false;
            this.gbTarget.ForeColor = System.Drawing.Color.DeepPink;
            this.gbTarget.Location = new System.Drawing.Point(682, 16);
            this.gbTarget.Margin = new System.Windows.Forms.Padding(5);
            this.gbTarget.Name = "gbTarget";
            this.gbTarget.Padding = new System.Windows.Forms.Padding(5);
            this.gbTarget.Size = new System.Drawing.Size(649, 76);
            this.gbTarget.TabIndex = 10;
            this.gbTarget.TabStop = false;
            this.gbTarget.Text = "目标";
            // 
            // btnConnect2
            // 
            this.btnConnect2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect2.ForeColor = System.Drawing.Color.DeepPink;
            this.btnConnect2.Location = new System.Drawing.Point(544, 18);
            this.btnConnect2.Margin = new System.Windows.Forms.Padding(5);
            this.btnConnect2.Name = "btnConnect2";
            this.btnConnect2.Size = new System.Drawing.Size(95, 46);
            this.btnConnect2.TabIndex = 13;
            this.btnConnect2.Text = "连接";
            this.btnConnect2.UseVisualStyleBackColor = true;
            this.btnConnect2.Click += new System.EventHandler(this.btnConnect2_Click);
            // 
            // cbTarget
            // 
            this.cbTarget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTarget.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.cbTarget.ForeColor = System.Drawing.Color.DeepPink;
            this.cbTarget.FormattingEnabled = true;
            this.cbTarget.Location = new System.Drawing.Point(84, 22);
            this.cbTarget.Margin = new System.Windows.Forms.Padding(5);
            this.cbTarget.Name = "cbTarget";
            this.cbTarget.Size = new System.Drawing.Size(426, 35);
            this.cbTarget.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 29);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 24);
            this.label1.TabIndex = 12;
            this.label1.Text = "连接：";
            // 
            // btnSync
            // 
            this.btnSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSync.ForeColor = System.Drawing.Color.DeepPink;
            this.btnSync.Location = new System.Drawing.Point(1210, 38);
            this.btnSync.Margin = new System.Windows.Forms.Padding(5);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(95, 46);
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
            this.dataGridView1.Location = new System.Drawing.Point(15, 211);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 29;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1316, 608);
            this.dataGridView1.TabIndex = 11;
            // 
            // gbSetting
            // 
            this.gbSetting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSetting.Controls.Add(this.btnDifferent);
            this.gbSetting.Controls.Add(this.btnSync);
            this.gbSetting.Controls.Add(this.cbIgnoreError);
            this.gbSetting.Controls.Add(this.cbSyncSchema);
            this.gbSetting.Controls.Add(this.btnSelectOther);
            this.gbSetting.Controls.Add(this.btnSelectAll);
            this.gbSetting.Enabled = false;
            this.gbSetting.ForeColor = System.Drawing.Color.DeepPink;
            this.gbSetting.Location = new System.Drawing.Point(16, 101);
            this.gbSetting.Margin = new System.Windows.Forms.Padding(5);
            this.gbSetting.Name = "gbSetting";
            this.gbSetting.Padding = new System.Windows.Forms.Padding(5);
            this.gbSetting.Size = new System.Drawing.Size(1315, 102);
            this.gbSetting.TabIndex = 12;
            this.gbSetting.TabStop = false;
            this.gbSetting.Text = "参数";
            // 
            // btnDifferent
            // 
            this.btnDifferent.ForeColor = System.Drawing.Color.DeepPink;
            this.btnDifferent.Location = new System.Drawing.Point(221, 38);
            this.btnDifferent.Margin = new System.Windows.Forms.Padding(5);
            this.btnDifferent.Name = "btnDifferent";
            this.btnDifferent.Size = new System.Drawing.Size(95, 46);
            this.btnDifferent.TabIndex = 19;
            this.btnDifferent.Text = "选差异";
            this.btnDifferent.UseVisualStyleBackColor = true;
            this.btnDifferent.Click += new System.EventHandler(this.btnDifferent_Click);
            // 
            // cbIgnoreError
            // 
            this.cbIgnoreError.Location = new System.Drawing.Point(532, 49);
            this.cbIgnoreError.Margin = new System.Windows.Forms.Padding(4);
            this.cbIgnoreError.Name = "cbIgnoreError";
            this.cbIgnoreError.Size = new System.Drawing.Size(141, 28);
            this.cbIgnoreError.TabIndex = 18;
            this.cbIgnoreError.Text = "忽略错误";
            this.cbIgnoreError.UseVisualStyleBackColor = true;
            // 
            // cbSyncSchema
            // 
            this.cbSyncSchema.Location = new System.Drawing.Point(370, 49);
            this.cbSyncSchema.Margin = new System.Windows.Forms.Padding(4);
            this.cbSyncSchema.Name = "cbSyncSchema";
            this.cbSyncSchema.Size = new System.Drawing.Size(141, 28);
            this.cbSyncSchema.TabIndex = 17;
            this.cbSyncSchema.Text = "同步架构";
            this.cbSyncSchema.UseVisualStyleBackColor = true;
            // 
            // btnSelectOther
            // 
            this.btnSelectOther.ForeColor = System.Drawing.Color.DeepPink;
            this.btnSelectOther.Location = new System.Drawing.Point(116, 38);
            this.btnSelectOther.Margin = new System.Windows.Forms.Padding(5);
            this.btnSelectOther.Name = "btnSelectOther";
            this.btnSelectOther.Size = new System.Drawing.Size(95, 46);
            this.btnSelectOther.TabIndex = 16;
            this.btnSelectOther.Text = "反选";
            this.btnSelectOther.UseVisualStyleBackColor = true;
            this.btnSelectOther.Click += new System.EventHandler(this.btnSelectOther_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.ForeColor = System.Drawing.Color.DeepPink;
            this.btnSelectAll.Location = new System.Drawing.Point(11, 38);
            this.btnSelectAll.Margin = new System.Windows.Forms.Padding(5);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(95, 46);
            this.btnSelectAll.TabIndex = 15;
            this.btnSelectAll.Text = "全选";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // FrmSync
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1349, 834);
            this.Controls.Add(this.gbSetting);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.gbTarget);
            this.Controls.Add(this.gbSource);
            this.Controls.Add(this.btnConnection);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmSync";
            this.Text = "跨库数据同步";
            this.Load += new System.EventHandler(this.FrmSync_Load);
            this.gbSource.ResumeLayout(false);
            this.gbSource.PerformLayout();
            this.gbTarget.ResumeLayout(false);
            this.gbTarget.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.gbSetting.ResumeLayout(false);
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
        private Button btnConnect2;
        private Button btnDifferent;
    }
}