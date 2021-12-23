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
using NewLife;
using NewLife.Configuration;
using NewLife.Log;
using XCode.DataAccessLayer;
using XCoder;

namespace CrazyCoder.Data
{
    [Category("数据工具")]
    [DisplayName("跨库数据同步")]
    public partial class FrmSync : Form, IXForm
    {
        DAL _source;
        IList<IDataTable> _tables;
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
                _tables = _source.Tables;
                _models = _tables.Select(e => new TableModel
                {
                    Name = e.TableName,
                    DisplayName = e.DisplayName,
                    EnableSync = true,
                    Table = e,
                }).ToList();
                Task.Run(FetchRows);

                dataGridView1.DataSource = _models;

                gbSource.Enabled = false;
                gbTarget.Enabled = true;
                gbSetting.Enabled = true;
                btn.Text = "断开";
            }
            else
            {

                gbSource.Enabled = true;
                gbTarget.Enabled = false;
                gbSetting.Enabled = false;
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
            var connName = cbTarget.SelectedItem + "";
            if (connName.IsNullOrEmpty()) return;

            var ts = _models.Select(e => e.Table).ToArray();
            if (ts.Length == 0) return;

            var syncSchema = cbSyncSchema.Enabled;
            var ignoreError = cbIgnoreError.Enabled;
            //_source.SyncAll(ts, connName, cbSyncSchema.Checked, cbIgnoreError.Checked);
            var dpk = new DbPackage
            {
                Dal = _source,
                IgnoreError = ignoreError,
                Log = XTrace.Log
            };
            //dpk.OnProgress = (p, dt) =>
            //{
            //    var m = _models.FirstOrDefault(e => e.Name == dt);
            //};
            Task.Run(() => dpk.SyncAll(ts, connName, syncSchema));
            //Task.Run(() =>
            //{
            //    foreach (var item in ts)
            //    {
            //        try
            //        {
            //            _source.Sync(item, connName, syncSchema, (p, dt) => { });
            //        }
            //        catch (Exception ex)
            //        {
            //            XTrace.WriteException(ex);

            //            if (!ignoreError) throw;
            //        }
            //    }
            //});
        }

        private void btnSelectAll_Click(Object sender, EventArgs e)
        {
            if (_models == null) return;

            foreach (var model in _models)
            {
                model.EnableSync = true;
            }

            dataGridView1.Refresh();
        }

        private void btnSelectOther_Click(Object sender, EventArgs e)
        {
            if (_models == null) return;

            foreach (var item in _models)
            {
                item.EnableSync = !item.EnableSync;
            }

            dataGridView1.Refresh();
        }
    }
}
