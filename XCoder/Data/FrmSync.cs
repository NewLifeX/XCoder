using System.ComponentModel;
using System.Data;
using CrazyCoder.Data.Models;
using NewLife;
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
                var conns = DAL.ConnStrs.Keys.Where(e => e != connName).OrderBy(e => e).ToArray();
                cbTarget.DataSource = conns;

                // 获取数据表
                _tables = _source.Tables;
                _models = _tables.Select(e => new TableModel
                {
                    Name = e.TableName,
                    DisplayName = e.DisplayName,
                    EnableSync = true,
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

        private void btnConnect2_Click(Object sender, EventArgs e)
        {
            var connName = cbTarget.SelectedItem + "";
            var dal = DAL.Create(connName);

            Task.Run(() => FetchRows2(dal));
        }

        void FetchRows2(DAL dal)
        {
            var tables = dal.Tables;
            if (tables == null) return;

            foreach (var item in _models)
            {
                if (tables.Any(e => e.TableName.EqualIgnoreCase(item.Name)))
                {
                    var sb = new SelectBuilder { Table = item.Name };
                    item.Total2 = dal.SelectCount(sb);
                }
            }

            Invoke(() => dataGridView1.Refresh());
        }

        private async void btnSync_Click(Object sender, EventArgs e)
        {
            var connName = cbTarget.SelectedItem + "";
            if (connName.IsNullOrEmpty()) return;

            //var ts = _models.Select(e => e.Table).ToArray();
            //if (ts.Length == 0) return;

            gbSetting.Enabled = false;

            var syncSchema = cbSyncSchema.Checked;
            var ignoreError = cbIgnoreError.Checked;
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
            //Task.Run(() => dpk.SyncAll(ts, connName, syncSchema));
            await Task.Run(() =>
            {
                var dal = DAL.Create(cbTarget.SelectedItem + "");
                var tables = dal.Tables ?? new List<IDataTable>();

                _source.Db.ShowSQL = false;
                _source.Session.ShowSQL = false;
                dal.Db.ShowSQL = false;
                dal.Session.ShowSQL = false;
                foreach (var item in _models)
                {
                    if (!item.EnableSync) continue;

                    try
                    {
                        if (!syncSchema && !tables.Any(e => e.TableName.EqualIgnoreCase(item.Name)))
                        {
                            item.Description = "目标表不存在，跳过！";
                        }
                        else
                        {
                            var tb = _tables.FirstOrDefault(e => e.TableName == item.Name);
                            var rs = _source.Sync(tb, connName, syncSchema, (p, dt) => { });
                            item.Finish = rs;
                            item.Description = "成功！";
                        }
                    }
                    catch (Exception ex)
                    {
                        item.Description = ex.Message;
                        XTrace.WriteException(ex);

                        if (!ignoreError) throw;
                    }

                    Invoke(() => dataGridView1.Refresh());
                }
                _source.Session.ShowSQL = true;
                _source.Db.ShowSQL = true;
                dal.Session.ShowSQL = true;
                dal.Db.ShowSQL = true;
            });

            gbSetting.Enabled = true;
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
