using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrazyCoder.Data.Models;
using NewLife.Configuration;
using XCode.DataAccessLayer;
using XCoder;

namespace CrazyCoder.Data
{
    [Category("数据工具")]
    [DisplayName("跨库数据同步")]
    public partial class FrmSync : Form, IXForm
    {
        DAL _source;
        DAL _target;
        IList<TableModel> _models;

        public FrmSync()
        {
            InitializeComponent();
        }

        private void FrmSync_Load(Object sender, EventArgs e)
        {
            cbConn.DataSource = DAL.ConnStrs.Keys.ToArray();

            Task.Run(() => DbHelper.AutoDetectDatabase(() => this.Invoke(SetDatabaseList, DAL.ConnStrs.Keys.OrderBy(e => e).ToList())));
        }

        void SetDatabaseList(ICollection<String> list)
        {
            var str = cbConn.Text;

            cbConn.DataSource = list;
            //cbConn.DisplayMember = "value";

            if (!String.IsNullOrEmpty(str)) cbConn.Text = str;

            if (cbConn.SelectedIndex < 0 && cbConn.Items != null && cbConn.Items.Count > 0) cbConn.SelectedIndex = 0;
        }

        private void btnConnection_Click(Object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var connName = cbConn.SelectedItem + "";
            _source = DAL.Create(connName);

            if (btn.Text == "连接")
            {
                var conns = DAL.ConnStrs.Keys.Where(e => e != connName).ToArray();
                cbTarget.DataSource = conns;

                // 获取数据表
                var tables = _source.Tables;
                _models = tables.Select(e => new TableModel
                {
                    Name = e.TableName,
                    DisplayName = e.DisplayName,
                    EnableSync = true
                }).ToList();
                Task.Run(FetchRows);

                dataGridView1.DataSource = _models;

                gbSource.Enabled = false;
                gbTarget.Enabled = true;
                btn.Text = "断开";
            }
            else
            {

                gbSource.Enabled = true;
                gbTarget.Enabled = false;
                btn.Text = "连接";
            }
        }

        void FetchRows()
        {
            foreach (var item in _models)
            {
                var sb = new SelectBuilder { Table = item.Name };
                item.Total = _source.SelectCount(sb);
            }

            Invoke(() => dataGridView1.Refresh());
        }

        private void btnSync_Click(Object sender, EventArgs e)
        {

        }
    }
}
