namespace XCoder
{
    partial class FrmModel
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Clean up any resources being used.</summary>
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnAddIndex = new System.Windows.Forms.Button();
            this.btnAddColumn = new System.Windows.Forms.Button();
            this.btnAddTable = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbTables = new System.Windows.Forms.ComboBox();
            this.gv = new System.Windows.Forms.DataGridView();
            this.pgTable = new System.Windows.Forms.PropertyGrid();
            this.pgColumn = new System.Windows.Forms.PropertyGrid();
            this.dgvIndex = new System.Windows.Forms.DataGridView();
            this.btnCreateTableSQL = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cbConn = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnCreateDb = new System.Windows.Forms.Button();
            this.btnCreateDbSQL = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIndex)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnAddIndex);
            this.groupBox1.Controls.Add(this.btnAddColumn);
            this.groupBox1.Controls.Add(this.btnAddTable);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbTables);
            this.groupBox1.Location = new System.Drawing.Point(9, 3);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(804, 73);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // btnAddIndex
            // 
            this.btnAddIndex.Location = new System.Drawing.Point(580, 18);
            this.btnAddIndex.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAddIndex.Name = "btnAddIndex";
            this.btnAddIndex.Size = new System.Drawing.Size(102, 45);
            this.btnAddIndex.TabIndex = 4;
            this.btnAddIndex.Text = "添加索引";
            this.btnAddIndex.UseVisualStyleBackColor = true;
            this.btnAddIndex.Click += new System.EventHandler(this.btnAddIndex_Click);
            // 
            // btnAddColumn
            // 
            this.btnAddColumn.Location = new System.Drawing.Point(470, 18);
            this.btnAddColumn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAddColumn.Name = "btnAddColumn";
            this.btnAddColumn.Size = new System.Drawing.Size(102, 45);
            this.btnAddColumn.TabIndex = 3;
            this.btnAddColumn.Text = "添加字段";
            this.btnAddColumn.UseVisualStyleBackColor = true;
            this.btnAddColumn.Click += new System.EventHandler(this.btnAddColumn_Click);
            // 
            // btnAddTable
            // 
            this.btnAddTable.Location = new System.Drawing.Point(358, 18);
            this.btnAddTable.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAddTable.Name = "btnAddTable";
            this.btnAddTable.Size = new System.Drawing.Size(102, 45);
            this.btnAddTable.TabIndex = 2;
            this.btnAddTable.Text = "添加表";
            this.btnAddTable.UseVisualStyleBackColor = true;
            this.btnAddTable.Click += new System.EventHandler(this.btnAddTable_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 30);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "数据表：";
            // 
            // cbTables
            // 
            this.cbTables.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTables.FormattingEnabled = true;
            this.cbTables.Location = new System.Drawing.Point(98, 23);
            this.cbTables.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbTables.Name = "cbTables";
            this.cbTables.Size = new System.Drawing.Size(250, 28);
            this.cbTables.TabIndex = 0;
            this.cbTables.SelectedIndexChanged += new System.EventHandler(this.cbTables_SelectedIndexChanged);
            // 
            // gv
            // 
            this.gv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gv.Location = new System.Drawing.Point(291, 87);
            this.gv.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gv.Name = "gv";
            this.gv.RowHeadersWidth = 51;
            this.gv.RowTemplate.Height = 23;
            this.gv.Size = new System.Drawing.Size(944, 477);
            this.gv.TabIndex = 3;
            this.gv.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.gv_RowEnter);
            // 
            // pgTable
            // 
            this.pgTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pgTable.Location = new System.Drawing.Point(9, 88);
            this.pgTable.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pgTable.Name = "pgTable";
            this.pgTable.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgTable.Size = new System.Drawing.Size(273, 687);
            this.pgTable.TabIndex = 4;
            // 
            // pgColumn
            // 
            this.pgColumn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgColumn.Location = new System.Drawing.Point(1245, 88);
            this.pgColumn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pgColumn.Name = "pgColumn";
            this.pgColumn.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgColumn.Size = new System.Drawing.Size(273, 687);
            this.pgColumn.TabIndex = 5;
            // 
            // dgvIndex
            // 
            this.dgvIndex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvIndex.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvIndex.Location = new System.Drawing.Point(291, 573);
            this.dgvIndex.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvIndex.Name = "dgvIndex";
            this.dgvIndex.RowHeadersWidth = 51;
            this.dgvIndex.RowTemplate.Height = 23;
            this.dgvIndex.Size = new System.Drawing.Size(944, 187);
            this.dgvIndex.TabIndex = 6;
            this.dgvIndex.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.gv_RowEnter);
            // 
            // btnCreateTableSQL
            // 
            this.btnCreateTableSQL.Location = new System.Drawing.Point(310, 18);
            this.btnCreateTableSQL.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCreateTableSQL.Name = "btnCreateTableSQL";
            this.btnCreateTableSQL.Size = new System.Drawing.Size(102, 45);
            this.btnCreateTableSQL.TabIndex = 6;
            this.btnCreateTableSQL.Text = "建表语句";
            this.btnCreateTableSQL.UseVisualStyleBackColor = true;
            this.btnCreateTableSQL.Click += new System.EventHandler(this.btnCreateTableSQL_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 30);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 20);
            this.label2.TabIndex = 8;
            this.label2.Text = "连接：";
            // 
            // cbConn
            // 
            this.cbConn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbConn.FormattingEnabled = true;
            this.cbConn.Location = new System.Drawing.Point(66, 23);
            this.cbConn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbConn.Name = "cbConn";
            this.cbConn.Size = new System.Drawing.Size(212, 28);
            this.cbConn.TabIndex = 7;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbConn);
            this.groupBox2.Controls.Add(this.btnCreateDb);
            this.groupBox2.Controls.Add(this.btnCreateDbSQL);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btnCreateTableSQL);
            this.groupBox2.Location = new System.Drawing.Point(822, 3);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(696, 73);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            // 
            // btnCreateDb
            // 
            this.btnCreateDb.Location = new System.Drawing.Point(562, 18);
            this.btnCreateDb.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCreateDb.Name = "btnCreateDb";
            this.btnCreateDb.Size = new System.Drawing.Size(102, 45);
            this.btnCreateDb.TabIndex = 10;
            this.btnCreateDb.Text = "建数据库";
            this.btnCreateDb.UseVisualStyleBackColor = true;
            this.btnCreateDb.Click += new System.EventHandler(this.btnCreateDb_Click);
            // 
            // btnCreateDbSQL
            // 
            this.btnCreateDbSQL.Location = new System.Drawing.Point(422, 18);
            this.btnCreateDbSQL.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCreateDbSQL.Name = "btnCreateDbSQL";
            this.btnCreateDbSQL.Size = new System.Drawing.Size(132, 45);
            this.btnCreateDbSQL.TabIndex = 9;
            this.btnCreateDbSQL.Text = "建所有表语句";
            this.btnCreateDbSQL.UseVisualStyleBackColor = true;
            this.btnCreateDbSQL.Click += new System.EventHandler(this.btnCreateDbSQL_Click);
            // 
            // FrmModel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1530, 780);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.dgvIndex);
            this.Controls.Add(this.pgColumn);
            this.Controls.Add(this.pgTable);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gv);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FrmModel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据架构管理";
            this.Load += new System.EventHandler(this.FrmModel_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIndex)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbTables;
        private System.Windows.Forms.DataGridView gv;
        private System.Windows.Forms.PropertyGrid pgTable;
        private System.Windows.Forms.PropertyGrid pgColumn;
        private System.Windows.Forms.DataGridView dgvIndex;
        private System.Windows.Forms.Button btnAddIndex;
        private System.Windows.Forms.Button btnAddColumn;
        private System.Windows.Forms.Button btnAddTable;
        private System.Windows.Forms.Button btnCreateTableSQL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbConn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnCreateDb;
        private System.Windows.Forms.Button btnCreateDbSQL;
    }
}