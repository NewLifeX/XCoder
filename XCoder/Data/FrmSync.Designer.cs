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
            this.btnSync = new System.Windows.Forms.Button();
            this.cbTarget = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.gbSource.SuspendLayout();
            this.gbTarget.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbSource
            // 
            this.gbSource.Controls.Add(this.cbConn);
            this.gbSource.Controls.Add(this.label4);
            this.gbSource.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.gbSource.ForeColor = System.Drawing.Color.DeepPink;
            this.gbSource.Location = new System.Drawing.Point(13, 13);
            this.gbSource.Margin = new System.Windows.Forms.Padding(4);
            this.gbSource.Name = "gbSource";
            this.gbSource.Padding = new System.Windows.Forms.Padding(4);
            this.gbSource.Size = new System.Drawing.Size(287, 63);
            this.gbSource.TabIndex = 9;
            this.gbSource.TabStop = false;
            this.gbSource.Text = "来源";
            // 
            // cbConn
            // 
            this.cbConn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbConn.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.cbConn.ForeColor = System.Drawing.Color.DeepPink;
            this.cbConn.FormattingEnabled = true;
            this.cbConn.Location = new System.Drawing.Point(87, 18);
            this.cbConn.Margin = new System.Windows.Forms.Padding(4);
            this.cbConn.Name = "cbConn";
            this.cbConn.Size = new System.Drawing.Size(186, 32);
            this.cbConn.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 24);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 19);
            this.label4.TabIndex = 12;
            this.label4.Text = "连接：";
            // 
            // btnConnection
            // 
            this.btnConnection.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnConnection.ForeColor = System.Drawing.Color.DeepPink;
            this.btnConnection.Location = new System.Drawing.Point(323, 27);
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
            this.gbTarget.Controls.Add(this.btnSync);
            this.gbTarget.Controls.Add(this.cbTarget);
            this.gbTarget.Controls.Add(this.label1);
            this.gbTarget.Enabled = false;
            this.gbTarget.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.gbTarget.ForeColor = System.Drawing.Color.DeepPink;
            this.gbTarget.Location = new System.Drawing.Point(439, 13);
            this.gbTarget.Margin = new System.Windows.Forms.Padding(4);
            this.gbTarget.Name = "gbTarget";
            this.gbTarget.Padding = new System.Windows.Forms.Padding(4);
            this.gbTarget.Size = new System.Drawing.Size(424, 63);
            this.gbTarget.TabIndex = 10;
            this.gbTarget.TabStop = false;
            this.gbTarget.Text = "目标";
            // 
            // btnSync
            // 
            this.btnSync.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSync.ForeColor = System.Drawing.Color.DeepPink;
            this.btnSync.Location = new System.Drawing.Point(311, 14);
            this.btnSync.Margin = new System.Windows.Forms.Padding(4);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(78, 38);
            this.btnSync.TabIndex = 14;
            this.btnSync.Text = "同步";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // cbTarget
            // 
            this.cbTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTarget.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.cbTarget.ForeColor = System.Drawing.Color.DeepPink;
            this.cbTarget.FormattingEnabled = true;
            this.cbTarget.Location = new System.Drawing.Point(88, 18);
            this.cbTarget.Margin = new System.Windows.Forms.Padding(4);
            this.cbTarget.Name = "cbTarget";
            this.cbTarget.Size = new System.Drawing.Size(186, 32);
            this.cbTarget.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 24);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 19);
            this.label1.TabIndex = 12;
            this.label1.Text = "连接：";
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.CheckBoxes = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(12, 83);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(851, 443);
            this.listView1.TabIndex = 11;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "选择";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "表名";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "描述";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "数据行";
            // 
            // FrmSync
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(878, 538);
            this.Controls.Add(this.listView1);
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
        private ListView listView1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
    }
}